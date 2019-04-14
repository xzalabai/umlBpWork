using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using System;

public class ClassManager : MonoBehaviour {

	private List<GameObject> allClasses = new List<GameObject>();
	private List<GameObject> allcylinders = new List<GameObject>();
	private List<dimensionalAssociation> allDimensionalAs = new List<dimensionalAssociation>();

	public GameObject cylinder;
	public bool isHidden = true;

	struct dimensionalAssociation
	{
		public LineRenderer line;
		public GameObject from;
		public GameObject to;
	};

	public float metricPositionX;
	public float metricPositionY;
	public float metricScaleRadius;
	public float metricScaleDelta;
	
	

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

	}

	
	//this function allow to show metric (later with algorhitm), or when the metrics are shown, hide them
	public void ShowMetric()
	{

		if (metricPositionX == 0)
			metricPositionX = 85;

		if (metricPositionY == 0)
			metricPositionY = 50;

		if (metricScaleRadius == 0)
			metricScaleRadius = 10;

		if (metricScaleDelta == 0)
			metricScaleDelta = 10;
		if (isHidden)
		{
			
			GameObject[] classes;
			classes = GameObject.FindGameObjectsWithTag("class");

			foreach (GameObject c in classes)
			{
				float widthOfCylinder = Int32.Parse(c.name) * metricScaleDelta;

				//create cylinder, set size
				var cylinder = GameObject.Instantiate(this.cylinder);
				cylinder.transform.parent = c.transform;
				cylinder.GetComponent<Renderer>().material.SetColor("_Color", UnityEngine.Random.ColorHSV());	
				cylinder.transform.localScale = new Vector3( metricScaleRadius, widthOfCylinder , metricScaleRadius);
				cylinder.transform.localPosition = new Vector3(metricPositionX, metricPositionY, widthOfCylinder * (-1));
				
				allcylinders.Add(cylinder);
			}
			isHidden = false;
		}

		else
		{
			foreach (GameObject c in allcylinders)
			{
				Destroy(c);
			}
			isHidden = true;
		}
	}
		//update dimensional associations
		public void updateDimensionalAssociation(LineRenderer line, GameObject from, GameObject to)
	{
		var points = new Vector3[2];
		points[0] = from.transform.position;
		points[1] = to.transform.position;
		line.SetPositions(points);
	}

//create association between 2 dimensions
public void createDimensionalAssociation(GameObject from, GameObject to)	
	{
		GameObject obj = new GameObject("line");
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

	//this creates a new Associations between 2 classes
	public void createAssociation(GameObject from, GameObject to, Graph gr) 
	{
		gr.AddEdge(from, to);
	}
	
	//this creates a new Class
	public GameObject createObject(float pointX, float pointY, Graph g)
	{
		var node = g.AddNode();
		GameObject n=(GameObject)node;
		n.transform.position = new Vector3(pointX, pointY, g.transform.position.z-2.0f);
		n.tag = "class";

		allClasses.Add(n);

		return n;
	}
}
