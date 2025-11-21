using UnityEngine;
using System.Collections.Generic;

namespace Game.Core.Dialogue
{
	[System.Serializable]
	public class Dialogue
	{
		public string speakerName;
		public int nodeBelongingToID;

		[TextArea(3, 10)]
		public List<string> sentences;

		[Header("Player Responses")]
		public List<PlayerResponse> playerResponse;

		public Dialogue(int nodeID, string name, List<string> sentences)
		{
			this.nodeBelongingToID = nodeID;
			this.speakerName = name;
			this.sentences = sentences;

			playerResponse = new List<PlayerResponse> ();
		}
	}

	[System.Serializable]
	public class ResponseDialogue
	{
		public string speakerName;
		public int nodeBelongingToID;

		[TextArea(3, 10)]
		public List<string> sentences;

		public ResponseDialogue(int nodeID, string name, List<string> sentences)
		{
			this.nodeBelongingToID = nodeID;
			this.speakerName = name;
			this.sentences = sentences;
		}
	}

	[System.Serializable]
	public class PlayerResponse
	{
		[TextArea(3, 10)]
		public string response;
		public int nodeBelongingToID;

		public List<ResponseDialogue> responseFollowingDialogues;

		public PlayerResponse(int nodeID, string response)
		{
			this.nodeBelongingToID = nodeID;
			this.response = response;
			this.responseFollowingDialogues = new List<ResponseDialogue> ();
		}
	}
}

