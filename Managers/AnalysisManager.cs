using RepriseReportLogAnalyzer.Analyses;
using RepriseReportLogAnalyzer.Enums;
using RepriseReportLogAnalyzer.Extensions;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Views;
using RepriseReportLogAnalyzer.Windows;
using RLMLogReader.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
        private void _notifyPropertyChanged([CallerMemberName] String propertyName_ = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName_));
        }

        public ObservableCollection<LicenseView> ListResultProduct { get; private set; } = new ObservableCollection<LicenseView>();

        public ObservableCollection<LicenseView> ListResultGroup { get; private set; } = new ObservableCollection<LicenseView>();


        private ListAnalysisStartShutdown _listStartShutdown  = new ListAnalysisStartShutdown();
        private ListAnalysisCheckOutIn _listCheckOutIn = new ListAnalysisCheckOutIn();
        private ListAnalysisLicenseCount _listLicenseCount = new ListAnalysisLicenseCount();
        private ListAnalysisLicenseGroupDuration _listUserDuration = new ListAnalysisLicenseGroupDuration(ANALYSIS_GROUP.USER);
        private ListAnalysisLicenseGroupDuration _listHostDuration = new ListAnalysisLicenseGroupDuration(ANALYSIS_GROUP.HOST);
        private ListAnalysisLicenseGroupDuration _listUserHostDuration = new ListAnalysisLicenseGroupDuration(ANALYSIS_GROUP.USER_HOST);

        public SortedSet<string> ListProduct { get; private set; } = new SortedSet<string>();
        public SortedSet<string> ListUser { get; private set; } = new SortedSet<string>();
        public SortedSet<string> ListHost { get; private set; } = new SortedSet<string>();
        public SortedSet<string> ListUserHost { get; private set; } = new SortedSet<string>();

        public SortedSet<DateTime> ListDate { get; private set; } = new SortedSet<DateTime>();


        public DateTime? StartDate { get => (ListDate.Any()==false)?null: ListDate.First(); }
        public DateTime? EndDate { get => (ListDate.Any() == false) ? null : ListDate.Last(); }


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

            _listUserDuration.Analysis(ListUser, _listCheckOutIn);
            _listHostDuration.Analysis(ListHost, _listCheckOutIn);
            _listUserHostDuration.Analysis(ListUserHost, _listCheckOutIn);

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

        public void SetDate(DateTime? date_)
        {
            ListResultProduct.Clear();
            ListResultGroup.Clear();
            if (date_ != null)
            {
                ListResultProduct.AddRange(_listLicenseCount.ListDayToProduct(date_??DateTime.Now));
            }


            _notifyPropertyChanged("ListResultProduct");
        }
    }
}
