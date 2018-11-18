using UnityEngine;
using Microsoft.Msagl.Miscellaneous;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Core.Routing;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Msagl.Core.DataStructures;
using System.Collections;

public class Graph : MonoBehaviour
{

	public GameObject nodePrefab;
	public GameObject edgePrefab;
	public float factor = 0.2f;
	public float depth;

	private GeometryGraph graph;
	private LayoutAlgorithmSettings settings;

	private Task router;
	private bool reroute = false;
	private bool redraw = true;

	private Transform units;

	public void Center()
	{
		graph.UpdateBoundingBox();
		units.localPosition = new Vector3(ToUnitySpace(graph.BoundingBox.Center.X), ToUnitySpace(graph.BoundingBox.Center.Y)) * -1.0f;
	}

	public void Layout()
	{
		LayoutHelpers.CalculateLayout(graph, settings, null);
		PositionNodes();
		RedrawEdges();
		Center();
	}

	public GameObject AddNode()
	{
		var go = GameObject.Instantiate(nodePrefab, units);

		//Following step required otherwise Size will return wrong rect
		Canvas.ForceUpdateCanvases();

		var unode = go.GetComponent<UNode>();
		double w = ToGraphSpace(unode.Size.width);
		double h = ToGraphSpace(unode.Size.height);

		Node node = new Node(CurveFactory.CreateRectangle(w, h, new Point()));
		node.UserData = go;
		unode.GraphNode = node;
		graph.Nodes.Add(node);

		return go;
	}

	public void RemoveNode(GameObject node)
	{
		var graphNode = node.GetComponent<UNode>().GraphNode;
		foreach (var edge in graphNode.Edges)
		{
			GameObject.Destroy((GameObject)edge.UserData);
			//in MSAGL edges are automatically removed, only UnityObjects have to be removed
		}
		graph.Nodes.Remove(graphNode);
		GameObject.Destroy(node);
	}

	public GameObject AddEdge(GameObject from, GameObject to)
	{
		var go = GameObject.Instantiate(edgePrefab, units);
		var uEdge = go.GetComponent<UEdge>();
		Edge edge = new Edge(from.GetComponent<UNode>().GraphNode, to.GetComponent<UNode>().GraphNode);
		edge.LineWidth = ToGraphSpace(uEdge.Width);
		edge.UserData = go;
		uEdge.graphEdge = edge;
		graph.Edges.Add(edge);
		return go;
	}

	public void RemoveEdge(GameObject edge)
	{
		graph.Edges.Remove(edge.GetComponent<UEdge>().graphEdge);
		GameObject.Destroy(edge);
	}

	double ToGraphSpace(float x)
	{
		return x / factor;
	}

	float ToUnitySpace(double x)
	{
		return (float)x * factor;
	}

	void Awake()
	{
		graph = new GeometryGraph();
		units = transform.Find("Units"); //extra object to center graph
		settings = new SugiyamaLayoutSettings();
		settings.EdgeRoutingSettings.EdgeRoutingMode = EdgeRoutingMode.RectilinearToCenter;
	}

	void PositionNodes()
	{
		foreach (var node in graph.Nodes)
		{
			var go = (GameObject)node.UserData;
			go.transform.localPosition = new Vector3(ToUnitySpace(node.Center.X), ToUnitySpace(node.Center.Y), 0.0f);
		}
	}

	void UpdateNodes()
	{
		foreach (var node in graph.Nodes)
		{
			var go = (GameObject)node.UserData;
			node.Center = new Point(ToGraphSpace(go.transform.localPosition.x), ToGraphSpace(go.transform.localPosition.y));
			var unode = go.GetComponent<UNode>();
			node.BoundingBox = new Rectangle(new Size(ToGraphSpace(unode.Size.width), ToGraphSpace(unode.Size.height)), node.Center);
		}
	}

	void RedrawEdges()
	{
		foreach (var edge in graph.Edges)
		{
			List<Vector3> vertices = new List<Vector3>();
			GameObject go = (GameObject)edge.UserData;

			Curve curve = edge.Curve as Curve;
			if (curve != null)
			{
				Point p = curve[curve.ParStart];
				vertices.Add(new Vector3(ToUnitySpace(p.X), ToUnitySpace(p.Y), 0));	//TODO: change values here
				foreach (ICurve seg in curve.Segments)
				{
					p = seg[seg.ParEnd];
					vertices.Add(new Vector3(ToUnitySpace(p.X), ToUnitySpace(p.Y), 0));
				}
			}
			else
			{
				LineSegment ls = edge.Curve as LineSegment;
				if (ls != null)
				{
					Point p = ls.Start;
					vertices.Add(new Vector3(ToUnitySpace(p.X), ToUnitySpace(p.Y),	0));
					p = ls.End;
					vertices.Add(new Vector3(ToUnitySpace(p.X), ToUnitySpace(p.Y), 0));
				}
			}

			var lineRenderer = go.GetComponent<LineRenderer>();
			lineRenderer.positionCount = vertices.Count;
			lineRenderer.SetPositions(vertices.ToArray());
		}
	}

	private async void ForgetRouter(Task t)
	{
		await t;
		router = null;
		redraw = true;
	}

	public void UpdateGraph()
	{
		reroute = true;
	}

	private void Update()
	{
		if (reroute)
		{
			if (router == null)
			{
				UpdateNodes();
				router = Task.Run(() => LayoutHelpers.RouteAndLabelEdges(graph, settings, graph.Edges));
				ForgetRouter(router);
				reroute = false;
			}
		}
		if (redraw)
		{
			RedrawEdges();
			redraw = false;
		}
	}


}
