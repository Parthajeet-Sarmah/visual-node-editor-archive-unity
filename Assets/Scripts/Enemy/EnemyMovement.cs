using UnityEngine;
using Game.Core;

namespace Game.Enemy
{
	public class EnemyMovement : Movement
	{
	
		private float moveDirection = 1f;
		private float targetDirectionWhileClimb = 1f;
		private bool isGrounded = true;
	
		[SerializeField]private float stoppingDistance = 1.2f;
		private float distanceToPlayerX;
	
		private bool wallHit;
		private bool isClimbing = false;
	
		[SerializeField]private Vector2 wallJumpForce;
		[SerializeField]private Transform player;
	
		[SerializeField]private Vector2 groundCheckBoxSize;
	
		[SerializeField]private bool hasClimbingCapability = false;
		[SerializeField]private bool hasWallJumpingCapability = false;
	
		[SerializeField]private Transform groundCheck;

		private void Update ()
		{
			if (!isClimbing)
				FollowTarget (player);
	
			MoveForTargetY (player);
		}

		private void FixedUpdate ()
		{
			isGrounded = Physics2D.OverlapBox (groundCheck.position, groundCheckBoxSize, 0f, GroundMask);
			MultiplyFallAmount ();
		}

		public void FollowTarget (Transform target)
		{
			moveDirection = Mathf.Sign (target.position.x - transform.position.x);
	
			distanceToPlayerX = Mathf.Abs (target.position.x - transform.position.x);
	
			if (distanceToPlayerX <= stoppingDistance)
				return;
	
			if (!isGrounded)
				return;
	
			Move (moveDirection, isGrounded);
		}

		private void MoveForTargetY (Transform target)
		{
			if (!hasClimbingCapability)
				return;
	
			if (Climb (moveDirection, 1f))
				isClimbing = true;
			else {
				if (isClimbing)
					isClimbing = false;
			}
	
			if (!hasWallJumpingCapability)
				return;
	
			targetDirectionWhileClimb = Mathf.Sign (target.position.x - transform.position.x);
	
			if (targetDirectionWhileClimb == -moveDirection) {
				if (CanWallJump (moveDirection) && isClimbing) {
					SingleDirectionJump (targetDirectionWhileClimb, wallJumpForce);
					moveDirection = targetDirectionWhileClimb;
					isClimbing = false;
				}
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube (groundCheck.position, new Vector3 (groundCheckBoxSize.x, groundCheckBoxSize.y, 1f));
		}
	}
}
