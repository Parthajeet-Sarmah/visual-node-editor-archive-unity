using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using FullSerializer;

public class NodeEditor : EditorWindow {

	private readonly string saveDataPath = "Assets/Editor/Resources/NodeInfoSaveData.asset";
	private readonly string dictionaryStorePath = "Assets/Editor/Saved Dialogue Graphs/NodeDict.json";
	public List<Node> nodes;
	private List<Connection> connections;

	private NodeSaveData saveData;
	private string saveDataKey, loadDataKey;

	private Game.Core.Dialogue.DialogueTrigger dialogueTrigger;

	private GUIStyle nodeStyle;
	private GUIStyle selectedNodeStyle;

	private GUIStyle inPointStyle;
	private GUIStyle outPointStyle;

	private ConnectionPoint selectedInPoint;
	private ConnectionPoint selectedOutPoint;

	private Vector2 offset;
	private Vector2 drag;

	private Rect dialogueTriggerRect, dialogueLabel;
	private Rect saveLabelRect, loadLabelRect;
	private Rect controlBoxRect, saveButtonRect, loadButtonRect, processTriggerButtonRect;

	private bool activeConnectionLine;

	[MenuItem("Window/Node Editor")]
	private static void OpenWindow()
	{
		NodeEditor editor = GetWindow<NodeEditor> ();
		editor.titleContent = new GUIContent ("Node Editor");
	}

	private void OnEnable()
	{
		saveDataKey = "";
		loadDataKey = "";

		nodeStyle = new GUIStyle ();
		nodeStyle.normal.background = Resources.Load<Texture2D> ("NodeTexture");
		nodeStyle.border = new RectOffset (12, 12, 12, 12);

		selectedNodeStyle = new GUIStyle ();
		selectedNodeStyle.normal.background = Resources.Load<Texture2D> ("NodeTexture_Selected");
		selectedNodeStyle.border = new RectOffset (12, 12, 12, 12);

		inPointStyle = new GUIStyle ();
		inPointStyle.normal.background = Resources.Load<Texture2D> ("NodeTexture_CP");
		inPointStyle.active.background = Resources.Load<Texture2D> ("NodeTexture_CP_Selected");
		inPointStyle.border = new RectOffset (4, 4, 12, 12);

		outPointStyle = new GUIStyle ();
		outPointStyle.normal.background = Resources.Load<Texture2D> ("NodeTexture_CP");
		outPointStyle.active.background = Resources.Load<Texture2D> ("NodeTexture_CP_Selected");
		outPointStyle.border = new RectOffset (4, 4, 12, 12);

		this.LoadSaveData ();

		controlBoxRect = new Rect (0f, 0f, 240f, 290f);

		loadButtonRect = new Rect (controlBoxRect.width / 2f - 108f, 90f, 100f, 40f);
		saveButtonRect = new Rect (controlBoxRect.width / 2f + 5f, 90f, 100f, 40f);
		processTriggerButtonRect = new Rect (controlBoxRect.width / 2f - 58f, 230f, 120f, 40f);

		saveLabelRect = new Rect (30f, 20f, 70f, 20f);
		loadLabelRect = new Rect (30f, 42f, 70f, 20f);

		dialogueLabel = new Rect (controlBoxRect.width / 2f - 58f, 160f, 150f, 20f);
		dialogueTriggerRect = new Rect (controlBoxRect.width / 2f - 98f, 180f, 200f, 17f);
	}

	private void OnGUI()
	{
		DrawGrid (20f, 0.2f, Color.grey);
		DrawGrid (100, 0.4f, Color.grey);

		DrawNodes ();
		DrawConnections ();

		DrawConnectionLine (Event.current);

		ProcessInspector ();
	
		ProcessNodeEvents (Event.current);
		ProcessEvents (Event.current);

		if (GUI.changed)
			Repaint ();
	}

