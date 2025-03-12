/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Attributes;

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
[Sort(90)]
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
    }

    //TIMEZONE minutes-west-of-UTC daylight rules # readable version of data
    //0        1                   2        3     
    [Sort(101)]
    public string MinutesWestOfUTC { get; private set; } = string.Empty;
    [Sort(102)]
    public string DayLight { get; private set; } = string.Empty;
    [Sort(103)]
    public string Rules { get; private set; } = string.Empty;
}
