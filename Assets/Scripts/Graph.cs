/**
  @author David Piegza

  Implements a graph structure.
  Consists of Graph, Nodes and Edges.


  Nodes:
  Create a new Node with an id. A node has the properties
  id, position and data.

  Example:
  node = new Node(1);
  node.position.x = 100;
  node.position.y = 100;
  node.data.title = "Title of the node";

  The data property can be used to extend the node with custom
  informations. Then, they can be used in a visualization.


  Edges:
  Connects to nodes together.
  
  Example:
  edge = new Edge(node1, node2);

  An edge can also be extended with the data attribute. E.g. set a
  type like "friends", different types can then be draw in differnt ways. 


  Graph:
  
  Parameters:
  options = {
    limit: <int>, maximum number of nodes
  }

  Methods:
  addNode(node) - adds a new node and returns true if the node has been added,
                  otherwise false.
  getNode(node_id) - returns the node with node_id or undefined, if it not exist
  addEdge(node1, node2) - adds an edge for node1 and node2. Returns true if the
                          edge has been added, otherwise false (e.g.) when the
                          edge between these nodes already exist.
  
  reached_limit() - returns true if the limit has been reached, otherwise false

 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph {

	public class Node{
		public int id;
		public string name;
		public List<Node> nodesTo;
		public List<Node> nodesFrom;
		public Vector3 position;
		public Layout layout;

		public struct Layout{
			public float offset_x, offset_y, offset_z;
			public float tmp_pos_x, tmp_pos_y, tmp_pos_z;
			public float force;
		}

		//public struct Position{ public float x,y,z; public Position(float mx,float my,float mz){x = mx;y = my;z = mz;}}
		
		public Node(int id, string name, float x, float y, float z) {
			//Random rnd = new Random();

			this.id = id;
			this.name = name;
			this.nodesTo = new List<Node>();
			this.nodesFrom = new List<Node>();
			//this.position = new Position(rnd.Next(-width, width), rnd.Next(-height, height), rnd.Next(-depth, depth));
			this.position = new Vector3(x, y, z);
			this.layout = new Layout();
		}		

		public void print(){
			Console.WriteLine ("id = " + id + "\nname = " + name + "\nx = " + position.x + " y = " + position.y + " z = " + position.z);
		}
	}
	
	public class Edge{
		public Node source;
		public Node target;
		
		public Edge(Node source, Node target) {
			this.source = source;
			this.target = target;
		}
	}

	public List<Node> nodes;
	public List<Edge> edges;

	public Graph() {
		this.nodes = new List<Node>();
		this.edges = new List<Edge>();
	}
	
	public void addNode(Node node) {
		nodes.Add (node);
	}
	
	public Node getNode(int node_id) {
		return nodes[node_id];
	}
	
	public void addEdge(int source, int target) {
		edges.Add (new Edge(nodes[source], nodes[target]));
		nodes[source].nodesTo.Add(nodes[target]);
		nodes[target].nodesFrom.Add(nodes[source]);
	}
}
