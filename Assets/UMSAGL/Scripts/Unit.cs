﻿using Microsoft.Msagl.Core.Layout;
using UnityEngine;

public abstract class Unit : MonoBehaviour {

	protected Graph graph;

	// Use this for initialization
	protected virtual void Awake () {
		graph = GetComponentInParent<Graph>();
	}

	protected abstract void OnDestroy();
}
