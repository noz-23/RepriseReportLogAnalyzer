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
using RepriseReportLogAnalyzer.Windows;
using System.IO;
using System.Text;

namespace RepriseReportLogAnalyzer.Files;

/// <summary>
/// Report Log イベント リスト化
/// </summary>
internal sealed class ConvertReportLog
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ConvertReportLog()
    {
        LogEventBase.Clear();
    }

    /// <summary>
    /// 変換内容
    /// </summary>
    private const string _CONVERT = "[File Read]";


    /// <summary>
    /// プログレスバー 解析処理 更新デリゲート
    /// </summary>
    public ProgressCountDelegate? ProgressCount = null;

    /// <summary>
    /// 解析開始
    /// </summary>
    /// <param name="filePath_"></param>
    public void Start(string filePath_)
    {
        //_startTime??=DateTime.Now;
        if (File.Exists(filePath_) == true)
        {
            var list = File.ReadAllLines(filePath_).Where(s_ => string.IsNullOrEmpty(s_) == false);

            int count = 0;
            int max = list.Count();

            ProgressCount?.Invoke(count, max, $"{_CONVERT}[{Path.GetFileName(filePath_)}]");
            foreach (var s in list)
            {
                var eventBase = LogEventBase.EventData(s);

                if (eventBase != null)
                {
                    _listEvent.Add(eventBase);
                }
                //ProgressCount?.Invoke(++count, max, $"{_CONVERT}[{Path.GetFileName(filePath_)}]");
            }
            ProgressCount?.Invoke(max, max, $"{_CONVERT}[{Path.GetFileName(filePath_)}]");
            //

            //ListEvent?.AddRange(File.ReadAllLines(filePath_).Where(s_ => string.IsNullOrEmpty(s_) == false).Select(s_ => LogEventBase.EventData(s_)).Where(x_=>x_!=null));
            LogFile.Instance.WriteLine($"Read:{_listEvent.Count()}");
        }
    }

    /// <summary>
    /// 解析終了
    /// </summary>
    public void End()
    {
        // ログの収量はシャットダウンとして扱う
        var end = new LogEventShutdown();
        _listEvent.Add(end);

        //_endTime ??= DateTime.Now;
    }

    //private DateTime? _startTime =null ;
    //private DateTime? _endTime = null;

    /// <summary>
    /// ログ イベント一覧
    /// </summary>
    private List<LogEventBase> _listEvent = new();

    /// <summary>
    /// 
    /// </summary>
    public IEnumerable<LogEventStart> ListStart { get => ListEvent<LogEventStart>(); }

    /// <summary>
    /// 
    /// </summary>
    public IEnumerable<LogEventLogFileEnd> ListEnd { get => ListEvent<LogEventLogFileEnd>(); }

    /// <summary>
    /// 
    /// </summary>
    public IEnumerable<LogEventShutdown> ListShutdown { get => ListEvent<LogEventShutdown>(); }


    public IEnumerable<string> ListProduct
    {
        get => ListProductEvent.Select(x_ => x_.Product).Distinct();
    }

    public IEnumerable<ILogEventProduct> ListProductEvent
    {
        get => _listEvent.AsParallel().Where(e_ => e_ is ILogEventProduct).Select(e_ => e_ as ILogEventProduct).Where(e_ => string.IsNullOrEmpty(e_?.Product ??string.Empty) == false).Distinct(new CompareProduct()).OrderBy(p_ => p_.Product).ThenBy(p_ => p_.Version);
    }

    public IEnumerable<string> ListUser
    {
        get => _listEvent.AsParallel().Where(e_ => e_ is ILogEventUser).Select(e_ => e_ as ILogEventUser).Select(e_ => e_?.User ??string.Empty).Where(e_ => string.IsNullOrEmpty(e_) == false).Distinct();
    }


    public IEnumerable<string> ListHost
    {
        get => _listEvent.AsParallel().Where(e_ => e_ is ILogEventHost).Select(e_ => e_ as ILogEventHost).Select(e_ => e_?.Host ?? string.Empty).Where(e_ => string.IsNullOrEmpty(e_) == false).Distinct();
    }

    public IEnumerable<string> ListUserHost
    {
        get => _listEvent.AsParallel().Where(e_ => e_ is ILogEventUserHost).Select(e_ => e_ as ILogEventUserHost).Select(e_ => e_?.UserHost ?? "@").Where(e_ => e_ != "@").Distinct();
    }

    public IEnumerable<DateTime> ListDateTime
    {
        get => _listEvent.AsParallel().Select(e_ => e_.EventDateTime).Where(e_ => e_ != LogEventBase.NotAnalysisEventTime).Distinct().OrderBy(x_ => x_);
    }

    public IEnumerable<DateTime> ListDate
    {
        get => ListDateTime.AsParallel().Select(t_ => t_.Date).Distinct();
    }

    public IEnumerable<LogEventBase> ListEvent(Type classType_)=> _listEvent.AsParallel().AsOrdered().Where(e_ => e_.GetType() == classType_);
    //{
    //    return _listEvent.AsParallel().AsOrdered().Where(e_ => e_.GetType() == classType_);
    //}
    public IEnumerable<T> ListEvent<T>(AnalysisStartShutdown? ss_ = null) where T : LogEventBase
    {
        if (ss_ == null)
        { 
            return _listEvent.AsParallel().AsOrdered().Where(e_ => e_ is T).Select(e_ => e_ as T);
        }
        return _listEvent.AsParallel().Where(e_ => (e_ is T) && (ss_.IsWithInRange(e_.EventNumber) == true)).Select(e_ => e_ as T);
    }
    //private Dictionary<Type, List<LogEventBase>> _listEvent = new();



    private List<string> _listToString(Type classType_)
    {
        //var rtn = new List<string>();

        //var method = this.GetType().GetMethods().FirstOrDefault(x_ => x_.Name == nameof(_listToString) && x_.IsGenericMethod == true);
        //method?.MakeGenericMethod(classType_);
        ////
        //List<string> list = method?.Invoke(this, null) as List<string>;
        //LogFile.Instance.WriteLine($"_listToString [{classType_}] [{list.Count}]");
        var list = ListEvent(classType_).Select(e_=>e_.ToString());

        //foreach (var data in list)
        //{
        //    rtn.Add(data.ToString());
        //}

        //return rtn;

        return ListEvent(classType_).Select(e_ => e_.ToString()).ToList();
    }

    private List<string> _listToString<T>() where T : LogEventBase
    {
        var rtn = new List<string>();
        var list = ListEvent<T>();

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