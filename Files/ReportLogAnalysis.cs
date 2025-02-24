using RepriseReportLogAnalyzer.Analyses;
using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Interfaces;
using RepriseReportLogAnalyzer.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RepriseReportLogAnalyzer.Files
{
    internal class ReportLogAnalysis
    {
        //
        public ReportLogAnalysis()
        {
            LogEventBase.Clear();

        }
        public void StartAnalysis(string filePath_)
        {
            if (File.Exists(filePath_) == true)
            {
                var listRead = new List<string>(File.ReadAllLines(filePath_));
                listRead.RemoveAll(s_ => string.IsNullOrEmpty(s_));
                foreach (var s in listRead)
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
            ListEvent.Add(new LogEventShutdown());
        }
        //
        public List<LogEventBase> ListEvent { get; private set; } = new List<LogEventBase>();
        //public List<LogEventBase> ListEventAll { get; private set; }

        //public List<LogEventBase> ListOverLap { get; private set; } = new List<LogEventBase>();


        public List<LogEventStart> ListStart { get => GetListEvent<LogEventStart>(); }
        public List<LogEventLogFileEnd> ListEnd { get => GetListEvent<LogEventLogFileEnd>(); }
        public List<LogEventShutdown> ListShutdown { get => GetListEvent<LogEventShutdown>(); }

        public List<LogEventCheckIn> ListCheckIn { get => GetListEvent<LogEventCheckIn>(); }
        public List<LogEventCheckOut> ListCheckOut { get => GetListEvent<LogEventCheckOut>(); }

        //public List<(string Product, string Version)> ListProduct
        public List<ILogEventProduct> ListProduct
        {
            get
            {
                //if (_listProduct == null)
                //{
                //    //_listProduct = new List<(string Product, string Version)>(ListEvent?.AsParallel().Where(e_ => e_ is ILogEventProduct)
                //    //    .Select(p_ => (Product: (p_ as ILogEventProduct).Product, Version: (p_ as ILogEventProduct).Version)).Distinct()
                //    //    .OrderBy(p_ => p_.Product).ThenBy(p_ => p_.Version));
                //    _listProduct = new List<ILogEventProduct>(ListEvent.AsParallel().Where(e_ => e_ is ILogEventProduct).Select(p_ => p_ as ILogEventProduct).Distinct(new CompareProduct()).OrderBy(p_ => p_.Product).ThenBy(p_ => p_.Version));
                //    _listProduct.RemoveAll(x_ => string.IsNullOrEmpty(x_.Product));
                //}
                _listProduct = _listProduct ?? ListEvent.AsParallel().Where(e_ => e_ is ILogEventProduct).Select(p_ => p_ as ILogEventProduct)
                                                                                               .Where(e_=>string.IsNullOrEmpty(e_.Product) ==false).Distinct(new CompareProduct()).OrderBy(p_ => p_.Product).ThenBy(p_ => p_.Version).ToList();

                return _listProduct;
            }
        }
        //private List<(string Product, string Version)> _listProduct = null;
        private List<ILogEventProduct>? _listProduct = null;

        public List<string> ListUser
        {
            get
            {
                //if (_listUser == null)
                //{
                //_listUser = new List<string>(ListEvent.AsParallel().Where(e_ => e_ is ILogEventUser).Select(p_ => (p_ as ILogEventUser).User).Distinct().OrderBy(x_ => x_));
                //}
                _listUser = _listUser ?? ListEvent.AsParallel().Where(e_ => e_ is ILogEventUser).Select(e_ => e_ as ILogEventUser)
                                                                                .Select(e_ => e_.User).Where(e_ => string.IsNullOrEmpty(e_) == false).Distinct().OrderBy(x_ => x_).ToList();
                return _listUser;
            }
        }
        List<string>? _listUser = null;


        public List<string> ListHost
        {
            get
            {
                //if (_listHost == null)
                //{
                //    _listHost = new List<string>(ListEvent.AsParallel().Where(e_ => e_ is ILogEventHost).Select(p_ => (p_ as ILogEventHost).Host).Distinct().OrderBy(x_ => x_));
                //    _listHost.RemoveAll(x_ => string.IsNullOrEmpty(x_));
                //}
                _listHost = _listHost?? ListEvent.AsParallel().Where(e_ => e_ is ILogEventHost).Select(e_ => e_ as ILogEventHost).Select(e_ => e_.Host).Distinct().OrderBy(x_ => x_).ToList();

                return _listHost;
            }
        }
        List<string>? _listHost = null;

        public List<string> ListUserHost
        {
            get
            {
                //if (_listUserHost == null)
                //{
                //    _listUserHost = new List<string>(ListEvent.AsParallel().Where(e_ => e_ is ILogEventUserHost).Select(p_ => (p_ as ILogEventUserHost).UserHost).Distinct().OrderBy(x_ => x_));
                //    _listUserHost.RemoveAll(x_ => x_ == "@");
                //}
                _listUserHost = _listUserHost?? ListEvent.AsParallel().Where(e_ => e_ is ILogEventUserHost).Select(e_ => e_ as ILogEventUserHost).Select(e_=>e_.UserHost).Where(e_=>e_ !="@").Distinct().OrderBy(x_ => x_).ToList();

                return _listUserHost;
            }
        }
        List<string>? _listUserHost = null;

        public List<DateTime> ListDateTime
        {
            get
            {
                _listDateTime = _listDateTime ?? ListEvent.AsParallel().Where(e_ => (e_ is LogEventRlmReportLogFormat) == false).Select(e_ => e_.EventDateTime).Distinct().OrderBy(e_ => e_).ToList();
                return _listDateTime;
            }
        }
        public List<DateTime>? _listDateTime = null;

        public List<DateTime> ListDate
        {
            get
            {
                _listDate = _listDate ?? ListDateTime.AsParallel().Select(t_ => t_.Date).Distinct().OrderBy(e_ => e_).ToList();
                return _listDate;
            }
        }
        private List<DateTime>? _listDate = null;

        //public List<DateTime> ListHour { get => ListDateTime.Select(t_ => new DateTime(t_.Ticks - (t_.Ticks % TimeSpan.TicksPerHour))).Distinct().ToList(); }

        public List<T> GetListEvent<T>(AnalysisStartShutdown? ss_ = null) where T : LogEventBase
        {
            //LogFile.Instance.WriteLine($"[{typeof(T)}]");

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
                    //var list =ListEvent.Select(e_ => e_ as T).Where(e_ => e_ !=null).OrderBy(x_ => x_.EventNumber);
                    //list?.ToList().ForEach(e_ => rtn.Add(e_));

                    //var list = ListEvent.Select(e_ => e_ as T).Where(e_ => e_ != null).OrderBy(x_ => x_.EventNumber);
                    //long count = -1;
                    //foreach (var e in list)
                    //{
                    //    var comp = (e is ILogEventCountCurrent evCount) ? evCount.CountCurrent : e.EventNumber;
                    //    if (comp == count)
                    //    {
                    //        continue;
                    //    }

                    //    rtn.Add(e);
                    //    count = comp;
                    //}
                    //rtn.AddRange(ListEvent.AsParallel().Select(e_ => e_ as T).Where(e_ => e_ != null).OrderBy(x_ => x_.EventNumber));
                    rtn.AddRange(ListEvent.AsParallel().Where(e_=> e_ is T).Select(e_ => e_ as T).OrderBy(x_ => x_.EventNumber));

                    _listEvent[typeof(T)] = rtn;
                }
            }
            else
            {
                //var list = ListEvent.Where(e_ => (e_ is T) && (ss_.IsWithInRange(e_.EventNumber) == true)).Select(e_ => e_ as T)?.OrderBy(x_ => x_.EventNumber);
                //long count = -1;
                //foreach (var e in list)
                //{
                //    var comp = (e is ILogEventCountCurrent evCount) ? evCount.CountCurrent : e.EventNumber;
                //    if (comp == count)
                //    {
                //        continue;
                //    }

                //    rtn.Add(e);
                //    count = comp;
                //}
                rtn.AddRange(ListEvent.AsParallel().Where(e_ => (e_ is T) && (ss_.IsWithInRange(e_.EventNumber) == true)).Select(e_ => e_ as T)?.OrderBy(x_ => x_.EventNumber));
            }
            LogFile.Instance.WriteLine($"[{typeof(T)}] {rtn.Count}");

            return rtn.Select(x_ => x_ as T).ToList();
            //return rtn;
        }
        private Dictionary<Type, List<LogEventBase>> _listEvent = new Dictionary<Type, List<LogEventBase>>();

        //public ListAnalysisStartShutdown ListRunning
        //{
        //    get
        //    {
        //        _listRunning = _listRunning ?? new ListAnalysisStartShutdown(this);
        //        return _listRunning;
        //    }
        //}
        //private ListAnalysisStartShutdown _listRunning = null;

        //public ListAnalysisCheckOutIn ListAnalysisCheckOutIn
        //{
        //    get
        //    {
        //        _listAnalysisCheckOutIn = _listAnalysisCheckOutIn ?? new ListAnalysisCheckOutIn(this);
        //        return _listAnalysisCheckOutIn;
        //    }
        //}
        //private ListAnalysisCheckOutIn _listAnalysisCheckOutIn = null;

        //public ListAnalysisLicenseDeny ListDeny
        //{
        //    get
        //    {
        //        _listDeny =_listDeny?? new ListAnalysisLicenseDeny(this);
        //        return _listDeny;
        //     }
        //}
        //private ListAnalysisLicenseDeny _listDeny = null;

        //public ListAnalysisLicenseCount ListAnalysisLicenseCount
        //{
        //    get
        //    {
        //        _listAnalysisLicenseCount = _listAnalysisLicenseCount ?? new ListAnalysisLicenseCount(this);
        //        return _listAnalysisLicenseCount;
        //    }
        //}
        //private ListAnalysisLicenseCount _listAnalysisLicenseCount = null;

        public void WriteSummy(string path_)
        {
            var list = new List<string>();

            list.Add("License");
            list.AddRange(ListProduct.Select(x_ => $"{x_.Product},{x_.Version}"));
            list.Add("\n");
            LogFile.Instance.WriteLine($"ListProduct:{ListProduct.Count}");

            list.Add("User");
            list.AddRange(ListUser);
            list.Add("\n");
            LogFile.Instance.WriteLine($"ListUser:{ListUser.Count}");

            list.Add("Host");
            list.AddRange(ListHost);
            list.Add("\n");
            LogFile.Instance.WriteLine($"ListHost:{ListHost.Count}");

            list.Add("User@Host");
            list.AddRange(ListUserHost);
            list.Add("\n");
            LogFile.Instance.WriteLine($"ListUserHost:{ListUserHost.Count}");

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

            list.Add(_getHeader<T>());
            list.AddRange(_listToString<T>());
            File.WriteAllLines(path_, list, Encoding.UTF8);

            LogFile.Instance.WriteLine($"Write:{path_}");
        }

        private string _getHeader<T>()
        {
            var listPropetyInfo = typeof(T).GetProperties(BindingFlags.Static | BindingFlags.Public);
            foreach (var prop in listPropetyInfo)
            {
                if (prop.Name == "HEADER")
                {
                    return prop?.GetValue(null).ToString();
                }
            }
            return string.Empty;
        }

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