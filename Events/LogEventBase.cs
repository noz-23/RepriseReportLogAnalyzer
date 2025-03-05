using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Files;
using System.Reflection;
using static RepriseReportLogAnalyzer.Events.LogEventBase;

namespace RepriseReportLogAnalyzer.Events
{
    internal partial class LogEventRegist: LogEventBase
    {
        public static LogEventRegist Instance = new LogEventRegist();
        private LogEventRegist()
        {
        }

        public void Create()
        {
            LogFile.Instance.WriteLine($"{LogEventBase.ListEventDataCount()}");
        }

        public static bool Regist(string key_, NewEvent event_)
        {
            _ListEventData.Add(key_, event_);
            return true;
        }
    }

    internal partial class LogEventBase
    {
        public delegate LogEventBase NewEvent(string [] list);
        protected static Dictionary<string, NewEvent> _ListEventData =new Dictionary<string, NewEvent>();

        public static int ListEventDataCount() => _ListEventData.Count;

        public readonly static DateTime NotAnalysisEventTime = DateTime.Now;

        public static DateTime NowDateTime = NotAnalysisEventTime;
        public static long NowEventNumber = 0;


        protected static long _NowYear { get => NowDateTime.Year; }
        protected static long _NowMonth { get => NowDateTime.Month; }
        protected static string _NowDate { get => NowDateTime.ToString("MM/dd/yyyy"); }
        //
        [ColumnSort(1)]
        public long EventNumber { get; protected set; } = 0;
        //
        [ColumnSort(2)]
        public DateTime EventDateTime
        {
            get => _eventDateTime;
            set
            {
                _eventDateTime = value;
                NowDateTime = value;
            }
        }
        private DateTime _eventDateTime;

        public DateTime EventDate()=> EventDateTimeUnit(TimeSpan.TicksPerDay);

        /// <summary>
        /// 日時を単位時間に分けた時の時間帯
        /// 15分単位なら 16:13 →16:00
        /// </summary>
        /// <param name="timeSpan_">時間帯</param>
        /// <returns></returns>
        public DateTime EventDateTimeUnit(long timeSpan_)
        {
            return new DateTime(EventDateTime.Ticks - (EventDateTime.Ticks % timeSpan_));
        }

        public static LogEventBase? EventData(string str_)
        {
            var list = _splitSpace(str_);

            //NewEvent? newEvent =null;
            //if (_ListEventData.TryGetValue(list[0], out newEvent) == false)
            //{
            //    if (list.Count() == 2 && list[0].Contains("/") == true && list[1].Contains(":") == true)
            //    {
            //        return new LogEventTimeStamp(list);
            //    }
            //    return null;
            //}

            //return newEvent(list);

            if (_ListEventData.TryGetValue(list[0], out var newEvent) == true)
            {
                return newEvent?.Invoke(list);
            }

            if (list.Count() == 2 && list[0].Contains("/") == true && list[1].Contains(":") == true)
            {
                return new LogEventTimeStamp(list);
            }
            return null;
        }

        public LogEventBase()
        {
            //
            EventNumber = ++NowEventNumber;
        }

        public static void Clear()
        {

            NowEventNumber = 0;
            NowDateTime = NotAnalysisEventTime;

        }

        // ｢a "b c" d｣→｢a,"b c",d｣
        private static string[] _splitSpace(string str_)
        {
            var list = str_.Split(' ');// ｢a "b c" d｣→｢a,"b,c",d｣
            var rtn = new List<string>();

            string tmp = string.Empty;
            foreach (var s in list)
            {
                if (string.IsNullOrEmpty(tmp) == true)
                {
                    if (s.Contains("\"") == true)
                    {
                        if (s.IndexOf("\"") == s.LastIndexOf("\""))
                        {
                            tmp += s;
                            continue;
                        }
                    }
                }
                else
                {
                    if (s.Contains("\"") == true)
                    {
                        tmp += s;
                        rtn.Add(tmp);
                        tmp = string.Empty;
                    }
                    else
                    {
                        tmp += s;
                    }
                    continue;
                }
                rtn.Add(s);
            }
            // ｢a,"b,c",d｣→｢a,"b c",d｣
            return rtn.ToArray();
        }

        protected static DateTime _GetDateTime(string date_, string time_)
        {
            var listDate = date_.Split('/');
            var month = int.Parse(listDate[0]);
            // New Year
            var year = (month < _NowMonth) ? (_NowYear + 1) : (_NowYear);
            return DateTime.Parse(date_ + "/" + year + " " + time_);
        }

        public static string Header<T>() 
        {
                var listColunm = new List<string>();
                var listPropetyInfo = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)?.OrderBy(s_ => (Attribute.GetCustomAttribute(s_, typeof(ColumnSortAttribute)) as ColumnSortAttribute)?.Sort);

                listPropetyInfo?.ToList().ForEach(prop =>
                {
                    listColunm.Add($"{prop.Name}");
                });

                return string.Join(",", listColunm);
        }

        public override string ToString()
        {

            var listValue = new List<string>();
            var listPropetyInfo = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)?.OrderBy(s_ => (Attribute.GetCustomAttribute(s_, typeof(ColumnSortAttribute)) as ColumnSortAttribute)?.Sort);

            listPropetyInfo?.ToList().ForEach(prop =>
            {
                listValue.Add($"{prop.GetValue(this)}");
            });

            return string.Join(",", listValue);
        }
    }
}
