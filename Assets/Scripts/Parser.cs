using System;
using System.Collections;


public class Parser{
	
	private string filepath;

	// Function to read one line from a specified stream.  Return value is
	// 1 if an EOF was encountered.  Otherwise 0.

	public Parser(string filepath)
	{
		this.filepath = filepath;
	}

	public Graph parse()
	{
		Graph graph = new Graph();
		string line;
		Random rng = new Random();

		// Read the file and line by line and fill the graph.
		System.IO.StreamReader file = 
			new System.IO.StreamReader(filepath);

		if (file == null) 
		{
			Console.WriteLine ("file not found");
			return null;
		}

		while((line = file.ReadLine()) != null)
		{
			if(line.Equals("  node"))
			{
				int id;
				string sid;
				string name;
				string[] words;
				char[] separatingChars = { ' ' };

				file.ReadLine(); // [

				sid = file.ReadLine(); // id ie. id 0
				words = sid.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries );
				id = Int32.Parse(words[1]);

				name = file.ReadLine(); // name ie. lable "Beak"
				words = name.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries );
				name = words[1];

				graph.addNode(new Graph.Node(id, name, rng.Next(-100, 100), rng.Next(-100, 100), rng.Next(-100, 100)));

			}
			else if(line.Equals("  edge"))
			{
				int target, source;
				string s;
				string[] words;
				char[] separatingChars = { ' ' };

				file.ReadLine(); // [

				s = file.ReadLine(); // source ie. source 19
				words = s.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries );
				source = Int32.Parse(words[1]);

				s = file.ReadLine(); // target ie. target 1
				words = s.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries );
				target = Int32.Parse(words[1]);

				graph.addEdge(source, target);
			}
		}

		file.Close();
		return graph;
	}
}
