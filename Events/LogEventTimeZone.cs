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
using System.ComponentModel.DataAnnotations.Schema;

namespace RepriseReportLogAnalyzer.Events;

/// <summary>
/// 文字列 と イベントの紐づけ登録
/// </summary>
internal sealed partial class LogEventRegist
{
    private bool _logEventTimeZone = Regist("TIMEZONE", (l_) => new LogEventTimeZone(l_));
}

/// <summary>
/// log file start
/// </summary>
[Sort(98)][Table("TbTimeZone")]
internal sealed class LogEventTimeZone : LogEventBase
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventTimeZone(string[] list_) : base()
    {
        // small
        // std
        // detailed
        MinutesWestOfUTC = list_[1];
        DayLight = list_[2];
        Rules = list_[3];
        //
        EventDateTime = NowDateTime;
        LogFormat = LogFormat.NONE;
    }

    //TIMEZONE minutes-west-of-UTC daylight rules # readable version of data
    //0        1                   2        3     
    [Column("Minutes West Of UTC", Order =101)]
    public string MinutesWestOfUTC { get; private set; } = string.Empty;

    [Column("DayLight", Order =102)]
    public string DayLight { get; private set; } = string.Empty;

    [Column("Rules", Order =103)]
    public string Rules { get; private set; } = string.Empty;
}
