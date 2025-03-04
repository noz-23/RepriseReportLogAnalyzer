using RepriseReportLogAnalyzer.Enums;
using RepriseReportLogAnalyzer.Extensions;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Managers;
using RepriseReportLogAnalyzer.Views;
using RepriseReportLogAnalyzer.Windows;
using System.IO;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RepriseReportLogAnalyzer.Analyses
{
    internal class ListAnalysisLicenseGroupDuration:Dictionary<string, ListAnalysisCheckOutIn>
    {

        public ListAnalysisLicenseGroupDuration(ANALYSIS_GROUP group_)
        {
            _group = group_;
        }

        public ProgressCountDelegate? ProgressCount = null;
        private const string _ANALYSIS = "[License Group Duration]";
        private ANALYSIS_GROUP _group = ANALYSIS_GROUP.NONE;

        public string Header { get => _group.Description() + ",Duration,Days,Count"; }

        //Dictionary<DateTime, List< LicenseView>> _listDayToGroup = new Dictionary<DateTime, List<LicenseView>>();

        private IEnumerable<string> _listGroup;

        public void Analysis(IEnumerable<string> listGroup_, ListAnalysisCheckOutIn listCheckOutIn_)
        {
            _listGroup = listGroup_;

            var minDate = listCheckOutIn_.Select(x => x.CheckOut().EventDate()).Min();
            var maxDate = listCheckOutIn_.Select(x => x.CheckOut().EventDate()).Max();
            //for (var date = minDate; date < maxDate.AddTicks(TimeSpan.TicksPerDay); date = date.AddTicks(TimeSpan.TicksPerDay))
            //{
            //    _listDayToGroup[date]= new List<LicenseView>();
            //}

            int count = 0;
            int max = listGroup_.Count();

            ProgressCount?.Invoke(0, max, _ANALYSIS + _group.Description());
            foreach (var group in listGroup_)
            {
                var list = new ListAnalysisCheckOutIn(listCheckOutIn_.ListDuplication().Where(x_ => x_.GroupName(_group) == group));
                this[group] = list;

                //_addDayToGroup(minDate, maxDate, group, list);
                ProgressCount?.Invoke(++count, max);
            }
        }


        private List<string> _listToString()
        {
            var rtn = new List<string>();

            int count = 0;
            int max = this.Keys.Count;
            ProgressCount?.Invoke(0, max, _ANALYSIS);
            foreach (var key in this.Keys)
            {
                var list = this[key];
                var sum = new TimeSpan(list.Sum(x => x.DurationDuplication().Ticks));
                var days = new HashSet<DateTime>(list.Select(x => x.CheckOutDateTime.Date));
                rtn.Add($"{key},{sum.ToString(@"d\.hh\:mm\:ss")},{days.Count},{list.Count}");
                ProgressCount?.Invoke(++count, max);
            }
            return rtn;
        }

        public void WriteText(string path_)
        {
            var list = new List<string>();
            list.Add(Header);
            list.AddRange(_listToString());
            File.WriteAllLines(path_, list, Encoding.UTF8);

            LogFile.Instance.WriteLine($"Write:{path_}");
        }

        //private void _addDayToGroup(DateTime minDate_, DateTime maxDate_, string group_,ListAnalysisCheckOutIn list_)
        //{
        //    for (var date = minDate_; date < maxDate_.AddTicks(TimeSpan.TicksPerDay); date = date.AddTicks(TimeSpan.TicksPerDay))
        //    {
        //        var listDay = list_.ListDuplication().Where(x_ => x_.CheckOut().EventDate() == date.Date);

        //        if (listDay.Any() == false)
        //        {
        //            continue;
        //        }

        //        var view = new LicenseView()
        //        {
        //            Name = group_,
        //            Duration = new TimeSpan(listDay.Sum(x_ => x_.Duration.Ticks)),
        //        };

        //        _listDayToGroup[date].Add(view);
        //    }
        //}

        //public IEnumerable<LicenseView> ListDayToGroup()
        //{
        //    var rtn = new List<LicenseView>();

        //    var list = new List<LicenseView>();
        //    _listDayToGroup.ToList().ForEach(x_ => list.AddRange(x_.Value));

        //    foreach (var group in _listGroup)
        //    {
        //        var view = new LicenseView()
        //        {
        //            Name = group,
        //            Count = list.Count(x_ => x_.Name == group),
        //            Max = list.Where(x_=> x_.Name == group).Max(x_ => x_.Count),
        //            Duration = new TimeSpan(list.Where(x_ => x_.Name == group).Sum(x_ => x_.Duration.Ticks))
        //        };
        //        rtn.Add(view);
        //    }

        //    return rtn;
        //}

        //public IEnumerable<LicenseView> ListDayToGroup(DateTime date_)
        //{
        //    if (_listDayToGroup.TryGetValue(date_, out var rtn) == true)
        //    {
        //        return rtn;
        //    }
        //    return new List<LicenseView>();
        //}
        //private string GetName(ANALYSIS_GROUP src_)
        //{
        //    var gm = src_.GetType().GetMember(src_.ToString());
        //    var attributes = gm[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
        //    var description = ((DescriptionAttribute)attributes[0]).Description;
        //    return description;
        //}

        public List<LicenseView> ListLicenseGroup(DateTime? date_)
        {
            var rtn = new List<LicenseView>();

            var flg = (date_ == null);

            foreach (var group in this.Keys)
            {
                var list = this[group]?.Where(x_ => ((x_.CheckOut().EventDateTime.Date == date_) && AnalysisManager.Instance.IsChecked(x_.Product) ==true )|| flg);

                if (list?.Count() > 0)
                {
                    var view = new LicenseView()
                    {
                        Name = group,
                        Count = list.Count(),
                        Duration = new TimeSpan(list.Sum(x_ => x_.Duration.Ticks)),
                    };
                    rtn.Add(view);
                }
            }

            return rtn;
        }
    }
}
