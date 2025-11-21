using System;
using System.Collections.Generic;
using UnityEngine;

public enum ConnectionPointType{In, Out}

public class ConnectionPoint
{
	public Rect rect;

	public int connectionPointIndex;
	public int totalConnectionPoints;

	public ConnectionPointType type;
	public Node node;

	public bool isConnected;
	public List<ConnectionPoint> connectedToPoints;

	public GUIStyle style;
	public Action<ConnectionPoint> OnClickConnectionPoint;

	public ConnectionPoint (Node node, ConnectionPointType type, GUIStyle style, Action<ConnectionPoint> OnClickConnectionPoint, int connectionPointIndex, int totalConnectionPoints)
	{
		rect = new Rect (0, 0, 10f, 20f);

		this.node = node;
		this.type = type;
		this.style = style;
		this.OnClickConnectionPoint = OnClickConnectionPoint;
		this.connectionPointIndex = connectionPointIndex;
		this.totalConnectionPoints = totalConnectionPoints;
		this.connectedToPoints = new List<ConnectionPoint> ();
	}

	public void Draw()
	{
		rect.y = node.rect.y + ((node.rect.height / (totalConnectionPoints + 1)) * connectionPointIndex) - rect.height * 0.5f;

		switch(type)
		{
		case ConnectionPointType.In:
			rect.x = node.rect.x - rect.width + 3f;
			break;
		case ConnectionPointType.Out:
			rect.x = node.rect.x + node.rect.width - 3f;
			break;
		}

		if(GUI.Button(rect, "", style))
		{
			if(OnClickConnectionPoint != null)
			{
				OnClickConnectionPoint (this);
			}
		}
	}
}

