using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class NodeSaveData : ScriptableObject
{
	public Dictionary<string, List<Node.NodeInfo>> nodeInfoDictionary;

	private void OnEnable()
	{
		if(nodeInfoDictionary == null)
		{
			nodeInfoDictionary = new Dictionary<string, List<Node.NodeInfo>> ();
		}
	}
}

