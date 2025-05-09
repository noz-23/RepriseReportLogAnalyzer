﻿/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Data;
using RepriseReportLogAnalyzer.Enums;
using RepriseReportLogAnalyzer.Events;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace RepriseReportLogAnalyzer.Analyses;

/// <summary>
///  スタートとシャットダウンの結合情報
///  重複情報
/// </summary>
[Table("TbJoinJoinEventStartShutdown")]
internal sealed class JoinEventStartShutdown : BaseToData
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="start_">スタート イベント</param>
    /// <param name="shutdown_">シャットダウン イベント</param>
    public JoinEventStartShutdown(LogEventStart start_, LogEventShutdown shutdown_)
    {
        StartNumber = start_.EventNumber;
        ShutdownNumber = shutdown_.EventNumber;
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetSkip() => IsSkip = (long)SelectData.ECLUSION;
}
