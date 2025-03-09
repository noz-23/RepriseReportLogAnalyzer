/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Analyses;
using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Extensions;
using RepriseReportLogAnalyzer.Interfaces;
using System.IO;
using System.Text;

namespace RepriseReportLogAnalyzer.Files;

/// <summary>
/// Report Log イベント リスト化
/// </summary>
internal sealed class AnalysisReportLog
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public AnalysisReportLog()
    {
        LogEventBase.Clear();
    }

    /// <summary>
    /// 解析開始
    /// </summary>
    /// <param name="filePath_"></param>
    public void StartAnalysis(string filePath_)
    {
        _startTime??=DateTime.Now;
        if (File.Exists(filePath_) == true)
        {
            //foreach (var s in File.ReadAllLines(filePath_).Where(s_ => string.IsNullOrEmpty(s_) == false))
            //{
            //    var eventBase = LogEventBase.EventData(s);

            //    if (eventBase != null)
            //    {
            //        ListEvent.Add(eventBase);
            //    }
            //}
            //

            ListEvent?.AddRange(File.ReadAllLines(filePath_).Where(s_ => string.IsNullOrEmpty(s_) == false).Select(s_ => LogEventBase.EventData(s_)).Where(x_=>x_!=null));
            _listFile.Add(filePath_);
            LogFile.Instance.WriteLine($"Read:{ListEvent.Count()}");
        }
    }

    /// <summary>
    /// 解析終了
    /// </summary>
    public void EndAnalysis()
    {
        // ログの収量はシャットダウンとして扱う
        var end = new LogEventShutdown();
        ListEvent.Add(end);

        _endTime ??= DateTime.Now;
    }

    private List<string> _listFile=new();
    private DateTime? _startTime =null ;
    private DateTime? _endTime = null;

    /// <summary>
    /// ログ イベント一覧
    /// </summary>
    public List<LogEventBase> ListEvent { get; private set; } = new();

    /// <summary>
    /// 
    /// </summary>
    public IEnumerable<LogEventStart> ListStart { get => GetListEvent<LogEventStart>(); }

    /// <summary>
    /// 
    /// </summary>
    public IEnumerable<LogEventLogFileEnd> ListEnd { get => GetListEvent<LogEventLogFileEnd>(); }

    /// <summary>
    /// 
    /// </summary>
    public IEnumerable<LogEventShutdown> ListShutdown { get => GetListEvent<LogEventShutdown>(); }

    //public List<LogEventCheckIn> ListCheckIn { get => GetListEvent<LogEventCheckIn>(); }
    //public List<LogEventCheckOut> ListCheckOut { get => GetListEvent<LogEventCheckOut>(); }

    public IEnumerable<string> ListProduct
    {
        //get
        //{
        //    _listProduct ??= new(ListProductEvent.Select(x_ => x_.Product));
        //    return _listProduct;
        //}
        get => ListProductEvent.Select(x_ => x_.Product).Distinct();
    }
    //private SortedSet<string>? _listProduct = null;

    public IEnumerable<ILogEventProduct> ListProductEvent
    {
    //    get
    //    {
    //        _listProductEvent ??= new(ListEvent.AsParallel().Where(e_ => e_ is ILogEventProduct).Select(e_ => e_ as ILogEventProduct)
    //                                                                                       .Where(e_ => string.IsNullOrEmpty(e_.Product) == false).Distinct(new CompareProduct()).OrderBy(p_ => p_.Product).ThenBy(p_ => p_.Version));

    //        return _listProductEvent;
    //    }
        get=> ListEvent.AsParallel().Where(e_ => e_ is ILogEventProduct).Select(e_ => e_ as ILogEventProduct)
                                                                                           .Where(e_ => string.IsNullOrEmpty(e_.Product) == false).Distinct(new CompareProduct()).OrderBy(p_ => p_.Product).ThenBy(p_ => p_.Version);
    }
    //private HashSet<ILogEventProduct>? _listProductEvent = null;

    public IEnumerable<string> ListUser
    {
        //get
        //{
        //    _listUser ??= new(ListEvent.AsParallel().Where(e_ => e_ is ILogEventUser).Select(e_ => e_ as ILogEventUser)
        //                                                    .Select(e_ => e_.User).Where(e_ => string.IsNullOrEmpty(e_) == false).Distinct());
        //    return _listUser;
        //}
        get => ListEvent.AsParallel().Where(e_ => e_ is ILogEventUser).Select(e_ => e_ as ILogEventUser)
                                                            .Select(e_ => e_.User).Where(e_ => string.IsNullOrEmpty(e_) == false).Distinct();
    }
    //SortedSet<string>? _listUser = null;


    public IEnumerable<string> ListHost
    {
        //get
        //{
        //    _listHost ??= new(ListEvent.AsParallel().Where(e_ => e_ is ILogEventHost).Select(e_ => e_ as ILogEventHost)
        //                                                    .Select(e_ => e_.Host).Where(e_ => string.IsNullOrEmpty(e_) == false));

        //    return _listHost;
        //}
        get => ListEvent.AsParallel().Where(e_ => e_ is ILogEventHost).Select(e_ => e_ as ILogEventHost)
                                                            .Select(e_ => e_.Host).Where(e_ => string.IsNullOrEmpty(e_) == false).Distinct();
    }
    //SortedSet<string>? _listHost = null;

    public IEnumerable<string> ListUserHost
    {
        //get
        //{
        //    _listUserHost ??= new(ListEvent.AsParallel().Where(e_ => e_ is ILogEventUserHost).Select(e_ => e_ as ILogEventUserHost)
        //                                                    .Select(e_ => e_.UserHost).Where(e_ => e_ != "@").Distinct());

        //    return _listUserHost;
        //}
        get => ListEvent.AsParallel().Where(e_ => e_ is ILogEventUserHost).Select(e_ => e_ as ILogEventUserHost)
                                                            .Select(e_ => e_.UserHost).Where(e_ => e_ != "@").Distinct();
    }
    //SortedSet<string>? _listUserHost = null;

    public IEnumerable<DateTime> ListDateTime
    {

        //get
        //{
        //    //_listDateTime ??=new ( ListEvent.AsParallel().Where(e_ => ( (e_ !=null) || (e_ is LogEventRlmReportLogFormat) == false))
        //    //                                                .Select(e_ => e_.EventDateTime));
        //    _listDateTime ??= new(ListEvent.AsParallel().Select(e_ => e_.EventDateTime).Where(e_ => e_ != LogEventBase.NotAnalysisEventTime));
        //    return _listDateTime;
        //}
        get => ListEvent.AsParallel().Select(e_ => e_.EventDateTime).Where(e_ => e_ != LogEventBase.NotAnalysisEventTime).Distinct().OrderBy(x_ => x_);
    }
    //public SortedSet<DateTime>? _listDateTime = null;

    public IEnumerable<DateTime> ListDate
    {
        //get
        //{
        //    var list = ListDateTime;
        //    _listDate ??= new(ListDateTime.AsParallel().Select(t_ => t_.Date));
        //    return _listDate;
        //}
        //get => ListDateTime.AsParallel().Select(t_ => t_.Date).Distinct().OrderBy(x_ => x_);
        get => ListDateTime.AsParallel().Select(t_ => t_.Date).Distinct();
    }
    //private SortedSet<DateTime>? _listDate = null;

    public IEnumerable<LogEventBase> GetListEvent(Type classType_, AnalysisStartShutdown? ss_ = null)
    {
        if (ss_ == null)
        {
            return ListEvent.AsParallel().AsOrdered().Where(e_ => e_.GetType()== classType_).OrderBy(x_ => x_.EventNumber);

        }
        return ListEvent.AsParallel().Where(e_ => (e_.GetType() == classType_) && (ss_.IsWithInRange(e_.EventNumber) == true)).OrderBy(x_ => x_.EventNumber).ToList();
    }
    public IEnumerable<T> GetListEvent<T>(AnalysisStartShutdown? ss_ = null) where T : LogEventBase
    {
       // List<T>? rtn = new();
        if (ss_ == null)
        {
            //if (_listEvent.TryGetValue(typeof(T), out var rtn) == true)
            //{
            //    LogFile.Instance.WriteLine($"[{typeof(T)}] {rtn.Count}");
            //    return rtn;
            //}
            //else
            //{
            //    rtn = new();
            //    rtn.AddRange(ListEvent.AsParallel().Where(e_ => e_ is T).Select(e_ => e_ as T).OrderBy(x_ => x_.EventNumber));

            //    _listEvent[typeof(T)] = rtn;
            //}
            //return ListEvent.AsParallel().Where(e_ => e_ is T).Select(e_ => e_ as T).OrderBy(x_ => x_.EventNumber).ToList();
            return ListEvent.AsParallel().AsOrdered().Where(e_ => e_ is T).Select(e_ => e_ as T).OrderBy(x_ => x_.EventNumber);
        }
        //else
        //{
        //    rtn.AddRange(ListEvent.AsParallel().Where(e_ => (e_ is T) && (ss_.IsWithInRange(e_.EventNumber) == true)).Select(e_ => e_ as T)?.OrderBy(x_ => x_.EventNumber));
        //}
        //LogFile.Instance.WriteLine($"[{typeof(T)}] {rtn.Count}");

        ////return rtn.Select(x_ => x_ as T).ToList();
        //return rtn;
        return ListEvent.AsParallel().Where(e_ => (e_ is T) && (ss_.IsWithInRange(e_.EventNumber) == true)).Select(e_ => e_ as T).OrderBy(x_ => x_.EventNumber);
    }
    private Dictionary<Type, List<LogEventBase>> _listEvent = new();

    public void WriteSummy(string path_)
    {
        var list = new List<string>();

        list.Add($"Analsis File Count:{_listFile.Count}");
        list.AddRange(_listFile.Select(x_ => Path.GetFileName(x_)));
        list.Add("\n");

        list.Add($"Analsis Time : {_endTime- _startTime}");
        list.Add($"Start   Time : {_startTime}");
        list.Add($"End     Time : {_endTime}");
        list.Add("\n");


        list.Add($"License Count : {ListProduct.Count()}");
        list.AddRange(ListProductEvent.Select(x_ => $"{x_.Product},{x_.Version}"));
        list.Add("\n");
        LogFile.Instance.WriteLine($"ListProduct:{ListProduct.Count()}");

        list.Add($"User Count : {ListUser.Count()}");
        list.AddRange(ListUser);
        list.Add("\n");
        LogFile.Instance.WriteLine($"ListUser:{ListUser.Count()}");

        list.Add($"Host Count : {ListHost.Count()}");
        list.AddRange(ListHost);
        list.Add("\n");
        LogFile.Instance.WriteLine($"ListHost:{ListHost.Count()}");

        list.Add($"User@Host Count : {ListUserHost.Count()}");
        list.AddRange(ListUserHost);
        list.Add("\n");
        LogFile.Instance.WriteLine($"ListUserHost:{ListUserHost.Count()}");

        File.WriteAllLines(path_, list, Encoding.UTF8);
        LogFile.Instance.WriteLine($"Write:{path_}");
    }


    private List<string> _listToString(Type classType_)
    {
        var rtn = new List<string>();
        var list = GetListEvent(classType_);

        foreach (var data in list)
        {
            rtn.Add(data.ToString());
        }

        return rtn;
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

    public void WriteEventText(string path_, Type classType_)
    {
        var list = new List<string>();

        //list.Add(_getHeader<T>());
        list.Add(LogEventBase.Header(classType_));
        list.AddRange(_listToString(classType_));
        File.WriteAllLines(path_, list, Encoding.UTF8);

        LogFile.Instance.WriteLine($"Write:{path_}");
    }

    public void WriteEventText<T>(string path_) where T : LogEventBase
    {
        var list = new List<string>();

        //list.Add(_getHeader<T>());
        list.Add(LogEventBase.Header(typeof(T)));
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