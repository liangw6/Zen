using System;
using System.Collections.Generic;
using System.Text;
using ZenLib.Tests.Network;
using ZenLib;
using static ZenLib.Language;
namespace ZenRouting
{
    public struct SimplePacket
    {
        public Ip DstIp { get; set; }
        public Ip SrcIp { get; set; }

        public static Zen<SimplePacket> Create (
            Zen<Ip> dstIp, Zen<Ip> srcIp)
        {
            return Language.Create<SimplePacket>(
                ("DstIp", dstIp),
                ("SrcIp", srcIp));
        }

        public override string ToString()
        {
            return $"simplePacket({this.DstIp}, {this.SrcIp})";
        }

    }

    static class SimplePacketExtensions
    {
        public static Zen<Ip> GetDstIp(this Zen<SimplePacket> packet)
        {
            return packet.GetField<SimplePacket, Ip>("DstIp");
        }

        public static Zen<Ip> GetSrcIp(this Zen<SimplePacket> packet)
        {
            return packet.GetField<SimplePacket, Ip>("SrcIp");
        }
    }

}
