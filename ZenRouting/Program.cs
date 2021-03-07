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
                    failed_links.Length() == 1));

			// TODO: test this!!!

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
            //failedLinks.Add(new Tuple<int, int>(5, 6));

            findPackets(dvp, result, failedLinks);
            dvp.cleanConstraints();
        }

		static void findFailedLinksWithPacket(DVP dvp, bool result)
		{
			var packet = new SimplePacket
            {
                SrcIp = new Ip { Value = 1 },
                DstIp = new Ip { Value = 2 },
            };			

			findLinks(dvp, result, packet);
			dvp.cleanConstraints();
		}

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            List<Tuple<int, int>> physicalEdges = new List<Tuple<int, int>>();
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

            // evaluateReachability(dvp);

            //findPackets(dvp, true);
            // findPacketsWithCost(dvp, false, maxCost: 2);
            // findPacketsWithIntermediateNode(dvp, false, new Ip {Value = 0});

            findPacketsWithFailedLinks(dvp, false);
            // findFailedLinksWithPacket(dvp, true);
        }
    }
}
