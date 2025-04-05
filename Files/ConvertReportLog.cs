/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Analyses;
using RepriseReportLogAnalyzer.Controls;
using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Interfaces;
using System.IO;
using System.Text;

namespace RepriseReportLogAnalyzer.Files;

/// <summary>
/// Report Log イベント リスト化
/// </summary>
internal sealed class ConvertReportLog: List<LogEventBase>
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ConvertReportLog()
    {
        ProgressCount = null;
        EventClear();
    }

    /// <summary>
    /// 変換内容
    /// </summary>
    private readonly string _CONVERT = "[File Read]";

    /// <summary>
    /// プログレスバー 解析処理 更新デリゲート
    /// </summary>
    public ProgressCountCallBack? ProgressCount;


    public static void EventClear() => LogEventBase.Clear();

    /// <summary>
    /// 解析開始
    /// </summary>
    /// <param name="filePath_"></param>
    public async Task Start(string filePath_)
    {
        if (File.Exists(filePath_) == false)
        {
            return;
        }
        var listLine = await File.ReadAllLinesAsync(filePath_);

        int count = 0;
        int max = listLine.Count();

        ProgressCount?.Invoke(count, max, $"{_CONVERT}[{Path.GetFileName(filePath_)}]");
        _addEvent(listLine.Where(s_ => string.IsNullOrEmpty(s_) == false));
        ProgressCount?.Invoke(max, max, $"{_CONVERT}[{Path.GetFileName(filePath_)}]");
    }

    /// <summary>
    /// 文字列からイベントを変換追加
    /// </summary>
    /// <param name="list_"></param>
    private void _addEvent(IEnumerable<string> list_)
    {
        foreach (var s in list_)
        {
            var eventBase = LogEventBase.EventData(s);

            if (eventBase != null)
            {
                this.Add(eventBase);
            }
        }
        //
        LogFile.Instance.WriteLine($"Read:{this.Count}");
    }


    /// <summary>
    /// 解析終了
    /// </summary>
    public void End()
    {
        // ログの収量はシャットダウンとして扱う
        var end = new LogEventShutdown();
        this.Add(end);
    }

    /// <summary>
    /// ログ イベント一覧
    /// </summary>
    //private List<LogEventBase> _listEvent = new();

    /// <summary>
    /// プロダクト
    /// </summary>
    public IEnumerable<string> ListProduct { get => ListProductEvent.Select(x_ => x_.Product).DistinctBy(x_=>x_); }

    /// <summary>
    /// プロダクトを持ってる イベント
    /// </summary>
    public IEnumerable<ILogEventProduct> ListProductEvent { get => this.AsParallel().OfType<ILogEventProduct>().Where(e_ => string.IsNullOrEmpty(e_?.Product ?? string.Empty) == false).DistinctBy(x_=>x_.ProductVersion).OrderBy(p_ => p_.Product).ThenBy(p_ => p_.Version); }

    /// <summary>
    /// ユーザー
    /// </summary>
    public IEnumerable<string> ListUser { get => this.AsParallel().OfType<ILogEventUser>().Select(e_ => e_?.User ?? string.Empty).Where(e_ => string.IsNullOrEmpty(e_) == false).Distinct(); }

    /// <summary>
    /// ホスト
    /// </summary>
    public IEnumerable<string> ListHost { get => this.AsParallel().OfType<ILogEventHost>().Select(e_ => e_?.Host ?? string.Empty).Where(e_ => string.IsNullOrEmpty(e_) == false).Distinct(); }

    /// <summary>
    /// ユーザーホスト
    /// </summary>
    public IEnumerable<string> ListUserHost { get => this.AsParallel().OfType<ILogEventUserHost>().Select(e_ => e_?.UserHost ?? "@").Where(e_ => e_ != "@").Distinct(); }

    /// <summary>
    /// 時間
    /// </summary>
    public IEnumerable<DateTime> ListDateTime { get => this.AsParallel().Select(e_ => e_.EventDateTime).Where(e_ => e_ != LogEventBase.NotAnalysisEventTime).Distinct().OrderBy(x_ => x_); }

    /// <summary>
    /// 日付
    /// </summary>
    public IEnumerable<DateTime> ListDate { get => ListDateTime.AsParallel().Select(t_ => t_.Date).Distinct(); }

    /// <summary>
    /// イベント リスト抽出(出力系で利用)
    /// </summary>
    /// <param name="classType_"></param>
    /// <returns></returns>
    public IEnumerable<LogEventBase> ListEvent(Type classType_) => this.Where(e_ => e_.GetType() == classType_);

    public IEnumerable<T> ListEvent<T>() => this.OfType<T>();

    /// <summary>
    /// イベント リスト抽出
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ss_"></param>
    /// <returns></returns>
    public IEnumerable<T> ListEvent<T>(AnalysisStartShutdown ss_) where T : LogEventBase => this.OfType<T>().Where(e_ => ss_.IsWithInRange(e_.EventNumber) == true);

    /// <summary>
    /// データ文字列変換
    /// </summary>
    /// <param name="classType_"></param>
    /// <returns></returns>
    private IEnumerable<string> _listToString(Type classType_) => ListEvent(classType_).Select(e_ => e_.ToString());

    private IEnumerable<string> _listToString<T>() where T : LogEventBase => ListEvent<T>().Select(e_ => e_.ToString());

    /// <summary>
    /// 各イベントのCsv保存
    /// </summary>
    /// <param name="path_"></param>
    /// <param name="classType_"></param>
    public async Task WriteEventText(string path_, Type classType_)
    {
        // ヘッダー
        var list = new List<string>() { LogEventBase.Header(classType_) };
        // データ
        list.AddRange(_listToString(classType_));
        await File.WriteAllLinesAsync(path_, list, Encoding.UTF8);

        LogFile.Instance.WriteLine($"Write:{path_}");
    }

    public async Task WriteEventText<T>(string path_) where T : LogEventBase
    {
        // ヘッダー
        var list = new List<string>() { LogEventBase.Header(typeof(T)) };

        // データ
        list.AddRange(_listToString<T>());
        await File.WriteAllLinesAsync(path_, list, Encoding.UTF8);

        LogFile.Instance.WriteLine($"Write:{path_}");
    }
}