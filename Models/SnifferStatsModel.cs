using MetaNet.ViewModels;
using PcapDotNet.Packets.IpV4;

namespace MetaNet.Models
{
    public class SnifferStatsModel : MainWindowBase
    {
         #region Protocol Stats
        public class Ipv4ProtocolStats : MainWindowBase
        {
            public IpV4Protocol Protocol { get; set; }

            private long _PacketCount;
            public long PacketCount
            {
                get { return _PacketCount; }
                set
                {
                    if (_PacketCount != value)
                    {
                        _PacketCount = value;
                        OnPropertyChanged(nameof(PacketCount));
                    }
                }
            }

            private long _ByteCount;
            public long ByteCount
            {
                get { return _ByteCount; }
                set
                {
                    if (_ByteCount != value)
                    {
                        _ByteCount = value;
                        OnPropertyChanged(nameof(ByteCount));
                    }
                }
            }
            public Ipv4ProtocolStats(IpV4Protocol protocol)
            {
                Protocol = protocol;
                PacketCount = 0;
                ByteCount = 0;
            }
        }

        #endregion

        public class Ipv4ConnectionStats
        {
            public IpV4Address AddressA { get; set; }
            public IpV4Address AddressB { get; set; }
            public long PacketCountAToB { get; set; }
            public long PacketCountBToA { get; set; }
            public long ByteCountAToB { get; set; }
            public long ByteCountBToA { get; set; }
            public Ipv4ConnectionStats(IpV4Address addressA, IpV4Address addressB)
            {
                AddressA = addressA;
                AddressB = addressB;
                PacketCountAToB = 0;
                PacketCountBToA = 0;
                ByteCountAToB = 0;
                ByteCountBToA = 0;
            }
        }

        public SnifferStatsModel()
        {

        }
    }
}
