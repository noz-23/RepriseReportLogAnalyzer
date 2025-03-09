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
using System.Runtime.CompilerServices;

namespace RepriseReportLogAnalyzer.Analyses;

/// <summary>
/// チェックアウト チェックイン 結合情報
/// 重複情報
/// </summary>
internal sealed class JoinEventCheckOutIn
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="checkOut_">チェックアウト イベント</param>
    /// <param name="checkIn_">チェックイン or シャットダウン イベント</param>
    public JoinEventCheckOutIn(LogEventCheckOut checkOut_, LogEventBase? checkIn_)
    {
        CheckOutNumber = checkOut_.EventNumber;
        if (checkIn_ is LogEventCheckIn checkIn)
        {
            CheckInNumber = checkIn.EventNumber;
        }
        else if (checkIn_ is LogEventShutdown shutdown)
        {
            ShutdownNumber = shutdown.EventNumber;
        }
        else
        {
            LogFile.Instance.WriteLine($"{checkIn_?.EventNumber} {checkIn_?.GetType()}");
        }
        _checkIn = checkIn_;
    }

    /// <summary>
    /// 文字列化のヘッダー
    /// </summary>
    public const string HEADER = "CheckOut,CheckIn,Shutdown,Duplication";

    /// <summary>
    /// 重複なし(AnalysisCheckOutInのデータそのまま)
    /// </summary>
    public const long NO_DUPLICATION = 0;

    /// <summary>
    /// 重複あり(利用しない)
    /// </summary>
    public const long HAVE_DUPLICATION = -1;

    /// <summary>
    /// チェックアウト イベント番号
    /// </summary>
    [Sort(101)]
    public long CheckOutNumber { get; private set; } = -1;

    /// <summary>
    /// チェックイン イベント番号
    /// </summary>
    [Sort(102)]
    public long CheckInNumber { get; private set; } = -1;

    /// <summary>
    /// シャットダウン イベント番号(チェックインの替わり)
    /// </summary>

    [Sort(103)]
    public long ShutdownNumber { get; private set; } = -1;


    /// <summary>
    /// -1  : Duplication
    ///  0  : Use CheckInNumber or ShutdownNumber;
    /// >0  : Use DuplicationNumber
    /// </summary>
    [Sort(104)]
    public long DuplicationNumber { get; private set; } = NO_DUPLICATION;

    /// <summary>
    /// 重複取り除いた場合のイベント
    /// </summary>
    private LogEventBase? _checkIn = null;
    public LogEventBase? CheckIn() => _checkIn;


    /// <summary>
    /// 
    /// 重複あり イベント セット
    /// </summary>
    /// <param name="checkIn_">更新したイベント</param>

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetDuplication(LogEventBase? checkIn_ = null)
    {
        if (checkIn_ == null)
        {
            DuplicationNumber = HAVE_DUPLICATION;
        }
        else
        {
            _checkIn = checkIn_;
            DuplicationNumber = _checkIn?.EventNumber ?? NO_DUPLICATION;
        }
    }

    /// <summary>
    /// 文字列化
    /// </summary>
    public override string ToString()
    {
        return $"{CheckOutNumber},{CheckInNumber},{ShutdownNumber},{DuplicationNumber}";
    }
}
