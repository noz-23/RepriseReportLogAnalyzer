/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Interfaces;

namespace RepriseReportLogAnalyzer.Events;

/// <summary>
/// 文字列 と イベントの紐づけ登録
/// </summary>
internal sealed partial class LogEventRegist
{
    private bool _logEventLicenseDenial = Regist("DENY", (l_) => new LogEventLicenseDenial(l_));
}

/// <summary>
/// license denial
/// </summary>
[Sort(13)]
internal sealed class LogEventLicenseDenial : LogEventBase, ILogEventUserHost, ILogEventProduct, ILogEventWhy
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventLicenseDenial(string[] list_) : base()
    {
        Product = list_[1];
        Version = list_[2];
        User = list_[3];
        Host = list_[4];
        IsvDef = list_[5];
        //
        Count = int.Parse(list_[6]);
        Why = int.Parse(list_[7]);
        LastAttempt = list_[8];
        ProcessId = list_[9];

        EventDateTime = _GetDateTime(list_[10], list_[11]);
    }

    //license denial
    //DENY product version user host “isv_def” count why last_attempt pid mm/dd hh:mm
    //DENY product version user host “isv_def” count why last_attempt pid mm/dd hh:mm:ss.tenths_of_msec
    //0    1       2       3    4     5          6     7   8            9   10    11
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
    //
    [Sort(101)]
    public string IsvDef { get; private set; } = string.Empty;
    [Sort(102)]
    public int Count { get; private set; } = -1;
    [Sort(103)]
    public int Why { get; private set; } = -1;
    [Sort(104)]
    public string LastAttempt { get; private set; } = string.Empty;
    [Sort(105)]
    public string ProcessId { get; private set; } = string.Empty;
    //

}
