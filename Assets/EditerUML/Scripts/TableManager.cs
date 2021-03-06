﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;




/*
 
	 
	TODO: CREATE ALGORHITM ON METRICS
	TODO: FIX DIMENSIONAL EDGE
	TODO: FIX UNDO MOVE ON DIMENSIONAL EDGE

*/

public class TableManager : MonoBehaviour {
	private Vector3 infinity = new Vector3(0, 0, -500000);
	public Camera cam;
	public GameObject tablePrefab;
	public GameObject graphPrefab;
	public GameObject classManager;
	public GameObject edgesManager;
	public GameObject copiedClass = null;
	public UnityEngine.UI.Slider tableNumber;
	//Resources.Load("TableMaterialOpaque.mat", typeof(Material)) as Material;
	public Table table1;
	public List<Table> allTables = new List<Table>();
	public Table table2;
	public int lastTableId = 0;
	public float lastTablePosition = 0;
	public Material opaque;
	public Material transparent;
	public int distanceBetweenTables = 310;
    public bool spawnOnStart = true;

	// Use this for initialization
	void Start () {
        if (spawnOnStart)
        {
            CreateNewTable();
            CreateNewTable();
        }

	}
	
	// Update is called once per frame
	void Update () {

	}

	public void CreateNewTable()
	{
        //creating new tablw with graph
        var go = GameObject.Instantiate(tablePrefab);
        InitNewTable(go);	
    }

    public void InitNewTable(GameObject instantiatedTable)
    {
        var go = instantiatedTable;
        Table table = go.GetComponent<Table>();
		Debug.Log(table.transform.position.z + " " + lastTablePosition + "  " + distanceBetweenTables);
        table.transform.position = new Vector3(table.transform.position.x, table.transform.position.y, table.transform.position.z + lastTablePosition + distanceBetweenTables);
		Debug.Log(table.transform.position);
        if (table.GetComponent<MeshRenderer>() != null)
        {
            table.GetComponent<MeshRenderer>().material.renderQueue = 2999;
        }

        //rise the max value in slider (one new table)
        tableNumber.maxValue++;

        lastTableId++;
        table.tag = "table";
        table.name = tableNumber.maxValue.ToString();
        table.uniqueId = System.Int32.Parse(tableNumber.maxValue.ToString());
        lastTablePosition = lastTablePosition + distanceBetweenTables;

        allTables.Add(table);

        if (tableShowcaseON)
		{
			AddTableDuringShowcase();
		}

    }

	public void DeleteTables()
	{
		int searchedTable = (int)tableNumber.value;

		//we delete it from scene
		GameObject obj = GameObject.Find(searchedTable.ToString());
		Destroy(obj);
			AfterDeleteTables(searchedTable);
    }

    public void AfterDeleteTables(int searchedTable)
    {
		if (tableShowcaseON)
		{
			AfterDeleteTablesDuringShowcase(searchedTable);
			return;
		}
			
		if (allTables.Count == 0)
			return;
		float lastPosition = 0;

		if (searchedTable == 1 && allTables.Count == 1)
		{
			Table t = allTables[0];
			tableNumber.maxValue--;
			lastTablePosition = 0;
			allTables = allTables.Where(x => x.name != searchedTable.ToString()).ToList();
			return;
		}
		//if we are going to delete last table
		if (searchedTable == allTables.Count)
		{
			Table t = allTables[allTables.Count - 2];
			tableNumber.maxValue--;
			lastTablePosition = t.transform.position.z;
			allTables = allTables.Where(x => x.name != searchedTable.ToString()).ToList();
			return;
		}      

		//we delete selected table from list of all tables
        allTables = allTables.Where(x => x.name != searchedTable.ToString()).ToList();
		if (edgesManager)
			edgesManager.GetComponent<DimensionalEdges>().DeleteNestedAssociations(searchedTable);
		
        //we will move all tables behind to 1 position in front, and every table gets new .name = id - 1;
        foreach (Table t in allTables)
        {
		
			int tableId = System.Int32.Parse(t.name);

			//we delete table from list 
			if (tableId == searchedTable)
				allTables.Remove(t);

			//we iterate through all tables, to move their positions
			if (tableId > searchedTable)
            {
                Vector3 pos = t.transform.position;
                pos.z -= distanceBetweenTables;
                t.transform.position = pos;
                int newId = System.Int32.Parse(t.name);
                newId--;
                t.name = newId.ToString();
                lastPosition = t.transform.position.z;
            }

			
        }
        tableNumber.maxValue--;
        lastTablePosition = lastPosition;
    }

