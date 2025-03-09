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
internal sealed class LogEventDynamicReservation : LogEventBase, ILogEventUserHost
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>

    public LogEventDynamicReservation(string[] list_) : base()
    {
        Reservation = (list_[1] == "create") ? ReservationType.CREATE : ReservationType.REMOVE;
        User = list_[2];
        Host = list_[3];
        //
        LicensePool = int.Parse(list_[4]);
        Count = int.Parse(list_[5]);
        //
        StringData = list_[6];
        EventDateTime = _GetDateTime(list_[7], list_[8]);
    }


    //dynamic reservation
    //DYNRES [create | remove] user host license-pool count “string” mm/dd hh:mm:ss
    //0      1                 2    3    4            5      6         7     8
    //
    [Sort(11)]
    public string User { get; private set; } = string.Empty;
    [Sort(12)]
    public string Host { get; private set; } = string.Empty;
    [Sort(13)]
    public string UserHost { get => User + "@" + Host; }
    [Sort(101)]
    public ReservationType Reservation { get; private set; } = ReservationType.CREATE;
    [Sort(102)]
    public int LicensePool { get; private set; } = -1;
    [Sort(103)]
    public int Count { get; private set; } = -1;
    [Sort(104)]
    public String StringData { get; private set; } = string.Empty;
    //
}
