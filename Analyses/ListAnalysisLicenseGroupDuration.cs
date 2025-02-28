using RepriseReportLogAnalyzer.Enums;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Windows;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace RepriseReportLogAnalyzer.Analyses
{
    internal class ListAnalysisLicenseGroupDuration:Dictionary<string, ListAnalysisCheckOutIn>
    {
        public ProgressCountDelegate? ProgressCount = null;
        private const string _ANALYSIS = "[License Group Duration]";

        public ListAnalysisLicenseGroupDuration(ANALYSIS_GROUP group_)
        {
            _group = group_;
        }

        public void Analysis(IEnumerable<string> listGroup_, ListAnalysisCheckOutIn listCheckOutIn_)
        {
            switch (_group)
            {
                case ANALYSIS_GROUP.USER:
                    foreach (var group in listGroup_)
                    {
                        this[group] = new ListAnalysisCheckOutIn(listCheckOutIn_.ListDuplication().Where(x_=>x_.User ==group));
                    }
                    break;
                case ANALYSIS_GROUP.HOST:
                    foreach (var group in listGroup_)
                    {
                        this[group] = new ListAnalysisCheckOutIn(listCheckOutIn_.ListDuplication().Where(x_ => x_.Host == group));
                    }
                    break;
                case ANALYSIS_GROUP.USER_HOST:
                    foreach (var group in listGroup_)
                    {
                        this[group] = new ListAnalysisCheckOutIn(listCheckOutIn_.ListDuplication().Where(x_ => x_.UserHost == group));
                    }
                    break;
                default:
                    break;
            }
        }

        private ANALYSIS_GROUP _group = ANALYSIS_GROUP.NONE;

        public string Header { get => GetName(_group) + ",Duration,Days,Count"; }

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

        // https://www.sejuku.net/blog/42539
        private string GetName(ANALYSIS_GROUP src_)
        {
            var gm = src_.GetType().GetMember(src_.ToString());
            var attributes = gm[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            var description = ((DescriptionAttribute)attributes[0]).Description;
            return description;
        }
    }
}
