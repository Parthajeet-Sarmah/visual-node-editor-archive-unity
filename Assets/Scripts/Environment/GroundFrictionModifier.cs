using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Environment
{
	public class GroundFrictionModifier : MonoBehaviour
	{
		private Rigidbody2D actorRigidbody;
	
		[SerializeField]private float frictionValue;

		public void SetActorRigidbody2D (Rigidbody2D rb)
		{
			actorRigidbody = rb;
		}

		public void ModifyActorFriction ()
		{
			if (actorRigidbody == null)
				return;

			if (actorRigidbody.sharedMaterial == null)
				return;
	
			if (actorRigidbody.sharedMaterial.friction == frictionValue)
				return;

			actorRigidbody.sharedMaterial.friction = frictionValue;
		}
	}
}
