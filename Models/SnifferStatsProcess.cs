using PcapDotNet.Packets;
using PcapDotNet.Packets.IpV4;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

using static MetaNet.Models.SnifferStatsModel;

namespace MetaNet.Models
{
    public static class SnifferStatsProcess
    {
        public static Stopwatch StopWatch { get; private set; }
        public static long PacketCount { get; private set; }
        public static long ByteCount { get; private set; }
        public static ObservableCollection<Ipv4ProtocolStats> ProtocolStats { get; private set; }
        public static ObservableCollection<Ipv4ConnectionStats> ConnectionStats { get; private set; }
        public static void Start()
        {
            StopWatch.Start();
            PacketCount = 0;
            ByteCount = 0;
            ConnectionStats.Clear();
            foreach (Ipv4ProtocolStats stat in ProtocolStats)
            {
                stat.ByteCount = 0;
                stat.PacketCount = 0;
            }
        }

        public static void Stop()
        {
            StopWatch.Stop();
        }

        public static void UpdateStats(Packet newPacket)
        {
            ByteCount += newPacket.Length;
            PacketCount++;

            if (newPacket.Ethernet.IpV4.Protocol == IpV4Protocol.Tcp)
            {
                ProtocolStats[0].PacketCount++;
                ProtocolStats[0].ByteCount += newPacket.Length;
            }
            else if (newPacket.Ethernet.IpV4.Protocol == IpV4Protocol.Udp)
            {
                ProtocolStats[1].PacketCount++;
                ProtocolStats[1].ByteCount += newPacket.Length;
            }
            else if (newPacket.Ethernet.IpV4.Protocol == IpV4Protocol.Ip)
            {
                ProtocolStats[2].PacketCount++;
                ProtocolStats[2].ByteCount += newPacket.Length;
            }
            else if (newPacket.Ethernet.IpV4.Protocol == IpV4Protocol.Stream)
            {
                ProtocolStats[3].PacketCount++;
                ProtocolStats[3].ByteCount += newPacket.Length;
            }
            else if (newPacket.Ethernet.IpV4.Protocol == IpV4Protocol.InternetControlMessageProtocol)
            {
                ProtocolStats[4].PacketCount++;
                ProtocolStats[4].ByteCount += newPacket.Length;
            }
            else if (newPacket.Ethernet.IpV4.Protocol == IpV4Protocol.InternetGroupManagementProtocol)
            {
                ProtocolStats[5].PacketCount++;
                ProtocolStats[5].ByteCount += newPacket.Length;
            }


            Ipv4ConnectionStats connStats = ConnectionStats.Where(c =>
            (c.AddressA == newPacket.Ethernet.IpV4.Source || c.AddressA == newPacket.Ethernet.IpV4.Destination) &&
            (c.AddressB == newPacket.Ethernet.IpV4.Source || c.AddressB == newPacket.Ethernet.IpV4.Destination)).FirstOrDefault();
            if (connStats == null)
            {
                connStats = new Ipv4ConnectionStats(newPacket.Ethernet.IpV4.Source, newPacket.Ethernet.IpV4.Destination);
                connStats.ByteCountAToB = newPacket.Length;
                connStats.PacketCountAToB++;
                ConnectionStats.Add(connStats);
            }
            else
            {
                if (connStats.AddressA == newPacket.Ethernet.IpV4.Source)
                {
                    connStats.PacketCountAToB++;
                    connStats.ByteCountAToB += newPacket.Length;
                }
                else
                {
                    connStats.PacketCountBToA++;
                    connStats.ByteCountBToA += newPacket.Length;
                }
            }
        }
        static SnifferStatsProcess()
        {
            StopWatch = new Stopwatch();
            ProtocolStats = new ObservableCollection<Ipv4ProtocolStats>()
            {
                new Ipv4ProtocolStats( IpV4Protocol.Tcp),
                new Ipv4ProtocolStats( IpV4Protocol.Udp),
                new Ipv4ProtocolStats( IpV4Protocol.Ip),
                new Ipv4ProtocolStats( IpV4Protocol.Stream),
                new Ipv4ProtocolStats( IpV4Protocol.InternetControlMessageProtocol),
                new Ipv4ProtocolStats( IpV4Protocol.InternetGroupManagementProtocol),
            };
            ConnectionStats = new ObservableCollection<Ipv4ConnectionStats>();
            PacketCount = 0;
            ByteCount = 0;
        }
    }
}
