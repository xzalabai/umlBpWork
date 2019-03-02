using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;




/*
 
	 
	TODO: FIX SPAWN CLASSOV DALEKO OD TABULE
	TODO: CREATE ALGORHITM ON METRICS
	TODO: MOVE GRAPH INSTANTIATE FROM CLASSMANAGER TO TABLEMANAGER
	TODO: FIX DIMENSIONAL EDGE
	TODO: URGENT !!! - SPAWN AND SHOW CLASSES ON TABLE SHOWCASE !!!!!!


*/

public class TableManager : MonoBehaviour {
	private Vector3 infinity = new Vector3(0, 0, -500000);
	public Camera cam;
	public GameObject prefab;
	public GameObject graphPrefab;
	public GameObject classManager;
	public GameObject edgesManager;
	public UnityEngine.UI.Slider tableNumber;
	//Resources.Load("TableMaterialOpaque.mat", typeof(Material)) as Material;
	public Table table1;
	public List<Table> allTables = new List<Table>();
	public Table table2;
	public static int lastTableId;
	public static float lastTablePosition;

	public Material opaque;
	public Material transparent;
	int distanceBetweenTables = 310;

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
		table1.name = "1";
		table2.name = "2";

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
		//creating new tablw with graph
		var go = Instantiate(prefab);
		Table table = go.GetComponent<Table>();
		table.transform.localScale = new Vector3(400,300,1);
		table.transform.position = new Vector3(0, 0, lastTablePosition + distanceBetweenTables);
		table.GetComponent<MeshRenderer>().material.renderQueue = 2999;

		//rise the max value in slider (one new table)
		tableNumber.maxValue++;

		lastTableId++;
		table.tag = "table";
		table.name = tableNumber.maxValue.ToString();
		table.uniqueId = System.Int32.Parse(tableNumber.maxValue.ToString());
		lastTablePosition = lastTablePosition + distanceBetweenTables;

		var graph = GameObject.Instantiate(graphPrefab);
		graph.transform.position = new Vector3(table.transform.position.x, table.transform.position.y, table.transform.position.z-10f);
		graph.transform.parent = table.transform;

		
		allTables.Add(table);

		//todo: lastTableId if work good
	}
	public void changeTransparencyTable1(float n)
	{
		GameObject obj = GameObject.Find("1");
		Color c = obj.GetComponent<MeshRenderer>().material.color; //TODO: change material in runtime
		c.a = n;
		obj.GetComponent<MeshRenderer>().material.color = c;
	}

	public void DeleteTables()
	{
		int searchedTable = (int)tableNumber.value;
		float lastPosition = 0;

		//we delete it from scene
		Debug.Log(searchedTable);
		GameObject obj = GameObject.Find(searchedTable.ToString());
		int deletedTableId = System.Int32.Parse(obj.name);
		Destroy(obj);

		//we delete selected table from list of all tables
		allTables = allTables.Where(x => x.name != searchedTable.ToString()).ToList();
		edgesManager.GetComponent<DimensionalEdges>().DeleteNestedAssociations(searchedTable);

		//we will move all tables behind to 1 position in front, and every table gets new .name = id - 1;
		foreach (Table t in allTables)
		{
			int tableId = System.Int32.Parse(t.name);
			if (tableId > deletedTableId)
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

	//returns radius
	public float Circuit()
	{
		float sizeOfTable = 400;
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
		startAng = -90;
		//go to preview positions
		if (!tableShowcaseON) {

			float radius = Circuit();
			float rightAngle = 180 / allTables.Count;
			Vector3 center = Camera.main.transform.position;
			foreach (Table t in allTables)
			{
				t.defaultPosition = t.transform.position;
				Vector3 pos = GetToCircle(center, radius, rightAngle);

				//t.transform.LookAt(Camera.main.transform.position);
				t.whereToLook = Camera.main.transform.position;
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
	int frontTableId;
	Vector3 frontPosition = new Vector3(10f, 10f, 10f);

	public void TableToFront()
	{
		Table t;
		if (!isInFront) {
			int searchedTable = (int)tableNumber.value;
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
			int searchedTable = (int)tableNumber.value;
			GameObject obj = GameObject.Find(searchedTable.ToString());
			t = obj.GetComponent<Table>();
			t.targetPosition = t.defaultPosition;
			t.whereToLook = infinity;
			t.moveTable = true;
			isInFront = false;
		}
	}

	bool moveToUserON = false;
	public void TableToUser()
	{
		Table t;
		if (!isInFront)
		{
			int searchedTable = (int)tableNumber.value;
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
			int searchedTable = (int)tableNumber.value;
			GameObject obj = GameObject.Find(searchedTable.ToString());
			t = obj.GetComponent<Table>();
			t.targetPosition = t.defaultPosition;
			t.whereToLook = infinity;
			t.moveTable = true;
			isInFront = false;
			t.distanceIsON = false;
		}
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
				i++; continue;  //we are skipping first item
			}
			if (i % 2 == 1)
			{
				//if table goes right
				targetPosition.x = lastRightPosition + XScale;
				targetPosition.z = lastZPosition - 150;             //new row (on EVEN number ,,i" because then we approach new row)	
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
				targetPosition.x = lastRightPosition + (XScale / 2 + 5);
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
}


