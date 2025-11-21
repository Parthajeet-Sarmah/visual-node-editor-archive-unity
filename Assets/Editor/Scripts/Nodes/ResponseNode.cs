using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ResponseNode : Node
{
	public Rect textRect;

	public string playerResponse;

	public NPCDialogueNode parentDialogueNode;

	public float textAreaWidth;
	public float textAreaHeight;

	public GUIStyle playerResponseRectStyle;
	public GUIStyle textRectStyle;

	public float heightOffset;

	public ResponseNode(int nodeID, Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, 
		Action<Node> OnClickRemoveNode,Action<ConnectionPoint> OnClickInPoint, 
		Action<ConnectionPoint> OnClickOutPoint, int totalInPoints = 1, int totalOutPoints = 1, string dialogueText = "Player Response") :
	base(nodeID, position, width, height, nodeStyle, selectedStyle, inPointStyle, outPointStyle, 
		OnClickRemoveNode, OnClickInPoint, OnClickOutPoint, totalInPoints, totalOutPoints)
	{
		if(nodeID == 0)
		{
			nodeID = UnityEngine.Random.Range (1, 100000000);
		}

		this.nodeID = nodeID;
		this.nodeInfo = new NodeInfo (nodeID, position, width, height, 2, 1, totalInPoints, totalOutPoints);

		this.playerResponse = dialogueText;

		this.textAreaWidth = width - 20f;
		this.textAreaHeight = height - 20f;

		this.heightOffset = 40f;

		textRect = new Rect (position.x + rect.width / 2f - textAreaWidth / 2f, position.y + rect.height / 2f - textAreaHeight / 2f + heightOffset, textAreaWidth, textAreaHeight - heightOffset);

		SetRectStyles ();
	}

	private void SetRectStyles ()
	{
		textRectStyle = new GUIStyle ();
		textRectStyle.normal.background = Resources.Load<Texture2D> ("NodeTexture_TextRect");
		textRectStyle.border = new RectOffset (12, 12, 12, 12);
		textRectStyle.normal.textColor = Color.white;
		textRectStyle.active.textColor = Color.white;
		textRectStyle.padding = new RectOffset (7, 0, 7, 0);
		textRectStyle.wordWrap = true;
	}
		
	public override void Drag (Vector2 delta)
	{
		base.Drag (delta);
		textRect.position += delta;
	}

	public override void Draw ()
	{
		base.Draw ();

		playerResponse = GUI.TextArea (textRect, playerResponse, textRectStyle);
		this.nodeInfo.textElements[0] = playerResponse;
	}
}

