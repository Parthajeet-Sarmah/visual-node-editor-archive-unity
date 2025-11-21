using UnityEngine;
using System.Collections.Generic;

namespace Game.Core.Dialogue
{
	public class DialogueTrigger : MonoBehaviour
	{
		[SerializeField]private List<Dialogue> dialogueList = new List<Dialogue>();

		public void TriggerDialogue ()
		{
			DialogueManager.instance.StartDialogue (dialogueList);
		}

		private void Update()
		{
			if(Input.GetKeyDown(KeyCode.F))
			{
				TriggerDialogue ();
			}
		}

		public Dialogue ReturnDialogueWithID(int nodeID)
		{
			for (int i = 0; i < dialogueList.Count; i++) 
			{
				if(dialogueList[i].nodeBelongingToID == nodeID)
				{
					return dialogueList [i];
				}
			}

			return null;
		}

		public PlayerResponse ReturnResponseWithDialogueAndID(Dialogue d, int id)
		{
			for (int i = 0; i < d.playerResponse.Count; i++) 
			{
				if(d.playerResponse[i].nodeBelongingToID == id)
				{
					return d.playerResponse [i];
				}
			}

			return null;
		}

		public bool CheckIfDialogueExists(int id)
		{
			for (int i = 0; i < dialogueList.Count; i++) 
			{
				if(dialogueList[i].nodeBelongingToID == id)
				{
					return true;
				}
			}

			return false;
		}

		public bool CheckIfResponseExists(int id)
		{
			for (int i = 0; i < dialogueList.Count; i++) 
			{
				for (int j = 0; j < dialogueList[i].playerResponse.Count; j++) 
				{
					if(dialogueList[i].playerResponse[j].nodeBelongingToID == id)
					{
						return true;
					}
				}
			}

			return false;
		}

		public bool CheckIfResponseDialogueExists(List<ResponseDialogue> list, int id)
		{
			for (int i = 0; i < list.Count; i++) 
			{
				if (list [i].nodeBelongingToID == id)
					return true;
			}

			return false;
		}

		public void AddDialogue(Dialogue d)
		{
			dialogueList.Add (d);
		}
	}
}