	private void ProcessInspector ()
	{
		GUI.Box (controlBoxRect, "");

		GUI.Label (saveLabelRect, "Save Key");
		GUI.Label (loadLabelRect, "Load Key");

		saveDataKey = GUI.TextArea (new Rect (100f, 20f, 100f, 20f), saveDataKey);
		loadDataKey = GUI.TextArea (new Rect (100f, 42f, 100f, 20f), loadDataKey);

		GUI.Label (dialogueLabel, "Processed Trigger");

		dialogueTrigger = (Game.Core.Dialogue.DialogueTrigger)EditorGUI.ObjectField (dialogueTriggerRect, "", dialogueTrigger, 
			typeof(Game.Core.Dialogue.DialogueTrigger), true);

		if (GUI.Button (loadButtonRect, "Load Data")) 
		{
			LoadNodeInfo (loadDataKey);
		}

		if (GUI.Button (saveButtonRect, "Save Data")) 
		{
			SaveNodeInfoInstance (saveDataKey);
		}

		if (GUI.Button (processTriggerButtonRect, "Process Trigger")) 
		{
			if(dialogueTrigger == null)
			{
				Debug.LogWarning ("No object to parse!");
				return;
			}

			ProcessTrigger ();
		}
	}

	private void DrawNodes()
	{
		if(nodes != null)
		{
			for (int i = 0; i < nodes.Count; i++) 
			{
				nodes [i].Draw ();
			}
		}
	}

	private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
	{
		int widthDivs = Mathf.CeilToInt (position.width / gridSpacing);
		int heightDivs = Mathf.CeilToInt (position.height / gridSpacing);

		Handles.BeginGUI ();
		Handles.color = new Color (gridColor.r, gridColor.g, gridColor.g, gridOpacity);

		offset += drag * 0.5f;
		Vector3 newOffset = new Vector3 (offset.x % gridSpacing, offset.y % gridSpacing, 0f);

		for (int i = 0; i < widthDivs; i++) 
		{
			Handles.DrawLine (new Vector3 (gridSpacing * i, -gridSpacing, 0f) + newOffset, new Vector3 (gridSpacing * i, position.height, 0f) + newOffset);
		}
		for (int i = 0; i < heightDivs; i++) 
		{
			Handles.DrawLine (new Vector3 (-gridSpacing, gridSpacing * i, 0f) + newOffset, new Vector3 (position.width, gridSpacing * i, 0f) + newOffset);
		}

		Handles.color = Color.white;
		Handles.EndGUI ();
	}

	private void DrawConnectionLine(Event e)
	{
		if(selectedInPoint != null && selectedOutPoint == null)
		{
			Handles.DrawBezier (selectedInPoint.rect.center, e.mousePosition, selectedInPoint.rect.center + Vector2.left * 50f, e.mousePosition - Vector2.left * 50f,
				Color.grey, null, 3f);

			activeConnectionLine = true;
			GUI.changed = true;
		}

		if(selectedInPoint == null && selectedOutPoint != null)
		{
			Handles.DrawBezier (selectedOutPoint.rect.center, e.mousePosition, selectedOutPoint.rect.center - Vector2.left * 50f, e.mousePosition + Vector2.left * 50f,
				Color.grey, null, 3f);
			
			activeConnectionLine = true;
			GUI.changed = true;
		}
	}

	private void ProcessContextMenu(Vector2 mousePosition)
	{
		GenericMenu gMenu = new GenericMenu ();
		gMenu.AddItem(new GUIContent("Add Dialogue Start Node"), false, () => OnClickAddNode(mousePosition, 0));
		gMenu.AddItem (new GUIContent ("Add NPC Dialogue Node (1 In 1 Out)"), false, () => OnClickAddNode (mousePosition, 1));
		gMenu.AddItem (new GUIContent ("Add Player Response Node (1 In 1 Out)"), false, () => OnClickAddNode (mousePosition, 2));
		gMenu.AddItem(new GUIContent("Add Dialogue End Node"), false, () => OnClickAddNode(mousePosition, 3));
		gMenu.ShowAsContext ();
	}

	private void ProcessEvents(Event e)
	{
		drag = Vector2.zero;

		switch(e.type)
		{
		case EventType.MouseDown:
			if (e.button == 1) 
			{
				ProcessContextMenu (e.mousePosition);
			}
			break;
		case EventType.MouseDrag:
			if (e.button == 0) 
			{
				OnDrag (e.delta);
			}
			break;
		}
	}

	private void OnDrag(Vector2 delta)
	{
		drag = delta;

		if(nodes != null)
		{
			for (int i = 0; i < nodes.Count; i++) 
			{
				nodes [i].Drag (delta);	
			}
		}

		GUI.changed = true;
	}

