using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace Game.Core.Dialogue
{
	public class MouseOverDialogueBox : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		private bool mouseOverBox = false;

		public bool MouseOverBox {
			get {
				return mouseOverBox;
			}
		}

		void IPointerEnterHandler.OnPointerEnter (PointerEventData eventData)
		{
			mouseOverBox = true;
		}

		void IPointerExitHandler.OnPointerExit (PointerEventData eventData)
		{
			mouseOverBox = false;
		}
	}
}

