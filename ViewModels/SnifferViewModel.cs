using System.Collections.ObjectModel;

namespace MetaNet.ViewModels
{
    internal class SnifferViewModel : MainWindowBase
    {
        ObservableCollection<object> _SnifferViews;
        public ObservableCollection<object> SnifferViews { get { return _SnifferViews; } }

        public SnifferViewModel()
        {
            _SnifferViews = new ObservableCollection<object>();
            _SnifferViews.Add(new SnifferCaptureViewModel());
            _SnifferViews.Add(new SnifferStatsViewModel());

        }
    }
}
