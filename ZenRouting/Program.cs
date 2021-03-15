using System;

namespace ZenRouting
{
    using ZenLib;
    using static ZenLib.Language;
    using ZenLib.ModelChecking;
    using System.Collections.Generic;
    using ZenLib.Tests.Network;
    using System.Linq;

    class Program
    {

        static void oneHopReachability(DVP dvp)
        {

            // 1. Evaluate
            Ip srcAddr = new Ip
            {
                Value = 1
            };

            Ip dstAddr = new Ip
            {
                Value = 2
            };

            ZenFunction<Ip, Ip, bool> f = Function<Ip, Ip, bool>(dvp.OneHopForward);
            var output = f.Evaluate(srcAddr, dstAddr);
            Console.WriteLine(output);

            // 2. Find
            var input = f.Find((src, dst, result) => And(And(And(src.GetField<Ip, uint>("Value") < 7,
                dst.GetField<Ip, uint>("Value") < 7), result == False()), src != dst));
            Console.WriteLine("Using powerful Zen Find!!!");

            Console.WriteLine(input);
            Console.WriteLine("printing single value");
            Console.WriteLine(input.Value);
        }
		
		static void evaluateReachability(DVP dvp, List<Tuple<int, int>> failedLinks = null)
		{
            if (failedLinks == null)
            {
                failedLinks = new List<Tuple<int, int>>();
            }

			ZenFunction<SimplePacket, IList<Tuple<int, int>> , bool> f = Function<SimplePacket, IList<Tuple<int, int>>, bool>(dvp.Forward);
            f.Compile();

            Console.WriteLine("Evaluating output");
            var correct_input = new SimplePacket
            {
                SrcIp = new Ip { Value = 1 },
                DstIp = new Ip { Value = 2 },
            };

            var wrong_input = new SimplePacket
            {
                SrcIp = new Ip { Value = 0 },
                DstIp = new Ip { Value = 0 },
            };

            Console.WriteLine("Evaluating correct input: \t" + correct_input);
            var output = f.Evaluate(correct_input, failedLinks);
            Console.WriteLine("\t Reachable? " + output);

            Console.WriteLine("Evaluating wrong input: \t" + wrong_input);
            output = f.Evaluate(wrong_input, failedLinks);
            Console.WriteLine("\t Reachable? " + output);
            Console.WriteLine();
		}

        static void findPackets(DVP dvp, bool desired_result, List<Tuple<int, int>> failedLinks = null)
        {
            if (failedLinks == null)
            {
                failedLinks = new List<Tuple<int, int>>();
            }

            Console.WriteLine("findPackets");

            ZenFunction<SimplePacket, IList<Tuple<int, int>> , bool> f = Function<SimplePacket, IList<Tuple<int, int>>, bool>(dvp.Forward);
            f.Compile();

            // Console.WriteLine("Using FindAll");
            // Console.WriteLine("Number of packets that cannot be delivered in the network:");
            Console.WriteLine(failedLinks);
            var input = f.FindAll((pkt, failed_links, result) => And(
                And(
                    And(
                    And(
                    pkt.GetDstIp().GetField<Ip, uint>("Value") < 7,
                    pkt.GetSrcIp().GetField<Ip, uint>("Value") < 7
                    ),
                    pkt.GetDstIp() != pkt.GetSrcIp()),
                    result == desired_result),
                    failed_links == failedLinks));

            Console.WriteLine("\tCount:\t" + input.Count());
            //Console.WriteLine();
            //Console.WriteLine(input);
			
            if (input.Count() != 0)
            {
                Console.WriteLine("\tPrinting inputs:");
                foreach (var x in input)
                {
                    Console.WriteLine("\t\t" + x);
                }
            }
        }

