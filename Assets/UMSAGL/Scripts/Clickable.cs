using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
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

		static GameObject a = null;
		static GameObject b = null;

		static Graph parent1 = null;
		static Graph parent2 = null;

		static GameObject firstNode = null, secondNode = null;

		private Vector3 screenPoint;
		private Vector3 offset;

		private TextMeshProUGUI textMesh;

		private float delta;

		private float time;

		private bool doubleClick = false;

		void Update()
		{
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

		void ChangeText()
		{
			Vector3 v3T = Input.mousePosition;
			Vector3 GOPos = gameObject.transform.position;
			v3T.z = GOPos.z;
			v3T = Camera.main.ScreenToWorldPoint(v3T);
			Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
			Debug.DrawRay(v3T, forward, Color.green, 1000.0f);

			//clanky - octobubbles Yolag
			RaycastHit[] hits;
			hits = Physics.RaycastAll(v3T, forward, 100.0F);

			for (int i = 0; i < hits.Length; i++)
			{
				RaycastHit hit = hits[i];
				Renderer rend = hit.transform.GetComponent<Renderer>();
				//TODO: set size for clicks (when too close to table, error)
				Debug.Log(hit.collider.transform.name);
			}

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

		//create New Association
		void OnRealMouseDown()
		{
			triggerAction.Invoke(gameObject);
			
			//assign first, then second class 
			if (a == null)
				a = gameObject;
			else
				b = gameObject;

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
				else
				{
					GameObject obj = GameObject.Find("DimensionalEdgesManager");
					obj.GetComponent<DimensionalEdges>().createDimensionalAssociation(a, b);
				}
				a = null; b = null;
				parent1 = null; parent2 = null;
			}
		}
		//create New Class
		private void OnDoubleClick()
		{
			triggerAction.Invoke(gameObject);
			Vector3 v3T = Input.mousePosition;
			Vector3 GOPos = gameObject.transform.position;
			v3T.z = GOPos.z;
			v3T = Camera.main.ScreenToWorldPoint(v3T);

			Graph graph = GetComponentInChildren<Graph>();
			Vector3 graphPosition = graph.transform.localPosition;
			GameObject a = graph.GetComponent<Graph>().AddNode();
			a.transform.localPosition = new Vector3(v3T.x, v3T.y, graphPosition.z - 10f);


			a.tag = "class";
			Debug.Log("Position of new class " +a.transform.position);
			Debug.Log("Position of table " +graph.transform.position);
			Debug.Log("Position of go " + gameObject.transform.position);
			graph.GetComponent<Graph>().UpdateGraph();
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
