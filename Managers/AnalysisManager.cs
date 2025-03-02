using RepriseReportLogAnalyzer.Analyses;
using RepriseReportLogAnalyzer.Enums;
using RepriseReportLogAnalyzer.Extensions;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Views;
using RepriseReportLogAnalyzer.Windows;
using RLMLogReader.Extensions;
using ScottPlot;
using ScottPlot.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

            //ListResultProduct.Add(new LicenseView() { Name = "test1" });
            //ListResultProduct.Add(new LicenseView() { Name = "test2" });

            //_notifyPropertyChanged("ListResultProduct");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void _notifyPropertyChanged([CallerMemberName] string propertyName_ = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName_));
        }

        public ObservableCollection<LicenseView> ListResultProduct { get; private set; } = new ObservableCollection<LicenseView>();

        public ObservableCollection<LicenseView> ListResultGroup { get; private set; } = new ObservableCollection<LicenseView>();


        private readonly ListAnalysisStartShutdown _listStartShutdown  = new ListAnalysisStartShutdown();
        private readonly ListAnalysisCheckOutIn _listCheckOutIn = new ListAnalysisCheckOutIn();
        private readonly ListAnalysisLicenseCount _listLicenseCount = new ListAnalysisLicenseCount();
        //
        private readonly ListAnalysisLicenseGroupDuration _listUserDuration = new ListAnalysisLicenseGroupDuration(ANALYSIS_GROUP.USER);
        private readonly ListAnalysisLicenseGroupDuration _listHostDuration = new ListAnalysisLicenseGroupDuration(ANALYSIS_GROUP.HOST);
        private readonly ListAnalysisLicenseGroupDuration _listUserHostDuration = new ListAnalysisLicenseGroupDuration(ANALYSIS_GROUP.USER_HOST);
        //
        private readonly Dictionary<DateTime, IEnumerable<LicenseView>> _listDateToView = new Dictionary<DateTime, IEnumerable<LicenseView>>();

        public SortedSet<string> ListProduct { get; private set; } = new SortedSet<string>();
        public SortedSet<string> ListUser { get; private set; } = new SortedSet<string>();
        public SortedSet<string> ListHost { get; private set; } = new SortedSet<string>();
        public SortedSet<string> ListUserHost { get; private set; } = new SortedSet<string>();

        public SortedSet<DateTime> ListDate { get; private set; } = new SortedSet<DateTime>();


        public DateTime? StartDate { get => (ListDate.Count()==0) ? null: ListDate.First(); }
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
            ListProduct.AddRange(analysis_.ListProduct.Select(x_=>x_.Product));
            ListUser.AddRange(analysis_.ListUser);
            ListHost.AddRange(analysis_.ListHost);
            ListUserHost.AddRange(analysis_.ListUserHost);
            ListDate.AddRange(analysis_.ListDate);

            //
            _listStartShutdown.Analysis(analysis_);
            _listCheckOutIn.Analysis(analysis_, _listStartShutdown.ListWithoutSkip());
            _listLicenseCount.Analysis(analysis_, _listCheckOutIn);
            //
            _listUserDuration.Analysis(ListUser, _listCheckOutIn);
            _listHostDuration.Analysis(ListHost, _listCheckOutIn);
            _listUserHostDuration.Analysis(ListUserHost, _listCheckOutIn);

            //
            foreach (var date in ListDate)
            {
                _listDateToView[date] = _listLicenseCount.ListDayToProduct(date);
            }

            //
            _notifyPropertyChanged("StartDate");
            _notifyPropertyChanged("EndDate");
        }

        void WriteText(string outFolder_)
        {
            _listStartShutdown.WriteText(outFolder_ + @"\ListAnalysisStartShutdown.csv",true);
            _listStartShutdown.WriteText(outFolder_ + @"\ListAnalysisStartShutdownAll.csv");
            //
            _listCheckOutIn.WriteText(outFolder_ + @"\ListAnalysisCheckOutIn.csv");
            _listCheckOutIn.WriteText(outFolder_ + @"\ListAnalysisCheckOutInDuplication.csv", true);
            _listCheckOutIn.WriteDuplicationText(outFolder_ + @"\ListJoinEventCheckOutIn.csv");
            //
            _listLicenseCount.WriteText(outFolder_ + @"\ListAnalysisLicenseCount.csv");
            _listLicenseCount.WriteTimeSpanText(outFolder_ + @"\LicenseCountDate.csv", TimeSpan.TicksPerDay);
            _listLicenseCount.WriteTimeSpanText(outFolder_ + @"\LicenseCountHour.csv", TimeSpan.TicksPerHour);
            _listLicenseCount.WriteTimeSpanText(outFolder_ + @"\LicenseCount15Minute.csv", TimeSpan.TicksPerMinute * 15);
            //
            _listUserDuration.WriteText(outFolder_ + @"\DurationUser.csv");
            _listHostDuration.WriteText(outFolder_ + @"\DurationHost.csv");
            _listUserHostDuration.WriteText(outFolder_ + @"\DurationUserHost.csv");

        }

        public void SetDate(DateTime? date_, int group_)
        {
            ListResultProduct.Clear();
            ListResultGroup.Clear();
            ListResultProduct.Clear();

            var date = date_ ?? DateTime.Now;

            if (_listDateToView.TryGetValue(date, out var list) == true)
            {
                ListResultProduct.AddRange(list);
            }


            switch ((ANALYSIS_GROUP)group_)
            {
                case ANALYSIS_GROUP.USER: ListResultGroup.AddRange(_listUserDuration.ListDayToGroup(date)); break;
                case ANALYSIS_GROUP.HOST: ListResultGroup.AddRange(_listHostDuration.ListDayToGroup(date)); break;
                case ANALYSIS_GROUP.USER_HOST: ListResultGroup.AddRange(_listUserHostDuration.ListDayToGroup(date)); break;
                default: break;
            }
            _notifyPropertyChanged("ListResultProduct");
            _notifyPropertyChanged("ListResultGroup");
        }

        public void SetPlot(DateTime? date_, WpfPlot plot_)
        {
            if (date_ == null)
            {
                plot_.Plot.Clear();
                plot_.Plot.XLabel("Date");
                var listX = new List<double>();
                var listYProduct = new List<LicenseView>();

                //foreach (var date in ListDate)
                //{
                //    listX.Add(date.ToOADate());
                //    //listX.Add(date);


                //    if (_listDateToView.TryGetValue(date, out var list) == true)
                //    {
                //        listYProduct.AddRange(list);
                //    }

                //}

                foreach (var list in _listDateToView.Values)
                {
                    listYProduct.AddRange(list);
                }


                foreach (var product in ListProduct)
                {
                    var listYCount = listYProduct.Where(x_ => x_.Name == product).Select(x_ => (double)x_.Count);
                    var listYMax = listYProduct.Where(x_ => x_.Name == product).Select(x_ => (double)x_.Max);

                    var plotCount =plot_.Plot.Add.Scatter(ListDate.ToArray(), listYCount.ToArray());
                    plotCount.Label=product +" Count";
                    var plotMax = plot_.Plot.Add.Scatter(ListDate.ToArray(), listYMax.ToArray());
                    plotMax.Label = product + " Max";
                }

                plot_.Plot.Axes.DateTimeTicksBottom();
                plot_.Refresh();

                //plot_.AddScatter(listX.ToArray(),);
            }

        }
    }
}
