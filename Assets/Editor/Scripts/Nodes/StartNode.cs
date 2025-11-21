using UnityEngine;
using System;

public class StartNode : Node
{
	public StartNode(int nodeID, Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, 
		Action<Node> OnClickRemoveNode,Action<ConnectionPoint> OnClickInPoint,Action<ConnectionPoint> OnClickOutPoint, int totalInPoints = 1, int totalOutPoints = 1) : 
	base(nodeID, position, width, height, nodeStyle, selectedStyle, inPointStyle, outPointStyle, 
		OnClickRemoveNode, OnClickInPoint, OnClickOutPoint, totalInPoints, totalOutPoints)
	{
		if(nodeID == 0)
		{
			nodeID = UnityEngine.Random.Range (1, 100000000);
		}

		this.nodeID = nodeID;
		this.nodeInfo = new NodeInfo (nodeID, position, width, height, 0, totalInPoints, totalOutPoints);
	}
}

