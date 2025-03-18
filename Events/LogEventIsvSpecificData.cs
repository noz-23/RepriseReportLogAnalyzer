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
    private bool _logEventIsvSpecificData = Regist("log", (l_) => new LogEventIsvSpecificData(l_));
}

/// <summary>
/// isv-specific data
/// </summary>
[Sort(83)][Table("TbIsvSpecificData")]
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
        EventDateTime = _GetDateTime(list_[1], list_[2]);
        IsvSpecificData = list_[3];
        LogFormat = LogFormat.NONE;
    }

    //isv-specific data
    //log mm/dd hh:mm:ss isv-specific-data-here
    //0   1     2        3
    [Column("Isv Specific Data Here", Order =101)]
    public string IsvSpecificData { get; private set; } = string.Empty;

}
