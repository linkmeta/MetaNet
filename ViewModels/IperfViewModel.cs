using MetaNet.Models;
using MetaNet.Interfaces;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.IO;

namespace MetaNet.ViewModels
{
    internal class IperfViewModel : MainWindowBase
    {
        public ProcessInterface iperfProcess;

        public IperfModel IperfModel { get; set; }
        PlotModel _mPlotModelData = new PlotModel();
        public PlotModel PlotModelData
        {
            get { return _mPlotModelData; }
            set { _mPlotModelData = value; OnPropertyChanged(nameof(PlotModelData)); }
        }

        public LinearAxis XTimeAxis;
        public LinearAxis YThroughputVal;
        public LineSeries lineSeriesCurrentVal;
        public int YZoomFactor = 1;
        public void InitPlot()
        {
            PlotModelData = new PlotModel();

            XTimeAxis = new LinearAxis()
            {
                Title = "Time(s)",
                Position = AxisPosition.Bottom,
                MajorGridlineStyle = LineStyle.None,
                Minimum = 0,
                AbsoluteMinimum = 0,

                Maximum = 100,
                //MajorStep = 100,
                FontSize = 13,
                //PositionTier = 6,
                Key = "Time",

                MinorGridlineStyle = LineStyle.Solid,
                IsPanEnabled = true,
                IsZoomEnabled = true

            };
            YThroughputVal = new LinearAxis()
            {
                Title = "Throughput(Mbps)",
                Position = AxisPosition.Left,
                MajorGridlineStyle = LineStyle.None,
                Minimum = 0,
                AbsoluteMinimum = 0,

                Maximum = 500.0,
                MajorStep = 50,
                FontSize = 13,
                PositionTier = 6,
                Key = "Throughput",

                MinorGridlineStyle = LineStyle.Solid,
                IsPanEnabled = true,
                IsZoomEnabled = true

            };

            PlotModelData.Axes.Add(XTimeAxis);
            PlotModelData.Axes.Add(YThroughputVal);


            lineSeriesCurrentVal = new OxyPlot.Series.LineSeries()
            {
                MarkerType = MarkerType.Circle,
                StrokeThickness = 1,
                YAxisKey = "Throughput",
                Title = "Throughput",
                Color = OxyColors.Red
            };

            PlotModelData.Series.Add(lineSeriesCurrentVal);
        }


        public void YZoomOut()
        {
            YThroughputVal.Maximum *= 2;
            YThroughputVal.MajorStep = (YThroughputVal.ActualMaximum - YThroughputVal.ActualMinimum) / 10;
        }
        public void YZoomIn()
        {
            YThroughputVal.Maximum /= 2;
            YThroughputVal.MajorStep = (YThroughputVal.ActualMaximum - YThroughputVal.ActualMinimum) / 10;

        }
        public void ClearPlot()
        {
            lineSeriesCurrentVal.Points.Clear();
            PlotModelData.InvalidatePlot(true);
        }
        internal CancellationTokenSource plotTokenSource;
        internal CancellationToken cancelPlotToken;
        internal void StopUpdatePlotTask()
        {
            plotTokenSource.Cancel();
        }
        //public void UpdatePlotTask()
        //{
        //    double val = 300;
        //    plotTokenSource = new CancellationTokenSource();
        //    cancelPlotToken = plotTokenSource.Token;
        //    Task.Run(
        //        () =>
        //        {
        //            while (true)
        //            {
        //                if (plotTokenSource.IsCancellationRequested)
        //                {
        //                    break;
        //                }
        //                val = Convert.ToDouble(IperfModel.Throughput);
        //                var date = DateTime.Now;
        //                lineSeriesCurrentVal.Points.Add(DateTimeAxis.CreateDataPoint(date, val));

        //                PlotModelData.InvalidatePlot(true);

        //                if (date.ToOADate() > XTimeAxis.ActualMaximum)
        //                {
        //                    var xPan = (XTimeAxis.ActualMaximum - XTimeAxis.DataMaximum) * XTimeAxis.Scale;
        //                    XTimeAxis.Pan(xPan);
        //                }
        //                Thread.Sleep(1000);
        //            }
        //        }, cancelPlotToken);
        //}
        
