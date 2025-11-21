using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Node {

	[System.Serializable]
	public class NodeInfo
	{
		public int id;
		public Vector2 position;
		public float width, height;
		public int nodeType;
		public List<ConnectionInfo> connectionInfos;
		public List<string> textElements;
		public int totalInPoints, totalOutPoints;

		public NodeInfo(int id, Vector2 pos, float width, float height, int type, int textElementLength, int inPoints, int outPoints)
		{
			this.id = id;
			this.position = pos;
			this.width = width;
			this.height = height;
			this.nodeType = type;
			this.totalInPoints = inPoints;
			this.totalOutPoints = outPoints;

			this.textElements = new List<string>();

			for (int i = 0; i < textElementLength; i++) 
			{
				this.textElements.Add("");
			}
		}

		public NodeInfo(int id, Vector2 pos, float width, float height, int type, int inPoints, int outPoints)
		{
			this.id = id;
			this.position = pos;
			this.width = width;
			this.height = height;
			this.nodeType = type;
			this.totalInPoints = inPoints;
			this.totalOutPoints = outPoints;
		}
	}

	[System.Serializable]
	public class ConnectionInfo
	{
		public int nodeConnectedToID;
		public ConnectionPointType type;
		public int pointIndex;
		public int pointConnectedIndex;

		public ConnectionInfo (int id, ConnectionPointType type, int index, int connectedNodeIndex)
		{
			this.nodeConnectedToID = id;
			this.type = type;
			this.pointIndex = index;
			this.pointConnectedIndex = connectedNodeIndex;
		}
	}

	public NodeInfo nodeInfo;
	public int nodeID;

	public Rect rect;
	public string title;
	public bool isDragged;
	public bool isSelected;

	public GUIStyle style;
	public GUIStyle defaultNodeStyle;
	public GUIStyle selectedNodeStyle;

	public Action<Node> OnRemoveNode;

	public List<Node> inNodesConnectedTo, outNodesConnectedTo;

	public ConnectionPoint[] inPoints;
	public ConnectionPoint[] outPoints;

	public Node (int nodeID, Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, 
		Action<Node> OnClickRemoveNode,Action<ConnectionPoint> OnClickInPoint,Action<ConnectionPoint> OnClickOutPoint, int totalInPoints = 1, int totalOutPoints = 1)
	{
		rect = new Rect (position.x, position.y, width, height);
		style = nodeStyle;
		defaultNodeStyle = nodeStyle;
		selectedNodeStyle = selectedStyle;

		if(totalInPoints > 0)
			inNodesConnectedTo = new List<Node> ();

		if(totalOutPoints > 0)
			outNodesConnectedTo = new List<Node> ();

		inPoints = new ConnectionPoint[totalInPoints];
		outPoints = new ConnectionPoint[totalOutPoints];
		OnRemoveNode = OnClickRemoveNode;

		for (int i = 0; i < inPoints.Length; i++) 
		{
			inPoints [i] = new ConnectionPoint (this, ConnectionPointType.In, inPointStyle, OnClickInPoint, i + 1, inPoints.Length);
		}

		for (int i = 0; i < outPoints.Length; i++) 
		{
			outPoints [i] = new ConnectionPoint (this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint, i + 1, outPoints.Length);
		}
	}

	public virtual void Drag(Vector2 delta)
	{
		rect.position += delta;
		nodeInfo.position = rect.position;
	}

	public virtual void Draw()
	{
		for (int i = 0; i < inPoints.Length; i++) 
		{
			inPoints [i].Draw ();
		}

		for (int i = 0; i < outPoints.Length; i++) 
		{
			outPoints [i].Draw ();
		}

		GUI.Box (rect, title, style);
	}

	private void OnClickRemoveNode()
	{
		if(OnRemoveNode != null)
		{
			OnRemoveNode (this);
		}
	}

	private void ProcessContextMenu()
	{
		GenericMenu gMenu = new GenericMenu ();
		gMenu.AddItem (new GUIContent ("Remove Node"), false, OnClickRemoveNode);
		gMenu.ShowAsContext ();
	}

	public bool ProcessEvents(Event e)
	{
		switch(e.type)
		{
		case EventType.MouseDown:
			if (e.button == 0) {
				if (rect.Contains (e.mousePosition)) 
				{
					isDragged = true;
					isSelected = true;
					GUI.changed = true;
					style = selectedNodeStyle;
				} 
				else 
				{
					GUI.changed = true;
					isSelected = false;
					style = defaultNodeStyle;
				}
			}
			if(e.button == 1 && isSelected && rect.Contains(e.mousePosition))
			{
				ProcessContextMenu ();
				e.Use ();
			}
			break;
		case EventType.MouseUp:
			isDragged = false;
			break;
		case EventType.MouseDrag:
			if (e.button == 0 && isDragged) 
			{
				Drag (e.delta);
				e.Use ();
				return true;
			}
			break;
		}

		return false;
	}
}
