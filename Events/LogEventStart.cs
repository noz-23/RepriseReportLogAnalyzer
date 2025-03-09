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
    private bool _logEventStart = Regist("START", (l_) => new LogEventStart(l_));
}

/// <summary>
/// log file start
/// </summary>

[Sort(1)]
internal sealed class LogEventStart : LogEventBase
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventStart(string[] list_) : base()
    {
        //LogEventBase.ListEventData.Add("START", (l_) => new LogEventStart(l_));
        //var start= LogEventBase.ListEventData["START"] = (l_) => new LogEventStart(l_);

        HostName = list_[1];
        EventDateTime = DateTime.Parse(list_[2] + " " + list_[3]);
    }


    //public const LogEventType LogType = LogEventType.Start;
    //START hostname mm/dd/yyyy hh:mm
    //0     1        2          3
    [Sort(101)]
    public string HostName { get; private set; } = string.Empty;
}
