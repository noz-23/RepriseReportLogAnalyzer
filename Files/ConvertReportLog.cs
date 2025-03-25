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
using RepriseReportLogAnalyzer.Interfaces;
using RepriseReportLogAnalyzer.Windows;
using System.IO;
using System.Linq;
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
    public async Task Start(string filePath_)
    {
        if (File.Exists(filePath_) == true)
        {
            var listLine = await File.ReadAllLinesAsync(filePath_);
            var list = listLine.Where(s_ => string.IsNullOrEmpty(s_) == false);

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
            }
            ProgressCount?.Invoke(max, max, $"{_CONVERT}[{Path.GetFileName(filePath_)}]");
            //
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
    }

    /// <summary>
    /// ログ イベント一覧
    /// </summary>
    private List<LogEventBase> _listEvent = new();

    /// <summary>
    /// スタート イベント
    /// </summary>
    //public IEnumerable<LogEventStart> ListStart { get => ListEvent<LogEventStart>(); }

    /// <summary>
    /// シャットダウン イベント
    /// </summary>
    //public IEnumerable<LogEventShutdown> ListShutdown { get => ListEvent<LogEventShutdown>(); }

    /// <summary>
    /// プロダクト
    /// </summary>
    public IEnumerable<string> ListProduct{get => ListProductEvent.Select(x_ => x_.Product).Distinct();}

    /// <summary>
    /// プロダクトを持ってる イベント
    /// </summary>
    //public IEnumerable<ILogEventProduct> ListProductEvent{get => _listEvent.AsParallel().Where(e_ => e_ is ILogEventProduct).Select(e_ => e_ as ILogEventProduct).Where(e_ => string.IsNullOrEmpty(e_?.Product ??string.Empty) == false).Distinct(new CompareProduct()).OrderBy(p_ => p_.Product).ThenBy(p_ => p_.Version);}
    public IEnumerable<ILogEventProduct> ListProductEvent { get => _listEvent.AsParallel().OfType<ILogEventProduct>().Where(e_ => string.IsNullOrEmpty(e_?.Product ?? string.Empty) == false).Distinct(new CompareProduct()).OrderBy(p_ => p_.Product).ThenBy(p_ => p_.Version); }

    /// <summary>
    /// ユーザー
    /// </summary>
    public IEnumerable<string> ListUser{get => _listEvent.AsParallel().OfType<ILogEventUser>().Select(e_ => e_?.User ??string.Empty).Where(e_ => string.IsNullOrEmpty(e_) == false).Distinct(); }

    /// <summary>
    /// ホスト
    /// </summary>
    public IEnumerable<string> ListHost{ get => _listEvent.AsParallel().OfType<ILogEventHost>().Select(e_ => e_?.Host ?? string.Empty).Where(e_ => string.IsNullOrEmpty(e_) == false).Distinct();}

    /// <summary>
    /// ユーザーホスト
    /// </summary>
    public IEnumerable<string> ListUserHost{get => _listEvent.AsParallel().OfType<ILogEventUserHost>().Select(e_ => e_?.UserHost ?? "@").Where(e_ => e_ != "@").Distinct();}

    /// <summary>
    /// 時間
    /// </summary>
    public IEnumerable<DateTime> ListDateTime{get => _listEvent.AsParallel().Select(e_ => e_.EventDateTime).Where(e_ => e_ != LogEventBase.NotAnalysisEventTime).Distinct().OrderBy(x_ => x_);}

    /// <summary>
    /// 日付
    /// </summary>
    public IEnumerable<DateTime> ListDate{get => ListDateTime.AsParallel().Select(t_ => t_.Date).Distinct();}

    /// <summary>
    /// イベント リスト抽出(出力系で利用)
    /// </summary>
    /// <param name="classType_"></param>
    /// <returns></returns>
    public IEnumerable<LogEventBase> ListEvent(Type classType_)=> _listEvent.Where(e_ => e_.GetType() == classType_);

    public IEnumerable<T> ListEvent<T>() => _listEvent.OfType<T>();

    /// <summary>
    /// イベント リスト抽出
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ss_"></param>
    /// <returns></returns>
    public IEnumerable<T> ListEvent<T>(AnalysisStartShutdown ss_) where T : LogEventBase => _listEvent.OfType<T>().Where(e_ => ss_.IsWithInRange(e_.EventNumber) == true);

    /// <summary>
    /// データ文字列変換
    /// </summary>
    /// <param name="classType_"></param>
    /// <returns></returns>
    private IEnumerable<string> _listToString(Type classType_) => ListEvent(classType_).Select(e_ => e_.ToString());

    private IEnumerable<string> _listToString<T>() where T : LogEventBase=> ListEvent<T>().Select(e_ => e_.ToString());

    /// <summary>
    /// 各イベントのCsv保存
    /// </summary>
    /// <param name="path_"></param>
    /// <param name="classType_"></param>
    public async Task WriteEventText(string path_, Type classType_)
    {
        var list = new List<string>();
        // ヘッダー
        list.Add(LogEventBase.Header(classType_));
        // データ
        list.AddRange(_listToString(classType_));
        await File.WriteAllLinesAsync(path_, list, Encoding.UTF8);

        LogFile.Instance.WriteLine($"Write:{path_}");
    }

    public async Task WriteEventText<T>(string path_) where T : LogEventBase
    {
        var list = new List<string>();
        // ヘッダー
        list.Add(LogEventBase.Header(typeof(T)));
        // データ
        list.AddRange(_listToString<T>());
        await File.WriteAllLinesAsync(path_, list, Encoding.UTF8);

        LogFile.Instance.WriteLine($"Write:{path_}");
    }

    /// <summary>
    /// プロダクトの比較
    /// </summary>
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
        public int GetHashCode(ILogEventProduct codeh_)=> codeh_.Product.GetHashCode() ^ codeh_.Version.GetHashCode();
    }
}