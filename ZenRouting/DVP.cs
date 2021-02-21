using System;
using System.Collections.Generic;

using ZenLib.Tests.Network;
using ZenLib;
using static ZenLib.Language;


/// <summary>
/// Distance Vector Protocol
/// </summary>
namespace ZenRouting
{
	public class DVP
	{
		public List<Node> nodes;
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
				this.nodes.Add(new Node { Address = new Ip { Value = (uint)i } , RoutingTable = new List<Route>()});
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

		/*public Zen<bool> Forward(Zen<int> src, Zen<int> dst)
        {
			return Forward(this.nodes[src].Address, this.nodes[dst].Address);
        }*/

		/// <summary>
		/// 
		/// </summary>
		/// <param name="src">Index to the nodes list</param>
		/// <param name="dst">Index to the nodes list</param>
		/// <returns></returns>
		public Zen<bool> OneHopForward(Zen<Ip> src, Zen<Ip> dst)
		{
			var currFoundNextHop = False();
			// find next hop
			for (int i = 0; i < this.nodes.Count; i++)
            {
				var currNode = this.nodes[i];
				// currFoundNextHop = Or(src == currNode.Address, currFoundNextHop);
				currFoundNextHop = Or(If(src == currNode.Address, currNode.hasNextHop(dst), False()), currFoundNextHop);
			}

			return currFoundNextHop;
		}

		public Zen<Boolean> Forward(Zen<SimplePacket> p)
        {
			HashSet<List<int>> pathSet = new HashSet<List<int>>();
			for (int i = 0; i < nodes.Count; i++)
            {
				for (int j = 0; j < nodes.Count; j++)
                {
					var nextHopIp = nodes[i].getNextHop(nodes[j].Address);
					if (nextHopIp.Value != GlobalVar.NULL_IP.Value)
                    {
						// there is a route
						List<int> currRoute = new List<int>();
						currRoute.Add(i);


						while (nextHopIp.Value != GlobalVar.NULL_IP.Value &&
							nextHopIp.Value != nodes[j].Address.Value)
                        {
							currRoute.Add((int)nextHopIp.Value);
							nextHopIp = nodes[(int)nextHopIp.Value].getNextHop(nodes[j].Address);
                        }


						currRoute.Add(j);
						pathSet.Add(currRoute);
					}
                }
            }
			//var a = new List<int>() { 1, 2 };
			//pathSet.Add(a);

			Zen<Boolean> forwardSuccess = False();
			foreach (List<int> path in pathSet)
            {
				var startNode = nodes[path[0]];
				var endNode = nodes[path[path.Count - 1]];
				forwardSuccess = If(And(p.GetSrcIp() == startNode.Address, p.GetDstIp() == endNode.Address),
					Forward(path.ToArray(), p).HasValue(),
					forwardSuccess);

				
			}
			Console.WriteLine("Final Expression:");
			Console.WriteLine(forwardSuccess);
			Console.WriteLine();
			return forwardSuccess;
        }


		public Zen<Option<SimplePacket>> Forward(int[] path, Zen<SimplePacket> p)
        {
			Zen<Option<SimplePacket>> x = Some(p);
			for (int i = 0; i < path.Length - 1; i++)
            {
				var currNode = nodes[path[i]];
				var nextNode = nodes[path[i + 1]];
				x = If(x.HasValue(), currNode.ForwardInAndOut(x.Value(), nextNode.Address), x);
            }
			return x;
        }


		/*
		public Zen<bool> ForwardWithCost(Zen<Ip> src, Zen<Ip> dst, int cost)
        {
			if (cost < 0)
            {
				return False();
            }

			return If(src == dst, True(), ForwardHelper(src, dst, cost));
		}

		private Zen<bool> ForwardHelper(Zen<Ip> src, Zen<Ip> dst, int cost)
        {
			var currFoundNextHop = False();
			// find next hop
			
			for (int i = 0; i < this.nodes.Count; i++)
			{
				var currNode = this.nodes[i]; // currNode is src
											  // currFoundNextHop = Or(src == currNode.Address, currFoundNextHop);
				//Console.WriteLine(src == currNode.Address);
				//Console.WriteLine("FH: ", src == currNode.Address, src.GetField<Ip, uint>("Value"), currNode.Address);
				//Console.WriteLine();

				currFoundNextHop = Or(If(src == currNode.Address, ForwardWithCost(currNode.getNextHop(dst), dst, cost-1), False()), currFoundNextHop);
			}

			return currFoundNextHop;
		}*/


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