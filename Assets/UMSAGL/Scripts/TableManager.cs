using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;


/*
 
	 
	TODO: FIX SPAWN CLASSOV DALEKO OD TABULE
	TODO: DELETE TABLE
	TODO: CREATE ALGORHITM ON METRICS
	TODO: TEXT IN THE CLASS
	TODO: SOLVE MISCLICKS (when creating, assigning ..)
	TODO: MOVE GRAPH INSTANTIATE FROM CLASSMANAGER TO TABLEMANAGER


*/

public class TableManager : MonoBehaviour {
	public Camera cam;
	public GameObject prefab;
	public GameObject graphPrefab;
	public UnityEngine.UI.Slider tableNumber;
	//Resources.Load("TableMaterialOpaque.mat", typeof(Material)) as Material;
	public Table table1;
	public List<Table> allTables = new List<Table>();
	public Table table2;
	public static int lastTableId;
	public static int lastTablePosition;

	public Material opaque;
	public Material transparent;

	// Use this for initialization
	void Start () {

		var go1 = GameObject.Instantiate(prefab);
		var go2 = GameObject.Instantiate(prefab);

		table1 = go1.GetComponent<Table>();
		table2 = go2.GetComponent<Table>();

		table1.transform.localScale = new Vector3(400, 300, 1);
		table1.transform.position = new Vector3(0, 0, 120);

		table2.transform.localScale = new Vector3(400, 300, 1);
		table2.transform.position = new Vector3(0, 0, 430);

		table1.GetComponent<MeshRenderer>().material.renderQueue = 2999;
		table2.GetComponent<MeshRenderer>().material.renderQueue = 2999;
		table1.name = "table1";
		table2.name = "table2";

		table1.tag = "table";
		table2.tag = "table";
 

		lastTableId = 2;
		lastTablePosition = 430;

		allTables.Add(table1);
		allTables.Add(table2);
	}
	
	// Update is called once per frame
	void Update () {

	}

	// show metrics for all tables 
	/*public void ShowMetrics()
	{
		foreach (Table t in allTables)
		{
			t.GetComponent<Table>().CreateMetric();
		}
	}*/

	public void CreateNewTable()
	{
		int distanceBetweenTables = 310;
		//TODO: pri vytvarani dalsich tabul sa classy zacnu spawnovat inde ako maju
		//creating new tablw with graph
		var go = Instantiate(prefab);
		Table table = go.GetComponent<Table>();
		table.transform.localScale = new Vector3(400,300,1);
		table.transform.position = new Vector3(0, 0, lastTablePosition + distanceBetweenTables);
		table.GetComponent<MeshRenderer>().material.renderQueue = 2999;
		lastTableId++;
		table.tag = "table";
		table.name = "table" + (lastTableId);
		table.uniqueId = lastTableId;
		lastTablePosition = lastTablePosition + distanceBetweenTables;

		var graph = GameObject.Instantiate(graphPrefab);
		graph.transform.position = table.transform.position;
		graph.transform.parent = table.transform;

		//rise the max value in slider (one new table)
		tableNumber.maxValue++;

		allTables.Add(table);

		//todo: lastTableId if work good
	}
	public void changeTransparencyTable1(float n)
	{
		GameObject obj = GameObject.Find("table1");
		Color c = obj.GetComponent<MeshRenderer>().material.color; //TODO: change material in runtime
		c.a = n;
		obj.GetComponent<MeshRenderer>().material.color = c;
	}

	public void WorkingMode()
	{

		int tablesCount = allTables.Count;
		if (tablesCount % 2 == 0)
		{
			OrderOddTables();
		}
		else
		{
			OrderEvenTables();
		}

	}

