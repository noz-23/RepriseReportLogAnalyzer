/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

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
[Table("TbMeterDecrement")]
internal sealed class LogEventMeterDecrement : LogEventBase
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventMeterDecrement(string[] list_) : base()
    {
        // small
        // std
        // detailed
        HandleLicense = list_[_INDEX_LICENSE_HANDLE];
        CounterMeter = list_[_INDEX_METER_COUNTER];
        AmountDecremented = list_[_INDEX_AMOUNT_DECREMENTED];
        //
        EventDateTime = _GetDateTime(list_[_INDEX_DATE], list_[_INDEX_TIME]);
    }

    //meter decrement
    //METER_DEC license_handle meter_counter amount_decremented mm/dd hh:mm:ss[.tenths_of_msec]
    //0         1              2             3                  4     5
    private const int _INDEX_LICENSE_HANDLE = 1;
    private const int _INDEX_METER_COUNTER = 2;
    private const int _INDEX_AMOUNT_DECREMENTED = 3;
    private const int _INDEX_DATE = 4;
    private const int _INDEX_TIME = 5;
    //
    [Column("License Handle", Order = 101)]
    public string HandleLicense { get; private set; } = string.Empty;

    [Column("Meter Counter", Order = 102)]
    public string CounterMeter { get; private set; } = string.Empty;

    [Column("Amount Decremented", Order = 103)]
    public string AmountDecremented { get; private set; } = string.Empty;
}
