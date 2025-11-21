using UnityEngine;
using Game.Core;

namespace Game.Player
{
	public class PlayerMovement : Movement
	{
		private float hMoveInput = 0f;
		private float vMoveInput = 0f;

		private float direction = 1f;
		private bool grounded;
		private bool canJump, canClimb;

		private RaycastHit2D groundHit;

		[SerializeField]private Transform groundCheck;
		[SerializeField]private float checkRadius;
		[SerializeField]private float jumpRayDistance = 2f;

		private void Update()
		{
			hMoveInput = Input.GetAxisRaw ("Horizontal");
			vMoveInput = Input.GetAxisRaw ("Vertical");

			if(hMoveInput != 0f)
			{
				direction = hMoveInput;
			}

			if(Input.GetKeyDown(KeyCode.Space) && (grounded || (CanWallJump(direction) && !canClimb)))
			{
				canJump = true;
			}

			if(Input.GetKey(KeyCode.LeftControl))
			{
				canClimb = true;
			}
			else if(Input.GetKeyUp(KeyCode.LeftControl))
			{
				canClimb = false;
			}
		}

		private void FixedUpdate()
		{
			groundHit = Physics2D.Raycast (groundCheck.position, Vector2.down, jumpRayDistance, GroundMask);

			if (groundHit)
				grounded = true;
			else
				grounded = false;

			Move (hMoveInput, grounded);
			Jump (ref canJump);
			ChangeDirection (hMoveInput);
			MultiplyFallAmount ();
			Climb (direction, vMoveInput, canClimb);
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere (groundCheck.position, checkRadius);
		}
	}
}
