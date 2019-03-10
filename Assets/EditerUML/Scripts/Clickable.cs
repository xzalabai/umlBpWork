using System;
using System.Collections;
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
		public float dragSpeed = 117.9f;
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

		private float idOfClass = 1;

		private float time;

		private bool dragON = false;
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
				string parsedText = t.Replace(";", "\n");
				whereToWrite.text = (t.Replace("; ", "\n"));
				//whereToWrite.text = t;

				//whereToWrite.text = "AHOj" + "\n" + "apfpp";
				whereToWrite = null;
				textManagerScript.GetComponent<TextManager>().HideOrUnihdeInputField(true);
				writingInClass = false;
			}
			
			/*Vector3 v3T = Input.mousePosition;
			Vector3 GOPos = gameObject.transform.position;
			v3T.z = GOPos.z;
			v3T = Camera.main.ScreenToWorldPoint(v3T);
			Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
			Debug.DrawRay(v3T, forward, Color.green, 1000.0f);*/
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
			else if (Input.GetKey(KeyCode.R)) {
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
			Debug.Log("xxx " + gameObject.tag);
			if (gameObject.tag == "class"){
				g.GetComponent<Graph>().RemoveNode(gameObject);
			}
			else if (gameObject.tag == "line"){
				g.GetComponent<Graph>().RemoveEdge(gameObject);
			}
			else if (gameObject.tag == "dimensionalLine"){
				Debug.Log("ZLEEEEEE");
				LineRenderer lr = gameObject.GetComponent<LineRenderer>();
				GameObject obj = GameObject.Find("DimensionalEdgesManager");
				obj.GetComponent<DimensionalEdges>().deleteDimensionalAssociation(lr);
			}
		}


		bool writingInClass = false;
		TextMeshProUGUI whereToWrite;
		public void ChangeText()
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
				//TODO: set size for clicks (when too close to table, error)
				Debug.Log(hit.collider.transform.name);
				if (hit.collider.transform.name == "Methods" || hit.collider.transform.name == "Header" || hit.collider.transform.name == "Attributes")
				{
					textManagerScript.GetComponent<TextManager>().HideOrUnihdeInputField(false);
					TextMeshProUGUI t = hit.collider.transform.GetComponent<TextMeshProUGUI>();
					textManagerScript.GetComponent<TextManager>().SetInputFieldText(t.text);
					writingInClass = true;
					whereToWrite = t;
				}

			}

			//yield return waitForKeyPress(KeyCode.Space);

			/*RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			//Debug.DrawRay(Input.mousePosition, ray.direction, Color.green);
			if (Physics.Raycast(ray, out hit))
			{
				Transform objectHit = hit.transform;

				Debug.Log(hit.collider.name);
			}*/


			//triggerAction.Invoke(gameObject);
			//TextMeshProUGUI textM = gameObject.GetComponent<TextMeshProUGUI>();
		
		}

		private IEnumerator waitForKeyPress(KeyCode key)
		{
			bool done = false;
			while (!done) // essentially a "while true", but with a bool to break out naturally
			{
				if (Input.GetKeyDown(key))
				{
					done = true; // breaks the loop
				}
				yield return null; // wait until next frame, then continue execution from here (loop continues)
			}

			// now this function returns
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
			if (isClass(a) == false || isClass(b) == false)
			{
				a = null; b = null; parent1 = null; parent2 = null;
			}

			//if the 2 tagged objects are same, we are not assigning them a association
			if (a == b)
			{
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
					//c.transform.localPosition = new Vector3(c.transform.localPosition.x, c.transform.localPosition.y, c.transform.localPosition.z + 13.0f);
					g.GetComponent<Graph>().UpdateGraph();

					c.tag = "line";
				}
				else if (a != null && b != null)
				{
					GameObject obj = GameObject.Find("DimensionalEdgesManager");
					obj.GetComponent<DimensionalEdges>().createDimensionalAssociation(a, b);

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
			a.transform.position = classPosition;
			a.transform.localPosition = new Vector3(a.transform.localPosition.x, a.transform.localPosition.y, a.transform.localPosition.z - 0.05f);

			a.tag = "class";
			a.name = "Class " + idOfClass++.ToString();
			//idOfClass++;
			Debug.Log("Position of new class " +a.transform.position);
			Debug.Log("Position of table " +graph.transform.position);
			Debug.Log("Position of go " + gameObject.transform.position);
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

		IEnumerator WaitForInstruction()
		{
			while (true)
			{
				if (Input.GetMouseButtonDown(0))
				{
					Debug.Log("CAKAM ---");
					RaycastHit hit = new RaycastHit();
					if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
					{
						if (hit.transform.gameObject.tag == "table")
						{
							Debug.Log("IDEEEE");
							yield break;
						}
					}
				}
				yield return null;
			}
			//not here yield return null;
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
			//TODO: is local Position good ?
			triggerAction.Invoke(gameObject);
			if (gameObject.tag == "table" || gameObject.tag == "class")
			{
				Vector3 delta = Input.mousePosition - lastMousePos;
				Vector3 pos = transform.localPosition;
				pos.y += delta.y * dragSpeed;
				pos.x += delta.x * dragSpeed;
				transform.localPosition = pos;
				lastMousePos = Input.mousePosition;

			}
		}

		GameObject dragNDropClass = null;
		GameObject dragNDropTable = null;
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
			}

			if (dragNDropClass && dragNDropTable)
			{
				//copy.transform.parent = dragNDropTable.transform;

				Graph graph = GetComponentInChildren<Graph>();
				GameObject newClass = graph.GetComponent<Graph>().AddNode();


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
				newClass.name = "Class " + idOfClass++.ToString();
				//idOfClass++;
				Debug.Log("Position of new class " + newClass.transform.position);
				Debug.Log("Position of table " + graph.transform.position);
				Debug.Log("Position of go " + gameObject.transform.position);
				graph.GetComponent<Graph>().UpdateGraph();
			}
			else Debug.Log("NEJDEEE");
			
		}
	}
}
