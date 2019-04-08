using CodeStory;
using Microsoft.Msagl.Core.Layout;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BackManager : MonoBehaviour {

	/* -----A C T I O N S --------------
	Delete Table -> deleteTable 
	Add Table    -> addTable 
	Move Table   -> moveTable

	Delete Class -> deleteClass 
	Add Class	 -> addClass 
	Move Class   -> moveClass
	
	Delete Assoc -> deleteAssociation
	Add Assoc    -> addAssociation
	
	Change Header	 -> changeHeader
	Change Methods-> changeMethods
	Change Attrib -> changeAttributes
	*/


		/*
		 
		TODO -> MAKE getLine - create unique ID to LINE!!! 
		TODO -> delete association UNDO BUTTON NOT CREATED !!!!!!!!!!!!
		TODO -> add table id to actions (then getClass use in Execute)
		 
		 */
	

	public struct Action
	{
		public string typeOfAction;
		public Table table;
		public GameObject class1;
		public Vector3 class1Position;
		public int class1Id;
		public string class1Header;
		public string class1WholeText;
		public string class1Methods;
		public string class1Attributes;
		public List<int> associatedWith;
		public GameObject class2;
		public int class2Id;
		public LineRenderer line;

	};
	public Stack<Action> previousActions = new Stack<Action>();
	Table t;
	Action undoMove;
	//public Clickable clickable;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ExecuteUndoMove()
	{
		if (previousActions.Count == 0)
		{
			Debug.Log("NO MORE UNDO ACTIONS");
			return;
		}
		undoMove = previousActions.Pop();
		Debug.Log("Undo action: " + undoMove.typeOfAction);
		switch (undoMove.typeOfAction)
		{
			case "addClass":
				ExecuteAddClass(); //this will delete a table
				break;
			case "deleteClass":
				ExecuteDeleteClass(); //this will add a deleted table
				break;
			case "addAssociation":
				ExecuteAddAssociation(); //this will add a deleted table
				break;
			case "deleteAssociation":
				ExecuteDeleteAssociation(); //this will add a deleted table
				break;
			case "changeHeader":
				ExecuteChangeText("header"); //this will add a deleted table
				break;
			case "changeMethods":
				ExecuteChangeText("methods"); //this will add a deleted table
				break;
			case "changeAttributes":
				ExecuteChangeText("attributes"); //this will add a deleted table
				break;
			default:
				Debug.Log("Unknown undo move");
				break;
		}
	}

	void ExecuteAddAssociation()
	{
		GameObject class1 = getClass(undoMove);
		GameObject class2 = getClass2(undoMove);

		Graph g = class1.GetComponentInParent<Graph>(); //we get the graph in which is line

		if (g == null) class2.GetComponentInParent<Graph>();

		GameObject line = getLine(undoMove);

		line.SetActive(false);
		//g.GetComponent<Graph>().RemoveEdge(undoMove.line.gameObject);
	}

	void ExecuteDeleteAssociation()
	{
		GameObject from = getClass(undoMove);
		GameObject to = getClass2(undoMove);

		Graph g = from.GetComponentInParent<Graph>();
		GameObject line = g.GetComponent<Graph>().AddEdge(undoMove.class1, undoMove.class2);
		line.tag = "line";
	}

	void ExecuteChangeText(string type)
	{
		GameObject undoClass = getClass(undoMove);
		Transform newBackground = undoClass.gameObject.transform.GetChild(0);
		
		if (type == "header"){
			newBackground.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = undoMove.class1Header;
		}
		if (type == "methods")
		{
			newBackground.gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = undoMove.class1Methods;
		}
		if (type == "attributes")
		{
			newBackground.gameObject.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = undoMove.class1Attributes;
		}
	}

	void ExecuteAddClass() 
	{
		GameObject classGO = getClass(undoMove);
		Graph g =classGO.GetComponentInParent<Graph>();
		classGO.SetActive(false);
		//g.GetComponent<Graph>().RemoveNode(undoMove.class1);
	}

	void ExecuteDeleteClass()
	{
		Table table = undoMove.table;
		Graph graph = table.GetComponentInChildren<Graph>();

		GameObject newClass = graph.GetComponent<Graph>().AddNode();
		Debug.Log("nova pozicia" + undoMove.class1Position);
		newClass.transform.position = new Vector3(undoMove.class1Position.x, undoMove.class1Position.y, newClass.transform.position.z);
		//newClass.transform.localPosition = new Vector3(newClass.transform.localPosition.x, newClass.transform.localPosition.y, newClass.transform.localPosition.z - 0.05f);
		newClass.tag = "class";
		newClass.name = undoMove.class1Id.ToString();
		foreach (int association in undoMove.associatedWith)
		{
			GameObject associatedClass = getClass(association, undoMove);
			Graph g = newClass.GetComponentInParent<Graph>();
			GameObject line = g.GetComponent<Graph>().AddEdge(associatedClass, newClass);
			line.tag = "line";

		}
		string[] text = undoMove.class1WholeText.Split('#');
		Transform newBackground = newClass.gameObject.transform.GetChild(0);
		newBackground.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text[0];
		newBackground.gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = text[1];
		newBackground.gameObject.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = text[2];

		graph.GetComponent<Graph>().UpdateGraph();
		
	}

	public void AddClassAction(GameObject classGO, Table table) {
		var action = new Action();
		//Action action;
		action.typeOfAction = "addClass";
		action.class1Id = Int32.Parse(classGO.transform.name);
		action.table = table;
		action.class1 = classGO;
		previousActions.Push(action);
	}

	public void DeleteClassAction(Table table, GameObject classGO, int id, Vector3 previousPos, string wholeText, List<int> associatedWith)
	{
		var action = new Action();
		action.typeOfAction = "deleteClass";
		action.class1 = classGO;
		Debug.Log("CLASS ID " + id);
		action.class1Id = id;
		action.class1Position = previousPos;
		action.table = table;
		action.associatedWith = associatedWith;
		action.class1WholeText = wholeText;
		previousActions.Push(action);
	}

	public void AddAssociationAction(Table table, LineRenderer line, GameObject c1,int id1, GameObject c2, int id2)
	{
		var action = new Action();
		action.typeOfAction = "addAssociation";
		action.class1 = c1;
		action.table = table;
		action.class1Id = id1;
		action.class2Id = id2;
		action.class2 = c2;
		action.line = line;
		previousActions.Push(action);
	}

	public void DeleteAssociationAction(LineRenderer line, GameObject c1, int id1, GameObject c2, int id2)
	{
		var action = new Action();
		action.typeOfAction = "deleteAssociation";
		action.class1 = c1;
		action.class2 = c2;
		action.class1Id = id1;
		action.class2Id = id2;
		action.line = line;
		previousActions.Push(action);
	}

	public void ChangeHeaderAction(Table table, GameObject c1, int id, string changedText)
	{
		var action = new Action();
		action.typeOfAction = "changeHeader";
		action.class1 = c1;
		action.class1Id = id;
		action.table = table;
		action.class1Header = changedText;
		previousActions.Push(action);
	}

	public void ChangeMethodsAction(Table table, GameObject c1, int id, string changedText)
	{
		var action = new Action();
		action.typeOfAction = "changeMethods";
		action.class1 = c1;
		action.class1Id = id;
		action.table = table;
		action.class1Methods = changedText;
		previousActions.Push(action);
	}

	public void ChangeAttributesAction(Table table, GameObject c1, int id, string changedText)
	{
		var action = new Action();
		action.typeOfAction = "changeAttributes";
		action.class1 = c1;
		action.class1Id = id;
		action.table = table;
		action.class1Attributes = changedText;
		previousActions.Push(action);
	}

	GameObject getLine(Action act)
	{
		int from = act.class1Id;
		int to = act.class2Id;

		if (act.line)
			return act.line.gameObject;

		Graph g = act.table.GetComponentInChildren<Graph>();
		
		GameObject units = g.transform.Find("Units").gameObject;

		var allEdges = FindEdgesInTable(units, "line");
		foreach(UEdge edge in allEdges)
		{
			Edge e = edge.graphEdge;
			if (e.Source.UserData.ToString().Split(' ')[0] == from.ToString() && e.Target.UserData.ToString().Split(' ')[0] == to.ToString())
			{
				
				return edge.gameObject;
			}

			if (e.Target.UserData.ToString().Split(' ')[0] == from.ToString() && e.Source.UserData.ToString().Split(' ')[0] == to.ToString())
			{
				return edge.gameObject;
			}

		}

		return null;
	//	Edge e = act.line.GetComponent<Edge>();
	//	Debug.Log(e.Source.UserData.ToString());
	//	Debug.Log(e.Target.UserData.ToString());
	}


	GameObject getClass(Action act)
	{
		string name = act.class1Id.ToString();

		if (act.class1) return act.class1;

		Graph g = act.table.GetComponentInChildren<Graph>();
		GameObject units = g.transform.Find("Units").gameObject;
		if (units.transform.Find(name).gameObject)
			return units.transform.Find(name).gameObject;
		Debug.Log("ERROR: Cannot find CLASS in table");
		return null;
	}

	GameObject getClass2(Action act)
	{
		string name = act.class2Id.ToString();

		if (act.class2) return act.class2;

		Graph g = act.table.GetComponentInChildren<Graph>();
		GameObject units = g.transform.Find("Units").gameObject;
		if (units.transform.Find(name).gameObject) 
			return units.transform.Find(name).gameObject;
		Debug.Log("ERROR: Cannot find CLASS in table");
		return null;
	}

	GameObject getClass(int num, Action act)
	{
		Graph g = act.table.GetComponentInChildren<Graph>();
		GameObject units = g.transform.Find("Units").gameObject;
		if (units.transform.Find(num.ToString()).gameObject)
			return units.transform.Find(num.ToString()).gameObject;
		Debug.Log("ERROR: Cannot find CLASS in table");
		return null;

	}

	public List<UEdge> FindEdgesInTable(GameObject parent, string tag)
	{
		List<UEdge> edges = new List<UEdge>();

		Transform t = parent.transform;
		for (int i = 0; i < t.childCount; i++)
		{
			Debug.Log("deti");
			if (t.GetChild(i).gameObject.tag == tag)
			{

				//Edge e = t.GetChild(i).gameObject.;
				GameObject g = t.GetChild(i).gameObject;
				//Edge e = g.GetComponent<Edge>();
				//Debug.Log(e.Target.UserData.ToString());
				edges.Add(t.GetChild(i).GetComponent<UEdge>());
				//return t.GetChild(i).gameObject;
			}
		}
		if (edges.Count == 0)
			return null;

		return edges;
	}

	public void WriteAction(string type, Table table, GameObject class1, int class1ID, GameObject class2, int class2ID, LineRenderer line, Vector3 previousPos, string writtenText, List<int> allAssociations)
	{
		switch (type)
		{
			case "addClass":
				GetComponent<BackManager>().AddClassAction(class1, table);
				break;
			case "deleteClass":
				Debug.Log(allAssociations.Count + "xxxxxxxxx");
				GetComponent<BackManager>().DeleteClassAction(table, class1, class1ID, previousPos, writtenText, allAssociations);
				break;
			case "addAssociation":
				GetComponent<BackManager>().AddAssociationAction(table, line, class1, class1ID, class2, class2ID);
				break;
			case "deleteAssociation":
				Debug.Log("UNDO ON delete is not working now");
				GetComponent<BackManager>().DeleteAssociationAction(line, class1, 0, class2, 0);
				break;
			case "changeHeader":
				GetComponent<BackManager>().ChangeHeaderAction(table, class1, class1ID, writtenText);
				break;
			case "changeAttributes":
				GetComponent<BackManager>().ChangeAttributesAction(table, class1, class1ID, writtenText);
				break;
			case "changeMethods":
				GetComponent<BackManager>().ChangeMethodsAction(table, class1, class1ID, writtenText);
				break;
			default:
				Debug.Log("WRONG BACK OPERATION");
				break;
		}

	}
}
