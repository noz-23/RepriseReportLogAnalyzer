﻿/*
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
    private bool _logEventMeterDecrement = Regist("METER_DEC", (l_) => new LogEventMeterDecrement(l_));
}

/// <summary>
/// meter decrement
/// </summary>
[Sort(73)]
internal sealed class LogEventMeterDecrement : LogEventBase
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventMeterDecrement(string[] list_) : base()
    {
        HandleLicense = list_[1];
        CounterMeter = list_[2];
        AmountDecremented = list_[3];
        //
        EventDateTime = _GetDateTime(list_[4], list_[5]);
    }

    //meter decrement
    //METER_DEC license_handle meter_counter amount_decremented mm/dd hh:mm:ss[.tenths_of_msec]
    //0         1              2             3                  4     5
    [Sort(101)]
    public string HandleLicense { get; private set; } = string.Empty;
    [Sort(102)]
    public string CounterMeter { get; private set; } = string.Empty;
    [Sort(103)]
    public string AmountDecremented { get; private set; } = string.Empty;

}
