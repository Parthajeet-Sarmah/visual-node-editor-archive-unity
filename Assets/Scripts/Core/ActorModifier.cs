using UnityEngine;
using System.Collections;
using Game.Interfaces;

namespace Game.Core
{
	public class ActorModifier : MonoBehaviour
	{
		private Collider2D currentPickup;
		private IModifier currentModifier;
		[SerializeField]private Vector2 modifierBoxSize;

		[SerializeField]private LayerMask pickupMask;

		private void Update()
		{
			if (!currentPickup)
				return;

			currentModifier = currentPickup.GetComponent<IModifier> ();
			currentModifier.Modify (this.gameObject);
			currentPickup = null;
			currentModifier = null;
		}

		private void FixedUpdate()
		{
			currentPickup = Physics2D.OverlapBox (transform.position, modifierBoxSize, 0f, pickupMask);
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube (transform.position, new Vector3 (modifierBoxSize.x, modifierBoxSize.y, 1f));
		}
	}
}

