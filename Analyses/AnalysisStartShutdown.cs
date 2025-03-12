/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Data;
using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Interfaces;
using System.Runtime.CompilerServices;

namespace RepriseReportLogAnalyzer.Analyses;

/// <summary>
/// スタートとシャットダウンの時間帯
/// (ログ集計の最後はシャットダウンとする)
/// </summary>
internal sealed class AnalysisStartShutdown:ToDataBase
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="start_">スタート イベント</param>
    /// <param name="shutdown_">シャットダウン イベント</param>
    public AnalysisStartShutdown(LogEventStart start_, LogEventBase? shutdown_)
    {
        _start = start_;
        _shutdown = shutdown_;

        _joinEvent = new JoinEventStartShutdown(start_, shutdown_);
    }

    /// <summary>
    /// スタート イベント番号
    /// </summary>
    [Sort(101)]
    public long StartNumber { get => _start.EventNumber; }

    /// <summary>
    /// シャットダウン イベント番号
    /// </summary>
    [Sort(102)]
    public long ShutdownNumber { get => _shutdown?.EventNumber ?? LogEventBase.NowEventNumber; }

    /// <summary>
    /// スタート 時間
    /// </summary>
    [Sort(111)]
    public DateTime StartDateTime { get => _start.EventDateTime; }

    /// <summary>
    /// シャットダウン 時間
    /// </summary>
    [Sort(112)]
    public DateTime ShutdownDateTime { get => _shutdown?.EventDateTime ?? LogEventBase.NowDateTime; }

    /// <summary>
    /// 稼働時間
    /// </summary>
    [Sort(113)]
    public TimeSpan Duration { get => (ShutdownDateTime - StartDateTime); }

    /// <summary>
    /// スタート イベント
    /// </summary>
    private readonly LogEventStart _start;

    /// <summary>
    /// シャットダウン イベント
    /// </summary>
    private readonly LogEventBase? _shutdown;

    /// <summary>
    /// スタートとシャットダウンの結合情報
    /// </summary>
    private readonly JoinEventStartShutdown _joinEvent;

    /// <summary>
    /// スタート イベント(リフレクションで呼び出さないため関数化)
    /// </summary>
    /// <returns></returns>
    public LogEventStart EventStart() => _start;

    /// <summary>
    /// シャットダウン イベント(リフレクションで呼び出さないため関数化)
    /// </summary>
    public LogEventBase? EventShutdown() => _shutdown;

    /// <summary>
    /// 結合情報 (リフレクションで呼び出さないため関数化)
    /// </summary>
    /// <returns></returns>
    public JoinEventStartShutdown JoinEvent() => _joinEvent;

    /// <summary>
    /// シャットダウン イベント番号
    /// </summary>
    public long ShudownNumber() => _shutdown?.EventNumber ?? LogEventBase.NowEventNumber;

    /// <summary>
    /// スタートとシャットダウンの間のイベントか？
    /// </summary>
    /// <param name="number_"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsWithInRange(long number_)
    {
        if (number_ < _start.EventNumber)
        {
            return false;
        }

        if (number_ > ShudownNumber())
        {
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
