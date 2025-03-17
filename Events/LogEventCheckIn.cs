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

namespace RepriseReportLogAnalyzer.Events;

/// <summary>
/// 文字列 と イベントの紐づけ登録
/// </summary>
internal sealed partial class LogEventRegist
{
    private bool _logEventCheckIn = Regist("IN", (l_) => new LogEventCheckIn(l_));
}

/// <summary>
/// check-in
/// </summary>
[Sort(12)][Table("TbCheckIn")]
internal sealed class LogEventCheckIn : LogEventBase, ILogEventUserHost, ILogEventCountCurrent, ILogEventWhy
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventCheckIn(string[] list_) : base()
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
            LogFormat = LogFormat.SMALL;

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
            CountCurrent = int.Parse(list_[8]);
            ResuseCurrent = int.Parse(list_[9]);
            //
            HandleServer = list_[10];
            //
            EventDateTime = _GetDateTime(list_[11], list_[12]);

            LogFormat = (list_[12].Contains(".") == true) ? LogFormat.DETAILED : LogFormat.STANDARD;
        }
    }


    //public const LogEventType LogType = LogEventType.CheckIn;
    //check-in
    //IN why product version       user  host “isv_def” count cur_use cur_resuse server_handle mm/dd hh:mm:ss
    //IN why product version       user  host “isv_def” count cur_use cur_resuse server_handle mm/dd hh:mm:ss.tenths_of_msec
    //IN why count   server_handle hh:mm
    //0  1   2       3             4     5     6          7     8       9          10            11    12
    [Sort(11)]
    [Column("Product")]
    public string Product { get; private set; } = string.Empty;

    [Sort(12)]
    [Column("Version")]
    public string Version { get; private set; } = string.Empty;

    [Sort(13)]
    [Column("Product Version")]
    public string ProductVersion { get => Product + " " + Version; }

    [Sort(21)]
    [Column("User")]
    public string User { get; private set; } = string.Empty;

    [Sort(22)]
    [Column("Host")]
    public string Host { get; private set; } = string.Empty;

    [Sort(23)]
    [Column("User@Host")]
    public string UserHost { get => User + "@" + Host; }
    //
    [Sort(101)]
    [Column("Why")]
    public StatusValue Why { get; private set; } = StatusValue.Success;
    //public int Why { get; private set; } = 0;

    [Sort(102)]
    [Column("Isv Def")]
    public string IsvDef { get; private set; } = string.Empty;
    //
    [Sort(103)]
    [Column("Count")]
    public int Count { get; private set; } = -1;

    [Sort(104)]
    [Column("Current Count")]
    public int CountCurrent { get; private set; } = -1;

    [Sort(105)]
    [Column("Current Resuse")]
    public int ResuseCurrent { get; private set; } = -1;

    [Sort(106)]
    [Column("Server Handle")]
    public string HandleServer { get; private set; } = string.Empty;
    //
}
