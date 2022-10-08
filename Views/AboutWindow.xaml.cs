using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MetaNet.Views
{
    /// <summary>
    /// AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            Height = 300;
            Width = Height / 0.675;
            this.ResizeMode = ResizeMode.NoResize;
        }
        private void GithubButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/linkmeta/MetaNet");

            Close();
        }
        private void GiteeButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://gitee.com/linkmeta/MetaNet");

            Close();
        }
        private void ButtonEx_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
