using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Windows;
using System.IO;
using System.Text;

namespace RepriseReportLogAnalyzer.Analyses
{
    internal class ListAnalysisCheckOutIn: List<AnalysisCheckOutIn>
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ListAnalysisCheckOutIn()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="list_">グループ集計</param>
        public ListAnalysisCheckOutIn(IEnumerable<AnalysisCheckOutIn> list_)
        {
            list_?.ToList().ForEach(item => this.Add(item));
        }

        public ProgressCountDelegate? ProgressCount = null;

        private const string _ANALYSIS = "[CheckOut - CheckIn]";

        public void Analysis(AnalysisReportLog log_, IEnumerable<AnalysisStartShutdown> listStartShutdown_)
        {
            int count = 0;
            int max = 0;
            var listCheckOutIn = new Dictionary<string, List<AnalysisCheckOutIn>>();
            foreach (var startShutdown in listStartShutdown_)
            {
                var listCheckOut = log_.GetListEvent<LogEventCheckOut>(startShutdown);
                var listCheckIn = log_.GetListEvent<LogEventCheckIn>(startShutdown);

                LogFile.Instance.WriteLine($"{startShutdown.StartDateTime.ToString()} - {startShutdown.ShutdownDateTime.ToString()} : {listCheckOut.Count}");

                count = 0;
                max = listCheckOut.Count;
                ProgressCount?.Invoke(0, max, _ANALYSIS + "Join");
                foreach (var checkOut in listCheckOut)
                {
                    AnalysisCheckOutIn data;
                    try
                    {
                        var checkIn = listCheckIn.AsParallel().AsOrdered().First(f_ => checkOut.IsFindCheckIn(f_));
                        data = new AnalysisCheckOutIn( checkOut, checkIn);

                        if (checkIn != null)
                        {
                            listCheckIn.Remove(checkIn);
                        }
                    }
                    catch
                    {
                        data = new AnalysisCheckOutIn(checkOut, startShutdown?.EventShutdown());
                        LogFile.Instance.WriteLine($"Not Found");
                    }

                    this.Add(data);
                    // 重複のチェック
                    var key = $"{data.Product} {data.UserHost}";
                    if (listCheckOutIn.ContainsKey(key) == false)
                    {
                        listCheckOutIn[key] = new List<AnalysisCheckOutIn>();
                    }
                    listCheckOutIn[key].Add(data);

                    ProgressCount?.Invoke(++count, max);
                }
            }

            // 重複のチェック
            count = 0;
            max = listCheckOutIn.Keys.Count;
            ProgressCount?.Invoke(0, max, _ANALYSIS + "Renew");
            foreach (var key in listCheckOutIn.Keys)
            {
                var listNoCheck = new List<AnalysisCheckOutIn>();
                var listKeyData = listCheckOutIn[key].OrderBy(x_=>x_.CheckOutNumber());

                // 時間内に含まれるデータは削除
                foreach (var data in listKeyData)
                {
                    if (listNoCheck.Contains(data) == true)
                    {
                        continue;
                    }
                    var list = listKeyData.AsParallel().Where(x_ => data.IsWithInRange(x_.CheckOutNumber()) && data.IsWithInRange(x_.CheckInNumber()));
                    list.ForAll(x_ => x_.JoinEvent().SetDuplication());

                    listNoCheck.AddRange(list);
                }

                foreach (var data in listKeyData)
                {
                    if (listNoCheck.Contains(data) == true)
                    {
                        continue;
                    }

                    var list = listKeyData.AsParallel().Where(x_ => data.IsWithInRange(x_.CheckOutNumber()) && (listNoCheck.Contains(x_) == false)).OrderBy(x_ => x_.CheckInNumber());
                    if (list.Count() > 0)
                    {
                        // 時間を更新して追加
                        var renew = list.Last();
                        data.JoinEvent().SetDuplication(renew.CheckIn());

                        listNoCheck.AddRange(list);

                        LogFile.Instance.WriteLine($"{data.CheckOutNumber()} : {data.CheckInNumber()} -> {renew.CheckInNumber()}");
                        continue;
                    }
                }
                ProgressCount?.Invoke(++count, max);
            }
        }


        public LogEventCheckOut? Find(LogEventCheckIn checkIn_)
        {
            return this.ToList().Find(x_ => x_.IsSame(checkIn_))?.CheckOut() ?? null;
        }

        private List<string> _listToString(bool duplication_)
        {
            var rtn = new List<string>();
            var list = (duplication_ == false) ? this : ListDuplication();
            foreach (var data in list)
            {
                rtn.Add(data.ToString(duplication_));
            }

            return rtn;
        }

        public void WriteText(string path_, bool duplication_ = false)
        {
            var list = new List<string>();
            list.Add(AnalysisCheckOutIn.HEADER);
            list.AddRange(_listToString(duplication_));
            File.WriteAllLines(path_, list, Encoding.UTF8);
        }

        public IEnumerable<AnalysisCheckOutIn> ListDuplication()
        {
            return this.Where(x_ => x_.JoinEvent().DuplicationNumber != JoinEventCheckOutIn.DUPLICATION);
        }

        public IEnumerable<JoinEventCheckOutIn> ListJointEvetn()
        {
            return this.Select(x_ => x_.JoinEvent());
        }


        public void WriteDuplicationText(string path_)
        {
            var list = new List<string>();
            list.Add(JoinEventCheckOutIn.HEADER);
            list.AddRange(ListJointEvetn().Select(x_=>x_.ToString()));
            File.WriteAllLines(path_, list, Encoding.UTF8);
        }


    }
}
