using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
namespace CodeStory
{
	[Serializable]
	public class GameObjectEvent : UnityEvent<GameObject> { };

	public class Clickable : MonoBehaviour
	{
		static bool firstN = false;
		public float dragSpeed = 0.09f;
		Vector3 lastMousePos;

		Vector3 dist;
		float posX;
		float posY;

		public GameObjectEvent triggerAction;
		public GameObject textManagerScript;
		static GameObject a = null;
		static GameObject b = null;
		public Camera camera;
		static Graph parent1 = null;
		static Graph parent2 = null;
		public GameObject testCube;
		static GameObject firstNode = null, secondNode = null;
		private Vector3 screenPoint;
		private Vector3 offset;

		private TextMeshProUGUI textMesh;

		private float delta;

		public int idOfClass = 1;

		private float time;

		private bool dragON = false;
		bool writingInClass = false;
		TextMeshProUGUI whereToWrite;
		GameObject dragNDropClass = null;
		GameObject dragNDropTable = null;
		//private GameObject dragNDropClass = null;

		private bool doubleClick = false;

		void Start()
		{
			textManagerScript = GameObject.Find("TextManager");
		}

		void Update()
		{

			if (writingInClass && Input.GetKey(KeyCode.Return))
			{
				string t = textManagerScript.GetComponent<TextManager>().GetInputFieldText();
				string parsedText = t.Replace(';', '\n');
				Debug.Log(parsedText);
				whereToWrite.text = parsedText;

				whereToWrite = null;
				textManagerScript.GetComponent<TextManager>().HideOrUnihdeInputField(true);
				writingInClass = false;
			}
		}

		private void OnMouseDown()
		{
			lastMousePos = Input.mousePosition;
			delta = Time.time - time;

			if (delta < 0.25)
			{
				if (Input.GetKey(KeyCode.E))
					OnDoubleClick();
			}
			else if (Input.GetKey(KeyCode.Q))
			{
				OnRealMouseDown();
			}
			else if (Input.GetKey(KeyCode.LeftShift)) {
				ChangeText();
			}
			else if (Input.GetKey(KeyCode.T))
			{
				DeleteObject();
			}
			else if (Input.GetKey(KeyCode.F))
			{
				DragNDropClass();
			}
			time = Time.time;
		}


		void DeleteObject()
		{
			triggerAction.Invoke(gameObject);
			Graph g = GetComponentInParent<Graph>();
			if (gameObject.tag == "class"){

				Transform background = gameObject.transform.GetChild(0);
				String header = background.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
				String method = background.gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text;
				String attributes = background.gameObject.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text;
				string writtenText = header + "#" + method + "#" + attributes;
				List<int> associatedWith = new List<int>();
				associatedWith = getAllAssociations(gameObject);

				WriteAction("deleteClass", g.GetComponentInParent<Table>(), null,Int32.Parse(gameObject.name), null,0, null, gameObject.transform.position, writtenText, associatedWith);
				g.GetComponent<Graph>().RemoveNode(gameObject);
			}
			else if (gameObject.tag == "line"){
				//WriteAction("deleteAssociation",g.GetComponentInParent<Table>(),)
				g.GetComponent<Graph>().RemoveEdge(gameObject);
			}
			else if (gameObject.tag == "dimensionalLine"){
				Debug.Log("ZLEEEEEE");
				LineRenderer lr = gameObject.GetComponent<LineRenderer>();
				GameObject obj = GameObject.Find("DimensionalEdgesManager");
				obj.GetComponent<DimensionalEdges>().deleteDimensionalAssociation(lr);
			}
		}


	
		public void ChangeText()
		{
			bool classWasHitted = false;
			Table table = null;
			GameObject classGO = null;
			string changedCell = null;
			string oldtext = null;
			Vector3 v3T = Input.mousePosition;
			v3T = Camera.main.ScreenToWorldPoint(v3T);
			Ray rayy = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] hits;
			hits = Physics.RaycastAll(rayy, 8000.0F).OrderBy(h => h.distance).ToArray(); ;
			for (int i = 0; i < hits.Length; i++)
			{
				RaycastHit hit = hits[i];
				Renderer rend = hit.transform.GetComponent<Renderer>();
				//TODO: set size for clicks (when too close to table, error)
				if (hit.collider.transform.name == "Methods" || hit.collider.transform.name == "Header" || hit.collider.transform.name == "Attributes")
				{
					classWasHitted = true;
					changedCell = hit.collider.transform.name;
					textManagerScript.GetComponent<TextManager>().HideOrUnihdeInputField(false);
					TextMeshProUGUI t = hit.collider.transform.GetComponent<TextMeshProUGUI>();
					oldtext = t.text;                                               
					textManagerScript.GetComponent<TextManager>().SetInputFieldText(t.text.Replace('\n', ';'));
					writingInClass = true;
					whereToWrite = t;
				}

				if (!table && hit.collider.transform.tag == "table")
				{
					table = hit.collider.transform.gameObject.GetComponent<Table>();
				}
					

				if (!classGO && hit.collider.transform.tag == "class")
					classGO = hit.collider.transform.gameObject;
			}
			if (classWasHitted && classGO)
				WriteAction("change" + changedCell, table, classGO,Int32.Parse(classGO.name), null,0, null, Vector3.zero, oldtext, null);
		}
		
