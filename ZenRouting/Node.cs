using System;
using System.Collections.Generic;

namespace ZenRouting
{
    // Network is just an array of nodes
    public class Node
    {
        // address of this node
        int Address;
        List<Route> RoutingTable;

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

        public void MergeRoute(Route newRoute)
        {

            if (newRoute.Destination == this.Address)
            {
                // CHANGED: so that we don't go to ourselves
                return;
            }

            int i;
            for (i = 0; i < RoutingTable.Count; i++)
            {
                Route currRoute = RoutingTable[i];
                if (newRoute.Destination == currRoute.Destination)
                {
                    if (newRoute.Cost + 1 < currRoute.Cost)
                        break;
                    else if (newRoute.NextHop == currRoute.NextHop)
                        break;
                    else
                        return;
                }
            }
            if (i == RoutingTable.Count)
            {
                // this is a new route
                RoutingTable.Add(new Route(newRoute));
            } else
            {
                RoutingTable[i] = new Route(newRoute);
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
                Route currSendRoute = new Route(currRoute);
                currSendRoute.NextHop = this.Address;
                routesToSend.Add(currSendRoute);
            }
            return routesToSend;
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
