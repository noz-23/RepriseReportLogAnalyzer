/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Data;
using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace RepriseReportLogAnalyzer.Analyses;

/// <summary>
/// スタートとシャットダウンの時間帯
/// (ログ集計の最後はシャットダウンとする)
/// </summary>
internal sealed class AnalysisStartShutdown : ToDataBase
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="start_">スタート イベント</param>
    /// <param name="shutdown_">シャットダウン イベント</param>
    public AnalysisStartShutdown(LogEventStart start_, LogEventShutdown shutdown_)
    {
        _start = start_;
        _shutdown = shutdown_;

        _joinEvent = new JoinEventStartShutdown(start_, shutdown_);
    }

    /// <summary>
    /// スタート イベント番号
    /// </summary>
    [Column("Start No", Order = 101)]
    public long StartNumber { get => _start.EventNumber; }

    /// <summary>
    /// シャットダウン イベント番号
    /// </summary>
    [Column("Shutdown No", Order = 102)]
    public long ShutdownNumber { get => _shutdown?.EventNumber ?? LogEventBase.NowEventNumber; }

    /// <summary>
    /// スタート 時間
    /// </summary>
    [Column("Start Date Time", Order = 111)]
    public DateTime StartDateTime { get => _start.EventDateTime; }

    /// <summary>
    /// シャットダウン 時間
    /// </summary>
    [Column("Shutdown Date Time", Order = 112)]
    public DateTime ShutdownDateTime { get => _shutdown?.EventDateTime ?? LogEventBase.NowDateTime; }

    /// <summary>
    /// 稼働時間
    /// </summary>
    [Column("Duration", Order = 112)]
    public TimeSpan Duration { get => (ShutdownDateTime - StartDateTime); }

    /// <summary>
    /// スタート イベント(リフレクションで呼び出さないため関数化)
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LogEventStart EventStart() => _start;
    private readonly LogEventStart _start;

    /// <summary>
    /// シャットダウン イベント(リフレクションで呼び出さないため関数化)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LogEventBase EventShutdown() => _shutdown;
    private readonly LogEventShutdown _shutdown;

    /// <summary>
    /// 結合情報 (リフレクションで呼び出さないため関数化)
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public JoinEventStartShutdown JoinEvent() => _joinEvent;
    private readonly JoinEventStartShutdown _joinEvent;

    /// <summary>
    /// シャットダウン イベント番号
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ShudownNumber() => _shutdown.EventNumber;

    /// <summary>
    /// スタートとシャットダウンの間のイベントか？
    /// </summary>
    /// <param name="number_"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsWithInRange(long number_)
    {
        if (number_ < _start.EventNumber)
        {
            // 範囲外
            return false;
        }

        if (number_ > ShudownNumber())
        {
            // 範囲外
            return false;
        }
        return true;
    }

    /// <summary>
    /// ヘッダー
    /// </summary>
    public static string Header() => ToDataBase.Header(typeof(AnalysisStartShutdown));

    /// <summary>
    /// リスト化したヘッダー項目
    /// </summary>
    /// <returns></returns>
    public static ListStringStringPair ListHeader() => ToDataBase.ListHeader(typeof(AnalysisStartShutdown));
}
