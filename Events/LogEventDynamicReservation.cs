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
using System.Globalization;

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
        Reservation = (list_[_INDEX_RESERVATION] == "create") ? ReservationType.CREATE : ReservationType.REMOVE;
        User = list_[_INDEX_USER];
        Host = list_[_INDEX_HOST];
        //
        LicensePool = int.Parse(list_[_INDEX_LICENSE_POOL], CultureInfo.InvariantCulture);
        Count = int.Parse(list_[_INDEX_COUNT], CultureInfo.InvariantCulture);
        //
        StringData = list_[_INDEX_STRING];

        EventDateTime = _GetDateTime(list_[_INDEX_DATE], list_[_INDEX_TIME]);
    }


    //dynamic reservation
    //DYNRES [create | remove] user host license-pool count “string” mm/dd hh:mm:ss
    //0      1                 2    3    4            5      6         7     8
    private const int _INDEX_RESERVATION = 1;
    private const int _INDEX_USER = 2;
    private const int _INDEX_HOST = 3;
    private const int _INDEX_LICENSE_POOL = 4;
    private const int _INDEX_COUNT = 5;
    private const int _INDEX_STRING = 6;
    private const int _INDEX_DATE = 7;
    private const int _INDEX_TIME = 8;
    //
    [Column("User", Order = 11, TypeName = "TEXT")]
    public string User { get; private set; } = string.Empty;

    [Column("Host", Order = 12, TypeName = "TEXT")]
    public string Host { get; private set; } = string.Empty;

    [Column("User@Host", Order = 13, TypeName = "TEXT")]
    public string UserHost { get => User + "@" + Host; }

    [Column("Reservation", Order = 101, TypeName = "INTEGER")]
    public ReservationType Reservation { get; private set; } = ReservationType.CREATE;

    [Column("License Pool", Order = 102, TypeName = "INTEGER")]
    public int LicensePool { get; private set; } = -1;

    [Column("Count", Order = 103, TypeName = "INTEGER")]
    public int Count { get; private set; } = -1;

    [Column("String Data", Order = 104, TypeName = "TEXT")]
    public string StringData { get; private set; } = string.Empty;
    //
}