	public void AfterDeleteTablesDuringShowcase(int searchedTable)
	{
		if (allTables.Count == 0)
			return;
		float lastPosition = 0;

		if (searchedTable == 1 && allTables.Count == 1)
		{
			Table t = allTables[0];
			tableNumber.maxValue--;
			lastTablePosition = 0;
			allTables = allTables.Where(x => x.name != searchedTable.ToString()).ToList();
			return;
		}
		//if we are going to delete last table
		if (searchedTable == allTables.Count)
		{
			Table t = allTables[allTables.Count - 2];
			tableNumber.maxValue--;
			lastTablePosition = t.defaultPosition.z;
			allTables = allTables.Where(x => x.name != searchedTable.ToString()).ToList();
			DeleteTableDuringShowcase();
			return;
		}

		//we delete selected table from list of all tables
		allTables = allTables.Where(x => x.name != searchedTable.ToString()).ToList();
		if (edgesManager)
			edgesManager.GetComponent<DimensionalEdges>().DeleteNestedAssociations(searchedTable);

		//we will move all tables behind to 1 position in front, and every table gets new .name = id - 1;
		foreach (Table t in allTables)
		{

			int tableId = System.Int32.Parse(t.name);

			//we delete table from list 
			if (tableId == searchedTable)
				allTables.Remove(t);

			//we iterate through all tables, to move their positions
			if (tableId > searchedTable)
			{
				Vector3 pos = t.defaultPosition;
				pos.z -= distanceBetweenTables;
				t.defaultPosition = pos;
				int newId = System.Int32.Parse(t.name);
				newId--;
				t.name = newId.ToString();
				lastPosition = t.defaultPosition.z;
			}


		}
		tableNumber.maxValue--;
		lastTablePosition = lastPosition;

		DeleteTableDuringShowcase();
	}

	public float BiggestScaleOfChildren(GameObject o)
	{
		var asf = o.transform.GetComponentsInChildren<Transform>();
		var max = 0f;
		foreach (var t in asf)
		{
			if (t.localScale.x > max)
			{
				max = t.localScale.x;
			}
		}
		return max;

	}


	//returns radius
	public float Circuit()
	{
		float sizeOfTable = BiggestScaleOfChildren(tablePrefab);
		float PI = 3.141f;
		float numOfTables = allTables.Count;

		float sizeOfAllTables = numOfTables * sizeOfTable;

		return (sizeOfAllTables / PI);
	}



	//function for table showcase (in a half circle)
	bool tableShowcaseON = false;
	float startAng = -90;
	public void TestTableShowcase()
	{
		if (allTables.Count == 2)
			startAng = -45;
		else if (allTables.Count == 3)
			startAng = -60;
		else
			startAng = -90;
		//go to preview positions
		if (!tableShowcaseON) {

			float radius = Circuit();
			float rightAngle = 180 / allTables.Count;
			Vector3 center = new Vector3(0,0,0);
			Debug.Log(center);
			foreach (Table t in allTables)
			{
				t.defaultPosition = t.transform.position;
				center.y = t.transform.position.y;
				Vector3 pos = GetToCircle(center, radius, rightAngle);
				Debug.Log(center + " " + radius + " " + rightAngle + " " + pos);
				t.whereToLook = center;
				t.GetComponent<Table>().targetPosition = pos;
				t.GetComponent<Table>().moveTable = true;
			}
			tableShowcaseON = true;
		}

		//get back to default positions
		else
		{
			foreach (Table t in allTables) {
				//t.transform.LookAt(Vector3.back);
				t.GetComponent<Table>().targetPosition = t.defaultPosition;
				t.whereToLook = infinity;
				t.GetComponent<Table>().moveTable = true;
			}
			tableShowcaseON = false;
		}
		
	}

	Vector3 DeleteTableDuringShowcase()
	{
		tableShowcaseON = false;
		if (allTables.Count == 2)
			startAng = -45;
		else if (allTables.Count == 3)
			startAng = -60;
		else
			startAng = -90;
		//go to preview positions
		if (!tableShowcaseON)
		{

			float radius = Circuit();
			float rightAngle = 180 / allTables.Count;
			Vector3 center = new Vector3(0, 0, 0);
			foreach (Table t in allTables)
			{
				if (t.defaultPosition == Vector3.zero)
					t.defaultPosition = t.transform.position;
				center.y = t.transform.position.y;
				Vector3 pos = GetToCircle(center, radius, rightAngle);
				t.whereToLook = center;
				t.GetComponent<Table>().targetPosition = pos;
				t.GetComponent<Table>().moveTable = true;
			}
			tableShowcaseON = true;
		}

		return Vector3.zero;
	}

