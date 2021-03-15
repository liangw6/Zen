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

		// Zen<IList> costs;

		public DVP_verify()
		{
		}

	// 	// each cost in costs corresponds to a node
	// 	public Zen<IList> buildNetwork(edges or None)
	// 	{
	// 		for each edge in edges
	// 			add the nodes to a ZenSet

	// 		then for each node in the ZenSet
	// 			find its neighbors
	// 			and put it into a map?

	// 		once we have nodes and neighbors map
	// 		costs = {c1 = min(), c2 = min(...), ...)
	// 		create n (num ofnodes) Zen Expressions like
	// 			c1 = min(c2 + ..., ...)
	// 			

	// 		return boolean expression expressing the cost constraints
	// 	}

	// 	buildNetwork.findall((costs, results) => propertyX(costs))

	// 		void buildNetwork(Zen<IList> costs) {
	// 			cost[0] = ...
	// 				cost[1] = ...
	// 				...
	// 		}

	// 		// can 1 reach 4
	// 		propertyX(Zen<IList> costs) {
	// 			cost[4] == 0 AND cost[1] == INF
	// 		}

	// 	------------------------------


	// 	propertyX.findall((costs, result) => buildNetwork(costs) and result == true)
	// 		bool buildNetowrk(Zen<IList> costs) {
	// 			c.length == 9 AND c[0]==min(c1...) AND c[1]==min(c2...)..

	// 		bool propertyX(costs)
	// 			cost[4] == 0 AND ...

	}
}

