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
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

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
[Table("TbDequeue")]
internal sealed class LogEventDequeue : LogEventBase, ILogEventUserHost, ILogEventProduct, ILogEventWhy
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventDequeue(string[] list_) : base()
    {
        //if (list_.Count() < 4)
        if (list_.Length < 4)
        {
            _initSmall(list_);
        }
        else
        {
            _initStandard(list_);
        }
    }

    private void _initSmall(string[] list_)
    {
        // small
        //Why = int.Parse(list_[1]);
        Why = (StatusValue)int.Parse(list_[1], CultureInfo.InvariantCulture);
        Count = int.Parse(list_[2], CultureInfo.InvariantCulture);
        HandleServer = list_[3];
        //
        EventDateTime = DateTime.Parse(_NowDate + " " + list_[4], CultureInfo.InvariantCulture);
        LogFormat = LogFormat.SMALL;
    }

    private void _initStandard(string[] list_)
    {
        // std
        // detailed
        Why = (StatusValue)int.Parse(list_[1], CultureInfo.InvariantCulture);
        Product = list_[2];
        Version = list_[3];
        User = list_[4];
        Host = list_[5];
        IsvDef = list_[6];
        //
        Count = int.Parse(list_[7], CultureInfo.InvariantCulture);
        //
        HandleServer = list_[8];
        //
        EventDateTime = _GetDateTime(list_[9], list_[10]);
        LogFormat = (list_[10].Contains('.') == true) ? LogFormat.DETAILED : LogFormat.STANDARD;
    }

    //dequeue
    //DEQUE why product version       user  host “isv_def” count server_handle mm/dd hh:mm:ss
    //DEQUE why product version       user  host “isv_def” count server_handle mm/dd hh:mm:ss.tenths_of_msec
    //DEQUE why count   server_handle hh:mm
    //0     1   2       3             4     5     6          7     8             9     10
    [Column("Product", Order = 11)]
    public string Product { get; private set; } = string.Empty;

    [Column("Version", Order = 12)]
    public string Version { get; private set; } = string.Empty;

    [Column("Product Version", Order = 13)]
    public string ProductVersion { get => Product + " " + Version; }
    //
    [Column("User", Order = 21)]
    public string User { get; private set; } = string.Empty;

    [Column("Host", Order = 22)]
    public string Host { get; private set; } = string.Empty;

    [Column("User@Host", Order = 23)]
    public string UserHost { get => User + "@" + Host; }

    [Column("Why", Order = 101)]
    public StatusValue Why { get; private set; } = StatusValue.Success;

    [Column("Isv Def", Order = 102)]
    public string IsvDef { get; private set; } = string.Empty;

    [Column("Count", Order = 103)]
    public int Count { get; private set; } = -1;

    [Column("Server Handle", Order = 104)]
    public string HandleServer { get; private set; } = string.Empty;
    //
}