		//create New Association
		void OnRealMouseDown()
		{
			GameObject taggedObj = RayForNewAssociation();
			
			//assign first, then second class 
			if (a == null)
				a = taggedObj;
			else
				b = taggedObj;

			//if we didnt tagged class
			if (isClass(a) == false || isClass(b) == false){
				a = null; b = null; parent1 = null; parent2 = null;
			}

			//if the 2 tagged objects are same, we are not assigning them a association
			if (a == b){
				a = null; b = null; parent1 = null; parent2 = null;
			}

			//assign parent of first, then parent of second
			if (parent1 == null)
				parent1 = GetComponentInParent<Graph>();
			else
				parent2 = GetComponentInParent<Graph>();

			if (a != null  && b != null && a.tag == "class" && b.tag == "class"){
				if (parent1 == parent2)
				{
					Graph g = GetComponentInParent<Graph>();
					GameObject c = g.GetComponent<Graph>().AddEdge(a, b);
					LineRenderer x = c.GetComponent<LineRenderer>();
					WriteAction("addAssociation",g.GetComponentInParent<Table>(),a, Int32.Parse(a.name),b,Int32.Parse(b.name),x,new Vector3(0,0,0),null, null);
					g.GetComponent<Graph>().UpdateGraph();

					c.tag = "line";
				}
				else if (a != null && b != null)
				{
					GameObject obj = GameObject.Find("DimensionalEdgesManager");
					obj.GetComponent<DimensionalEdges>().createDimensionalAssociation(a, b);
					//WriteAction("addAssociation", g.GetComponentInParent<Table>(), a, b, c, new Vector3(0, 0, 0), null);

				}
				else
				{
					//Debug.Log("Not selected");
				}
				a = null; b = null;
				parent1 = null; parent2 = null;
			}
		}

		public bool isClass(GameObject g)
		{
			if (g == null)
				return true;
			if (g.tag == "class")
				return true;
			return false;
		}

		public bool isTable(GameObject g)
		{
			//Debug.Log(g.tag);
			if (g.tag == "table")
				return true;
			return false;
		}

		//create New Class
		private void OnDoubleClick()
		{
			Vector3 classPosition = RayForNewClass();
			triggerAction.Invoke(gameObject);

			//if we didnt click on table (but on class ..) we will not create another class
			if (!isTable(gameObject))
				return;
			
			Graph graph = GetComponentInChildren<Graph>();
			GameObject a = graph.GetComponent<Graph>().AddNode();

            //classPosition.z = graph.transform.position.z;
            float originalZPosizition = a.transform.localPosition.z;

            a.transform.position = classPosition;
			a.transform.localPosition = new Vector3(a.transform.localPosition.x, a.transform.localPosition.y, originalZPosizition);

			a.tag = "class";
			a.name = idOfClass.ToString();

			WriteAction("addClass", graph.GetComponentInParent<Table>(), a,idOfClass, null,0, null, new Vector3(0,0,0), null, null);
			idOfClass++;
			graph.GetComponent<Graph>().UpdateGraph();
		}

