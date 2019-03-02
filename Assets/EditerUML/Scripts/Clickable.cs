using System;
using System.Collections;
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
		public float dragSpeed = 0.3f;
		Vector3 lastMousePos;

		Vector3 dist;
		float posX;
		float posY;

		public GameObjectEvent triggerAction;

		public GameObject textManagerScript;

		static GameObject a = null;
		static GameObject b = null;

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
			Vector3 GOPos = gameObject.transform.position;
			v3T.z = GOPos.z;
			v3T = Camera.main.ScreenToWorldPoint(v3T);
			Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
			Debug.DrawRay(v3T, forward, Color.green, 1000.0f);

			RaycastHit[] hits;
			hits = Physics.RaycastAll(v3T, forward, 500.0F);
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
			triggerAction.Invoke(gameObject);
			
			//assign first, then second class 
			if (a == null)
				a = gameObject;
			else
				b = gameObject;

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
					Vector3 p = c.transform.position;
					p.z -= 2.0f;
					c.transform.position = p;
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
			Vector3 classPosition = Ray();
			triggerAction.Invoke(gameObject);

			//if we didnt click on table (but on class ..) we will not create another class
			if (!isTable(gameObject))
				return;
			
			Graph graph = GetComponentInChildren<Graph>();
			GameObject a = graph.GetComponent<Graph>().AddNode();

			classPosition.z = graph.transform.position.z;
			a.transform.position = classPosition;


			a.tag = "class";
			a.name = "Class " + idOfClass++.ToString();
			//idOfClass++;
			Debug.Log("Position of new class " +a.transform.position);
			Debug.Log("Position of table " +graph.transform.position);
			Debug.Log("Position of go " + gameObject.transform.position);
			graph.GetComponent<Graph>().UpdateGraph();
		}

		Vector3 Ray()
		{
			Vector3 v3T = Input.mousePosition;
			v3T = Camera.main.ScreenToWorldPoint(v3T);
			Vector3 forward = transform.TransformDirection(Vector3.back) * 100;
			//Debug.DrawRay(v3T, forward, Color.green, 100022.0f);
			RaycastHit[] hits;
			hits = Physics.RaycastAll(v3T, forward, 50022.0F);
			for (int i = 0; i < hits.Length; i++)
			{
				RaycastHit hit = hits[i];
				Renderer rend = hit.transform.GetComponent<Renderer>();

				if (hit.collider.transform.tag == "table")
				{
					Debug.Log("AAAAAAA");
					Vector3 localHit = hit.point;

					Debug.DrawLine(Camera.main.transform.position, hit.point);
					//Debug.Log("local hit is " + localHit);

					GameObject c = Instantiate(testCube);
					c.transform.position =localHit;
					return localHit;
				}
			}

			return new Vector3(0, 0, 0);
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
	}
}
