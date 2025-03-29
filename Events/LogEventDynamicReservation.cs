/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Enums;
using RepriseReportLogAnalyzer.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepriseReportLogAnalyzer.Events;

/// <summary>
/// 文字列 と イベントの紐づけ登録
/// </summary>
internal sealed partial class LogEventRegist
{
    private bool _logEventDynamicReservation = Regist("DYNRES", (l_) => new LogEventDynamicReservation(l_));
}

/// <summary>
/// dynamic reservation
/// </summary>
[Sort(71)]
[Table("TbDynamicReservation")]
internal sealed class LogEventDynamicReservation : LogEventBase, ILogEventUserHost
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>

    public LogEventDynamicReservation(string[] list_) : base()
    {
        // small
        // std
        // detailed
        Reservation = (list_[1] == "create") ? ReservationType.CREATE : ReservationType.REMOVE;
        User = list_[2];
        Host = list_[3];
        //
        LicensePool = int.Parse(list_[4]);
        Count = int.Parse(list_[5]);
        //
        StringData = list_[6];
        EventDateTime = _GetDateTime(list_[7], list_[8]);
        LogFormat = LogFormat.NONE;
    }


    //dynamic reservation
    //DYNRES [create | remove] user host license-pool count “string” mm/dd hh:mm:ss
    //0      1                 2    3    4            5      6         7     8
    //
    [Column("User", Order = 11)]
    public string User { get; private set; } = string.Empty;

    [Column("Host", Order = 12)]
    public string Host { get; private set; } = string.Empty;

    [Column("User@Host", Order = 13)]
    public string UserHost { get => User + "@" + Host; }

    [Column("Reservation", Order = 101)]
    public ReservationType Reservation { get; private set; } = ReservationType.CREATE;

    [Column("License Pool", Order = 102)]
    public int LicensePool { get; private set; } = -1;

    [Column("Count", Order = 103)]
    public int Count { get; private set; } = -1;

    [Column("String Data", Order = 104)]
    public string StringData { get; private set; } = string.Empty;
    //
}
