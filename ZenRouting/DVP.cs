using System;
using System.Collections.Generic;

using ZenLib.Tests.Network;


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
			/*
			this.nodes = new List<Node>();
			for (int i = 0; i < numNodes; i++)
            {
				this.nodes.Add(new Node { Address = new Ip { Value = i} });
            }

			this.edges = new HashSet<Tuple<Ip, Ip>>();
			*/

			init(numNodes);
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

			/*
			this.nodes = new List<Node>();
			this.edges = new HashSet<Tuple<Ip, Ip>>();
			for (int i = 0; i < edgeNodes.Count; i++)
			{
				this.nodes.Add(new Node { Address = new Ip { Value = i } });
			}
			*/
			init(edgeNodes.Count);

			foreach (Tuple<int, int> r in physicalEdges)
			{
				var (i, j) = r;
				edges.Add(new Tuple<int, int>(i, j));
			}

			this.initializeRoutingTables();
		}

		private void init(int count)
        {
			this.nodes = new List<Node>();
			this.edges = new HashSet<Tuple<int, int>>();
			for (int i = 0; i < count; i++)
			{
				this.nodes.Add(new Node { Address = new Ip { Value = (uint)i } });
			}
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

					Route r_ij = new Route
					{
						Destination = nodes[j].Address,
						NextHop = nodes[j].Address,
						Cost = 1,
						TTL = GlobalVar.MAX_TTL
					};
					Route r_ji = new Route
					{
						Destination = nodes[i].Address,
						NextHop = nodes[i].Address,
						Cost = 1,
						TTL = GlobalVar.MAX_TTL
					};

					if (edges.Contains(new Tuple<int, int>(i, j)))
                    {
						nodes[i].MergeRoute(r_ij);
						nodes[j].MergeRoute(r_ji);
					}
					else
                    {
						r_ij.NextHop = GlobalVar.NULL_IP;
						r_ij.Cost = GlobalVar.MAX_HOPS;
						r_ji.NextHop = GlobalVar.NULL_IP;
						r_ji.Cost = GlobalVar.MAX_HOPS;
						nodes[i].MergeRoute(r_ij);
						nodes[j].MergeRoute(r_ji);
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
					updatedNodes.Add(new Node { Address = n.Address, RoutingTable = new List<Route>(n.RoutingTable) });
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