using UnityEngine;
using Game.Interfaces;
using Game.Core;

namespace Game.Modifiers
{
	public class HealthModifier : MonoBehaviour, IModifier
	{
		[SerializeField]private float healthAddAmount;
		private CharacterStats stats;

		public void Modify (GameObject actor)
		{
			stats = actor.GetComponent<CharacterStats> ();
			stats.Health += healthAddAmount;
			stats.UpdateUI (typeof(HealthModifier), healthAddAmount);

			Destroy (this.gameObject);
		}
	}
}
