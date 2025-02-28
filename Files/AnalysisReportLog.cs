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
                foreach (var s in File.ReadAllLines(filePath_).Where(s_ => string.IsNullOrEmpty(s_)==false))
                {
                    var eventBase = LogEventBase.EventData(s);

                    if (eventBase != null)
                    {
                        ListEvent.Add(eventBase);
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
        public List<LogEventBase> ListEvent { get; private set; } = new List<LogEventBase>();

        public List<LogEventStart> ListStart { get => GetListEvent<LogEventStart>(); }
        public List<LogEventLogFileEnd> ListEnd { get => GetListEvent<LogEventLogFileEnd>(); }
        public List<LogEventShutdown> ListShutdown { get => GetListEvent<LogEventShutdown>(); }

        public List<LogEventCheckIn> ListCheckIn { get => GetListEvent<LogEventCheckIn>(); }
        public List<LogEventCheckOut> ListCheckOut { get => GetListEvent<LogEventCheckOut>(); }

        public IEnumerable<ILogEventProduct> ListProduct
        {
            get
            {
                _listProduct = _listProduct ?? ListEvent.AsParallel().Where(e_ => e_ is ILogEventProduct).Select(e_ => e_ as ILogEventProduct)
                                                                                               .Where(e_=>string.IsNullOrEmpty(e_.Product) ==false).Distinct(new CompareProduct()).OrderBy(p_ => p_.Product).ThenBy(p_ => p_.Version);

                return _listProduct;
            }
        }
        private IEnumerable<ILogEventProduct>? _listProduct = null;

        public IEnumerable<string> ListUser
        {
            get
            {
                _listUser = _listUser ?? ListEvent.AsParallel().Where(e_ => e_ is ILogEventUser).Select(e_ => e_ as ILogEventUser)
                                                                                .Select(e_ => e_.User).Where(e_ => string.IsNullOrEmpty(e_) == false).Distinct().OrderBy(x_ => x_);
                return _listUser;
            }
        }
        IEnumerable<string>? _listUser = null;


        public IEnumerable<string> ListHost
        {
            get
            {
                _listHost = _listHost?? ListEvent.AsParallel().Where(e_ => e_ is ILogEventHost).Select(e_ => e_ as ILogEventHost).Select(e_ => e_.Host).Distinct().OrderBy(x_ => x_);

                return _listHost;
            }
        }
        IEnumerable<string>? _listHost = null;

        public IEnumerable<string> ListUserHost
        {
            get
            {
                _listUserHost = _listUserHost?? ListEvent.AsParallel().Where(e_ => e_ is ILogEventUserHost).Select(e_ => e_ as ILogEventUserHost).Select(e_=>e_.UserHost).Where(e_=>e_ !="@").Distinct().OrderBy(x_ => x_);

                return _listUserHost;
            }
        }
        IEnumerable<string>? _listUserHost = null;

        public IEnumerable<DateTime> ListDateTime
        {
            get
            {
                _listDateTime = _listDateTime ?? ListEvent.AsParallel().Where(e_ => (e_ is LogEventRlmReportLogFormat) == false).Select(e_ => e_.EventDateTime).Distinct().OrderBy(e_ => e_);
                return _listDateTime;
            }
        }
        public IEnumerable<DateTime>? _listDateTime = null;

        public IEnumerable<DateTime> ListDate
        {
            get
            {
                _listDate = _listDate ?? ListDateTime.AsParallel().Select(t_ => t_.Date).Distinct().OrderBy(e_ => e_);
                return _listDate;
            }
        }
        private IEnumerable<DateTime>? _listDate = null;


        public List<T> GetListEvent<T>(AnalysisStartShutdown? ss_ = null) where T : LogEventBase
        {
            List<LogEventBase>? rtn = new List<LogEventBase>();
            if (ss_ == null)
            {
                if (_listEvent.TryGetValue(typeof(T), out rtn) == true)
                {
                    LogFile.Instance.WriteLine($"[{typeof(T)}] {rtn.Count}");
                }
                else
                {
                    rtn = new List<LogEventBase>();
                    rtn.AddRange(ListEvent.AsParallel().Where(e_=> e_ is T).Select(e_ => e_ as T).OrderBy(x_ => x_.EventNumber));

                    _listEvent[typeof(T)] = rtn;
                }
            }
            else
            {
                rtn.AddRange(ListEvent.AsParallel().Where(e_ => (e_ is T) && (ss_.IsWithInRange(e_.EventNumber) == true)).Select(e_ => e_ as T)?.OrderBy(x_ => x_.EventNumber));
            }
            LogFile.Instance.WriteLine($"[{typeof(T)}] {rtn.Count}");

            return rtn.Select(x_ => x_ as T).ToList();
            //return rtn;
        }
        private Dictionary<Type, List<LogEventBase>> _listEvent = new Dictionary<Type, List<LogEventBase>>();

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