		Vector3 RayForNewClass()
		{
			Vector3 v3T = Input.mousePosition;
			v3T = Camera.main.ScreenToWorldPoint(v3T);
			Ray rayy = Camera.main.ScreenPointToRay(Input.mousePosition);
			//Debug.DrawRay(rayy.origin, rayy.direction * 1000, Color.green, 800000.0f);
			RaycastHit[] hits;
			hits = Physics.RaycastAll(rayy, 8000.0F).OrderBy(h => h.distance).ToArray(); ;
			for (int i = 0; i < hits.Length; i++)
			{
				RaycastHit hit = hits[i];
				Renderer rend = hit.transform.GetComponent<Renderer>();
				if (hit.collider.transform.tag == "table")
				{
					Debug.Log(hit.collider.transform.name);
					Debug.DrawLine(Camera.main.transform.position, hit.point);
					return hit.point;
				}
			}
			return new Vector3(0, 0, 0);
		}


		GameObject RayForNewAssociation()
		{
			Vector3 v3T = Input.mousePosition;
			v3T = Camera.main.ScreenToWorldPoint(v3T);
			Ray rayy = Camera.main.ScreenPointToRay(Input.mousePosition);
			//Debug.DrawRay(rayy.origin, rayy.direction * 1000, Color.green, 800000.0f);
			RaycastHit[] hits; 
			hits = Physics.RaycastAll(rayy, 8000.0F).OrderBy(h => h.distance).ToArray(); ;
			for (int i = 0; i < hits.Length; i++)
			{
				RaycastHit hit = hits[i];
				Debug.Log("xx " + hit.collider.transform.tag);
				Renderer rend = hit.transform.GetComponent<Renderer>();
				if (hit.collider.transform.tag == "class")
				{
					Debug.DrawLine(Camera.main.transform.position, hit.point);
					return hit.collider.gameObject;
				}
			}

			return null;
		}

