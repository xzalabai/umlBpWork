﻿using CodeStory;
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
	public UnityEngine.UI.Slider tableNumber;
	Table t;
	Action undoMove;
	//public Clickable clickable;
	public GameObject clicksManager;

	// Use this for initialization
	void Start () {
		clicksManager = GameObject.Find("ClicksManager");
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
		g.GetComponent<Graph>().RemoveEdge(undoMove.line.gameObject);
	}

	void ExecuteDeleteAssociation()
	{
		Graph g = undoMove.class1.GetComponentInParent<Graph>();
		g.GetComponent<Graph>().AddEdge(undoMove.class1, undoMove.class2);
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
		Graph g = undoMove.class1.GetComponentInParent<Graph>();
		g.GetComponent<Graph>().RemoveNode(undoMove.class1);
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
			g.GetComponent<Graph>().AddEdge(associatedClass, newClass);

		}
		string[] text = undoMove.class1WholeText.Split('#');
		Transform newBackground = newClass.gameObject.transform.GetChild(0);
		newBackground.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text[0];
		newBackground.gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = text[1];
		newBackground.gameObject.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = text[2];

		graph.GetComponent<Graph>().UpdateGraph();
		
	}

	public void AddClassAction(GameObject classGO) {
		var action = new Action();
		//Action action;
		action.typeOfAction = "addClass";
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
}