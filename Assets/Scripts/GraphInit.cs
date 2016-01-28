using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphInit : MonoBehaviour {

	public GameObject NodeCube;
	Graph graph;
	Layout layout;
	List<GameObject> nodes;

	void Start () {
		nodes = new List<GameObject> ();
		Parser parser = new Parser ("F:\\cse165\\p2\\project\\Assets\\Scripts\\dolphinNetwork.gml");
		graph = parser.parse();
		if (graph == null)
			return;
		layout = new Layout (graph);
		layout.init ();

		cubeInit ();
		//Instantiate(NodeCube, new Vector3(0, 0, 0), Quaternion.identity);
	}

	void Update () {
		layout.generate ();
		cubeUpdate ();
		edgeUpdate ();
	}

	void cubeInit(){
		foreach (Graph.Node node in graph.nodes)
		{
			float x, y, z;
			x = node.position.x;
			y = node.position.y;
			z = node.position.z;
			nodes.Add((GameObject) Instantiate(NodeCube, new Vector3(x, y, z), Quaternion.identity));
		}
	}

	void cubeUpdate(){
		foreach (Graph.Node node in graph.nodes)
		{
			float x, y, z;
			x = node.position.x;
			y = node.position.y;
			z = node.position.z;
			nodes[node.id].transform.position = new Vector3(x, y, z);
		}
	}

	void edgeUpdate(){
		foreach (Graph.Edge edge in graph.edges) {
			Debug.DrawLine (edge.source.position, edge.target.position, Color.black);
		}
	}
}
