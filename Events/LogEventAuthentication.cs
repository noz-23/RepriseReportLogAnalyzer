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
    private bool _logEventAuthentication = Regist("AUTH", (l_) => new LogEventAuthentication(l_));
}

/// <summary>
/// Authentication data
/// </summary>
[Sort(70)]
internal sealed class LogEventAuthentication : LogEventBase
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventAuthentication(string[] list_) : base()
    {
        Section = list_[1];
        Signature = list_[2];

        EventDateTime = NowDateTime;
    }

    //AUTH section signature
    //0    1       2
    [Sort(101)]
    public string Section { get; private set; } = string.Empty;
    [Sort(102)]
    public string Signature { get; private set; } = string.Empty;
}
