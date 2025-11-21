using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NPCDialogueNode : Node
{
	public Rect textRectSpeaker;
	public Rect textRectDialogue;
	public Rect monologueSerialRect;

	public ResponseNode responseNodeFollowedNode;

	public GUIStyle monologueRectStyle;
	public GUIStyle textRectStyle;
	public GUIStyle textRectSpeakerStyle;

	public int totalAllotedMonologues = 3;
	public float speakerAreaWidth;

	public float heightOffset;
	public float textAreaWidth;
	public float textAreaHeight;

	public string speakerNameText;
	public List<string> dialogueTexts;

	private bool showMonologueMenu;
	private int instanceNo = 0;

	public NPCDialogueNode (int nodeID, Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, 
		Action<Node> OnClickRemoveNode,Action<ConnectionPoint> OnClickInPoint,
		Action<ConnectionPoint> OnClickOutPoint, int totalInPoints = 1, int totalOutPoints = 1, List<string> dialogueTexts = null,  string speakerName = "Speaker Name") :
	base (nodeID, position, width, height, nodeStyle, selectedStyle, inPointStyle, outPointStyle, OnClickRemoveNode, OnClickInPoint, OnClickOutPoint, totalInPoints, totalOutPoints)
	{
		if(nodeID == 0)
		{
			nodeID = UnityEngine.Random.Range (1, 100000000);
		}

		this.nodeID = nodeID;
		this.nodeInfo = new NodeInfo (nodeID, position, width, height, 1, totalAllotedMonologues + 1, totalInPoints, totalOutPoints);

		this.textAreaWidth = width - 20f;
		this.textAreaHeight = height - 20f;

		this.speakerAreaWidth = width - 20f;

		this.dialogueTexts = dialogueTexts;

		if(this.dialogueTexts == null)
		{
			this.dialogueTexts = new List<string> ();
			for (int i = 0; i < totalAllotedMonologues; i++) 
			{
				this.dialogueTexts.Add ("Dialogue Text");
			}
		}

		this.speakerNameText = speakerName;

		this.heightOffset = 60f;

		textRectDialogue = new Rect (position.x + rect.width / 2f - textAreaWidth / 2f, position.y + rect.height / 2f - textAreaHeight / 2f + heightOffset, textAreaWidth, textAreaHeight - heightOffset);
		textRectSpeaker = new Rect (position.x + rect.width / 2f - speakerAreaWidth / 2f, position.y + 45f, speakerAreaWidth, 20f);
		monologueSerialRect = new Rect (position.x + 5f, position.y + 5f, 20f, 15f);
		SetRectStyles ();
	}

	private void SetRectStyles ()
	{
		textRectStyle = new GUIStyle ();
		textRectStyle.normal.background = Resources.Load<Texture2D> ("NodeTexture_TextRect");
		textRectStyle.border = new RectOffset (12, 12, 12, 12);
		textRectStyle.normal.textColor = Color.white;
		textRectStyle.active.textColor = Color.white;
		textRectStyle.padding = new RectOffset (5, 5, 5, 5);
		textRectSpeakerStyle = new GUIStyle ();
		textRectStyle.wordWrap = true;

		textRectSpeakerStyle = new GUIStyle ();
		textRectSpeakerStyle.normal.background = Resources.Load<Texture2D> ("NodeTexture_TextRect");
		textRectSpeakerStyle.border = new RectOffset (12, 12, 12, 12);
		textRectSpeakerStyle.normal.textColor = Color.white;
		textRectSpeakerStyle.active.textColor = Color.white;
		textRectSpeakerStyle.padding = new RectOffset (5, 0, 3, 0);
		textRectSpeakerStyle.wordWrap = true;

		monologueRectStyle = new GUIStyle ();
		monologueRectStyle.normal.background = Resources.Load<Texture2D> ("NodeTexture_TextRect");
		monologueRectStyle.active.background = Resources.Load<Texture2D> ("NodeTexture_ButtonRect_Selected");
		monologueRectStyle.border = new RectOffset (12, 12, 12, 12);
		monologueRectStyle.normal.textColor = Color.white;
		monologueRectStyle.active.textColor = Color.black;
		monologueRectStyle.alignment = TextAnchor.MiddleCenter;
	}

	public override void Drag (Vector2 delta)
	{
		base.Drag (delta);

		textRectDialogue.position += delta;
		textRectSpeaker.position += delta;
		monologueSerialRect.position += delta;
	}

	private void CreateDropdownMenu(Rect parentRect)
	{
		GenericMenu menu = new GenericMenu ();

		menu.AddItem(new GUIContent("1"), false, () => SetInstanceNo(0));
		menu.AddItem(new GUIContent("2"), false, () => SetInstanceNo(1));
		menu.AddItem(new GUIContent("3"), false, () => SetInstanceNo(2));

		menu.DropDown (new Rect (parentRect.position, new Vector2 (10f, 12f)));
	}

	private void SetInstanceNo(int s)
	{
		instanceNo = s;
	}

	public bool CheckIfAllInvalidSentences()
	{
		int c = 0;

		for (int i = 0; i < dialogueTexts.Count; i++) 
		{
			if(dialogueTexts[i] == "Dialogue Text")
			{
				c++;
			}
		}

		if (c == dialogueTexts.Count)
			return true;

		return false;
	}

	public override void Draw ()
	{
		base.Draw ();

		showMonologueMenu = GUI.Button(monologueSerialRect, (instanceNo + 1).ToString(), monologueRectStyle);

		if(showMonologueMenu)
		{
			CreateDropdownMenu (monologueSerialRect);
		}

		dialogueTexts[instanceNo] = GUI.TextArea (textRectDialogue, dialogueTexts[instanceNo], textRectStyle);
		speakerNameText = GUI.TextArea (textRectSpeaker, speakerNameText, textRectSpeakerStyle);

		this.nodeInfo.textElements [this.nodeInfo.textElements.Count - 1] = speakerNameText;
		this.nodeInfo.textElements [instanceNo] = dialogueTexts[instanceNo];
	}
}

