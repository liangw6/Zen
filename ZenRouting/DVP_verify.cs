using System;
using System.Collections.Generic;

using ZenLib.Tests.Network;
using ZenLib;
using static ZenLib.Language;
/// <summary>
/// Distance Vector Protocol with proper (hopefully) Control plane verification
/// </summary>
namespace ZenRouting
{
	public class DVP_verify
	{
        // Each DVP_Verify is a fixed topology with property
        List<Tuple<int, int>> physicalEdges;
        int dst_node;
        Dictionary<int, HashSet<int>> node2neighbors;


        public DVP_verify(List<Tuple<int, int>> physicalEdges, int dst_node)
		{
            this.physicalEdges = physicalEdges;
            this.dst_node = dst_node;
            this.node2neighbors = new Dictionary<int, HashSet<int>>();


            foreach (Tuple<int, int> edge in physicalEdges)
            {
                var (i, j) = edge;
                if (!node2neighbors.ContainsKey(i))
                {
                    node2neighbors[i] = new HashSet<int>();
                }
                node2neighbors[i].Add(j);

                if (!node2neighbors.ContainsKey(j))
                {
                    node2neighbors[j] = new HashSet<int>();
                }
                node2neighbors[j].Add(i);
            }
        }

        public Zen<bool> Build_Network(Zen<IList<int>> costs)
        {
            Zen<bool> network_expr = True();
            foreach (KeyValuePair<int, HashSet<int>> kvp in node2neighbors)
            {
                // costs.At(1).Value() == Min(costs.At(0).Value() + 1, costs.At(2).Value() + 1),
                Zen<ushort> i = (Zen<ushort>)kvp.Key;
                // this is just i, but it doesn't allow me to cast -_-
                if (kvp.Key == dst_node)
                {
                    continue;
                }
                Console.Write(kvp.Key + ": [");

                Zen<bool> lower_limit_expr = True();
                foreach (Zen<ushort> j in kvp.Value)
                {
                    Console.Write(j + ", ");
                    lower_limit_expr = And(lower_limit_expr, costs.At(i).Value() <= costs.At(j).Value() + 1);
                }

                Zen<bool> upper_limit_expr = False();
                foreach (Zen<ushort> j in kvp.Value)
                {
                    upper_limit_expr = Or(upper_limit_expr, costs.At(i).Value() >= costs.At(j).Value() + 1);
                }
                Console.WriteLine("]");

                network_expr = And(network_expr, And(lower_limit_expr, upper_limit_expr));
            }
            Console.WriteLine("physical edges" + physicalEdges.Count);
            Console.WriteLine("Network Expression " + network_expr);
            return network_expr;
        }

        // 0 - 1 - 2 (0 - 2)
        // The function encodes the property we want to verify
        public Zen<bool> Simple_CPV(Zen<IList<int>> costs, int src_node)
        {
            return And(And(costs.At((Zen<ushort>)this.dst_node).Value() == 0, costs.At((Zen<ushort>)src_node).Value() <= GlobalVar.MAX_HOPS), costs.Length() == 7);
        }

    }
}

