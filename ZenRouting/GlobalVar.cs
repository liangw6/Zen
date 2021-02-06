using System;
using ZenLib.Tests.Network;

namespace ZenRouting
{
	public class GlobalVar
	{
		public static int MAX_TTL = 120;
		public static int MAX_HOPS = 16;
		public static Ip NULL_IP = new Ip { Value = 0xFFFFFFFF }; 
	}
}
