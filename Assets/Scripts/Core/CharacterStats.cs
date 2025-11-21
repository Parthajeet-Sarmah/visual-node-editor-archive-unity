using System;
using System.Collections;
using UnityEngine;

namespace Game.Core
{
	public class CharacterStats : MonoBehaviour
	{
		[SerializeField]private float health;
		[SerializeField]private float damage;

		private UI.CharacterStatsUI statsUI;

		public float Health {
			get {
				return health;
			}
			set {
				health = value;
			}
		}
		public float Damage {
			get {
				return damage;
			}
			set {
				damage = value;
			}
		}

		private void Awake()
		{
			statsUI = GetComponent<UI.CharacterStatsUI> ();
		}

		public void UpdateUI(Type type, object t)
		{
			statsUI.onUIValueChanged.Invoke(type, t);
		}
	}
}