	Vector3 AddTableDuringShowcase() {
		tableShowcaseON = false;
		if (allTables.Count == 2)
			startAng = -45;
		else if (allTables.Count == 3)
			startAng = -60;
		else
			startAng = -90;
		//go to preview positions
		if (!tableShowcaseON)
		{

			float radius = Circuit();
			float rightAngle = 180 / allTables.Count;
			Vector3 center = new Vector3(0, 0, 0);
			foreach (Table t in allTables)
			{
				if (t.defaultPosition == Vector3.zero)
					t.defaultPosition = t.transform.position;
				center.y = t.transform.position.y;
				Vector3 pos = GetToCircle(center, radius, rightAngle);
				t.whereToLook = center;
				t.GetComponent<Table>().targetPosition = pos;
				t.GetComponent<Table>().moveTable = true;
			}
			tableShowcaseON = true;
		}

		return Vector3.zero;
	}

	Vector3 GetToCircle(Vector3 center, float radius, float rightAngle)
	{
		float ang = startAng;
		startAng += rightAngle;
		Vector3 pos;
		pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
		pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
		pos.y = center.y;
		return pos;
	}

	//let camera fly to certain camera (by slider)
	public void FlyToTable()
	{
		//search value that is in slider, then find table with that ID
		int searchedTable = (int)tableNumber.value;
		GameObject obj = GameObject.Find(searchedTable.ToString());
		
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
	int searchedTable;
	int frontTableId;
	Vector3 frontPosition = new Vector3(10f, 10f, 10f);

	public void TableToFront()
	{
		Table t;
		if (!isInFront) {
			 searchedTable = (int)tableNumber.value;
			GameObject obj = GameObject.Find(searchedTable.ToString());
			t = obj.GetComponent<Table>();
			t.defaultPosition = t.transform.position;
			t.targetPosition = new Vector3(0, 0, 0);
			t.whereToLook = infinity;
			t.moveTable = true;
			isInFront = true;
		}

		else
		{
			//int searchedTable = (int)tableNumber.value;
			GameObject obj = GameObject.Find(searchedTable.ToString());
			t = obj.GetComponent<Table>();
			t.targetPosition = t.defaultPosition;
			t.whereToLook = infinity;
			t.moveTable = true;
			isInFront = false;
			searchedTable = 0;
		}
	}

	bool moveToUserON = false;

	public void ExecuteUndoMove()
	{
		int searchedTable = (int)tableNumber.value;
		GameObject obj = GameObject.Find(searchedTable.ToString());
		Table t = obj.GetComponent<Table>();
		BackManager backManagerOfTable = t.GetComponentInChildren<BackManager>();
		backManagerOfTable.GetComponent<BackManager>().ExecuteUndoMove();

	}

	//int searchedTable;
	public void TableToUser()
	{
		Table t;
		if (!isInFront)
		{
			searchedTable = (int)tableNumber.value;
			GameObject obj = GameObject.Find(searchedTable.ToString());
			t = obj.GetComponent<Table>();
			t.defaultPosition = t.transform.position;
			t.targetPosition = Camera.main.transform.position;
			t.whereToLook = Camera.main.transform.position;
			t.distanceIsON = true;
			t.moveTable = true;
			isInFront = true;
		}

		else
		{
			//int searchedTable = (int)tableNumber.value;
			GameObject obj = GameObject.Find(searchedTable.ToString());
			t = obj.GetComponent<Table>();
			t.targetPosition = t.defaultPosition;
			t.whereToLook = infinity;
			t.moveTable = true;
			isInFront = false;
			t.distanceIsON = false;
			searchedTable = 0;
		}
	}
	public void CopyClass(GameObject dragNDropTable, int idOfClass, GameObject newClass)
	{
		CopyClass(copiedClass, dragNDropTable, idOfClass, newClass);
	}

	public void CopyClass( GameObject copiedClass, GameObject dragNDropTable, int idOfClass, GameObject newClass)
	{
		//Graph graph = GetComponentInChildren<Graph>();
		//GameObject newClass = graph.GetComponent<Graph>().AddNode();

		//get strings from old table
		Transform background = copiedClass.gameObject.transform.GetChild(0);
		String header = background.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
		String method = background.gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text;
		String attributes = background.gameObject.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text;


		//set that old strings to new class
		Transform newBackground = newClass.gameObject.transform.GetChild(0);
		newBackground.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = header;
		newBackground.gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = method;
		newBackground.gameObject.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = attributes;

		newClass.transform.position = dragNDropTable.transform.position;
		newClass.transform.localPosition = new Vector3(newClass.transform.localPosition.x, newClass.transform.localPosition.y, newClass.transform.localPosition.z - 1.05f);

		newClass.tag = "class";
		newClass.name = idOfClass.ToString();

		//WriteAction("addClass", graph.GetComponentInParent<Table>(), newClass, Int32.Parse(newClass.name), null, 0, null, new Vector3(0, 0, 0), null, null);

		//idOfClass++;
		Debug.Log("Position of new class " + newClass.transform.position);
		//Debug.Log("Position of table " + graph.transform.position);
		Debug.Log("Position of go " + gameObject.transform.position);
		//graph.GetComponent<Graph>().UpdateGraph();
	}
}


