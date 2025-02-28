using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Views;
using RepriseReportLogAnalyzer.Windows;
using System;
using System.IO;
using System.Text;
using System.Windows.Documents;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RepriseReportLogAnalyzer.Analyses
{
    /// <summary>
    /// ライセンスの利用状況集計
    /// 　EventBase   :基本となるイベント(時間)
    /// 　CountProduct:プロダクト-集計処理でのカウント
    /// 　MaxProduct  :プロダクト-最大数
    /// 　OutInProduct:プロダクト-ログの数値
    /// </summary>
    internal class ListAnalysisLicenseCount : List<(LogEventBase EventBase, Dictionary<string, int> CountProduct, Dictionary<string, int> MaxProduct, Dictionary<string, int> OutInProduct)>
    {
        public ProgressCountDelegate? ProgressCount = null;
        private const string _ANALYSIS = "[License Count]";


        public ListAnalysisLicenseCount()
        { 
        }

        private SortedSet<string> _listProduct = new SortedSet<string>();

        /// <summary>
        /// 集計処理でのカウント
        /// </summary>
        private Dictionary<string, int> _listCount = new Dictionary<string, int>();

        /// <summary>
        /// 最大数
        /// </summary>
        private Dictionary<string, int> _listHave = new Dictionary<string, int>();

        /// <summary>
        /// ログの数値(=_listCount)
        /// </summary>
        private Dictionary<string, int> _listCountOutIn = new Dictionary<string, int>();


        Dictionary<DateTime, IEnumerable<LicenseView>> _listDayToProduct = new Dictionary<DateTime, IEnumerable<LicenseView>>();

        public void Analysis(AnalysisReportLog log_, ListAnalysisCheckOutIn listCheckOutIn_)
        {
            _listProduct.UnionWith(log_.ListProduct.Select(x_ => x_.Product));

            _clearCount();

            int count = 0;
            int max = log_.ListEvent.Count;

            ProgressCount?.Invoke(0, max, _ANALYSIS);
            foreach (var ev in log_.ListEvent)
            {
                if (ev is LogEventRlmReportLogFormat)
                {
                    // 時間が定まってないため処理しない
                    continue;
                }

                if (ev is LogEventProduct eventProduct)
                {
                    if (_setHave(eventProduct.Product, eventProduct.Count) == true)
                    {
                        _add(eventProduct);
                    }
                }

                if (ev is LogEventCheckOut eventCheckOut)
                {
                    if (_setCountUp(eventCheckOut.Product, eventCheckOut.CountCurrent) == true)
                    {
                        _add(eventCheckOut);
                    }
                }
                if (ev is LogEventCheckIn eventCheckIn)
                {
                    var flg = true;
                    if (string.IsNullOrEmpty(eventCheckIn.Product) == false)
                    {
                        flg =_setCountDown(eventCheckIn.Product, eventCheckIn.CountCurrent);
                    }
                    else
                    {
                        LogFile.Instance.WriteLine($"CheckIn: {eventCheckIn.EventNumber} {eventCheckIn.EventDateTime} {eventCheckIn.Product}");

                        var checkOut = listCheckOutIn_.Find(eventCheckIn);
                        if (checkOut != null)
                        {
                            LogFile.Instance.WriteLine($"CheckOut: {checkOut.EventNumber} {checkOut.EventDateTime} {checkOut.Product}");
                            flg = _setCountDown(checkOut.Product, eventCheckIn.CountCurrent);

                        }
                        else
                        {
                            LogFile.Instance.WriteLine($"Not Fount: {eventCheckIn.EventNumber} {eventCheckIn.EventDateTime} {eventCheckIn.Product}");
                        }

                    }
                    if (flg == true)
                    {
                        _add(eventCheckIn);
                    }
                }
                if (ev is LogEventShutdown eventShutdown)
                {
                    _clearCount();
                    _add(eventShutdown);
                }
                if (ev is LogEventTimeStamp eventTimeStamp)
                {
                    _add(eventTimeStamp);

                }
                ProgressCount?.Invoke(++count, max);
                //_add(ev);
            }

            var minDate = this.Select(x => x.EventBase.EventDate()).Min();
            var maxDate = this.Select(x => x.EventBase.EventDate()).Max();
            for (var date = minDate; date < maxDate.AddDays(1); date = date.AddTicks(TimeSpan.TicksPerDay))
            {
                _listDayToProduct[date] = _getDayToProduct(date);
            }

        }

        public void _add(LogEventBase logEventBase_)
        {
            // その時の値を入れる
            this.Add((logEventBase_, new Dictionary<string, int>(_listCount), new Dictionary<string, int>(_listHave), new Dictionary<string, int>(_listCountOutIn)));
        }

        private void _clearCount()
        {
            foreach (var product in _listProduct)
            {
                _listCount[product] = 0;
                _listHave[product] = 0;
                _listCountOutIn[product] = 0;
            }
        }

        private bool _setHave(string product_, int count_)
        {
            if (string.IsNullOrEmpty(product_) == true)
            {
                return false;
            }
            _listHave[product_] = count_;
            return true;
        }

        private bool _setCountUp(string product_, int count_)
        {
            if (string.IsNullOrEmpty(product_) == true)
            {
                return false;
            }
            if (_listCountOutIn[product_] == count_)
            {
                // 重複チェック
                return false;
            }

            _listCount[product_]++;
            _listCountOutIn[product_] = count_;
            return true;
        }

        private bool _setCountDown(string product_, int count_)
        {
            if (string.IsNullOrEmpty(product_) == true)
            {
                return false;
            }
            if (_listCountOutIn[product_] == count_)
            {
                // 重複チェック
                return false;
            }
            _listCount[product_]--;
            _listCountOutIn[product_] = count_;
            return true;
        }

        public string Header
        {
            get
            {
                var list = new List<string>();
                list.Add("Date");
                list.Add("Time");
                foreach (var product in _listProduct)
                {
                    //list.Add($"{product}[Use]");
                    list.Add($"{product}[Have]");
                    list.Add($"{product}[OutIn]");
                }
                return string.Join(",", list);
            }
        }

        private List<string> _listToString()
        {
            var rtn = new List<string>();
            foreach (var d in this)
            {
                var add = new List<string>();
                var dateTime = d.EventBase.EventDateTime.ToString().Split(" ");
                add.Add(dateTime[0]);
                add.Add(dateTime[1]);
                foreach (var product in _listProduct)
                {
                    //add.Add(d.countProduct[product].ToString());
                    add.Add(d.MaxProduct[product].ToString());
                    add.Add(d.OutInProduct[product].ToString());
                }
                rtn.Add(string.Join(",", add));
            }
            return rtn;
        }

        private List<string> _listTimeSpanString(long timeSpan_)
        {
            var rtn = new List<string>();

            var listNowMax = new Dictionary<string, int>();
            _listProduct.ToList().ForEach(product => listNowMax[product] = 0);

            var listTimeSpan = _getListTimeSpan(timeSpan_);
            foreach (var dateTimeSpan in listTimeSpan)
            {
                var add = new List<string>();
                add.Add(dateTimeSpan.Date.ToShortDateString());
                add.Add(dateTimeSpan.TimeOfDay.ToString());
                //
                var listData = this.Where(d_ => d_.EventBase.EventDateTimeUnit(timeSpan_) == dateTimeSpan);
                foreach (var product in _listProduct)
                {
                    if ((listData?.Count() ?? 0) == 0)
                    {
                        add.Add("0");
                        add.Add(listNowMax[product].ToString());
                        continue;
                    }
                    var countMax = listData?.Select(x_ => x_.CountProduct[product]).Max() ?? 0;
                    var haveMax = listData?.Select(x_ => x_.MaxProduct[product]).Max() ?? 0;
                    var outIn = listData?.Select(x_ => x_.OutInProduct[product]).Max() ?? 0;

                    add.Add(countMax.ToString());
                    add.Add(haveMax.ToString());

                    listNowMax[product] = haveMax;
                }

                rtn.Add(string.Join(",", add));
            }

            return rtn;
        }

        private IEnumerable<LicenseView> _getDayToProduct(DateTime date_)
        {
            var rtn = new List<LicenseView>();

            var listSelectDay = this.Where(x_ => x_.EventBase.EventDateTime.Date == date_);
            foreach (var product in _listProduct)
            {
                if (listSelectDay.Any() == false)
                {
                    continue;
                }
                //var minTime = listSelectDay?.Select(x_ => x_.EventBase.EventDateTime).Min() ?? DateTime.Now;
                //var maxTime = listSelectDay?.Select(x_ => x_.EventBase.EventDateTime).Max() ?? DateTime.Now;

                var view = new LicenseView()
                {
                    Name = product,
                    Date = date_,
                    Count = listSelectDay?.Select(x_ => x_.CountProduct[product]).Max() ?? 0,
                    Max = listSelectDay?.Select(x_ => x_.MaxProduct[product]).Max() ?? 0,
                    //Duration = maxTime - minTime
                };
                rtn.Add(view);
            }
            return rtn;
        }


        public IEnumerable<LicenseView> ListDayToProduct(DateTime date_)
        {
            if (_listDayToProduct.TryGetValue(date_, out var rtn) == true)
            {
                return rtn;
            }
            return new List<LicenseView>();
        }
        //public List<LicenseView> GetCount(DateTime date_, long timeSpan_, string product_)
        //{
        //    var rtn = new List<LicenseView>();

        //    //TimeSpan.TicksPerDay
        //    var listSelectDay = this.Where(x_ => x_.EventBase.EventDateTime.Date == date_);
        //    for(var time = date_;time<date_.AddDays(1); time = time.AddTicks(timeSpan_))
        //    {
        //        var view = new LicenseView()
        //        {
        //            Date = time,
        //            Count = listSelectDay?.Select(x_ => x_.CountProduct[product_]).Max() ?? 0,
        //            Max = listSelectDay?.Select(x_ => x_.MaxProduct[product_]).Max() ?? 0
        //        };

        //        rtn.Add(view);
        //    }

        //    return rtn;
        //}

        public void WriteText(string path_)
        {
            var list = new List<string>();
            list.Add(Header);
            list.AddRange(_listToString());
            File.WriteAllLines(path_, list, Encoding.UTF8);

            LogFile.Instance.WriteLine($"Write:{path_}");
        }

        public void WriteTimeSpanText(string path_, long timeSpan_)
        {
            var list = new List<string>();
            list.Add(Header);
            list.AddRange(_listTimeSpanString(timeSpan_));
            File.WriteAllLines(path_, list, Encoding.UTF8);

            LogFile.Instance.WriteLine($"Write:{path_}");
        }

        private List<DateTime> _getListTimeSpan(long timeSpan_)
        {
            var rtn = new List<DateTime>();

            var minDate = this.Select(x => x.EventBase.EventDate()).Min();
            var maxDate = this.Select(x => x.EventBase.EventDate()).Max();

            for (var date = minDate; date < maxDate.AddDays(1); date = date.AddTicks(timeSpan_))
            {
                rtn.Add(date);
            }

            return rtn;
        }
    }
}