	private void ProcessNodeEvents(Event e)
	{
		if(nodes != null)
		{
			for (int i = nodes.Count - 1; i >= 0; i--) 
			{
				bool guiChanged = nodes [i].ProcessEvents (e);

				if(guiChanged)
				{
					GUI.changed = true;
				}
			}
		}
	}

	private void DrawConnections()
	{
		if(connections != null)
		{
			for (int i = 0; i < connections.Count; i++) 
			{
				connections [i].Draw ();
			}
		}	
	}

	private void OnClickInPoint(ConnectionPoint inPoint)
	{
		selectedInPoint = inPoint;

		if(selectedOutPoint != null)
		{
			if(selectedOutPoint.node != selectedInPoint.node)
			{
				CreateConnection ();
				ClearConnectionSelection ();
			}
			else
			{
				ClearConnectionSelection (); 
			}
		}
	}

	private void OnClickOutPoint(ConnectionPoint outPoint)
	{
		selectedOutPoint = outPoint;

		if(selectedInPoint != null)
		{
			if(selectedOutPoint.node != selectedInPoint.node)
			{
				CreateConnection ();
				ClearConnectionSelection ();
			}
			else
			{
				ClearConnectionSelection ();
			}
		}
	}

	private void OnClickRemoveConnection(Connection connection)
	{
		RemoveConnectionDependencies (connection);
		connections.Remove (connection);
	}

