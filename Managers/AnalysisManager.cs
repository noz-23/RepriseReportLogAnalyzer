using RepriseReportLogAnalyzer.Analyses;
using RepriseReportLogAnalyzer.Enums;
using RepriseReportLogAnalyzer.Extensions;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Views;
using RepriseReportLogAnalyzer.Windows;
using RLMLogReader.Extensions;
using ScottPlot;
using ScottPlot.WPF;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RepriseReportLogAnalyzer.Managers
{
    class AnalysisManager : INotifyPropertyChanged
    {
        public static AnalysisManager Instance = new AnalysisManager();
        private AnalysisManager()
        {
        }

        public void Create()
        {
            LogFile.Instance.WriteLine("Create");

        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void _notifyPropertyChanged([CallerMemberName] string propertyName_ = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName_));
        }

        public ObservableCollection<LicenseView> ListResultProduct { get; private set; } = new ();

        public ObservableCollection<LicenseView> ListResultGroup { get; private set; } = new ();

        private readonly ListAnalysisStartShutdown _listStartShutdown = new ();
        private readonly ListAnalysisCheckOutIn _listCheckOutIn = new ();
        private readonly ListAnalysisLicenseCount _listLicenseCount = new ();
        //
        private readonly ListAnalysisLicenseGroupDuration _listUserDuration = new (ANALYSIS_GROUP.USER);
        private readonly ListAnalysisLicenseGroupDuration _listHostDuration = new (ANALYSIS_GROUP.HOST);
        private readonly ListAnalysisLicenseGroupDuration _listUserHostDuration = new (ANALYSIS_GROUP.USER_HOST);
        //
        //private readonly Dictionary<DateTime, IEnumerable<LicenseView>> _listDateToView = new Dictionary<DateTime, IEnumerable<LicenseView>>();
        //private Dictionary<DateTime, IEnumerable<LicenseView>> ListDayToProduct() => _listLicenseCount.ListDayToProduct();

        //private readonly Dictionary<string, bool> _listProductChecked = new ();


        public SortedSet<string> ListProduct { get; private set; } = new ();
        public SortedSet<string> ListUser { get; private set; } = new ();
        public SortedSet<string> ListHost { get; private set; } = new ();
        public SortedSet<string> ListUserHost { get; private set; } = new ();

        public SortedSet<DateTime> ListDate { get; private set; } = new ();

        public DateTime? StartDate { get => (ListDate.Count() == 0) ? null : ListDate.First(); }
        public DateTime? EndDate { get => (ListDate.Count() == 0) ? null : ListDate.Last(); }

        public void SetProgressCount(ProgressCountDelegate progressCount_)
        {
            _listStartShutdown.ProgressCount = progressCount_;
            _listCheckOutIn.ProgressCount = progressCount_;
            _listLicenseCount.ProgressCount = progressCount_;
            _listUserDuration.ProgressCount = progressCount_;
            _listHostDuration.ProgressCount = progressCount_;
            _listUserHostDuration.ProgressCount = progressCount_;

        }

        public void Clear()
        {
            ListProduct.Clear();
            ListUser.Clear();
            ListHost.Clear();
            ListUserHost.Clear();
            ListDate.Clear();
            //
            //_listProductChecked.Clear();

            //
            _listStartShutdown.Clear();
            _listCheckOutIn.Clear();
            _listLicenseCount.Clear();
            _listUserDuration.Clear();
            _listHostDuration.Clear();
            _listUserHostDuration.Clear();
        }
        public void Analysis(AnalysisReportLog analysis_)
        {
            Clear();
            //
            ListProduct.AddRange(analysis_.ListProduct.Select(x_ => x_.Product));
            ListUser.AddRange(analysis_.ListUser);
            ListHost.AddRange(analysis_.ListHost);
            ListUserHost.AddRange(analysis_.ListUserHost);
            ListDate.AddRange(analysis_.ListDate);
            //
            //foreach (var product in ListProduct)
            //{
            //    _listProductChecked[product]=true;
            //}
            //
            _listStartShutdown.Analysis(analysis_);
            _listCheckOutIn.Analysis(analysis_, _listStartShutdown.ListWithoutSkip());
            _listLicenseCount.Analysis(analysis_, _listCheckOutIn);
            //
            _listUserDuration.Analysis(ListUser, _listCheckOutIn);
            _listHostDuration.Analysis(ListHost, _listCheckOutIn);
            _listUserHostDuration.Analysis(ListUserHost, _listCheckOutIn);

            //
            //foreach (var date in ListDate)
            //{
            //    _listDateToView[date] = _listLicenseCount.ListDayToProduct(date);
            //}

            //
            _notifyPropertyChanged("StartDate");
            _notifyPropertyChanged("EndDate");
        }

        public void WriteText(string outFolder_)
        {
            _listStartShutdown.WriteText(outFolder_ + @"\ListAnalysisStartShutdown.csv", true);
            _listStartShutdown.WriteText(outFolder_ + @"\ListAnalysisStartShutdownAll.csv");
            //
            _listCheckOutIn.WriteText(outFolder_ + @"\ListAnalysisCheckOutIn.csv");
            _listCheckOutIn.WriteText(outFolder_ + @"\ListAnalysisCheckOutInDuplication.csv", true);
            _listCheckOutIn.WriteDuplicationText(outFolder_ + @"\ListJoinEventCheckOutIn.csv");
            //
            _listLicenseCount.WriteText(outFolder_ + @"\ListAnalysisLicenseCount.csv");
            _listLicenseCount.WriteTimeSpanText(outFolder_ + @"\LicenseCountDate.csv", TimeSpan.TicksPerDay);
            _listLicenseCount.WriteTimeSpanText(outFolder_ + @"\LicenseCountHour.csv", TimeSpan.TicksPerHour);
            _listLicenseCount.WriteTimeSpanText(outFolder_ + @"\LicenseCount30Minute.csv", TimeSpan.TicksPerMinute * 30);
            //
            _listUserDuration.WriteText(outFolder_ + @"\DurationUser.csv");
            _listHostDuration.WriteText(outFolder_ + @"\DurationHost.csv");
            _listUserHostDuration.WriteText(outFolder_ + @"\DurationUserHost.csv");

        }

        public bool IsChecked(string product_)=> ListResultProduct.Where(x_ => x_.Name == product_).Select(x_ => x_.IsChecked).FirstOrDefault();
        //{
            //foreach (var view in ListResultProduct)
            //{
            //    if (view.Name == product_)
            //    {
            //        return view.IsChecked;
            //    }
            //}
            //return true;
        //}

        
        public void SetDate(DateTime? date_, int group_)
        {
            // Product は使いまわし(チェックのため)
            foreach (var view in _listLicenseCount.ListLicenseProduct(date_)) 
            {
                ListResultProduct.SetView(view);
            }

            // Groupはいったんクリア
            ListResultGroup.Clear();
            switch ((ANALYSIS_GROUP)group_)
            {
                case ANALYSIS_GROUP.USER: ListResultGroup.AddRange(_listUserDuration.ListLicenseGroup(date_)); break;
                case ANALYSIS_GROUP.HOST: ListResultGroup.AddRange(_listHostDuration.ListLicenseGroup(date_)); break;
                case ANALYSIS_GROUP.USER_HOST: ListResultGroup.AddRange(_listUserHostDuration.ListLicenseGroup(date_)); break;
                default: break;
            }
            //
            _notifyPropertyChanged("ListResultProduct");
            _notifyPropertyChanged("ListResultGroup");
        }

        public void SetAllPlot(WpfPlot plot_)
        {
            plot_.Plot.Clear();
            plot_.Plot.Title("Product Date - Count");
            plot_.Plot.XLabel("Date");
            plot_.Plot.YLabel("Count");
            //var listX = new List<double>();
            var listYProduct = new List<LicenseView>();

            foreach (var date in ListDate) 
            {
                listYProduct.AddRange(_listLicenseCount.ListLicenseProduct(date));
            }

            foreach (var product in ListProduct)
            {
                if (IsChecked(product) == false)
                {
                    continue;
                }

                var listYCount = listYProduct.Where(x_ => x_.Name == product).Select(x_ => (double)x_.Count);
                var listYMax = listYProduct.Where(x_ => x_.Name == product).Select(x_ => (double)x_.Max);

                var plotCount = plot_.Plot.Add.Scatter(ListDate.ToArray(), listYCount.ToArray());
                plotCount.LegendText= product + " Count";
                var plotMax = plot_.Plot.Add.Scatter(ListDate.ToArray(), listYMax.ToArray());
                plotMax.LegendText = product + " Max";
            }

            plot_.Plot.Axes.DateTimeTicksBottom();
            plot_.Refresh();
        }

        public void SetDatePlot(WpfPlot plot_, DateTime date_, int group_)
        {
            plot_.Plot.Clear();
            plot_.Plot.Title($"Product {date_.ToShortDateString()} - Count");
            plot_.Plot.XLabel("Time(30 Minitue)");
            plot_.Plot.YLabel("Count");

            long timeSpan = 30 * TimeSpan.TicksPerMinute;

            var listX = new List<DateTime>();
            var listYProduct = new List<LicenseView>();

            for (var time = date_; time < date_.AddTicks(TimeSpan.TicksPerDay); time = time.AddTicks(timeSpan))
            {
                listX.Add(time);
                
                switch ((ANALYSIS_GROUP)group_)
                {
                    case ANALYSIS_GROUP.USER:
                        break;
                    case ANALYSIS_GROUP.HOST:
                        break;
                    case ANALYSIS_GROUP.USER_HOST:
                        break;
                    case ANALYSIS_GROUP.NONE:
                    default:
                        listYProduct.AddRange(_listLicenseCount.ListLicenseProduct(time, timeSpan));
                        break;
                }
            }

            foreach (var product in ListProduct)
            {
                if (IsChecked(product) == false)
                {
                    continue;
                }

                var listYCount = listYProduct.Where(x_ => x_.Name == product).Select(x_ => (double)x_.Count);
                var listYMax = listYProduct.Where(x_ => x_.Name == product).Select(x_ => (double)x_.Max);

                var plotCount = plot_.Plot.Add.Scatter(listX.ToArray(), listYCount.ToArray());
                plotCount.LegendText = product + " Count";
                var plotMax = plot_.Plot.Add.Scatter(listX.ToArray(), listYMax.ToArray());
                plotMax.LegendText = product + " Max";
            }

            plot_.Plot.Axes.DateTimeTicksBottom();
            plot_.Refresh();

        }


    }
}
