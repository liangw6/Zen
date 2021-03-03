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

        static void fullPathReachability(DVP dvp, Zen<IList<Tuple<int, int>>> failedLinks)
        {
            ZenFunction<SimplePacket, IList<Tuple<int, int>> , bool> f = Function<SimplePacket, IList<Tuple<int, int>>, bool>(dvp.Forward);
            f.Compile();

            // 1. Evaluate
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
            var output = f.Evaluate(correct_input, new List<Tuple<int, int>>());
            Console.WriteLine("\t Reachable? " + output);

            Console.WriteLine("Evaluating wrong input: \t" + wrong_input);
            output = f.Evaluate(wrong_input, new List<Tuple<int, int>>());
            Console.WriteLine("\t Reachable? " + output);
            Console.WriteLine();

            // 2. FindAll
            Console.WriteLine("Using FindAll");
            Console.WriteLine("Number of packets that cannot be delivered in the network:");
            var input = f.FindAll((pkt, failed_links, result) => And(
                And(
                    And(
                    And(
                    pkt.GetDstIp().GetField<Ip, uint>("Value") < 7,
                    pkt.GetSrcIp().GetField<Ip, uint>("Value") < 7
                    ),
                    pkt.GetDstIp() != pkt.GetSrcIp()),
                    result == false),
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

        static void fullPathReachabilityWithCost(DVP dvp, int maxCost)
        {
            dvp.maxCost = maxCost;
            fullPathReachability(dvp, EmptyList<Tuple<int, int>>());
            dvp.cleanConstraints();
        }

        static void fullPathReachabilityWithoutCrossingNode(DVP dvp, Ip intermediateNodeIp)
        {
            dvp.intermediateNode = intermediateNodeIp;
            fullPathReachability(dvp, EmptyList<Tuple<int, int>>());
            dvp.cleanConstraints();
        }

        static void fullPathReachabilityWithFailedLinks(DVP dvp)
        {
			// Given failed links, find packets
            // Create failedLinks

			var failedLinks = EmptyList<Tuple<int, int>>();
			failedLinks.Add(new Tuple<int, int>(0, 1))
			failedLinks.Add(new Tuple<int, int>(5, 6))
			fullPathReachability(dvp, failedLinks)
            dvp.cleanConstraints();
        }

		static void fullPathReachabilityWithFailedLinks2(DVP dvp)
		{
			// TODO: do this
		}

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // ZenFunction<int, int, int> function = Function<int, int, int>(MultiplyAndAdd);
            // var output = function.Evaluate(3, 2); // output = 11
            // Console.WriteLine(output);

            //ZenFunction<uint, uint> f = Function<uint, uint>(i => i + 1);

            // create a set transformer from the function
            //StateSetTransformer<uint, uint> t = f.Transformer();

            // find the set of all inputs where the output is no more than 10,000
            //StateSet<uint> inputSet = t.InputSet((x, y) => y <= 10000);
            //StateSet<uint> outputSet = t.TransformForward(inputSet);

            //Option<uint> example = inputSet.Element(); // example.Value = 0
            //Console.WriteLine(example);

            //var f = Function<IList<byte>, IList<byte>>(l => Sort(l));

            //foreach (var list in f.GenerateInputs(listSize: 3))
            //{
            //Console.WriteLine($"[{string.Join(",", list)}]");
            //}


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

            Console.WriteLine("Init ");
            Console.WriteLine(dvp);

            dvp.runDVP(5);
            Console.WriteLine(dvp);

            //fullPathReachabilityWithCost(dvp, maxCost: 2);
            fullPathReachabilityWithoutCrossingNode(dvp, new Ip { Value = 0 });
        }
    }
}
