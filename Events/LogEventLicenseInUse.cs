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
    private bool _logEventLicenseInUse = Regist("INUSE", (l_) => new LogEventLicenseInUse(l_));
}

/// <summary>
/// license in use
/// </summary>
[Sort(15)][Table("TbLicenseInUse")]
internal sealed class LogEventLicenseInUse : LogEventBase, ILogEventUserHost, ILogEventProduct
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventLicenseInUse(string[] list_) : base()
    {
        // small
        // std
        // detailed
        Product = list_[1];
        Version = list_[2];
        Pool = list_[3];
        User = list_[4];
        Host = list_[5];
        IsvDef = list_[6];
        //
        Count = int.Parse(list_[7]);
        HandleServer = list_[8];
        HandleShare = list_[9];
        ProcessId = list_[10];

        EventDateTime = _GetDateTime(list_[11], list_[12]);
        LogFormat = LogFormat.NONE;
    }

    //license in use
    //INUSE product version pool# user host “isv_def” count server_handle share_handle process_id mm/dd hh:mm:ss
    //0     1       2       3     4    5     6          7     8             9            10         11    12
    [Sort(11)]
    [Column("Product")]
    public string Product { get; private set; } = string.Empty;

    [Sort(12)]
    [Column("Version")]
    public string Version { get; private set; } = string.Empty;

    [Sort(13)]
    [Column("Product Version")]
    public string ProductVersion { get => Product + " " + Version; }
    //
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
    [Column("Pool")]
    public string Pool { get; private set; } = string.Empty;

    [Sort(102)]
    [Column("Isv Def")]
    public string IsvDef { get; private set; } = string.Empty;

    [Sort(103)]
    [Column("Count")]
    public int Count { get; private set; } = -1;

    [Sort(104)]
    [Column("Server Handle")]
    public string HandleServer { get; private set; } = string.Empty;

    [Sort(105)]
    [Column("Share Handle")]
    public string HandleShare { get; private set; } = string.Empty;

    [Sort(106)]
    [Column("Process ID")]
    public string ProcessId { get; private set; } = string.Empty;
    //

}
