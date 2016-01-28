/**
@author David Piegza

Implements a force-directed layout, the algorithm is based on Fruchterman and Reingold and
the JUNG implementation.

Needs the graph data structure Graph.js:
https://github.com/davidpiegza/Graph-Visualization/blob/master/Graph.js

Parameters:
graph - data structure
options = {
layout: "2d" or "3d"
attraction: <float>, attraction value for force-directed layout
repulsion: <float>, repulsion value for force-directed layout
iterations: <int>, maximum number of iterations
width: <int>, width of the viewport
height: <int>, height of the viewport

positionUpdated: <function>, called when the position of the node has been updated
}

Examples:

create:
layout = new Layout.ForceDirected(graph, {width: 2000, height: 2000, iterations: 1000, layout: "3d"});

call init when graph is loaded (and for reset or when new nodes has been added to the graph):
layout.init();

call generate in a render method, returns true if it's still calculating and false if it's finished
layout.generate();


Feel free to contribute a new layout!

*/

using UnityEngine;
using System.Collections;
using System;


public class Layout {

	/*
	private struct Options {
		//string layout; // "2d" or "3d"
		float attraction; // attraction value for force-directed layout
		float repulsion; // repulsion value for force-directed layout
		float iterations; // maximum number of iterations
		int width; // width of the viewport
		int height; // height of the viewport
		int depth;
	}
	*/

	//string layout = "2d";
	float attraction_multiplier = 5;
	float repulsion_multiplier = 0.75f;
	int max_iterations = 1000;
	Graph graph;
	int width = 200;
	int height = 200;
	int depth = 200;
	bool finished = false;
	
	//var callback_positionUpdated = options.positionUpdated;
	
	float EPSILON = 0.000001f;
	float attraction_constant;
	float repulsion_constant;
	float forceConstant;
	int layout_iterations = 0;
	float temperature = 40;
	float nodes_length;
	float edges_length;
	//var that = this;
	
	// performance test
	// var mean_time = 0;

	public Layout(Graph graph) {

		//layout = opts.layout;
		/*
		attraction_multiplier = opts.;
		repulsion_multiplier = opts.repulsion;
		max_iterations = opts.iterations;
		graph = graph;
		width = opts.width;
		height = opts.height;
		finished = false;	
		*/
		attraction_multiplier = 5;
		repulsion_multiplier = 0.75f;
		max_iterations = 1000;
		this.graph = graph;
		width = 200;
		height = 200;
		depth = 200;
		finished = false;
	}

	/**
		* Initialize parameters used by the algorithm.
		*/
	public void init() {
		finished = false;
		temperature = width;
		nodes_length = graph.nodes.Count;
		edges_length = graph.edges.Count;
		forceConstant = (float)Math.Sqrt(height * width / nodes_length);
		attraction_constant = attraction_multiplier * forceConstant;
		repulsion_constant = repulsion_multiplier * forceConstant;
	}
	
