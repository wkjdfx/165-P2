using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class GraphInit : MonoBehaviour {

	public GameObject NodeCube;
	public Text textName;

	Graph graph;
	Layout layout;
	List<GameObject> nodes;
	GameObject selected;
	bool[] isSelected = new bool[62];

	void Start () {
		nodes = new List<GameObject> ();
		Parser parser = new Parser ("F:\\cse165\\p2\\project\\Assets\\Scripts\\dolphinNetwork.gml");
		graph = parser.parse();
		if (graph == null)
			return;
		layout = new Layout (graph);

		layout.init ();
		cubeInit ();	
	}

	// TODO: replace keyboard/mouse control with kinect control
	void Update () {

		/**
		 * selection
		 */
		if (Input.GetMouseButtonDown (0)) 
			select ();
		/**
		 * deletion
		 */
		if (Input.GetKeyDown ("d")) 
			delete ();
		/**
		 * fullview
		 */
		if (Input.GetKeyDown ("f")) 
			fullview ();
		if (Input.GetKeyDown ("i"))
			graphRotate (100, 100, 100);



		if (layout.generate ())
			cubeUpdate ();
		edgeUpdate ();
	}

	void cubeInit(){
		foreach (Graph.Node node in graph.nodes)
		{
			GameObject newNode;

			newNode = (GameObject) Instantiate(NodeCube, node.position, Quaternion.identity); // initialize the cube
			newNode.transform.parent = this.transform; // set the cube as child of dolphinGraph
			newNode.name = node.id.ToString(); // set the cube name
			nodes.Add(newNode); // add the cube to the list
		}
	}

	void cubeUpdate(){
		foreach (Graph.Node node in graph.nodes) {
			if(!node.deleted)
				nodes[node.id].transform.localPosition = node.position;
		}
	}

	void edgeUpdate(){
		foreach (Graph.Edge edge in graph.edges) {

			GameObject source = nodes[edge.source.id];
			GameObject target = nodes[edge.target.id];

			if(edge.source.deleted || edge.target.deleted) // if one of the node is deleted, skip
				continue;

			if(source == selected || target == selected)
				Debug.DrawLine (source.transform.position, target.transform.position, Color.red);
			else
				Debug.DrawLine (source.transform.position, target.transform.position, Color.black);

		}
	}

	void select(){			
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		
		if (Physics.Raycast (ray, out hit)) {
			selected = hit.collider.gameObject; // get the selected object
			int id = Int32.Parse (selected.name); // get the id of the selected object
			selected.GetComponent<Renderer> ().material.color = Color.yellow; // highlight cube
			textName.text = graph.nodes [id].name; // display name
			Vector3 newPos = new Vector3 (350, 0, 0);
			selected.transform.position = newPos;
			graph.nodes [id].position = newPos;
			isSelected [id] = true;
			layout.center = id;
			layout.init ();
		} else {
			textName.text = "DolphinName";
			selected = null;
			for (int i = 0; i < isSelected.Length; i++) {
				isSelected [i] = false;
				if (!graph.nodes [i].deleted)
					nodes [i].GetComponent<Renderer> ().material.color = Color.white;
			}
			layout.center = -1;
		}
	}

	void delete(){
		for(int i = 0; i < isSelected.Length; i++){
			if(isSelected[i]){
				Destroy(nodes[i]);
				graph.nodes[i].deleted = true;
			}
			isSelected[i] = false;
		}
	}

	void fullview(){
		foreach (Graph.Node node in graph.nodes) {
			node.position = node.original;
			layout.init ();
		}	
	}

	// this doesn't work
	void graphRotate(float x, float y, float z){
		this.transform.rotation.Set(
			this.transform.rotation.x + x,
			this.transform.rotation.y + y,
			this.transform.rotation.z + z,
			this.transform.rotation.w
			);
	}
}