		GameObject RayForDragNDropClass()
		{
			Vector3 v3T = Input.mousePosition;
			v3T = Camera.main.ScreenToWorldPoint(v3T);
			Ray rayy = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] hits;
			hits = Physics.RaycastAll(rayy, 8000.0F).OrderBy(h => h.distance).ToArray(); ;
			for (int i = 0; i < hits.Length; i++)
			{
				RaycastHit hit = hits[i];
				Renderer rend = hit.transform.GetComponent<Renderer>();
				if (hit.collider.transform.tag == "class")
				{
					return hit.collider.transform.gameObject;
				}
			}
			return null;
		}


		GameObject RayForDragNDropTable()
		{
			Vector3 v3T = Input.mousePosition;
			v3T = Camera.main.ScreenToWorldPoint(v3T);
			Ray rayy = Camera.main.ScreenPointToRay(Input.mousePosition);
			//Debug.DrawRay(rayy.origin, rayy.direction * 1000, Color.green, 800000.0f);
			RaycastHit[] hits;
			hits = Physics.RaycastAll(rayy, 8000.0F).OrderBy(h => h.distance).ToArray(); ;
			for (int i = 0; i < hits.Length; i++)
			{
				RaycastHit hit = hits[i];
				Renderer rend = hit.transform.GetComponent<Renderer>();
				if (hit.collider.transform.tag == "table")
				{
					return hit.collider.transform.gameObject;
				}
			}
			return null;
		}

		void OnMouseUp()
		{
			doubleClick = false;
		}

		void OnMouseDrag()
		{
			triggerAction.Invoke(gameObject);
			if (/*gameObject.tag == "table" || */gameObject.tag == "class")
			{
				Vector3 delta = Input.mousePosition - lastMousePos;
				Vector3 pos = transform.localPosition;
				pos.y += delta.y * dragSpeed;
				pos.x += delta.x * dragSpeed;
				transform.localPosition = pos;
				lastMousePos = Input.mousePosition;

			}
		}

		void DragNDropClass()
		{
			if (gameObject.transform.tag == "class")
			{
				dragNDropClass = RayForDragNDropClass();
				GameObject classManager = GameObject.Find("TableManager");
				classManager.GetComponent<TableManager>().copiedClass = dragNDropClass;
			}
			else if (gameObject.transform.tag == "table")
			{
				dragNDropTable = RayForDragNDropTable();
				GameObject classManager = GameObject.Find("TableManager");
				dragNDropClass = classManager.GetComponent<TableManager>().copiedClass;
		
				if (!dragNDropClass) return;

				Graph graph = GetComponentInChildren<Graph>();
				GameObject newClass = graph.GetComponent<Graph>().AddNode();
				classManager.GetComponent<TableManager>().CopyClass(dragNDropTable, idOfClass++, newClass);

				WriteAction("addClass", graph.GetComponentInParent<Table>(), newClass, Int32.Parse(newClass.name), null, 0, null, new Vector3(0, 0, 0), null, null);

			}

			if (dragNDropClass && dragNDropTable)
			{
				/*
				//Graph graph = GetComponentInChildren<Graph>();
				//GameObject newClass = graph.GetComponent<Graph>().AddNode();

				
				//get strings from old table
				Transform background = dragNDropClass.gameObject.transform.GetChild(0);
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
				newClass.name = idOfClass++.ToString();

				WriteAction("addClass", graph.GetComponentInParent<Table>(), newClass, Int32.Parse(newClass.name), null, 0, null, new Vector3(0, 0, 0), null, null);

				//idOfClass++;
				Debug.Log("Position of new class " + newClass.transform.position);
				Debug.Log("Position of table " + graph.transform.position);
				Debug.Log("Position of go " + gameObject.transform.position);
				graph.GetComponent<Graph>().UpdateGraph();
				*/
			}
			else Debug.Log("NEJDE");
			
		}



		public void WriteAction(string type, Table table, GameObject class1, int class1ID, GameObject class2, int class2ID, LineRenderer line, Vector3 previousPos, string writtenText, List<int> allAssociations)
		{
			BackManager backManager = table.GetComponentInChildren<BackManager>();
			backManager.GetComponent<BackManager>().WriteAction(type, table, class1, class1ID, class2, class2ID, line, previousPos, writtenText, allAssociations, null);
			/*switch (type)
			{
				case "addClass":
					backManager.GetComponent<BackManager>().AddClassAction(class1, table);
					break;
				case "deleteClass":
					Debug.Log(allAssociations.Count + "xxxxxxxxx");
					backManager.GetComponent<BackManager>().DeleteClassAction(table, class1, class1ID, previousPos, writtenText, allAssociations);
					break;
				case "addAssociation":
					backManager.GetComponent<BackManager>().AddAssociationAction(table, line, class1, class1ID, class2, class2ID);
					break;
				case "deleteAssociation":
					Debug.Log("UNDO ON delete is not working now");
					backManager.GetComponent<BackManager>().DeleteAssociationAction(line, class1,0, class2, 0);
					break;
				case "changeHeader":
					backManager.GetComponent<BackManager>().ChangeHeaderAction(table, class1, class1ID, writtenText);
					break;
				case "changeAttributes":
					backManager.GetComponent<BackManager>().ChangeAttributesAction(table, class1, class1ID, writtenText);
					break;
				case "changeMethods":
					backManager.GetComponent<BackManager>().ChangeMethodsAction(table, class1, class1ID, writtenText);
					break;
				default:
					Debug.Log("WRONG BACK OPERATION");
					break;
			}*/
			
		}

		public List<int> getAllAssociations(GameObject o)
		{
			int myID = Int32.Parse(o.transform.name);
			List<int> allAssociations = new List<int>();
			int name;
			var graphNode = o.GetComponent<UNode>().GraphNode;
			foreach (var edge in graphNode.Edges)
			{
				if (edge.Source.UserData.Equals(o) == false)
				{
					name = Int32.Parse(edge.Source.UserData.ToString().Split(' ')[0]);
					allAssociations.Add(name);
				}
				if (edge.Target.UserData.Equals(o) == false)
				{
					name = Int32.Parse(edge.Target.UserData.ToString().Split(' ')[0]);
					allAssociations.Add(name);
				}
			}
			return allAssociations;
		}

	}
}