		static void findLinks(DVP dvp, bool desired_result, SimplePacket packet)
		{
			Console.WriteLine("findLinks");

            ZenFunction<SimplePacket, IList<Tuple<int, int>> , bool> f = Function<SimplePacket, IList<Tuple<int, int>>, bool>(dvp.Forward);
            f.Compile();

            // Console.WriteLine("Using FindAll");
            // Console.WriteLine("Number of packets that cannot be delivered in the network:");
            var input = f.FindAll((pkt, failed_links, result) => And(
                And(
                    And(
                    And(
                    pkt.GetDstIp().GetField<Ip, uint>("Value") == packet.DstIp.Value,
                    pkt.GetSrcIp().GetField<Ip, uint>("Value") == packet.SrcIp.Value
                    ),
                    pkt.GetDstIp() != pkt.GetSrcIp()),
                    result == desired_result),
                // For DVP, no route requires more then two links to fail (no multiple path routing)
                // Therefore, we limit length to 1 here
                    failed_links.Length() == 1));

            Console.WriteLine("\tCount:\t" + input.Count());
            //Console.WriteLine();
            //Console.WriteLine(input);
			
            if (input.Count() != 0)
            {
                Console.WriteLine("\tPrinting inputs:");
                foreach (var x in input)
                {
                    Console.Write("\t\t" + x.Item1 + " with List [");
                    foreach (var i in x.Item2)
                    {
                        Console.Write(i + ", ");
                    }
                    Console.WriteLine("]");
                }
            }
		}

        static void findPacketsWithCost(DVP dvp, bool result, int maxCost)
        {
			// Zen will only check paths under the maxCost
            dvp.maxCost = maxCost;
            findPackets(dvp, result);
            dvp.cleanConstraints();
        }

        static void findPacketsWithIntermediateNode(DVP dvp, bool result, Ip intermediateNodeIp)
        {
			// Zen will only check  paths w/o intermediateNode
            dvp.intermediateNode = intermediateNodeIp;
            findPackets(dvp, result);
            dvp.cleanConstraints();
        }

        static void findPacketsWithFailedLinks(DVP dvp, bool result)
        {
			var failedLinks = new List<Tuple<int, int>>();
            failedLinks.Add(new Tuple<int, int>(0, 1));
            failedLinks.Add(new Tuple<int, int>(5, 6));

            findPackets(dvp, result, failedLinks);
            dvp.cleanConstraints();
        }

		static void findFailedLinksWithPacket(DVP dvp, bool result)
		{
			var packet = new SimplePacket
            {
                SrcIp = new Ip { Value = 6 },
                DstIp = new Ip { Value = 1 },
            };			

			findLinks(dvp, result, packet);
			dvp.cleanConstraints();
		}

        static Zen<IList<T>> Sort<T>(Zen<IList<T>> expr)
        {
            return expr.Case(empty: EmptyList<T>(), cons: (hd, tl) => Insert(hd, Sort(tl)));
        }

        static Zen<IList<T>> Insert<T>(Zen<T> elt, Zen<IList<T>> list)
        {
            return list.Case(
                empty: Language.Singleton(elt),
                cons: (hd, tl) => If(elt <= hd, list.AddFront(elt), Insert(elt, tl).AddFront(hd)));
        }

        // 0 - 1 - 2 (0 - 2)
        // The function encodes the property we want to verify
        static Zen<bool> Simple_CPV (Zen<IList<int>> costs)
        {
            
            return And(And(costs.At(0).Value() == 0, costs.At(2).Value() <= GlobalVar.MAX_HOPS), costs.Length() == 3);
        }

