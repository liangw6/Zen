using System;

namespace ZenRouting
{
    using ZenLib;
    using static ZenLib.Language;
    using ZenLib.ModelChecking;
    using System.Collections.Generic;
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


        }
    }
}
