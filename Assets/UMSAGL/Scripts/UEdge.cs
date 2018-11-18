using Microsoft.Msagl.Core.Layout;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UEdge : Unit
{
	public Edge graphEdge { get; set; }

	public float Width {
		get
		{
			var lr = GetComponent<LineRenderer>();
			return Mathf.Max(lr.startWidth, lr.endWidth);
		}
	}

	protected override void OnDestroy()
	{
		graph.RemoveEdge(gameObject);
	}
}