	private void CreateConnection()
	{
		if (selectedInPoint == null || selectedOutPoint == null)
			return;

		if (selectedInPoint.isConnected && selectedInPoint.connectedToPoints.Contains(selectedOutPoint))
			return;

		if(selectedInPoint.node.inNodesConnectedTo.Contains(selectedOutPoint.node))
		{
			for (int i = 0; i < selectedInPoint.node.outPoints.Length; i++) 
			{
				for (int j = 0; j < selectedInPoint.node.outPoints[i].connectedToPoints.Count; j++) 
				{
					if (selectedInPoint.node.outPoints[i].connectedToPoints[j] != null
						&& selectedInPoint.node.outPoints [i].connectedToPoints[j].node == selectedOutPoint.node)
						return;	
				}
			}
		}

		if (selectedInPoint.node.nodeInfo.connectionInfos == null)
			selectedInPoint.node.nodeInfo.connectionInfos = new List<Node.ConnectionInfo> ();

		if (selectedOutPoint.node.nodeInfo.connectionInfos == null)
			selectedOutPoint.node.nodeInfo.connectionInfos = new List<Node.ConnectionInfo> ();

		selectedInPoint.node.nodeInfo.connectionInfos.Add (new Node.ConnectionInfo (selectedOutPoint.node.nodeID,
			ConnectionPointType.In, Array.FindIndex(selectedInPoint.node.inPoints, item => item == selectedInPoint), 
			Array.FindIndex(selectedOutPoint.node.outPoints, item => item == selectedOutPoint)));

		selectedOutPoint.node.nodeInfo.connectionInfos.Add (new Node.ConnectionInfo (selectedInPoint.node.nodeID, 
			ConnectionPointType.Out, Array.FindIndex(selectedOutPoint.node.outPoints, item => item == selectedOutPoint),
			Array.FindIndex(selectedInPoint.node.inPoints, item => item == selectedInPoint)));

		if(connections == null)
		{
			connections = new List<Connection> ();
		}

		connections.Add (new Connection (selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
	}

	private void CreateConnection(ConnectionPoint inPoint, ConnectionPoint outPoint)
	{
		if (inPoint == null || outPoint == null)
			return;

		if(inPoint.node.inNodesConnectedTo.Contains(outPoint.node))
		{
			for (int i = 0; i < inPoint.node.outPoints.Length; i++) 
			{
				for (int j = 0; j < inPoint.node.outPoints[i].connectedToPoints.Count; j++) 
				{
					if (inPoint.node.outPoints[i].connectedToPoints[j] != null
						&& inPoint.node.outPoints [i].connectedToPoints[j].node == outPoint.node)
						return;	
				}
			}
		}

		if(connections == null)
		{
			connections = new List<Connection> ();
		}

		connections.Add (new Connection (inPoint, outPoint, OnClickRemoveConnection));
	}

	private void ClearConnectionSelection()
	{
		selectedInPoint = null;
		selectedOutPoint = null;
	}

	private void ClearConnectionSelection(ConnectionPoint inPoint, ConnectionPoint outPoint)
	{
		inPoint = null;
		outPoint = null;
	}

	private void OnClickRemoveNode(Node node)
	{
		if(connections != null)
		{
			List<Connection> connectionsToRemove = new List<Connection> ();

			for (int i = 0; i < connections.Count; i++) 
			{
				if (Array.Exists(node.inPoints, s => s == connections[i].inPoint) || Array.Exists(node.outPoints, s => s == connections[i].outPoint))
				{
					connectionsToRemove.Add (connections [i]);
				}
			}

			for (int i = 0; i < connectionsToRemove.Count; i++) 
			{
				connections.Remove (connectionsToRemove [i]);
				RemoveConnectionDependencies (connectionsToRemove [i]);
			}

			connectionsToRemove = null;
		}

		nodes.Remove (node);
	}

	private void OnClickAddNode(Vector2 mousePosition, int nodeType)
	{
		Node newNode = null;

		if(nodes == null)
		{
			nodes = new List<Node> ();
		}

		switch(nodeType)
		{
		case 0:

			bool foundStart = false;

			for (int i = 0; i < nodes.Count; i++) {
				if (nodes [i].GetType () == typeof(StartNode)) {
					foundStart = true;
					break;
				}
			}

			if (foundStart) 
			{
				Debug.LogWarning ("Only one start node is allowed!");
				break;
			}

			newNode = new StartNode (0, mousePosition, 100f, 50f, nodeStyle, selectedNodeStyle, inPointStyle, 
				outPointStyle, OnClickRemoveNode, OnClickInPoint, OnClickOutPoint, 0, 1);
			nodes.Add (newNode);
			break;
		case 1:
			newNode = new NPCDialogueNode (0, mousePosition, 200f, 200f, nodeStyle, selectedNodeStyle, inPointStyle, 
				outPointStyle, OnClickRemoveNode, OnClickInPoint, OnClickOutPoint, 1, 1);
			nodes.Add (newNode);
			break;
		case 2:
			newNode = new ResponseNode (0, mousePosition, 200f, 150f, nodeStyle, selectedNodeStyle, inPointStyle, 
				outPointStyle, OnClickRemoveNode, OnClickInPoint, OnClickOutPoint, 1, 1);
			nodes.Add (newNode);
			break;
		case 3:

			bool foundEnd = false;

			for (int i = 0; i < nodes.Count; i++) {
				if (nodes [i].GetType () == typeof(EndNode)) {
					foundEnd = true;
					break;
				}
			}

			if (foundEnd) 
			{
				Debug.LogWarning ("Only one end node is allowed!");
				break;
			}

			newNode = new EndNode (0, mousePosition, 100f, 50f, nodeStyle, selectedNodeStyle, inPointStyle, 
				outPointStyle, OnClickRemoveNode, OnClickInPoint, OnClickOutPoint, 1, 0);
			nodes.Add (newNode);
			break;
			
		}

		CreateActiveConnectionLine (newNode);
	}

	private void CreateActiveConnectionLine (Node newNode)
	{
		if (newNode == null)
			return;

		if (activeConnectionLine) {
			if (selectedInPoint != null && selectedOutPoint == null) {

				if(newNode.outPoints == null || newNode.outPoints.Length <= 0)
				{
					Debug.LogError ("Node does not have any out points to connect");
					return;
				}

				selectedOutPoint = newNode.outPoints [0];

				CreateConnection ();
				activeConnectionLine = false;
				ClearConnectionSelection ();
			}
			if (selectedOutPoint != null && selectedInPoint == null) {

				if(newNode.inPoints == null || newNode.inPoints.Length <= 0)
				{
					Debug.LogError ("Node does not have any in points to connect");
					return;
				}

				selectedInPoint = newNode.inPoints [0];

				CreateConnection ();
				activeConnectionLine = false;
				ClearConnectionSelection ();
			}
			GUI.changed = true;
		}
	}

	private void LoadSaveData()
	{
		saveData = ScriptableObjectUtility.LoadOrCreateSaveData<NodeSaveData> (saveDataPath);

		if (!File.Exists (dictionaryStorePath))
			return;

		string json = File.ReadAllText (dictionaryStorePath);

		saveData.nodeInfoDictionary = (Dictionary<string, List<Node.NodeInfo>>)StringSerializationAPI.Deserialize (typeof(Dictionary<string, 
			List<Node.NodeInfo>>), json);
	}

	private void LoadNodeInfo (string newKey)
	{
		if (newKey == string.Empty)
		{
			Debug.LogWarning ("Load Key is Empty");
			return;
		}	
		
		if(!saveData.nodeInfoDictionary.ContainsKey(newKey))
		{
			Debug.LogError ("Key " + newKey + " does not exist");
			return;
		}

		if(saveData == null)
		{
			Debug.LogError ("Save data is empty!");
			return;
		}

		CreateLoadedNodes (newKey);
	}

	private void ProcessTrigger()
	{
		ProcessNodeEditorToTrigger.ProcessNodesToTrigger (nodes, dialogueTrigger);
	}

	private void RemoveConnectionDependencies (Connection connection)
	{
		if(connection.outPoint.connectedToPoints.Count <= 0)
		{
			connection.inPoint.isConnected = false;
		}

		if(connection.inPoint.connectedToPoints.Contains(connection.outPoint))
		{
			connection.inPoint.connectedToPoints.Remove (connection.outPoint);
		}

		if(connection.outPoint.connectedToPoints.Count <= 0)
		{
			connection.outPoint.isConnected = false;
		}

		if(connection.outPoint.connectedToPoints.Contains(connection.inPoint))
		{
			connection.outPoint.connectedToPoints.Remove (connection.inPoint);
		}

		connection.inPoint.node.inNodesConnectedTo.Remove (connection.outPoint.node);
		connection.outPoint.node.outNodesConnectedTo.Remove (connection.inPoint.node);
	}

	private void SaveNodeInfoInstance(string newNodeInfoInstance)
	{
		if (newNodeInfoInstance == string.Empty)
		{
			Debug.LogWarning ("Save Key is Empty");
			return;
		}

		if(saveData != null && nodes.Count > 0)
		{
			if(saveData.nodeInfoDictionary == null)
			{
				saveData.nodeInfoDictionary = new Dictionary<string, List<Node.NodeInfo>> ();
			}

			if (!saveData.nodeInfoDictionary.ContainsKey (newNodeInfoInstance))
				saveData.nodeInfoDictionary [newNodeInfoInstance] = new List<Node.NodeInfo> ();
			else
				saveData.nodeInfoDictionary [newNodeInfoInstance].Clear ();
			
			for (int i = 0; i < nodes.Count; i++) 
			{
				saveData.nodeInfoDictionary [newNodeInfoInstance].Add(nodes [i].nodeInfo);
			}
		}

		if (saveData.nodeInfoDictionary == null || saveData.nodeInfoDictionary.Count <= 0)
			return;

		string json = StringSerializationAPI.Serialize (typeof(Dictionary<string, List<Node.NodeInfo>>), saveData.nodeInfoDictionary);

		File.WriteAllText (dictionaryStorePath, json);
	}

	private void CreateLoadedNodes(string nodeInfoInstance)
	{
		if(nodes == null)
		{
			nodes = new List<Node> ();
		}
		else
		{
			nodes.Clear ();
		}

		if(saveData.nodeInfoDictionary.ContainsKey(nodeInfoInstance))
		{
			for (int i = 0; i < saveData.nodeInfoDictionary[nodeInfoInstance].Count; i++) 
			{
				Node.NodeInfo nInfo = saveData.nodeInfoDictionary [nodeInfoInstance] [i];

				if(nInfo.nodeType == 0)
				{
					nodes.Add (new StartNode (nInfo.id, nInfo.position, 100f, 50f, nodeStyle, selectedNodeStyle, inPointStyle, 
						outPointStyle, OnClickRemoveNode, OnClickInPoint, OnClickOutPoint, 0, 1));
				}
				else if(nInfo.nodeType == 1)
				{
					string name = nInfo.textElements [nInfo.textElements.Count - 1];
					nInfo.textElements.RemoveAt (nInfo.textElements.Count - 1);

					nodes.Add (new NPCDialogueNode (nInfo.id, nInfo.position, nInfo.width, nInfo.height, nodeStyle, selectedNodeStyle, inPointStyle, 
						outPointStyle, OnClickRemoveNode, OnClickInPoint, OnClickOutPoint, nInfo.totalInPoints, nInfo.totalOutPoints, nInfo.textElements, name));
				}
				else if(nInfo.nodeType == 2)
				{
					nodes.Add (new ResponseNode (nInfo.id, nInfo.position, nInfo.width, nInfo.height, nodeStyle, selectedNodeStyle, inPointStyle, 
						outPointStyle, OnClickRemoveNode, OnClickInPoint, OnClickOutPoint, nInfo.totalInPoints, nInfo.totalOutPoints, nInfo.textElements[0]));
				}
				else if(nInfo.nodeType == 3)
				{
					nodes.Add (new EndNode (nInfo.id, nInfo.position, 100f, 50f, nodeStyle, selectedNodeStyle, inPointStyle, 
						outPointStyle, OnClickRemoveNode, OnClickInPoint, OnClickOutPoint, 1, 0));
				}
			}

			CreateLoadedNodeConnections (nodeInfoInstance);
		}
	}

	private Node GetNode(int nodeID)
	{
		if (nodes == null || nodes.Count <= 0)
			return null;

		for (int i = 0; i < nodes.Count; i++) 
		{
			if (nodes [i].nodeID == nodeID)
			{
				return nodes [i];
			}
		}

		return null;
	}

	private void CreateLoadedNodeConnections(string nodeInfoInstance)
	{
		if(connections == null)
		{
			connections = new List<Connection> ();
		}
		else
		{
			connections.Clear ();
		}

		if (nodes == null)
			return;

		if(saveData.nodeInfoDictionary.ContainsKey(nodeInfoInstance))
		{
			for (int i = 0; i < saveData.nodeInfoDictionary[nodeInfoInstance].Count; i++) 
			{
				Node.NodeInfo nInfo = saveData.nodeInfoDictionary [nodeInfoInstance] [i];

				if(nInfo.connectionInfos != null)
				{
					for (int j = 0; j < nInfo.connectionInfos.Count; j++) 
					{
						Node.ConnectionInfo cInfo = nInfo.connectionInfos [j];

						Node node = GetNode (nInfo.id);
						Node connectedNode = GetNode (cInfo.nodeConnectedToID);

						#region ConnectionChecks
						if (node.GetType () == typeof(StartNode)) {
							if (AreConnectionPointsConnected (connectedNode.inPoints [cInfo.pointConnectedIndex], node.outPoints [cInfo.pointIndex]))
								continue;
						} else if (connectedNode.GetType () == typeof(StartNode)) {
							if (AreConnectionPointsConnected (node.inPoints [cInfo.pointIndex], connectedNode.outPoints [cInfo.pointConnectedIndex]))
								continue;
						} else if (node.GetType () == typeof(EndNode)) {
							if (AreConnectionPointsConnected (node.inPoints [cInfo.pointIndex], connectedNode.outPoints [cInfo.pointConnectedIndex]))
								continue;
						} else if (connectedNode.GetType () == typeof(EndNode)) {
							if (AreConnectionPointsConnected (connectedNode.inPoints [cInfo.pointConnectedIndex], node.outPoints [cInfo.pointIndex]))
								continue;
						} else {
							if (AreConnectionPointsConnected (node.inPoints [cInfo.pointIndex], connectedNode.outPoints [cInfo.pointConnectedIndex]))
								continue;	
						}
						#endregion

						if(nInfo.connectionInfos[j].type == ConnectionPointType.In)
						{
							CreateConnection (node.inPoints [cInfo.pointIndex], connectedNode.outPoints [cInfo.pointConnectedIndex]);
						}
						else if (nInfo.connectionInfos[j].type == ConnectionPointType.Out)
						{
							CreateConnection (connectedNode.inPoints [cInfo.pointConnectedIndex], node.outPoints [cInfo.pointIndex]);
						}
					}
				}
			}
		}
	}

	private bool AreConnectionPointsConnected(ConnectionPoint first, ConnectionPoint second)
	{
		if (first.connectedToPoints.Contains (second))
			return true;
		else if (second.connectedToPoints.Contains (first))
			return true;

		return false;
	}

	private void OnDestroy()
	{
		
	}
}
