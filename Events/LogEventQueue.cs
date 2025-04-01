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
    private bool _logEventQueue = Regist("QUE", (l_) => new LogEventQueue(l_));
}

/// <summary>
/// queue
/// </summary>
[Sort(16)]
[Table("TbQueue")]
internal sealed class LogEventQueue : LogEventBase, ILogEventUserHost, ILogEventProduct
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>

    public LogEventQueue(string[] list_) : base()
    {
        //if (list_.Count() < 12)
        if (list_.Length < 12)
        {
            // small
            Product = list_[1];
            Version = list_[2];
            User = list_[3];
            Host = list_[4];
            IsvDef = list_[5];
            Count = int.Parse(list_[6], CultureInfo.InvariantCulture);
            //
            HandleServer = list_[7];
            //
            EventDateTime = DateTime.Parse(_NowDate + " " + list_[8], CultureInfo.InvariantCulture);
            LogFormat = LogFormat.SMALL;
        }
        else
        {
            // std
            // detailed
            Product = list_[1];
            Version = list_[2];
            User = list_[3];
            Host = list_[4];
            IsvDef = list_[5];
            //
            Count = int.Parse(list_[6]);
            //
            HandleServer = list_[7];
            //
            Project = list_[8];
            RequestedProduct = list_[9];
            RequestedVersion = list_[10];
            //
            EventDateTime = _GetDateTime(list_[11], list_[12]);
            LogFormat = (list_[12].Contains('.') == true) ? LogFormat.DETAILED : LogFormat.STANDARD;
        }
    }

    //queue
    //QUE product version user host “isv_def” count server_handle “project” “requested product” “requested version” mm/dd hh:mm:ss
    //QUE product version user host “isv_def” count server_handle “project” “requested product” “requested version” mm/dd hh:mm:ss.tenths_of_msec
    //QUE product version user host “isv_def” count server_handle hh:mm
    //0   1       2       3    4     5          6     7             8            9                     10                   11    12
    [Column("Product", Order = 11)]
    public string Product { get; private set; } = string.Empty;

    [Column("Version", Order = 12)]
    public string Version { get; private set; } = string.Empty;

    [Column("Product Version", Order = 13)]
    public string ProductVersion { get => Product + " " + Version; }

    [Column("User", Order = 21)]
    public string User { get; private set; } = string.Empty;

    [Column("Host", Order = 22)]
    public string Host { get; private set; } = string.Empty;

    [Column("User@Host", Order = 23)]
    public string UserHost { get => User + "@" + Host; }

    [Column("Isv Def", Order = 101)]
    public string IsvDef { get; private set; } = string.Empty;

    [Column("Count", Order = 102)]
    public int Count { get; private set; } = -1;

    [Column("Server Handle", Order = 103)]
    public string HandleServer { get; private set; } = string.Empty;

    [Column("Project", Order = 104)]
    public string Project { get; private set; } = string.Empty;

    [Column("Requested Product", Order = 105)]
    public string RequestedProduct { get; private set; } = string.Empty;

    [Column("Requested Version", Order = 108)]
    public string RequestedVersion { get; private set; } = string.Empty;
    //
}
