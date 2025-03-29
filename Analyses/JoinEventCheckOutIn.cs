/*
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
/// チェックアウト チェックイン 結合情報
/// 重複情報
/// </summary>
[Table("TbJoinEventCheckOutIn")]
internal sealed class JoinEventCheckOutIn : ToDataBase
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="checkOut_">チェックアウト イベント</param>
    /// <param name="checkIn_">チェックイン or シャットダウン イベント</param>
    public JoinEventCheckOutIn(LogEventCheckOut checkOut_, LogEventBase checkIn_)
    {
        CheckOutNumber = checkOut_.EventNumber;
        _checkIn = checkIn_;
        if (checkIn_ is LogEventCheckIn checkIn)
        {
            CheckInNumber = checkIn.EventNumber;
        }
        else if (checkIn_ is LogEventShutdown shutdown)
        {
            ShutdownNumber = shutdown.EventNumber;
        }
    }

    /// <summary>
    /// チェックアウト イベント番号
    /// </summary>
    [Column("CheckOut No", Order = 101)]
    public long CheckOutNumber { get; private set; } = -1;

    /// <summary>
    /// チェックイン イベント番号
    /// </summary>
    [Column("CheckIn No", Order = 102)]
    public long CheckInNumber { get; private set; } = -1;

    /// <summary>
    /// シャットダウン イベント番号(チェックインの替わり)
    /// </summary>
    [Column("Shutdown No", Order = 103)]
    public long ShutdownNumber { get; private set; } = -1;


    /// <summary>
    /// -1  : Duplication
    ///  0  : Use CheckInNumber or ShutdownNumber;
    /// >0  : Use DuplicationNumber
    /// </summary>
    [Column("Duplication StNo", Order = 104)]
    public long DuplicationNumber { get; private set; } = (long)SelectData.ALL;

    /// <summary>
    /// 重複取り除いた場合のイベント
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LogEventBase CheckIn() => _checkIn;
    private LogEventBase _checkIn;

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
            // 重複(別のチェックイン-チェックアウト内)
            DuplicationNumber = (long)SelectData.ECLUSION;
        }
        else
        {
            // 重複(チェックアウトが別のチェックイン-チェックアウト内)
            _checkIn = checkIn_;
            DuplicationNumber = _checkIn.EventNumber;
        }
    }
}