	/**
		* Generates the force-directed layout.
		*
		* It finishes when the number of max_iterations has been reached or when
		* the temperature is nearly zero.
		*/
	public bool generate() {
		if (layout_iterations < this.max_iterations && temperature > 0.000001) {
			//var start = new Date().getTime();
			
			// calculate repulsion
			for (int i = 0; i < nodes_length; i++) {
				Graph.Node node_v = graph.nodes [i];
				//node_v.layout = node_v.layout || {};
				//if (i == 0) {
					node_v.layout.offset_x = 0;
					node_v.layout.offset_y = 0;
					node_v.layout.offset_z = 0;
				//}
				
				node_v.layout.force = 0;
				node_v.layout.tmp_pos_x = node_v.position.x;
				node_v.layout.tmp_pos_y = node_v.position.y;
				node_v.layout.tmp_pos_z = node_v.position.z;
				
				for (int j = i + 1; j < nodes_length; j++) {
					Graph.Node node_u = graph.nodes [j];
					if (i != j) {
						//node_u.layout = node_u.layout || {};
						node_u.layout.tmp_pos_x = node_u.position.x;
						node_u.layout.tmp_pos_y = node_u.position.y;
						node_u.layout.tmp_pos_z = node_u.position.z;
						
						float delta_x = node_v.layout.tmp_pos_x - node_u.layout.tmp_pos_x;
						float delta_y = node_v.layout.tmp_pos_y - node_u.layout.tmp_pos_y;
						float delta_z = node_v.layout.tmp_pos_z - node_u.layout.tmp_pos_z;
						
						float delta_length = (float)Math.Max (EPSILON, Math.Sqrt ((delta_x * delta_x) + (delta_y * delta_y)));
						float delta_length_z = (float)Math.Max (EPSILON, Math.Sqrt ((delta_z * delta_z) + (delta_y * delta_y)));
						
						float force = (repulsion_constant * repulsion_constant) / delta_length;
						//var force_z = (repulsion_constant * repulsion_constant) / delta_length_z;
						
						node_v.layout.force += force;
						node_u.layout.force += force;
						
						node_v.layout.offset_x += (delta_x / delta_length) * force;
						node_v.layout.offset_y += (delta_y / delta_length) * force;
						
						if (i == 0) {
							node_u.layout.offset_x = 0;
							node_u.layout.offset_y = 0;
							node_u.layout.offset_z = 0;
						}
						node_u.layout.offset_x -= (delta_x / delta_length) * force;
						node_u.layout.offset_y -= (delta_y / delta_length) * force;

						node_v.layout.offset_z += (delta_z / delta_length_z) * force;
						node_u.layout.offset_z -= (delta_z / delta_length_z) * force;
					}
				}
			}
			
			// calculate attraction
			for (int i = 0; i < edges_length; i++) {
				Graph.Edge edge = graph.edges [i];
				float delta_x = edge.source.layout.tmp_pos_x - edge.target.layout.tmp_pos_x;
				float delta_y = edge.source.layout.tmp_pos_y - edge.target.layout.tmp_pos_y;
				float delta_z = edge.source.layout.tmp_pos_z - edge.target.layout.tmp_pos_z;
				
				float delta_length = (float)Math.Max (EPSILON, Math.Sqrt ((delta_x * delta_x) + (delta_y * delta_y)));
				float delta_length_z = (float)Math.Max (EPSILON, Math.Sqrt ((delta_z * delta_z) + (delta_y * delta_y)));
				float force = (delta_length * delta_length) / attraction_constant;
				//var force_z = (delta_length_z * delta_length_z) / attraction_constant;
				
				edge.source.layout.force -= force;
				edge.target.layout.force += force;
				
				edge.source.layout.offset_x -= (delta_x / delta_length) * force;
				edge.source.layout.offset_y -= (delta_y / delta_length) * force;
				edge.source.layout.offset_z -= (delta_z / delta_length_z) * force;
				
				edge.target.layout.offset_x += (delta_x / delta_length) * force;
				edge.target.layout.offset_y += (delta_y / delta_length) * force;
				edge.target.layout.offset_z += (delta_z / delta_length_z) * force;
			}
			
			// calculate positions
			for (var i = 0; i < nodes_length; i++) {
				Graph.Node node = graph.nodes [i];
				float delta_length = (float)Math.Max (EPSILON, Math.Sqrt (node.layout.offset_x * node.layout.offset_x + node.layout.offset_y * node.layout.offset_y));
				float delta_length_z = (float)Math.Max (EPSILON, Math.Sqrt (node.layout.offset_z * node.layout.offset_z + node.layout.offset_y * node.layout.offset_y));
				
				node.layout.tmp_pos_x += (node.layout.offset_x / delta_length) * Math.Min (delta_length, temperature);
				node.layout.tmp_pos_y += (node.layout.offset_y / delta_length) * Math.Min (delta_length, temperature);
				node.layout.tmp_pos_z += (node.layout.offset_z / delta_length_z) * Math.Min (delta_length_z, temperature);
				
				//bool updated = true;
				node.position.x -= (node.position.x - node.layout.tmp_pos_x) / 10;
				node.position.y -= (node.position.y - node.layout.tmp_pos_y) / 10;
				node.position.z -= (node.position.z - node.layout.tmp_pos_z) / 10;

				/*
				// execute callback function if positions has been updated
				if (updated && typeof callback_positionUpdated == = 'function') {
					callback_positionUpdated(node);
				}
				*/
			}

			temperature *= (1 - ((float)layout_iterations / (float)this.max_iterations));
			Debug.Log ("iterations = " + layout_iterations + "\ntemperature = " + temperature);
			layout_iterations++;
			
			//var end = new Date().getTime();
			//mean_time += end - start;
		} else
			return false;
		/*
		else {
			if (!this.finished) {
				console.log("Average time: " + (mean_time / layout_iterations) + " ms");
			}
			this.finished = true;
			return false;
		}
		*/
		return true;
	}
	
	/**
		* Stops the calculation by setting the current_iterations to max_iterations.
		*/

	private void stop_calculating() {
		layout_iterations = this.max_iterations;
	}

}