using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableManager : MonoBehaviour {
	public Camera cam;
	public GameObject prefab;
	public GameObject graphPrefab;
	//Resources.Load("TableMaterialOpaque.mat", typeof(Material)) as Material;
	public Table table1;
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
		table2.transform.position = new Vector3(0, 0, 306);

		table1.GetComponent<MeshRenderer>().material.renderQueue = 2999;
		table2.GetComponent<MeshRenderer>().material.renderQueue = 2999;
		table1.name = "table1";
		table2.name = "table2";

		table1.tag = "table";
		table2.tag = "table";
 

		lastTableId = 2;
		lastTablePosition = 306;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void CreateNewTable()
	{
		//TODO: pri vytvarani dalsich tabul sa classy zacnu spawnovat inde ako maju
		//creating new tablw with graph
		var go = Instantiate(prefab);
		Table table = go.GetComponent<Table>();
		table.transform.localScale = new Vector3(400,300,1);
		Debug.Log(lastTablePosition);
		table.transform.position = new Vector3(0, 0, lastTablePosition + 286);
		table.GetComponent<MeshRenderer>().material.renderQueue = 2999;
		table.tag = "table";
		lastTableId = lastTableId + 1;
		table.name = "table" + (lastTableId);
		lastTablePosition = lastTablePosition + 286;

		var graph = GameObject.Instantiate(graphPrefab);
		graph.transform.position = table.transform.position;
		graph.transform.parent = table.transform;

	}
	public void changeTransparencyTable1(float n)
	{
		GameObject obj = GameObject.Find("table1");
		Color c = obj.GetComponent<MeshRenderer>().material.color; //TODO: change material in runtime
		c.a = n;
		obj.GetComponent<MeshRenderer>().material.color = c;
	}

	public void tablePreview()
	{
		GameObject obj = GameObject.Find("table1");
		Table script = obj.GetComponent<Table>();
		script.separateTable = true;

		FlyCamera scriptCamera = cam.GetComponent<FlyCamera>();


		if (obj.transform.position.z > cam.transform.position.z && obj.transform.position.x > cam.transform.position.x)
			scriptCamera.direction = "FrontRight";
		else if (obj.transform.position.z > cam.transform.position.z && obj.transform.position.x < cam.transform.position.x)
			scriptCamera.direction = "FrontLeft";
		else if (obj.transform.position.z < cam.transform.position.z && obj.transform.position.x > cam.transform.position.x)
			scriptCamera.direction = "BackLeft";
		else if (obj.transform.position.z < cam.transform.position.z && obj.transform.position.x < cam.transform.position.x)
			scriptCamera.direction = "BackRight";
		else Debug.Log("TODOOOOOOOOOOOO POHYB AK SA ROVNAJU"); //TODO: ak sa rovnaju ??

		scriptCamera.searchedTable = obj;
		scriptCamera.goToTable = true;
	}
}
