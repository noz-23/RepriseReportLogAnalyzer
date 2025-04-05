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
    private bool _logEventTimeJump = Regist("TIMEJUMP", (l_) => new LogEventTimeJump(l_));
}

/// <summary>
/// server time jump
/// </summary>
[Sort(97)]
[Table("TbTimeJump")]
internal sealed class LogEventTimeJump : LogEventBase
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventTimeJump(string[] list_) : base()
    {
        // small
        // std
        // detailed
        Minutes = list_[_INDEX_MINUTES];

        EventDateTime = _GetDateTime(list_[_INDEX_DATE], list_[_INDEX_DATE]);
    }

    //server time jump
    //TIMEJUMP[+ | -]minutes mm/dd hh:mm:ss
    //0        1             2     3
    private const int _INDEX_MINUTES = 1;
    private const int _INDEX_DATE = 2;
    private const int _INDEX_TIME = 3;
    //
    //
    [Column("Minutes", Order = 101)]
    public string Minutes { get; private set; } = string.Empty;
}