        static Zen<bool> Build_Network(Zen<IList<int>> costs, List<Tuple<int, int>> physicalEdges, int dst_node)
        {
            Dictionary<int, HashSet<int>> node2neighbors = new Dictionary<int, HashSet<int>>();
            //// dst_node needs to be set 0
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

            Zen<bool> network_expr = True();
            foreach (KeyValuePair<int, HashSet<int>> kvp in node2neighbors)
            {
                // costs.At(1).Value() == Min(costs.At(0).Value() + 1, costs.At(2).Value() + 1),
                Zen<ushort> i = (Zen<ushort>) kvp.Key;
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

                Zen<bool> upper_limit_expr = True();
                foreach (Zen<ushort> j in kvp.Value)
                {
                    upper_limit_expr = Or(upper_limit_expr, costs.At(i).Value() >= costs.At(j).Value() + 1);
                }
                Console.WriteLine("]");

                network_expr = And(network_expr, And(lower_limit_expr, upper_limit_expr));
            }
            Console.WriteLine("physical edges" + physicalEdges.Count());
            Console.WriteLine("Network Expression " + network_expr);
            return network_expr;

        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

           /* List<Tuple<int, int>> physicalEdges = new List<Tuple<int, int>>();
            physicalEdges.Add(new Tuple<int, int>(0, 1));
            physicalEdges.Add(new Tuple<int, int>(0, 2));
            physicalEdges.Add(new Tuple<int, int>(0, 4));
            physicalEdges.Add(new Tuple<int, int>(0, 5));
            physicalEdges.Add(new Tuple<int, int>(1, 2));
            physicalEdges.Add(new Tuple<int, int>(2, 3));
            physicalEdges.Add(new Tuple<int, int>(3, 6));
            physicalEdges.Add(new Tuple<int, int>(5, 6));

            DVP dvp = new DVP(physicalEdges);

            Console.WriteLine("Init");
            Console.WriteLine(dvp);

            dvp.runDVP(5);
            Console.WriteLine(dvp);
           */

            // evaluateReachability(dvp);

            //findPackets(dvp, true);
            // findPacketsWithCost(dvp, false, maxCost: 2);
            // findPacketsWithIntermediateNode(dvp, false, new Ip {Value = 0});

            //findPacketsWithFailedLinks(dvp, false);
            // findFailedLinksWithPacket(dvp, false);


            var f = Function<IList<byte>, IList<byte>>(l => Sort(l));
            var input = f.Find((inlist, outlist) => inlist.Length() != outlist.Length());
            Console.WriteLine("Zen list input " + input);

            f = Function<IList<byte>, IList<byte>>(Sort);
            input = f.Find((inlist, outlist) => inlist.Length() != outlist.Length());
            Console.WriteLine("Second Zen list input " + input);

            // Step 1. Encode the property we want to verify
            var f2 = Function<IList<int>, bool>(Simple_CPV);


            // NOTE: This does not work (evaluating to false even when there is a true assignemnt)
            // Guess: this uses checks, not constraints 
            /*var input2 = f2.FindAll((costs, results) => And(results == true,
                And(
                    // Constraint to the source node -> not work
                    //And(costs.At(0).Value() == Min(costs.At(1).Value() + 1, costs.At(2).Value() + 1),
                costs.At(1).Value() == Min(costs.At(0).Value() + 1, costs.At(2).Value() + 1),
                costs.At(2).Value() == Min(costs.At(1).Value() + 1, costs.At(0).Value() + 1))
                ));*/

            var physicalEdges = new List<Tuple<int, int>>();
            physicalEdges.Add(new Tuple<int, int>(0, 1));
            physicalEdges.Add(new Tuple<int, int>(0, 2));
            physicalEdges.Add(new Tuple<int, int>(1, 2));
            var input2 = f2.FindAll((costs, results) => And(results == true, Build_Network(costs, physicalEdges, 0)));

            // NOTE: This works. we'll do it this way

            // Step 2. Encode the constraints enforced by the topology of the network
            /*var input2 = f2.FindAll((costs, result) => And(result == true, // a control plane that works (true) / not works (false)
                And(
                    And(
                        And(
                                costs.At(0).Value() <= costs.At(1).Value() + 1, costs.At(0).Value() <= costs.At(2).Value() + 1
                                //Or(costs.At(0).Value() >= costs.At(1).Value() + 1, costs.At(0).Value() >= costs.At(2).Value() + 1)
                            ),
                        And(
                            And(costs.At(1).Value() <= costs.At(0).Value() + 1, costs.At(1).Value() <= costs.At(2).Value() + 1),
                            Or(costs.At(1).Value() >= costs.At(0).Value() + 1, costs.At(1).Value() >= costs.At(2).Value() + 1)
                            )
                        ),
                    And(
                        And(costs.At(2).Value() <= costs.At(0).Value() + 1, costs.At(2).Value() <= costs.At(1).Value() + 1),
                        Or(costs.At(2).Value() >= costs.At(0).Value() + 1, costs.At(2).Value() >= costs.At(1).Value() + 1)
                        )
                    )
                )
            );*/

            Console.WriteLine("Control plane!");
            //Console.WriteLine(input2.Value);
            //Console.WriteLine("Has Value: " + input2.HasValue);
            Console.WriteLine("\tCount:\t" + input2.Count());
            //Console.WriteLine();
            //Console.WriteLine(input);

            if (input2.Count() != 0)
            {
                Console.WriteLine("\tPrinting inputs:");
                foreach (var x in input2)
                {
                    Console.Write("\t\t with List [");
                    foreach (var i in x)
                    {
                        Console.Write(i + ", ");
                    }
                    Console.WriteLine("]");
                }
            }


        }
    }
}
