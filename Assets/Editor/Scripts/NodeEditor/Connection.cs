using System;
using UnityEditor;
using UnityEngine;

public class Connection
{
	public ConnectionPoint inPoint;
	public ConnectionPoint outPoint;

	public Action<Connection> OnClickRemoveConnection;

	public Connection(ConnectionPoint inPoint, ConnectionPoint outPoint, Action<Connection> OnClickRemoveConnection)
	{
		this.inPoint = inPoint;
		this.outPoint = outPoint;
		this.OnClickRemoveConnection = OnClickRemoveConnection;

		this.inPoint.isConnected = true;
		this.inPoint.connectedToPoints.Add (outPoint);

		this.outPoint.isConnected = true;
		this.outPoint.connectedToPoints.Add(inPoint);

		this.inPoint.node.inNodesConnectedTo.Add (this.outPoint.node);
		this.outPoint.node.outNodesConnectedTo.Add (this.inPoint.node);

		SetTriggerPropertiesFromNodes (inPoint, outPoint);
	}

	private void SetTriggerPropertiesFromNodes (ConnectionPoint inPoint, ConnectionPoint outPoint)
	{
		if (outPoint.node.GetType () == typeof(NPCDialogueNode) && inPoint.node.GetType () == typeof(ResponseNode)) 
		{
			ResponseNode responseNode = (ResponseNode)inPoint.node;
			NPCDialogueNode npcNode = (NPCDialogueNode)outPoint.node;

			if(responseNode.inNodesConnectedTo.Count > 1)
			{
				if(responseNode.parentDialogueNode != null)
				{
					responseNode.parentDialogueNode = null;
				}
			}

			responseNode.parentDialogueNode = npcNode;
		}
		else if (outPoint.node.GetType () == typeof(ResponseNode) && inPoint.node.GetType () == typeof(NPCDialogueNode)) 
		{
			ResponseNode responseNode = (ResponseNode)outPoint.node;
			NPCDialogueNode npcNode = (NPCDialogueNode)inPoint.node;

			if(npcNode.inNodesConnectedTo.Count > 1)
			{
				if(npcNode.responseNodeFollowedNode != null)
				{
					npcNode.responseNodeFollowedNode = null;
				}
				return;
			}

			npcNode.responseNodeFollowedNode = responseNode;
		}
		else if (outPoint.node.GetType () == typeof(NPCDialogueNode) && inPoint.node.GetType () == typeof(NPCDialogueNode)) 
		{
			NPCDialogueNode outNode = (NPCDialogueNode)outPoint.node;

			if (outNode.responseNodeFollowedNode == null)
				return;

			NPCDialogueNode inNode = (NPCDialogueNode)inPoint.node;

			if (inNode.inNodesConnectedTo.Count > 1)
			{
				if (inNode.responseNodeFollowedNode != null)
					inNode.responseNodeFollowedNode = null;

				return;
			}
			
			inNode.responseNodeFollowedNode = outNode.responseNodeFollowedNode;
		}
	}

	public void Draw()
	{
		Handles.DrawBezier (inPoint.rect.center, outPoint.rect.center, inPoint.rect.center + Vector2.left * 50f, 
			outPoint.rect.center - Vector2.left * 50f, Color.black, null, 3f);

		if(Handles.Button((inPoint.rect.center + outPoint.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
		{
			if(OnClickRemoveConnection != null)
			{
				OnClickRemoveConnection (this);
			}
		}
	}
}

