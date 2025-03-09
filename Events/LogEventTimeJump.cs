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
    private bool _logEventTimeJump = Regist("TIMEJUMP", (l_) => new LogEventTimeJump(l_));
}

/// <summary>
/// server time jump
/// </summary>
[Sort(91)]
internal sealed class LogEventTimeJump : LogEventBase
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventTimeJump(string[] list_) : base()
    {
        Minutes = list_[1];
        EventDateTime = DateTime.Parse(list_[2] + " " + list_[3]);
    }

    //server time jump
    //TIMEJUMP[+ | -]minutes mm/dd hh:mm:ss
    //0        1             2     3
    [Sort(101)]
    public string Minutes { get; private set; } = string.Empty;
}
