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
    [Column("Product", Order =11)]
    public string Product { get; private set; } = string.Empty;

    [Column("Version", Order =12)]
    public string Version { get; private set; } = string.Empty;

    [Column("Product Version", Order =13)]
    public string ProductVersion { get => Product + " " + Version; }
    //
    [Column("User", Order =21)]
    public string User { get; private set; } = string.Empty;

    [Column("Host", Order =22)]
    public string Host { get; private set; } = string.Empty;

    [Column("User@Host", Order =23)]
    public string UserHost { get => User + "@" + Host; }
    //
    [Column("Pool", Order =101)]
    public string Pool { get; private set; } = string.Empty;

    [Column("Isv Def", Order =102)]
    public string IsvDef { get; private set; } = string.Empty;

    [Column("Count", Order =103)]
    public int Count { get; private set; } = -1;

    [Column("Server Handle", Order =104)]
    public string HandleServer { get; private set; } = string.Empty;

    [Column("Share Handle", Order =105)]
    public string HandleShare { get; private set; } = string.Empty;

    [Column("Process ID", Order =106)]
    public string ProcessId { get; private set; } = string.Empty;
    //

}
