using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Diagnostics;
using MetaNet.Models;
using System.Collections.ObjectModel;

namespace MetaNet.Interfaces
{
    public class WebSocketConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string str = "";
                var recv = (ObservableCollection<string>)values[0];
                int cnt = (int)values[1];
                foreach (var item in recv)
                {
                    str += System.Convert.ToString(item);
                }
                return str;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
