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

        static Zen<int> MultiplyAndAdd(Zen<int> x, Zen<int> y)
        {
            return 3 * x + y;
        }

        static Zen<IList<T>> Sort<T>(Zen<IList<T>> expr)
        {
            return expr.Case(empty: EmptyList<T>(), cons: (hd, tl) => Insert(hd, Sort(tl)));
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

            // ZenFunction<Ip, Ip, bool> f = Function<Ip, Ip, bool>(dvp.Forward);
            ZenFunction<Ip, Ip, bool> f = Function<Ip, Ip, bool>(dvp.Forward);
            var srcAddr = new Ip { Value = 1 };
            var dstAddr = new Ip { Value = 2 };
            var output = f.Evaluate(srcAddr, dstAddr);
            Console.WriteLine("Found it!!!!!");
            Console.WriteLine(output);

            // var input = function.Find((x, y, result) => And(x <= 0, result == 11));

            var input = f.Find((src, dst, result) => And(And(And(src.GetField<Ip, uint>("Value") < 7,
                dst.GetField<Ip, uint>("Value") < 7), result == False()), src != dst));
            Console.WriteLine("Using powerful Zen Find!!!");

            Console.WriteLine(input);
            Console.WriteLine(input.Value);

            /*
            Console.WriteLine("Count: ");
            Console.WriteLine(input.Count());

            foreach (var x in input)
            {
                Console.WriteLine(x);
            }*/

            // Console.WriteLine(input.Take(5));


        }
    }
}
