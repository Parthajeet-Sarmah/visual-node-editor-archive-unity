using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core 
{
	public class ActorPusher : MonoBehaviour {
	
		[SerializeField]private float pushForce;

		private Transform currentWall;
		private int pushDirection;

		private Rigidbody2D actorRb;

		private void Awake()
		{
			actorRb = GetComponentInParent<Rigidbody2D> ();
		}

		private void OnTriggerEnter2D(Collider2D col)
		{
			if(IsTouchingWall(col.gameObject))
			{
				currentWall = col.gameObject.transform;
				pushDirection = CalculateDirection (currentWall);
			}
		}

		private void OnTriggerExit2D(Collider2D col)
		{
			if(IsTouchingWall(col.gameObject))
			{
				PushActor (col.transform);
			}
		}

		private int CalculateDirection(Transform t)
		{
			if(t.position.x > transform.position.x)
			{
				return 1;
			}
			else if(t.position.x < transform.position.x)
			{
				return -1;
			}

			return 0;
		}

		private void PushActor(Transform t)
		{
			if(IsAtTop(t))
			{
				actorRb.AddForce (new Vector2 (pushDirection * pushForce, 0f), ForceMode2D.Impulse);
			}
		}

		private bool IsAtTop(Transform t)
		{
			if(transform.position.y > t.position.y + t.lossyScale.y / 2f)
			{
				return true;
			}

			return false;
		}

		private bool IsTouchingWall(GameObject g)
		{
			if(g.layer == LayerMask.NameToLayer("Wall And Ground"))
			{
				return true;
			}

			return false;
		}
	}
}
