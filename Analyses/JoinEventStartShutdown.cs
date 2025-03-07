/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Files;

namespace RepriseReportLogAnalyzer.Analyses;

/// <summary>
///  スタートとシャットダウンの結合情報
///  重複情報
/// </summary>
internal sealed class JoinEventStartShutdown
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="start_">スタート イベント</param>
    /// <param name="shutdown_">シャットダウン イベント</param>
    public JoinEventStartShutdown(LogEventStart start_, LogEventBase? shutdown_)
    {
        StartNumber = start_.EventNumber;
        if (shutdown_ is LogEventShutdown shutdown)
        {
            ShutdownNumber = shutdown.EventNumber;
        }
        else
        {
            LogFile.Instance.WriteLine($"{shutdown_?.EventNumber} {shutdown_?.GetType()}");
        }
    }

    /// <summary>
    /// 文字列化のヘッダー
    /// </summary>
    public const string HEADER = "StartNumber,ShutdownNumber,IsSkip";

    /// <summary>
    /// スキップする情報
    /// </summary>
    public const long SKIP = 1;

    /// <summary>
    /// スキップなし
    /// </summary>
    public const long JOIN_SHUTDOWN = 0;

    /// <summary>
    /// スタート イベント番号
    /// </summary>

    [Sort(101)]
    public long StartNumber { get; private set; } = -1;

    /// <summary>
    /// シャットダウン イベント番号
    /// </summary>
    [Sort(102)]
    public long ShutdownNumber { get; private set; } = -1;

    /// <summary>
    /// -1  : Skip
    ///  0  : Shutdown
    /// >0  : Other Event
    /// </summary>
    [Sort(102)]
    public long IsSkip { get; private set; } = JOIN_SHUTDOWN;

    /// <summary>
    /// スキップ セット
    /// </summary>
    public void SetSkip()
    {
        IsSkip = SKIP;
    }

    /// <summary>
    /// 文字列化
    /// </summary>
    public override string ToString()
    {
        return $"{StartNumber},{ShutdownNumber},{IsSkip}";
    }
}
