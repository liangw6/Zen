using System;
using System.Collections.Generic;
using ZenLib;
using static ZenLib.Language;

using ZenLib.Tests.Network;

namespace ZenRouting
{
    // Network is just an array of nodes
    public class Node
    {
        // address of this node
        public Ip Address { get; set; }
        public List<Route> RoutingTable { get; set; }

        /*
        public Node(int address)
        {
            this.Address = address;
            this.RoutingTable = new List<Route>();
        }

        public Node(int address, List<Route> RoutingTable)
        {
            this.Address = address;
            this.RoutingTable = new List<Route>(RoutingTable);
        }

        public Node(Node n)
        {
            this.Address = n.Address;
            this.RoutingTable = new List<Route>(n.RoutingTable);
        }
        */

        public void MergeRoute(Route newRoute)
        {

            if (newRoute.Destination.Equals(this.Address))

            {
                // CHANGED: so that we don't go to ourselves
                return;
            }

            int i;
            for (i = 0; i < RoutingTable.Count; i++)
            {
                Route currRoute = RoutingTable[i];
                if (newRoute.Destination.Equals(currRoute.Destination))
                {
                    if (newRoute.Cost + 1 < currRoute.Cost)
                        break;
                    else if (newRoute.NextHop.Equals(currRoute.NextHop))
                        break;
                    else
                        return;
                }
            }

            if (i == RoutingTable.Count)
            {
                // this is a new route
                RoutingTable.Add(newRoute);
            } else
            {
                RoutingTable[i] = newRoute;
                RoutingTable[i].TTL = GlobalVar.MAX_TTL;

                if (RoutingTable[i].Cost < GlobalVar.MAX_HOPS)
                {
                    RoutingTable[i].Cost += 1;
                }
                
            }
        }

        public void UpdateRoutingTable(List<Route> newRoutes)
        {
            foreach (Route newR in newRoutes)
            {
                MergeRoute(newR);
            }
        }

        public List<Route> sendRoutingTable()
        {
            List<Route> routesToSend = new List<Route>();
            foreach (Route currRoute in this.RoutingTable)
            {
                Route currSendRoute = new Route
                {
                    Destination = currRoute.Destination,
                    NextHop = this.Address,
                    Cost = currRoute.Cost,
                    TTL = currRoute.TTL,
                };
                routesToSend.Add(currSendRoute);
            }
            return routesToSend;
        }

        public Zen<bool> hasNextHop(Zen<Ip> dst)
        {
            Zen<bool> currFoundNextHop = False();

            for (int i = 0; i < RoutingTable.Count; i++)
            {
                currFoundNextHop = Or(RoutingTable[i].Destination == dst, currFoundNextHop);
            }

            return currFoundNextHop;
        }

        public Zen<Ip> getNextHop(Zen<Ip> dst)
        {
            Zen<Ip> nextHop = GlobalVar.Z_NULL_IP;

            for (int i = 0; i < RoutingTable.Count; i++)
            {
                nextHop = If(RoutingTable[i].Destination == dst, RoutingTable[i].NextHop, nextHop);
            }

            return nextHop;
        }

        public override string ToString()
        {
            var output = "Address: " + this.Address + "\n"; 
            foreach (Route r in this.RoutingTable)
            {
                output += "    Route: " + r.ToString() + "\n";
            }
            return output;
        }
    }
}
