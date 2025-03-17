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
using RepriseReportLogAnalyzer.Interfaces;

namespace RepriseReportLogAnalyzer.Events;

/// <summary>
/// 文字列 と イベントの紐づけ登録
/// </summary>
internal sealed partial class LogEventRegist
{
    private bool _logEventDequeue = Regist("DEQUE", (l_) => new LogEventDequeue(l_));
}

/// <summary>
/// dequeue
/// </summary>
[Sort(14)]
internal sealed class LogEventDequeue : LogEventBase, ILogEventUserHost, ILogEventProduct, ILogEventWhy
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventDequeue(string[] list_) : base()
    {
        if (list_.Count() < 4)
        {
            // small
            //Why = int.Parse(list_[1]);
            Why = (StatusValue)int.Parse(list_[1]);
            Count = int.Parse(list_[2]);
            HandleServer = list_[3];
            //
            EventDateTime = DateTime.Parse(_NowDate + " " + list_[4]);
        }
        else
        {
            // std
            // detailed
            //Why = int.Parse(list_[1]);
            Why = (StatusValue)int.Parse(list_[1]);
            Product = list_[2];
            Version = list_[3];
            User = list_[4];
            Host = list_[5];
            IsvDef = list_[6];
            //
            Count = int.Parse(list_[7]);
            //
            HandleServer = list_[8];
            //
            EventDateTime = _GetDateTime(list_[9], list_[10]);
        }
    }

    //dequeue
    //DEQUE why product version       user  host “isv_def” count server_handle mm/dd hh:mm:ss
    //DEQUE why product version       user  host “isv_def” count server_handle mm/dd hh:mm:ss.tenths_of_msec
    //DEQUE why count   server_handle hh:mm
    //0     1   2       3             4     5     6          7     8             9     10
    [Sort(11)]
    public string Product { get; private set; } = string.Empty;
    [Sort(12)]
    public string Version { get; private set; } = string.Empty;
    [Sort(13)]
    public string ProductVersion { get => Product + " " + Version; }
    //
    [Sort(21)]
    public string User { get; private set; } = string.Empty;
    [Sort(22)]
    public string Host { get; private set; } = string.Empty;
    [Sort(23)]
    public string UserHost { get => User + "@" + Host; }

    [Sort(101)]
    public StatusValue Why { get; private set; } = StatusValue.Success;
    //public int Why { get; private set; } = 0;
    [Sort(102)]
    public string IsvDef { get; private set; } = string.Empty;
    [Sort(103)]
    public int Count { get; private set; } = -1;
    [Sort(104)]
    public string HandleServer { get; private set; } = string.Empty;
    //
}
