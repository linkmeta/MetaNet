using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace MetaNet.Interfaces
{
    public class IpInterface
    {
        public PingReply PingSweep(string ip)
        {
            PingReply reply = null;
            Ping pingSender = null;
            try
            {
                pingSender = new Ping();

                PingOptions options = new PingOptions();
                options.DontFragment = true;

                string data = "hello";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 1000;

                IPAddress ipa = IPAddress.Parse(ip);
                PingReply replyPing = pingSender.Send(ipa, timeout, buffer, options);// .Send(ip, timeout, buffer, options);
                reply = replyPing;
            }
            catch (Exception ex)
            {
                reply = null;
            }
            finally
            {
                pingSender.Dispose();
            }
            return reply;
        }

        public string GetHostName(string ip)
        {
            string host = null;
            try
            {
                host = Dns.GetHostEntry(ip).HostName;
                //host = Dns.GetHostEntryAsync(ip).HostName;
            }
            catch (Exception ex)
            {
                host = null;
            }
            return host;
        }
        public void GetIP()
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            int len = interfaces.Length;

            for (int i = 0; i < len; i++)
            {
                NetworkInterface ni = interfaces[i];

                if (ni.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties property = ni.GetIPProperties();
                    foreach (UnicastIPAddressInformation ip in property.UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            string address = ip.Address.ToString();
                            string niname = ni.Name.ToString();
                            Console.WriteLine("【" + niname + "】：" + address);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Converts a uint representation of an Ip address to a
        /// string.
        /// </summary>
        /// <param name="IPAddr">The IP address to convert</param>
        /// <returns>A string representation of the IP address.</returns>
        public string LongToIPAddress(uint IPAddr)
        {
            //return new System.Net.IPAddress(IPAddr).ToString();
            byte a = (byte)((IPAddr & 0xFF000000) >> 24);
            byte b = (byte)((IPAddr & 0x00FF0000) >> 16);
            byte c = (byte)((IPAddr & 0x0000FF00) >> 8);
            byte d = (byte)(IPAddr & 0x000000FF);
            string ipStr = String.Format("{0}.{1}.{2}.{3}", a, b, c, d);
            return ipStr;
        }
        //public string Int2IP(uint IPAddr)
        //{
        //    byte a = (byte)((IPAddr & 0xFF000000) >> 24);
        //    byte b = (byte)((IPAddr & 0x00FF0000) >> 16);
        //    byte c = (byte)((IPAddr & 0x0000FF00) >> 8);
        //    byte d = (byte)(IPAddr & 0x000000FF);
        //    string ipStr = String.Format(" {0}.{1}.{2}.{3} ", a, b, c, d);
        //    return ipStr;
        //}
        /// <summary>
        /// Converts a string representation of an IP address to a
        /// uint. This encoding is proper and can be used with other
        /// networking functions such
        /// as the System.Net.IPAddress class.
        /// </summary>
        /// <param name="IPAddr">The Ip address to convert.</param>
        /// <returns>Returns a uint representation of the IP
        /// address.</returns>
        public uint IPAddressToLong(string IPAddr)
        {
            System.Net.IPAddress oIP = System.Net.IPAddress.Parse(IPAddr);
            byte[] byteIP = oIP.GetAddressBytes();


            uint ip = (uint)byteIP[3] << 24;
            ip += (uint)byteIP[2] << 16;
            ip += (uint)byteIP[1] << 8;
            ip += (uint)byteIP[0];

            return ip;
        }
        /// <summary>
        /// This encodes the string representation of an IP address
        /// to a uint, but backwards so that it can be used to
        /// compare addresses. This function is used internally
        /// for comparison and is not valid for valid encoding of
        /// IP address information.
        /// </summary>
        /// <param name="IPAddr">A string representation of the IP
        /// address to convert</param>
        /// <returns>Returns a backwards uint representation of the
        /// string.</returns>
        public uint IPAddressToLongBackwards(string IPAddr)
        {
            System.Net.IPAddress oIP = System.Net.IPAddress.Parse(IPAddr);
            byte[] byteIP = oIP.GetAddressBytes();


            uint ip = (uint)byteIP[0] << 24;
            ip += (uint)byteIP[1] << 16;
            ip += (uint)byteIP[2] << 8;
            ip += (uint)byteIP[3];

            return ip;
        }
    }
}
