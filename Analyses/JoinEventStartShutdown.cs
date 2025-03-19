/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Enums;
using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Files;
using System.ComponentModel.DataAnnotations.Schema;

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
    /// スタート イベント番号
    /// </summary>

    [Column("Start No", Order = 101)]
    public long StartNumber { get; private set; } = -1;

    /// <summary>
    /// シャットダウン イベント番号
    /// </summary>
    [Column("Shutdonw No", Order = 102)]
    public long ShutdownNumber { get; private set; } = -1;

    /// <summary>
    /// -1  : Skip
    ///  0  : Shutdown
    /// >0  : Other Event
    /// </summary>
    [Column("Skip StNo", Order = 103)]
    public long IsSkip { get; private set; } = (long)SelectData.ALL;

    /// <summary>
    /// スキップ セット
    /// </summary>
    public void SetSkip()=> IsSkip = (long)SelectData.ECLUSION;
    

    /// <summary>
    /// 文字列化
    /// </summary>
    public override string ToString()=> $"{StartNumber},{ShutdownNumber},{IsSkip}";
}
