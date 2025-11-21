using UnityEngine;
using Game.Core;
using Game.Interfaces;

namespace Game.Modifiers
{
	public class SpeedModifier : MonoBehaviour, IModifier
	{
		[SerializeField]private float speedModifierValue;
		private Movement actorMovement;

		public void Modify (GameObject actor)
		{
			actorMovement = actor.GetComponent<Movement> ();
			actorMovement.Acceleration += speedModifierValue;

			Destroy (this.gameObject);
		}
	}
}