	public void OrderEvenTables()
	{
		int i = 0;												   //table n.	
		int iteration = 1;                                         //row n.	
		string direction = "";

		Table firstTable = allTables[0];						   //we grab first table	
		Vector3 targetPosition = firstTable.transform.position;	   //default target position

		float lastZPosition = firstTable.transform.position.z;	   //last row position	
		float lastRightPosition = firstTable.transform.position.x; //last position of Right table
		float lastLeftPosition = firstTable.transform.position.x;  //last position of Left table
		float XScale = firstTable.transform.localScale.x;		   //difference between tables	

		foreach (Table t in allTables)
		{
			if (i == 0) {
				i++; continue;	//we are skipping first item
			}
			if (i % 2 == 1)
			{
				//if table goes right
				targetPosition.x = lastRightPosition + XScale;
				targetPosition.z = lastZPosition - 150;				//new row (on EVEN number ,,i" because then we approach new row)	
				lastZPosition = targetPosition.z;
				lastRightPosition = targetPosition.x;
				iteration++;
				direction = "right";
			}
			else
			{
				//if table goes left
				targetPosition.x = lastLeftPosition - XScale;
				targetPosition.z = lastZPosition;				
				lastLeftPosition = targetPosition.x;
				direction = "left";
			}

			t.GetComponent<Table>().targetPosition = targetPosition;
			t.GetComponent<Table>().moveTable = true;
			t.GetComponent<Table>().direction = direction;

			i++;
		}
	}

	public void OrderOddTables()
	{
		int i = 0;                                                 //table n.	
		int iteration = 1;                                         //row n.	
		string direction = "";

		Table firstTable = allTables[0];                           //we grab first table	
		Vector3 targetPosition = firstTable.transform.position;    //default target position

		float lastZPosition = firstTable.transform.position.z;     //last row position	
		float lastRightPosition = firstTable.transform.position.x; //last position of Right table
		float lastLeftPosition = firstTable.transform.position.x;  //last position of Left table
		float XScale = firstTable.transform.localScale.x;          //difference between tables	

		foreach (Table t in allTables)
		{
			if (i == 0)
			{
				targetPosition.x = lastRightPosition + (XScale/2 + 5);
				direction = "none";
			}
			else if (i == 1)
			{
				targetPosition.x = lastLeftPosition - (XScale / 2 + 5);
				targetPosition.z = lastZPosition;
				direction = "none";
				iteration++;
			}
			else if (i % 2 == 0)
			{
				//if table goes right
				if (i == 2) XScale = XScale * 1.35f;
				targetPosition.x = lastRightPosition + XScale;
				targetPosition.z = lastZPosition - 200;             //new row (on EVEN number ,,i" because then we approach new row)	
				lastZPosition = targetPosition.z;
				lastRightPosition = targetPosition.x;
				iteration++;
				direction = "right";
			}
			else
			{
				//if table goes left
				targetPosition.x = lastLeftPosition - XScale;
				targetPosition.z = lastZPosition;
				lastLeftPosition = targetPosition.x;
				direction = "left";
			}

			Debug.Log(targetPosition.z);

			t.GetComponent<Table>().targetPosition = targetPosition;
			t.GetComponent<Table>().moveTable = true;
			t.GetComponent<Table>().direction = direction;

			i++;
		}
	}

	//let camera fly to certain camera (by slider)
	public void FlyToTable()
	{
		//search value that is in slider, then find table with that ID
		int searchedTable = (int)tableNumber.value;
		GameObject obj = GameObject.Find("table"+searchedTable);

		//we want position of camera in front of table, not in camera
		//TODO: WHY LOCAL POSITION IS WORKING ???
		Vector3 pos = obj.transform.localPosition;
		pos.z -= 200f;

		FlyCamera scriptCamera = cam.GetComponent<FlyCamera>();
		scriptCamera.GetComponent<FlyCamera>().searchedPosition = pos;			//where to go
		scriptCamera.GetComponent<FlyCamera>().searchedTable = obj.transform;	//where to look
		scriptCamera.GetComponent<FlyCamera>().goToTable = true;				//release function in update
	}

	
	//let table go to front (by slider)
	bool isInFront = false;
	Vector3 defaultPosition;
	int frontTableId;
	Vector3 frontPosition = new Vector3(10f, 10f, 10f);

	public void TableToFront()
	{
		if (isInFront)
		{
			GameObject obj = GameObject.Find("table" + frontTableId);
			obj.GetComponent<Table>().targetPositionToFront = defaultPosition;
			obj.GetComponent<Table>().flyToFront = true;
			isInFront = false;
		}
		else
		{
			//find table
			int searchedTable = (int)tableNumber.value;
			GameObject obj = GameObject.Find("table" + searchedTable);

			//assign values, because then we want to move table to default position
			defaultPosition = obj.transform.position;
			isInFront = true;
			frontTableId = searchedTable;

			//run script that move table to front
			obj.GetComponent<Table>().targetPositionToFront = frontPosition;
			obj.GetComponent<Table>().flyToFront = true;
		}
	}

}
