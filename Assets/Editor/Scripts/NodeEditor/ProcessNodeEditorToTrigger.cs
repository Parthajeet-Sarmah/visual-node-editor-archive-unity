using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class ProcessNodeEditorToTrigger
{
	private static List<Node> currentNodeLayer;

	private static List<Node> GetNextLayer(List<Node> currentLayer)
	{
		List<Node> newLayer = new List<Node> ();

		for (int i = 0; i < currentLayer.Count; i++) 
		{
			newLayer.AddRange (GetNextLayer (currentLayer [i]));
		}

		return newLayer;
	}

	private static List<Node> GetNextLayer(Node node)
	{
		return node.outNodesConnectedTo;
	}

	private static Node GetStartNode(List<Node> nodes)
	{
		for (int i = 0; i < nodes.Count; i++) 
		{
			if (nodes [i].GetType () == typeof(StartNode))
				return nodes [i];
		}
			
		return null;
	}

	private static bool CheckProhibitions(List<Node> nodes)
	{
		#region ONLY One Outgoing Start Node Connection
		Node startNode = GetStartNode (nodes);

		if(startNode == null)
		{
			Debug.LogError ("Start node does not exist");
			return false;
		}

		if (startNode.outNodesConnectedTo.Count > 1) {
			Debug.LogError ("Multiple outgoing connections from start node");
			return false;
		}
		#endregion
		#region Multiple Outgoing NPC Dialogue Nodes from one NPC Dialogue Node
		List<NPCDialogueNode> allNPCNodes = new List<NPCDialogueNode> ();

		for (int i = 0; i < nodes.Count; i++) {
			if (nodes [i].GetType () == typeof(NPCDialogueNode)) {
				allNPCNodes.Add ((NPCDialogueNode)nodes [i]);
			}
		}

		for (int i = 0; i < allNPCNodes.Count; i++) {
			int count = 0;
			Node node = null;
			for (int j = 0; j < allNPCNodes [i].outNodesConnectedTo.Count; j++) {
				node = allNPCNodes [i].outNodesConnectedTo [j];
				if (node.GetType () == typeof(NPCDialogueNode)) {
					count++;
				}
			}

			if (count > 1) {
				Debug.LogError ("Multiple outgoing NPC Dialogue nodes connected to NPCDialogueNode with ID: " + node.nodeID);
				return false;
			}
		}
		#endregion
		#region Multiple Outgoing Response Nodes from one Response Node
		List<ResponseNode> allResponseNodes = new List<ResponseNode> ();

		for (int i = 0; i < nodes.Count; i++) {
			if (nodes [i].GetType () == typeof(ResponseNode)) {
				allResponseNodes.Add ((ResponseNode)nodes [i]);
			}
		}

		for (int i = 0; i < allResponseNodes.Count; i++) {
			int count = 0;
			Node node = null;
			for (int j = 0; j < allResponseNodes [i].outNodesConnectedTo.Count; j++) {
				node = allResponseNodes [i].outNodesConnectedTo [j];
				if (node.GetType () == typeof(ResponseNode)) {
					count++;
				}
			}

			if (count > 1) {
				Debug.LogError ("Multiple outgoing Response nodes connected to Response node with ID: " + node.nodeID);
				return false; 
			}
		}
		#endregion
		#region Multiple Outgoing NPCDialogue Nodes from one Response Node
		for (int i = 0; i < allResponseNodes.Count; i++) {
			int count = 0;
			Node node = null;
			for (int j = 0; j < allResponseNodes [i].outNodesConnectedTo.Count; j++) {
				node = allResponseNodes [i].outNodesConnectedTo [j];
				if (node.GetType () == typeof(NPCDialogueNode)) {
					count++;
				}
			}

			if (count > 1) {
				Debug.LogError ("Multiple outgoing NPCDialogue nodes connected to Response node with ID: " + node.nodeID);
				return false; 
			}
		}
		#endregion
		#region Multiple Incoming ResponseNodes to one ResponseNode
		for (int i = 0; i < allResponseNodes.Count; i++) {
			int responseNodeCount = 0;
			for (int j = 0; j < allResponseNodes [i].inNodesConnectedTo.Count; j++) {
				if (allResponseNodes [i].GetType () == typeof(ResponseNode))
					responseNodeCount++;
			}

			if (responseNodeCount > 1) {
				Debug.LogError ("Multiple incoming ResponseNode connections to ResponseNode with node ID: " + allResponseNodes [i].nodeID);
				return false;
			}
		}
		#endregion
		#region Multiple Incoming NPCDialogueNodes to one ResponseNode
		for (int i = 0; i < allResponseNodes.Count; i++) {
			int npcDialogueNodeCount = 0;
			for (int j = 0; j < allResponseNodes [i].inNodesConnectedTo.Count; j++) {
				if (allResponseNodes [i].GetType () == typeof(NPCDialogueNode))
					npcDialogueNodeCount++;
			}

			if (npcDialogueNodeCount > 1) {
				Debug.LogError ("Multiple incoming NPCDialogueNode connections to ResponseNode with node ID: " + allResponseNodes [i].nodeID);
				return false;
			}
		}
		#endregion
		#region Multi Type Incoming Connections To NPCDialogueNode or ResponseNode
		for (int i = 0; i < allNPCNodes.Count; i++) {
			Node node = allNPCNodes [i];

			if (node.inNodesConnectedTo.Find (item => item.GetType () == typeof(ResponseNode)) != null &&
			   node.inNodesConnectedTo.Find (item => item.GetType () == typeof(NPCDialogueNode)) != null) {
				Debug.LogError ("NPCDialogue node with ID " + node.nodeID + " contains incoming connections to a NPCDialogue node " +
				"and a Response node");
				return false;
			}
		}
		for (int i = 0; i < allResponseNodes.Count; i++) {
			Node node = allResponseNodes [i];

			if (node.inNodesConnectedTo.Find (item => item.GetType () == typeof(ResponseNode)) != null &&
			   node.inNodesConnectedTo.Find (item => item.GetType () == typeof(NPCDialogueNode)) != null) {
				Debug.LogError ("NPCDialogue node with ID " + node.nodeID + " contains incoming connections to a NPCDialogue node " +
				"and a Response node");
				return false;
			}
		}
		#endregion
		#region Multi Type Outgoing Connections from NPCDialogueNode or ResponseNode
		for (int i = 0; i < allNPCNodes.Count; i++) {
			Node node = allNPCNodes [i];

			if (node.outNodesConnectedTo.Find (item => item.GetType () == typeof(NPCDialogueNode)) != null &&
				node.outNodesConnectedTo.Find (item => item.GetType () == typeof(ResponseNode)) != null) 
			{
				Debug.LogError ("NPCDialogue node with ID " + node.nodeID + " contains outgoing connections to a NPCDialogue node " +
					"and a Response node");
				return false;
			}	
		}
		for (int i = 0; i < allResponseNodes.Count; i++) {
			Node node = allResponseNodes [i];

			if (node.outNodesConnectedTo.Find (item => item.GetType () == typeof(NPCDialogueNode)) != null &&
				node.outNodesConnectedTo.Find (item => item.GetType () == typeof(ResponseNode)) != null) 
			{
				Debug.LogError ("Response node with ID " + node.nodeID + " contains outgoing connections to a NPCDialogue node " +
					"and a Response node");
				return false;
			}	
		}
		#endregion
		#region Incomplete Connections
		for (int i = 0; i < nodes.Count; i++) {

			for (int j = 0; j < nodes [i].outPoints.Length; j++) {
				if (!nodes [i].outPoints [j].isConnected) {
					Debug.LogError ("Out point of node with id " + nodes [i] + " is not connected");
					return false;
				}
			}

			for (int j = 0; j < nodes [i].inPoints.Length; j++) {
				if (!nodes [i].inPoints [j].isConnected) {
					Debug.LogError ("In point of node with id " + nodes [i] + " is not connected");
					return false;
				}
			}
		}
		#endregion

		return true;
	}

	public static void ProcessNodesToTrigger(List<Node> nodes, Game.Core.Dialogue.DialogueTrigger trigger)
	{
		if(nodes == null)
		{
			Debug.LogError ("Nodes list is null");
			return;
		}
		else if(nodes.Count <= 0)
		{
			Debug.LogError ("Nodes list has no nodes in it");
			return;
		}

		if(!CheckProhibitions(nodes))
		{
			Debug.Log ("Failed to complete prohibition test");
			return;
		}

		Node startNode = GetStartNode (nodes);

		currentNodeLayer = new List<Node> ();
		currentNodeLayer.Add (startNode);

		next_layer_process:

		currentNodeLayer = GetNextLayer (currentNodeLayer);

		if(CheckEndOfProccessing(currentNodeLayer))
		{
			return;
		}
			
		Node currentNode = null;
		NPCDialogueNode currentNPCNode = null;
		ResponseNode currentResponseNode = null;

		for (int i = 0; i < currentNodeLayer.Count; i++) 
		{
			currentNode = currentNodeLayer [i];
			if(currentNode.GetType() == typeof(NPCDialogueNode))
			{
				currentNPCNode = (NPCDialogueNode)currentNode;
				ProcessNPCDialogueNode (trigger, currentNPCNode);
			}
			else if(currentNode.GetType() == typeof(ResponseNode))
			{
				currentResponseNode = (ResponseNode)currentNode;
				ProcessResponseNode (trigger, currentResponseNode);
			}
		}

		goto next_layer_process;
	}

	private static bool CheckEndOfProccessing(List<Node> currentLayer)
	{
		int count = 0;
		for (int i = 0; i < currentLayer.Count; i++) 
		{
			if(currentLayer[i].GetType() == typeof(EndNode))
			{
				count++;
			}
		}

		if(count == currentLayer.Count && count != 0)
		{
			return true;
		}

		return false;
	}

	private static List<string> ModifyForInvalidNPCDialogue(NPCDialogueNode node)
	{
		List<string> newDialogueTexts = new List<string> ();

		for (int i = 0; i < node.dialogueTexts.Count; i++) 
		{
			if(node.dialogueTexts[i] != "Dialogue Text" && node.dialogueTexts[i] != "")
			{
				newDialogueTexts.Add (node.dialogueTexts [i]);
			}
		}

		return newDialogueTexts;
	}

	private static void ProcessNPCDialogueNode(Game.Core.Dialogue.DialogueTrigger trigger, NPCDialogueNode node)
	{
		//Check if node dialogue already exists.
		if (trigger.CheckIfDialogueExists (node.nodeID))
			return;

		List<string> dialogueTextsModified = new List<string> ();

		//If not, first check for invalid sentences in the dialogues provided in the nodes and modify accordingly;
		if(node.dialogueTexts.Contains("Dialogue Text") || node.dialogueTexts.Contains(""))
		{
			dialogueTextsModified = ModifyForInvalidNPCDialogue (node);
		}

		//If no discrepancies check if it has a not null responseFollowingNode
		if (node.responseNodeFollowedNode != null) {
			//If yes create a ResponseDialogue Component to PlayerResponse corresponding to responseNodeFollowedNode;
			Game.Core.Dialogue.Dialogue dialogue = trigger.ReturnDialogueWithID (node.responseNodeFollowedNode.parentDialogueNode.nodeID);

			if (dialogue == null) {
				Debug.LogError ("The corresponding dialogue to NPCDialogueNode with ID: " + node.responseNodeFollowedNode.nodeID + " does not exist !");
				return;
			}

			//Get the PlayerResponse of the original dialogue i.e that the dialogue the responeNodeFollowedNode is attached to outcoming
			Game.Core.Dialogue.PlayerResponse dialogueResponse = trigger.ReturnResponseWithDialogueAndID (dialogue, node.responseNodeFollowedNode.nodeID);

			if(dialogueResponse == null)
			{
				Debug.LogError ("No player responses found for ResponseNode with ID: " + node.responseNodeFollowedNode.nodeID);
				return;
			}

			//Check if the ResponseDialogue already exists. If yes, then don't create another one
			if (trigger.CheckIfResponseDialogueExists (dialogueResponse.responseFollowingDialogues, node.nodeID))
				return;

			dialogueResponse.responseFollowingDialogues.Add (new Game.Core.Dialogue.ResponseDialogue (node.nodeID, node.speakerNameText, dialogueTextsModified));
		}
		else
		{
			Game.Core.Dialogue.Dialogue newDialogue = new Game.Core.Dialogue.Dialogue (node.nodeID, node.speakerNameText, dialogueTextsModified);
			trigger.AddDialogue (newDialogue);
		}
	}

	private static void ProcessResponseNode(Game.Core.Dialogue.DialogueTrigger trigger, ResponseNode node)
	{
		Game.Core.Dialogue.Dialogue dialogue = trigger.ReturnDialogueWithID (node.parentDialogueNode.nodeID);

		if(dialogue == null)
		{
			Debug.LogError ("ResponseNode is attached to an invalid NPCDialogueNode");
			return;
		}

		if(trigger.ReturnResponseWithDialogueAndID(dialogue, node.nodeID) != null)
		{
			return;
		}

		if(node.playerResponse == "" || node.playerResponse == "Player Response")
		{
			Debug.Log ("Invalid player response for ResponseNode with ID: " + node.nodeID);
			return;
		}

		dialogue.playerResponse.Add (new Game.Core.Dialogue.PlayerResponse (node.nodeID, node.playerResponse));
	}
}

