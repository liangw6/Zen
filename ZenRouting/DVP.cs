using System;
using System.Collections.Generic;


/// <summary>
/// Distance Vector Protocol
/// </summary>
namespace ZenRouting
{
	public class DVP
	{
		List<Node> nodes;
		// int is the index to nodes
		HashSet<Tuple<int, int>> edges;

		public DVP(int numNodes)
		{
			this.nodes = new List<Node>();
			for (int i = 0; i < numNodes; i++)
            {
				this.nodes.Add(new Node(i));
            }

			this.edges = new HashSet<Tuple<int, int>>();
		}

		public DVP(List<Tuple<int, int>> physicalEdges)
        {
			HashSet<int> edgeNodes = new HashSet<int>();
			foreach (Tuple<int, int> r in physicalEdges)
            {
				var (i, j) = r;
				edgeNodes.Add(i);
				edgeNodes.Add(j);
			}

			this.nodes = new List<Node>();
			this.edges = new HashSet<Tuple<int, int>>();
			for (int i = 0; i < edgeNodes.Count; i++)
			{
				this.nodes.Add(new Node(i));
			}

			foreach (Tuple<int, int> r in physicalEdges)
			{
				var (i, j) = r;
				edges.Add(new Tuple<int, int>(i, j));
			}

			this.initializeRoutingTables();
		}

		public void initializeRoutingTables()
        {
			// i, j, inf
			// i, j, 1 if there is a link

			for (int i = 0; i < this.nodes.Count; i++)
            {
				for (int j = 0; j < this.nodes.Count; j++)
                {
					if (i == j)
                    {
						continue;
                    }
					if (edges.Contains(new Tuple<int, int>(i, j)))
                    {
						nodes[i].MergeRoute(new Route(j, j, 1, GlobalVar.MAX_TTL));
						nodes[j].MergeRoute(new Route(i, i, 1, GlobalVar.MAX_TTL));
					}
					else
                    {
						nodes[i].MergeRoute(new Route(j, -1, GlobalVar.MAX_HOPS, GlobalVar.MAX_TTL));
						nodes[j].MergeRoute(new Route(i, -1, GlobalVar.MAX_HOPS, GlobalVar.MAX_TTL));
					}
                }
            }

        }
		

		/// <summary>
		/// 
		/// </summary>
		/// <param name="A">Index to nodes list</param>
		/// <param name="B">Index to nodes list</param>
		public void addEdge(int A, int B)
        {
			edges.Add(new Tuple<int, int>(A, B));
        }


		public void runDVP(int max_iter)
		{
			for (int iter = 0; iter < max_iter; iter++)
            {

				var updatedNodes = new List<Node>();
				foreach (Node n in this.nodes)
                {
					updatedNodes.Add(new Node(n));
                }

				foreach (Tuple<int, int> edge in this.edges)
                {
					var (i, j) = edge;

					updatedNodes[i].UpdateRoutingTable(nodes[j].sendRoutingTable());
					updatedNodes[j].UpdateRoutingTable(nodes[i].sendRoutingTable());
				}

				for (int i = 0; i < this.nodes.Count; i++)
                {
					this.nodes[i] = updatedNodes[i];
                }
            }
        }



		public override String ToString()
        {
			var output = "";
			foreach (Node n in this.nodes)
            {
				output += n.ToString() + "\n";
            }
			return output;
		}
	}
}