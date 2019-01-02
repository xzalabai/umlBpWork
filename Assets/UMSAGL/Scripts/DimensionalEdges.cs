using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionalEdges : MonoBehaviour {

	struct dimensionalAssociation
	{
		public LineRenderer line;
		public GameObject from;
		public GameObject to;
	};
	List<dimensionalAssociation> allDimensionalAs = new List<dimensionalAssociation>();
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		foreach (var dimAs in allDimensionalAs)
			updateDimensionalAssociation(dimAs.line, dimAs.from, dimAs.to);
	}

	public void deleteDimensionalAssociation(LineRenderer line)
	{
		dimensionalAssociation structure = allDimensionalAs.Find(x => x.line == line);
		Debug.Log(structure.from.name);
	}

	public void updateDimensionalAssociation(LineRenderer line, GameObject from, GameObject to)
	{
		var points = new Vector3[2];
		Vector3 fromPoint = from.transform.position;
		Vector3 toPoint = to.transform.position;
		fromPoint.y -= 20;
		toPoint.y -= 20;
		points[0] = fromPoint;
		points[1] = toPoint;
		line.SetPositions(points);
	}


	public void createDimensionalAssociation(GameObject from, GameObject to)
	{
		GameObject obj = new GameObject("3dLine");
		obj.tag = "dimensionalLine";
		LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();

		dimensionalAssociation newAssociation;
		//LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();

		//create line between 2 classes
		var points = new Vector3[2];
		points[0] = from.transform.position;
		points[1] = to.transform.position;
		lineRenderer.SetPositions(points);

		//create struct (later added to list of structures with connections
		newAssociation.line = lineRenderer;
		newAssociation.from = from;
		newAssociation.to = to;

		//add connection to the list of 3d connections
		allDimensionalAs.Add(newAssociation);
	}
}
