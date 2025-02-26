using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Files;
using System.Reflection;

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

        public static DateTime NowDateTime =DateTime.Now;
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

        public DateTime EventDateTimeUnit(long timeSpan_)
        {
            return new DateTime(EventDateTime.Ticks - (EventDateTime.Ticks % timeSpan_));
        }

        public static LogEventBase? EventData(string str_)
        {
            var list = _splitSpace(str_);

            NewEvent? newEvent =null;
            if (_ListEventData.TryGetValue(list[0], out newEvent) == false)
            {
                if (list.Count() == 2 && list[0].Contains("/") == true && list[1].Contains(":") == true)
                {
                    return new LogEventTimeStamp(list);
                }
                return null;
            }

            return newEvent(list);
        }

        public LogEventBase()
        {
            //
            EventNumber = ++NowEventNumber;
        }

        public static void Clear()
        {

            NowEventNumber = 0;
            NowDateTime = DateTime.Now;

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
