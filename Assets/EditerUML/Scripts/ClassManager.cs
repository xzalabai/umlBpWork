using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using System;

public class ClassManager : MonoBehaviour {

	public GameObject prefab;
	public GameObject table1;
	public GameObject table2;
	public Graph graph1;
	public Graph graph2;
	GameObject test1, test2, test3, test4, test5;
	private List<Vector3> sample = new List<Vector3>();
	private List<GameObject> allClasses = new List<GameObject>();
	[SerializeField]
	private float maxAngle = 70f;

	struct dimensionalAssociation{
		public LineRenderer line;
		public GameObject from;
		public GameObject to;
	};
	
	List<dimensionalAssociation> allDimensionalAs = new List<dimensionalAssociation>();

	// Use this for initialization
	void Start () {
		var go = GameObject.Instantiate(prefab);
		var go2 = GameObject.Instantiate(prefab);
		graph1 = go.GetComponent<Graph>();
		graph2 = go2.GetComponent<Graph>();

		graph1.transform.position = new Vector3(0.0f, 0.0f, 120.0f);
		graph2.transform.position = new Vector3(0.0f, 0.0f, 430.0f);

		table1 = GameObject.Find("1");
		graph1.transform.parent = table1.transform;
		graph1.transform.position = new Vector3(table1.transform.position.x, table1.transform.position.y, table1.transform.position.z - 10f);
		

		table2 = GameObject.Find("2");
		graph2.transform.parent = table2.transform;
		graph2.transform.position = new Vector3(table2.transform.position.x, table2.transform.position.y, table2.transform.position.z - 10f);
		//GameObject a = createObject(50,50, graph1);
		//GameObject b = createObject(120, 120, graph1);
	}
	
	// Update is called once per frame
	void Update () {

	}


	public GameObject cylinder;
	List<GameObject> allcylinders = new List<GameObject>();
	public bool isHidden = true;

	//this function allow to show metric (later with algorhitm), or when the metrics are shown, hide them
	public void ShowMetric()
	{
		if (isHidden)
		{
			GameObject[] classes;
			classes = GameObject.FindGameObjectsWithTag("class");

			foreach (GameObject c in classes)
			{
				string[] nameOfClass = c.name.Split(' ');
				float sizeOfCylinder = Int32.Parse(nameOfClass[1]);
				
				var cy = GameObject.Instantiate(cylinder);
				cy.GetComponent<Renderer>().material.SetColor("_Color", UnityEngine.Random.ColorHSV());
			
				cy.transform.position = new Vector3(c.transform.position.x + 40f, c.transform.position.y + 20f, c.transform.position.z - sizeOfCylinder);

				Vector3 scale = cy.transform.localScale;
				scale.y = sizeOfCylinder;
				cy.transform.localScale = scale;

				cy.transform.parent = c.transform;
				allcylinders.Add(cy);
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

		//return gr.AddEdge(from, to);
	}
	
	//this creates a new Class
	public GameObject createObject(float pointX, float pointY, Graph g)
	{
		var node = g.AddNode();
		GameObject n=(GameObject)node;
		//set a position of diagram

		n.transform.position = new Vector3(pointX, pointY, g.transform.position.z-2.0f);
		n.tag = "class";

		allClasses.Add(n);

		return n;
	}

	public Vector3 getRightTopCorner(List<Vector3> points)
	{

		float maxDistance = 0;
		Vector3 rightTopCorner = Vector3.zero;
		foreach (Vector3 point in points)
		{
			if (Vector3.Distance(Vector3.zero, new Vector3(point.x + 1000, point.y + 1000, point.z)) > maxDistance)
			{
				rightTopCorner = point;
				maxDistance = Vector3.Distance(Vector3.zero, new Vector3(point.x + 1000, point.y + 1000, point.z));
			}
		}
		return rightTopCorner;
	}

	public Vector3 getLeftTopCorner(List<Vector3> points)
	{
		return getCorner(points, getRightTopCorner(points), Vector3.up);
	}
	public Vector3 getLeftBottomCorner(List<Vector3> points)
	{
		return getCorner(points, getLeftTopCorner(points), Vector3.left);
	}
	public Vector3 getRightBottomCorner(List<Vector3> points)
	{
		return getCorner(points, getRightTopCorner(points), Vector3.right);
	}

	public Vector3 getCorner(List<Vector3> points, Vector3 cornerPoint, Vector3 type)
	{
		float max = 0f;
		Vector3 maxPoint = Vector3.zero;
		foreach (Vector3 point in points)
		{
			float actValue = Vector3.Distance(cornerPoint, point) * Vector3.Angle(type, (cornerPoint - point));
			if (actValue > max)
			{
				max = actValue;
				maxPoint = point;
			}
		}
		return maxPoint;
	}

	public List<Vector3> getAllCorners(List<Vector3> points)
	{
		Vector3 rightTop = getRightTopCorner(points);
		Vector3 rightBottom = getRightBottomCorner(points);
		Vector3 leftTop = getLeftTopCorner(points);
		Vector3 leftBottom = getLeftBottomCorner(points);

		leftTop.y = rightTop.y = (rightTop.y + leftTop.y) / 2;

		rightBottom.y = leftBottom.y = (leftBottom.y + rightBottom.y) / 2;

		rightBottom.x = rightTop.x = (rightTop.x + rightBottom.x) / 2;
		leftBottom.x = leftTop.x = (leftTop.x + leftBottom.x) / 2;

		List<Vector3> corners = new List<Vector3>();
		corners.Add(rightTop);
		corners.Add(rightBottom);
		corners.Add(leftBottom);
		corners.Add(leftTop);

		return corners;
	}
}
