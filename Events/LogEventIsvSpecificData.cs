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
    private bool _logEventIsvSpecificData = Regist("log", (l_) => new LogEventIsvSpecificData(l_));
}

/// <summary>
/// isv-specific data
/// </summary>
[Sort(83)]
[Table("TbIsvSpecificData")]
internal sealed class LogEventIsvSpecificData : LogEventBase
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>

    public LogEventIsvSpecificData(string[] list_) : base()
    {
        // small
        // std
        // detailed
        EventDateTime = _GetDateTime(list_[_INDEX_DATE], list_[_INDEX_TIME]);
        IsvSpecificData = list_[_INDEX_ISV_SPECIFIC_DATA_HERE];
    }

    //isv-specific data
    //log mm/dd hh:mm:ss isv-specific-data-here
    //0   1     2        3
    private const int _INDEX_DATE = 1;
    private const int _INDEX_TIME = 2;
    private const int _INDEX_ISV_SPECIFIC_DATA_HERE = 3;
    //
    [Column("Isv Specific Data Here", Order = 101, TypeName = "TEXT")]
    public string IsvSpecificData { get; private set; } = string.Empty;

}
