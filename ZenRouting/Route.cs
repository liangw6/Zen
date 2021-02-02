using System;
using System.Collections.Generic;
using System.Text;

namespace ZenRouting
{

    public class Route
    {
        public int Destination;  // Address of destination
        public int NextHop;      // Address of next hop
        public int Cost;            // Distance Metric
        public int TTL;             // Time to live

        public Route(int Destination, int NextHop, int Cost, int TTL)
        {
            this.Destination = Destination;
            this.NextHop = NextHop;
            this.Cost = Cost;
            this.TTL = TTL;
        }

        public Route(Route route)
        {
            this.Destination = route.Destination;
            this.NextHop = route.NextHop;
            this.Cost = route.Cost;
            this.TTL = route.TTL;
        }

        public override string ToString()
        {
            return "Dest: " + Destination + " Next Hop: " + NextHop + " Cost: " + Cost;
        }
    }
}

    

   
