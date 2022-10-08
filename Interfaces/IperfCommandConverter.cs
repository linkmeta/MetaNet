using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Diagnostics;
using MetaNet.Models;

namespace MetaNet.Interfaces
{
    public class IperfCommandConverter : IMultiValueConverter
    {
        //Binding sequences start ----->

        // <Binding Path = "IperfModel.Role" />
        //< Binding Path="IperfModel.ServerIp"/>
        //<Binding Path = "IperfModel.Port" />
        //< Binding Path="IperfModel.Parallel"/>
        //<Binding Path = "IperfModel.Time" />
        //< Binding Path="IperfModel.Interval"/>
        //<Binding Path = "IperfModel.TcpFlag" />
        //< Binding Path="IperfModel.TcpWindowSize"/>
        //<Binding Path = "IperfModel.TcpWindowUnit" />
        //< Binding Path="IperfModel.UdpFlag"/>
        //<Binding Path = "IperfModel.BandWidth" />
        //< Binding Path="IperfModel.BandWidthUnit"/>
        //<Binding Path = "IperfModel.PacketLen" />
        //< Binding Path="IperfModel.Reverse"/>

        //Binding sequences end <-----
        //public string Command { get; set; }
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                string command = "";
                string role = values[0].ToString();

                string serverIp = values[1].ToString();
                int port = (int)values[2];
                int parallel = (int)values[3];
                int time = (int)values[4];
                int interval = (int)values[5];
                bool tcpFlag = (bool)values[6];
                int tcpWindowSize = (int)values[7];
                string tcpWindowUnit = values[8].ToString();
                bool udpFlag = (bool)values[9];
                int bandwidth = (int)values[10];
                string bandwidthUnit = values[11].ToString();
                int packetLen = (int)values[12];
                bool reverse = (bool)values[13];
                if (role == "-s")
                {
                    command += role;
                    command += " -p ";
                    command += port;
                    command += " -i ";
                    command += interval;

                }else if (role == "-c")
                {
                    command += role;
                    command += " ";
                    command += serverIp;
                    command += " -p ";
                    command += port;
                    command += " -P ";
                    command += parallel;
                    command += " -t ";
                    command += time;
                    command += " -i ";
                    command += interval;
                    if (tcpFlag)
                    {
                        command += " -w ";
                        command += tcpWindowSize;
                        command += tcpWindowUnit;
                    }
                    if (udpFlag)
                    {
                        command += " -b ";
                        command += bandwidth;
                        command += bandwidthUnit;
                        command += " -u ";
                    }
                    if (packetLen > 0)
                    {
                        command += " -l ";
                        command += packetLen;
                    }
                    if (reverse)
                    {
                        command += " -R";
                    }
                }
                else
                {
                    command = "command not support!";
                }
                return command;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
