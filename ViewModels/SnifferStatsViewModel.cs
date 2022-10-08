using MetaNet.Models;
using System;
using System.Windows.Threading;
using System.Collections.ObjectModel;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Diagnostics;
using static MetaNet.Models.SnifferStatsModel;

namespace MetaNet.ViewModels
{
    public class SnifferStatsViewModel : MainWindowBase
    {
        public ObservableCollection<Ipv4ProtocolStats> ProtocolStats { get; private set; }
        public ObservableCollection<Ipv4ConnectionStats> ConnectionStats { get; private set; }

        private string captureTime;
        public string CaptureTime
        {
            get { return captureTime; }
            set
            {
                if (captureTime != value)
                {
                    captureTime = value;
                    OnPropertyChanged(nameof(CaptureTime));
                }
            }
        }

        private long packetCount;
        public long PacketCount
        {
            get { return packetCount; }
            set
            {
                if (packetCount != value)
                {
                    packetCount = value;
                    OnPropertyChanged(nameof(PacketCount));
                }
            }
        }

        private long byteCount;
        public long ByteCount
        {
            get { return byteCount; }
            set
            {
                if (byteCount != value)
                {
                    byteCount = value;
                    OnPropertyChanged(nameof(ByteCount));
                }
            }
        }

        public PlotModel PlotModelData { get; private set; }
        private readonly DispatcherTimer PlotTimer = new DispatcherTimer();
        private long prevTimeElapsed;
        private long prevByteCount;

        private void InitPlot()
        {
            PlotModelData = new PlotModel { Title = "Bandwidth (kB/s)" };
            PlotModelData.Axes.Add(new LinearAxis { Title = "Time (s)", Position = AxisPosition.Bottom });
            PlotModelData.Series.Add(new LineSeries { Title = "Bandwidth", LineStyle = LineStyle.Solid });

            PlotTimer.Interval = TimeSpan.FromMilliseconds(500);
            PlotTimer.Tick += UpdatePlotModel;
            PlotTimer.Start();
        }

        public void UpdatePlotModel(object sender, EventArgs e)
        {
            long timeElapsed = SnifferStatsProcess.StopWatch.ElapsedMilliseconds;
            if (timeElapsed - prevTimeElapsed > 0)
            {
                TimeSpan ts = SnifferStatsProcess.StopWatch.Elapsed;
                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    PacketCount = SnifferStatsProcess.PacketCount;
                    ByteCount = SnifferStatsProcess.ByteCount;
                    CaptureTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
                }));

                long diffCount = (ByteCount - prevByteCount) > 0 ? (ByteCount - prevByteCount) : 0;
                double kBPerSec = diffCount / (timeElapsed - prevTimeElapsed) * 0.001 * 1024;
                lock (PlotModelData.SyncRoot)
                {
                    LineSeries bandWidthLine = (LineSeries)PlotModelData.Series[0];
                    bandWidthLine.Points.Add(new DataPoint(timeElapsed * 0.001, kBPerSec));
                }
                PlotModelData.InvalidatePlot(true);
                prevTimeElapsed = timeElapsed;
                prevByteCount = ByteCount;
            }
        }

        public SnifferStatsViewModel()
        {
            InitPlot();
            ProtocolStats = SnifferStatsProcess.ProtocolStats;
            ConnectionStats = SnifferStatsProcess.ConnectionStats;
            prevTimeElapsed = 0;
            prevByteCount = 0;

        }
    }
}
