using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Game.Modifiers;

namespace Game.Core.UI
{
	public class CharacterStatsUI : MonoBehaviour
	{
		public delegate void OnUIValueChange(Type type, object t);
		public OnUIValueChange onUIValueChanged;	

		[SerializeField]private Slider healthBar;

		private void Awake()
		{
			onUIValueChanged += SetUI;
		}

		private void SetUI(Type type, object t)
		{
			if(type == typeof(HealthModifier) && t is float)
			{
				healthBar.value += (float)t;
			}
		}
	}
}

