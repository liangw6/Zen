using System;
using ZenLib;
using ZenLib.Tests.Network;
using static ZenLib.Language;

namespace ZenRouting
{
	public class GlobalVar
	{
		public static int MAX_TTL = 120;
		public static int MAX_HOPS = 16;
		public static Ip NULL_IP = new Ip { Value = 0xFFFFFFFF }; 
		public static Zen<Ip> Z_NULL_IP = new Ip { Value = 0xFFFFFFFF };
	}
}
