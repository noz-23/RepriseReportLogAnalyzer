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
    private bool _logEventTimeJump = Regist("TIMEJUMP", (l_) => new LogEventTimeJump(l_));
}

/// <summary>
/// server time jump
/// </summary>
[Sort(97)][Table("TbTimeJump")]
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
        Minutes = list_[1];
        EventDateTime = DateTime.Parse(list_[2] + " " + list_[3]);
        LogFormat = LogFormat.NONE;
    }

    //server time jump
    //TIMEJUMP[+ | -]minutes mm/dd hh:mm:ss
    //0        1             2     3
    [Column("Minutes", Order =101)]
    public string Minutes { get; private set; } = string.Empty;
}
