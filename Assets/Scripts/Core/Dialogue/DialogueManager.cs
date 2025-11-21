using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Core.Dialogue
{
	public class DialogueManager : MonoBehaviour
	{
		public static DialogueManager instance;

		[SerializeField]private Text speakerText;
		[SerializeField]private Text dialogueText;

		[SerializeField]private GameObject dialogueBox;
		[SerializeField]private Animator dialogueBoxAnimator;

		[SerializeField]private List<GameObject> choiceButtons;
		[SerializeField]private GameObject continueButton;

		private Queue<string> currentDialogueSentences = new Queue<string> ();
		private List<Dialogue> dialogues = new List<Dialogue> ();

		private MouseOverDialogueBox mDialogueBox;
		private Dialogue activeDialogue;
		private int activeDialogueIndex;

		private bool showChoiceButtons = false;
		private bool responseDialogueActive = false, lastResponseDialogueActive = false;
		private int choiceIndex = -1;
		private int responseDialogueIndex = 0, responseDialogueCurrentSentenceIndex = 0;

		private void Awake ()
		{
			if (instance == null)
				instance = this;
			else
				Destroy (this);

			mDialogueBox = dialogueBox.GetComponent<MouseOverDialogueBox> ();
		}

		public void StartDialogue (List<Dialogue> newDialogueList)
		{
			dialogues = newDialogueList;

			if (dialogues.Count <= 0) {
				Debug.LogError ("No sentences to read");
				return;
			}

			dialogueBoxAnimator.SetBool ("isOpen", true);

			ChangeActiveDialogue ();
			DisplayNextSentence ();
		}

		public void SetChoiceIndex(int index)
		{
			choiceIndex = index;

			for (int i = 0; i < choiceButtons.Count; i++) 
			{
				if (!choiceButtons [i].activeSelf)
					continue;

				choiceButtons [i].SetActive (false);
			}

			DisplayNextSentence ();
		}

		public void DisplayNextSentence ()
		{
			if (currentDialogueSentences.Count <= 0) {

				responseDialogueActive = CheckForPlayerResponseFollowingDialogue (ref choiceIndex);

				if (responseDialogueActive || lastResponseDialogueActive)
				{
					if (lastResponseDialogueActive)
						lastResponseDialogueActive = false;

					return;
				}

				ChangeActiveDialogue ();
			} else {
				string sentence = currentDialogueSentences.Dequeue ();

				if(currentDialogueSentences.Count <= 0)
				{
					showChoiceButtons = true;
				}

				StopAllCoroutines ();

				if(!responseDialogueActive)
					speakerText.text = dialogues [activeDialogueIndex].speakerName;
				
				StartCoroutine (TypeSentence (sentence));
			}
		}

		private bool CheckForPlayerResponseFollowingDialogue(ref int cIndex)
		{
			if (cIndex < 0)
				return false;

			if (dialogues [activeDialogueIndex].playerResponse.Count <= 0)
				return false;

			if (cIndex >= dialogues [activeDialogueIndex].playerResponse.Count)
				return false;

			PlayerResponse currentResponse = dialogues [activeDialogueIndex].playerResponse [cIndex];
			ResponseDialogue currentResponseDialogue = currentResponse.responseFollowingDialogues[responseDialogueIndex];

			speakerText.text = currentResponseDialogue.speakerName;
			string sentence = currentResponseDialogue.sentences [responseDialogueCurrentSentenceIndex];

			StopAllCoroutines ();
			StartCoroutine (TypeSentence (sentence));

			responseDialogueCurrentSentenceIndex++;

			if(responseDialogueCurrentSentenceIndex >= currentResponseDialogue.sentences.Count)
			{
				responseDialogueIndex++;

				if(responseDialogueIndex >= currentResponse.responseFollowingDialogues.Count)
				{
					responseDialogueIndex = 0;
					responseDialogueCurrentSentenceIndex = 0;
					lastResponseDialogueActive = true;
					cIndex = -1;
					return false;
				}
			}

			return true;
		}

		private void EnqueueDialogueSentences ()
		{
			foreach (string sentence in activeDialogue.sentences) {
				currentDialogueSentences.Enqueue (sentence);
			}
		}

		private void ChangeActiveDialogue ()
		{
			if (activeDialogue == null) {
				activeDialogue = dialogues [0];
				activeDialogueIndex = 0;
				EnqueueDialogueSentences ();
				speakerText.text = dialogues [0].speakerName;
				return;
			}
	
			activeDialogueIndex++;
	
			if (activeDialogueIndex > dialogues.Count - 1) {
				EndDialogue ();
				return;
			}
	
			activeDialogue = dialogues [activeDialogueIndex];
			EnqueueDialogueSentences ();
			DisplayNextSentence ();
		}

		private void QuickSentenceType(int startingIndex, string sentence)
		{
			dialogueText.text += sentence.Substring (startingIndex);
		}

		private IEnumerator TypeSentence (string sentence)
		{
			int currentIndex = 0;
			continueButton.SetActive (false);
			dialogueText.text = "";

			char[] letters = sentence.ToCharArray ();

			for (int i = 0; i < letters.Length; i++) 
			{
				dialogueText.text += letters[i];
				currentIndex = i;

				if(Input.GetMouseButton(0) && mDialogueBox.MouseOverBox)
				{
					QuickSentenceType (currentIndex + 1, sentence);
					break;
				}

				yield return null;
			}

			if(showChoiceButtons)
			{
				SetChoiceButtons ();
				showChoiceButtons = false;
			}
			else if(!continueButton.activeSelf)
			{
				continueButton.SetActive (true);
			}
		}

		private void SetChoiceButtons()
		{
			continueButton.SetActive (false);

			int count = dialogues [activeDialogueIndex].playerResponse.Count;

			RectTransform buttonRect = (RectTransform)choiceButtons [0].transform;
			RectTransform parentRect = (RectTransform)choiceButtons [0].transform.parent;

			float limitXLeft = parentRect.position.x - parentRect.rect.width / 2f + 10f;
			float limitXRight = parentRect.position.x + parentRect.rect.width / 2f - 10f;

			float placableAreaX = limitXRight - limitXLeft;
			float eachButtonXPos = limitXLeft / 2f + placableAreaX / (count + 1);

			float yPos = parentRect.position.y - parentRect.rect.height - buttonRect.rect.height / 2f;

			for (int i = 0; i < count; i++) 
			{
				choiceButtons [i].transform.position = new Vector3 (eachButtonXPos * (i + 1), yPos, choiceButtons [i].transform.position.z);
				choiceButtons [i].SetActive (true);

				choiceButtons [i].GetComponentInChildren<Text> ().text = dialogues [activeDialogueIndex].playerResponse [i].response;
			}
		}

		private void EndDialogue ()
		{
			print ("End of conversation");
			continueButton.SetActive (false);

			dialogueBoxAnimator.SetBool ("isOpen", false);
		}
	}
}

