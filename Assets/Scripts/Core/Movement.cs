using UnityEngine;
using System.Collections;
using Game.Environment;

namespace Game.Core
{
	public class Movement : MonoBehaviour
	{
		private Rigidbody2D rb;

		private RaycastHit2D hitWall;

		private bool touchingWall;
		private bool backwardTouchWall;
		private float initialSpeed;
		private float maxSpeed;
		private float originalGravityScale;

		[SerializeField]private float acceleration;
		[SerializeField]private float jumpForce;
		[SerializeField]private float wallClimbSpeed;
		[SerializeField]private float wallSlidingSpeed;
		[SerializeField]private float fallMultiplier;
		[Range(0f, 20f)][SerializeField]private float maxSpeedMultiplier = 1.5f;
		[Range(0f, 1f)][SerializeField]private float hMoveDamping, hAirDamping;

		[SerializeField]private float wallHitRayDistance = 1f, wallHitBackRayDistance = 1f;
		[SerializeField]private LayerMask wallMask;
		[SerializeField]private LayerMask groundMask;
		[SerializeField]private Vector3 wallRaycastOffset;

		//Properties
		public float Acceleration {
			get {
				return acceleration;
			}
			set {
				acceleration = value;
			}
		}
		public float JumpForce {
			get {
				return jumpForce;
			}
			set {
				jumpForce = value;
			}
		}
		public bool TouchingWall
		{
			get
			{
				return touchingWall || backwardTouchWall;
			}
		}
		public LayerMask GroundMask {
			get {
				return groundMask;
			}
		}
		public LayerMask WallMask
		{
			get
			{
				return wallMask;
			}
		}
		public float WallHitRayDistance {
			get {
				return wallHitRayDistance;
			}
		}
		//

		private void Awake ()
		{
			rb = GetComponent<Rigidbody2D> ();
			maxSpeed = maxSpeedMultiplier * acceleration;
			originalGravityScale = rb.gravityScale;
		}

		public void Move (float direction, bool grounded)
		{
			if (direction == 0f)
				return;

			initialSpeed += acceleration * direction;

			if(!grounded && !touchingWall)
			{
				initialSpeed *= Mathf.Pow (1f - hAirDamping, Time.deltaTime * 10f);
			}
			else
			{
				initialSpeed *= Mathf.Pow (1f - hMoveDamping, Time.deltaTime * 10f);
			}
				
			initialSpeed = Mathf.Clamp (initialSpeed, -maxSpeed, maxSpeed);

			rb.velocity = new Vector2 (initialSpeed, rb.velocity.y);
		}

		public void Jump(ref bool jump)
		{
			if (!jump)
				return;

			rb.velocity = new Vector2 (rb.velocity.x, jumpForce * Time.deltaTime);

			jump = false;
		}

		public void SingleDirectionJump(float direction, Vector2 force, bool changeYDirection = false)
		{
			if(changeYDirection)
				rb.AddForce (new Vector2 (force.x * direction, force.y * direction));
			else
				rb.AddForce (new Vector2 (force.x * direction, force.y));
		}

		public void MultiplyFallAmount()
		{
			if(rb.velocity.y < 0f)
			{
				rb.velocity += Vector2.up * Physics.gravity.y * (fallMultiplier - 1f) * Time.deltaTime;
			}
		}

		public bool CanWallJump(float faceDirection)
		{
			if(IsTouchingWallForward(faceDirection) || IsTouchingWallBackward(faceDirection))
			{
				return true;
			}

			return false;
		}

		public void ResetGravityScale ()
		{
			if (rb.gravityScale == 0f) {
				rb.gravityScale = originalGravityScale;
			}
		}

		public void ResetYVelocity()
		{
			if(rb.velocity.y != 0f)
			{
				rb.velocity = new Vector2 (rb.velocity.x, 0f);
			}	
		}

		public bool Climb(float actorFaceDirection, float verticalMoveDirection, bool canClimb = true)
		{
			if (!IsTouchingWallForward (actorFaceDirection))
			{
				ResetGravityScale ();

				return false;
			}

			if(!canClimb)
			{
				ResetGravityScale ();

				return false;
			}

			if(rb.gravityScale > 0f)
			{
				rb.gravityScale = 0f;
			}

			if (verticalMoveDirection != 0f) {
				rb.velocity = new Vector2 (0f, wallClimbSpeed * verticalMoveDirection);
			} 
			else
			{
				rb.velocity = new Vector2 (0f, -wallSlidingSpeed);
			}

			return true;
		}

		private bool IsTouchingWallBackward(float faceDirection)
		{
			backwardTouchWall = Physics2D.Raycast (transform.position + wallRaycastOffset, Vector2.left * faceDirection, wallHitBackRayDistance, wallMask);

			return backwardTouchWall;
		}

		private bool IsTouchingWallForward(float faceDirection)
		{
			hitWall = Physics2D.Raycast (transform.position + wallRaycastOffset, Vector2.right * faceDirection, wallHitRayDistance, wallMask);

			if(hitWall.collider.sharedMaterial != null)
			{
				hitWall.collider.sharedMaterial.friction = 0f;
			}
			
			touchingWall = hitWall.collider != null ? true : false;

			Debug.DrawRay (transform.position + wallRaycastOffset, Vector3.right * faceDirection * wallHitRayDistance, Color.red);
			return touchingWall;
		}

		public void ChangeDirection(float direction)
		{
			if(direction > 0f && rb.velocity.x < 0f)
			{
				Flip ();
			}
			else if(direction < 0f && rb.velocity.x > 0f)
			{
				Flip ();
			}
		}

		private void Flip()
		{
			Vector3 scaler = transform.localScale;
			scaler.x *= -1f;
			transform.localScale = scaler;
		}
	}
}