        public void RunIperf(object parameter)
        {
            string iperfPath = @"Third-party\iperf\" + IperfModel.Version;

            iperfProcess.StartProcess(iperfPath, (string)parameter, processOutputDataReceived);
            lineSeriesCurrentVal.Points.Clear();
            PlotModelData.InvalidatePlot(true);

        }
        public void plotData(string str)
        {
            string pattern;
            if(IperfModel.Parallel > 1)
            {
                pattern = @"\[SUM\]*\s*(?<a>[1-9]\d*.\d*|0.\d*[1-9]\d*)\s*-\s*(?<b>[1-9]\d*.\d*|0.\d*[1-9]\d*)\s*sec\s*(?<c>[1-9]\d*.\d*|0.\d*[1-9]\d*)\s*.Bytes\s*(?<d>[1-9]\d*.\d*|0.\d*[1-9]\d*)\s*Mbits/sec";

            }
            else
            {
                pattern = @"(?<a>[1-9]\d*.\d*|0.\d*[1-9]\d*)\s*-\s*(?<b>[1-9]\d*.\d*|0.\d*[1-9]\d*)\s*sec\s*(?<c>[1-9]\d*.\d*|0.\d*[1-9]\d*)\s*.Bytes\s*(?<d>[1-9]\d*.\d*|0.\d*[1-9]\d*)\s*Mbits/sec";

            }
            Match m = Regex.Match(str, pattern);
            
            //str = "";
            //Regex r = new Regex("/(\\d+)[^\\d]+Mbits/sec/");
            
            if (m.Success)
            {
                //return "此次验证不通过";
                string timeA = m.Groups["a"].Value;
                string timeB = m.Groups["b"].Value;
                string bytes = m.Groups["c"].Value;
                string bandwidth = m.Groups["d"].Value;
                IperfModel.Throughput = Convert.ToDouble(bandwidth);
                var val = Convert.ToDouble(IperfModel.Throughput);
                var time = Convert.ToDouble(timeB);

                YThroughputVal.MajorStep = (YThroughputVal.ActualMaximum - YThroughputVal.ActualMinimum) /10;
                lineSeriesCurrentVal.Points.Add(new DataPoint(time, val));

                PlotModelData.InvalidatePlot(true);
                if (val > YThroughputVal.ActualMaximum)
                {
                    var xPan = (YThroughputVal.ActualMaximum - YThroughputVal.DataMaximum - 50) * YThroughputVal.Scale;
                    YThroughputVal.Pan(xPan);
                }
            }
            else
            {

            }
        }
        private void processOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            IperfModel.Output += e.Data;
            IperfModel.Output += "\n";
            if(e.Data != null)
            {
                plotData(e.Data);
            }
        }
        public void StopIperf(object parameter)
        {
            //iperfProcess.Kill();
            iperfProcess.StopProcess();
        }
        public ICommand RunIperfCommand
        {
            get
            {
                return new RelayCommand(param => RunIperf(param));
            }
        }
        public ICommand StopIperfCommand
        {
            get
            {
                return new RelayCommand(param => StopIperf(param));
            }
        }
        public void ClearOutput(object parameter)
        {
            IperfModel.Output = "";

        }
        public ICommand ClearOutputCommand
        {
            get
            {
                return new RelayCommand(param => ClearOutput(param));
            }
        }
        public async void SaveOutput(object parameter)
        {
            ///*IperfModel.Output*/ = "";
            SaveFileDialog ReceDataSaveFileDialog = new SaveFileDialog
            {
                Title = "save iperf result",
                FileName = DateTime.Now.ToString("yyyyMMddmmss"),
                DefaultExt = ".txt",
                //Filter = string.Format("en", "*.*")
                Filter = "txt files(*.txt)|*.txt|All files(*.*)|*.*"
        };

            if (ReceDataSaveFileDialog.ShowDialog() == true)
            {
                string DataRecvPath = ReceDataSaveFileDialog.FileName;
                //SavePathInfo = string.Format(CultureInfos, "{0}", DataRecvPath);
                using (StreamWriter DefaultReceDataPath = new StreamWriter(DataRecvPath, true))
                {
                    await DefaultReceDataPath.WriteAsync(IperfModel.Output).ConfigureAwait(false);
                }
            }

        }
        public ICommand SaveOutputCommand
        {
            get
            {
                return new RelayCommand(param => SaveOutput(param));
            }
        }
        public void IperfHelp(object parameter)
        {
            string iperfPath = @"Third-party\iperf\" + IperfModel.Version;
            iperfProcess.StartProcess(iperfPath, "--help", processOutputDataReceived);

        }
        public ICommand IperfHelpCommand
        {
            get
            {
                return new RelayCommand(param => IperfHelp(param));
            }
        }
        public IperfViewModel()
        {
            iperfProcess  = new ProcessInterface();
            IperfModel = new IperfModel();

            InitPlot();
        }
    }
}
