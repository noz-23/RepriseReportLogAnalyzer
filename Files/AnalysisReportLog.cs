using RepriseReportLogAnalyzer.Analyses;
using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Interfaces;
using System.IO;
using System.Text;

namespace RepriseReportLogAnalyzer.Files
{
    internal class AnalysisReportLog
    {
        //
        public AnalysisReportLog()
        {
            LogEventBase.Clear();

        }
        public void StartAnalysis(string filePath_)
        {
            if (File.Exists(filePath_) == true)
            {
                foreach (var s in File.ReadAllLines(filePath_).Where(s_ => string.IsNullOrEmpty(s_) == false))
                {
                    var eventBase = LogEventBase.EventData(s);

                    if (eventBase != null)
                    {
                        ListEvent.Add(eventBase);

                        //if (eventBase.EventDateTime > DateTime.Now.AddDays(-1))
                        //{
                        //    LogFile.Instance.WriteLine($"{eventBase.GetType()} : {eventBase.EventNumber}");

                        //}
                        //} else
                        //{
                        //    Trace.WriteLine($"{s}");
                    }
                }
                LogFile.Instance.WriteLine($"Read:{ListEvent.Count}");

            }
        }

        public void EndAnalysis()
        {
            var end = new LogEventShutdown();
            ListEvent.Add(end);
        }
        //
        public List<LogEventBase> ListEvent { get; private set; } = new();

        public List<LogEventStart> ListStart { get => GetListEvent<LogEventStart>(); }
        public List<LogEventLogFileEnd> ListEnd { get => GetListEvent<LogEventLogFileEnd>(); }
        public List<LogEventShutdown> ListShutdown { get => GetListEvent<LogEventShutdown>(); }

        public List<LogEventCheckIn> ListCheckIn { get => GetListEvent<LogEventCheckIn>(); }
        public List<LogEventCheckOut> ListCheckOut { get => GetListEvent<LogEventCheckOut>(); }

        public HashSet<ILogEventProduct> ListProduct
        {
            get
            {
                _listProduct ??= new(ListEvent.AsParallel().Where(e_ => e_ is ILogEventProduct).Select(e_ => e_ as ILogEventProduct)
                                                                                               .Where(e_ => string.IsNullOrEmpty(e_.Product) == false).Distinct(new CompareProduct()).OrderBy(p_ => p_.Product).ThenBy(p_ => p_.Version));

                return _listProduct;
            }
        }
        private HashSet<ILogEventProduct>? _listProduct = null;

        public SortedSet<string> ListUser
        {
            get
            {
                _listUser ??= new(ListEvent.AsParallel().Where(e_ => e_ is ILogEventUser).Select(e_ => e_ as ILogEventUser)
                                                                .Select(e_ => e_.User).Where(e_ => string.IsNullOrEmpty(e_) == false).Distinct());
                return _listUser;
            }
        }
        SortedSet<string>? _listUser = null;


        public SortedSet<string> ListHost
        {
            get
            {
                _listHost ??= new(ListEvent.AsParallel().Where(e_ => e_ is ILogEventHost).Select(e_ => e_ as ILogEventHost)
                                                                .Select(e_ => e_.Host).Where(e_ => string.IsNullOrEmpty(e_) == false));

                return _listHost;
            }
        }
        SortedSet<string>? _listHost = null;

        public SortedSet<string> ListUserHost
        {
            get
            {
                _listUserHost ??= new(ListEvent.AsParallel().Where(e_ => e_ is ILogEventUserHost).Select(e_ => e_ as ILogEventUserHost)
                                                                .Select(e_ => e_.UserHost).Where(e_ => e_ != "@").Distinct());

                return _listUserHost;
            }
        }
        SortedSet<string>? _listUserHost = null;

        public SortedSet<DateTime> ListDateTime
        {

            get
            {
                //_listDateTime ??=new ( ListEvent.AsParallel().Where(e_ => ( (e_ !=null) || (e_ is LogEventRlmReportLogFormat) == false))
                //                                                .Select(e_ => e_.EventDateTime));
                _listDateTime ??= new(ListEvent.AsParallel().Select(e_ => e_.EventDateTime).Where(e_ => e_ != LogEventBase.NotAnalysisEventTime));
                return _listDateTime;
            }
        }
        public SortedSet<DateTime>? _listDateTime = null;

        public SortedSet<DateTime> ListDate
        {
            get
            {
                _listDate ??= new(ListDateTime.AsParallel().Select(t_ => t_.Date));
                return _listDate;
            }
        }
        private SortedSet<DateTime>? _listDate = null;


        public List<T> GetListEvent<T>(AnalysisStartShutdown? ss_ = null) where T : LogEventBase
        {
            List<LogEventBase>? rtn = new();
            if (ss_ == null)
            {
                if (_listEvent.TryGetValue(typeof(T), out rtn) == true)
                {
                    LogFile.Instance.WriteLine($"[{typeof(T)}] {rtn.Count}");
                }
                else
                {
                    rtn = new();
                    rtn.AddRange(ListEvent.AsParallel().Where(e_ => e_ is T).Select(e_ => e_ as T).OrderBy(x_ => x_.EventNumber));

                    _listEvent[typeof(T)] = rtn;
                }
            }
            else
            {
                rtn.AddRange(ListEvent.AsParallel().Where(e_ => (e_ is T) && (ss_.IsWithInRange(e_.EventNumber) == true)).Select(e_ => e_ as T)?.OrderBy(x_ => x_.EventNumber));
            }
            LogFile.Instance.WriteLine($"[{typeof(T)}] {rtn.Count}");

            return rtn.Select(x_ => x_ as T).ToList();
        }
        private Dictionary<Type, List<LogEventBase>> _listEvent = new();

        public void WriteSummy(string path_)
        {
            var list = new List<string>();

            list.Add("License");
            list.AddRange(ListProduct.Select(x_ => $"{x_.Product},{x_.Version}"));
            list.Add("\n");
            LogFile.Instance.WriteLine($"ListProduct:{ListProduct.Count()}");

            list.Add("User");
            list.AddRange(ListUser);
            list.Add("\n");
            LogFile.Instance.WriteLine($"ListUser:{ListUser.Count()}");

            list.Add("Host");
            list.AddRange(ListHost);
            list.Add("\n");
            LogFile.Instance.WriteLine($"ListHost:{ListHost.Count()}");

            list.Add("User@Host");
            list.AddRange(ListUserHost);
            list.Add("\n");
            LogFile.Instance.WriteLine($"ListUserHost:{ListUserHost.Count()}");

            File.WriteAllLines(path_, list, Encoding.UTF8);
            LogFile.Instance.WriteLine($"Write:{path_}");
        }

        private List<string> _listToString<T>() where T : LogEventBase
        {
            var rtn = new List<string>();
            var list = GetListEvent<T>();

            foreach (var data in list)
            {
                rtn.Add(data.ToString());
            }

            return rtn;
        }

        public void WriteText<T>(string path_) where T : LogEventBase
        {
            var list = new List<string>();

            //list.Add(_getHeader<T>());
            list.Add(LogEventBase.Header<T>());
            list.AddRange(_listToString<T>());
            File.WriteAllLines(path_, list, Encoding.UTF8);

            LogFile.Instance.WriteLine($"Write:{path_}");
        }

        //private string _getHeader<T>()
        //{
        //    var listPropetyInfo = typeof(T).GetProperties(BindingFlags.Static | BindingFlags.Public);
        //    foreach (var prop in listPropetyInfo)
        //    {
        //        if (prop.Name == "HEADER")
        //        {
        //            return prop?.GetValue(null).ToString();
        //        }
        //    }
        //    return string.Empty;
        //}

        private class CompareProduct : IEqualityComparer<ILogEventProduct>
        {
            public bool Equals(ILogEventProduct? a_, ILogEventProduct? b_)
            {
                if (a_ == null)
                {
                    return false;
                }
                if (b_ == null)
                {
                    return false;
                }

                if (a_.Product != b_.Product)
                {
                    return false;
                }
                if (a_.Version != b_.Version)
                {
                    return false;
                }

                return true;
            }
            public int GetHashCode(ILogEventProduct codeh_)
            {
                return codeh_.Product.GetHashCode() ^ codeh_.Version.GetHashCode();
            }
        }
    }
}