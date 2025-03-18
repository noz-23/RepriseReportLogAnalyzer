﻿/*
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
    private bool _logEventLicenseTemporary = Regist("TEMP", (l_) => new LogEventLicenseTemporary(l_));
}

/// <summary>
/// Temporary license creation/removal
/// </summary>
[Sort(74)][Table("TbLicenseTemporary")]
internal sealed class LogEventLicenseTemporary : LogEventBase, ILogEventUserHost, ILogEventProduct
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventLicenseTemporary(string[] list_) : base()
    {
        // small
        // std
        // detailed
        switch (list_[1])
        {
            default:
            case "create": LicenseTemporary = LicenseTemporaryType.CREATE; break;
            case "remove": LicenseTemporary = LicenseTemporaryType.REMOVE; break;
            case "restart": LicenseTemporary = LicenseTemporaryType.RESTART; break;
            case "expired": LicenseTemporary = LicenseTemporaryType.EXPIRED; break;
        }

        Product = list_[2];
        Version = list_[3];
        LicensePool = list_[4];
        User = list_[5];
        Host = list_[6];
        IsvDef = list_[7];
        //
        ExpiredDate = list_[8];
        ExpiredTime = list_[9];
        HandleServer = list_[10];

        EventDateTime = _GetDateTime(list_[11], list_[12]);
        LogFormat = LogFormat.NONE;
    }

    //Temporary license creation/removal
    //TEMP[create | remove | restart | expired] product version license-pool user host “isv_def” expdate exptime server_handle mm/dd hh:mm:ss
    //0    1                                    2       3       4            5    6     7          8       9       10            11    12
    [Sort(11)]
    [Column("Product", Order =11)]
    public string Product { get; private set; } = string.Empty;

    [Sort(12)]
    [Column("Version", Order =12)]
    public string Version { get; private set; } = string.Empty;

    [Sort(13)]
    [Column("Product Version", Order =13)]
    public string ProductVersion { get => Product + " " + Version; }
    //
    [Sort(21)]
    [Column("User", Order =21)]
    public string User { get; private set; } = string.Empty;

    [Sort(22)]
    [Column("Host", Order =22)]
    public string Host { get; private set; } = string.Empty;

    [Sort(23)]
    [Column("User@Host", Order =23)]
    public string UserHost { get => User + "@" + Host; }

    [Sort(101)]
    [Column("License Temporary", Order =10)]
    public LicenseTemporaryType LicenseTemporary { get; private set; } = LicenseTemporaryType.CREATE;

    [Sort(102)]
    [Column("License Pool", Order =102)]
    public string LicensePool { get; private set; } = string.Empty;

    [Sort(103)]
    [Column("Isv Def", Order =103)]
    public string IsvDef { get; private set; } = string.Empty;
    //
    [Sort(104)]
    [Column("Expired Date", Order =104)]
    public string ExpiredDate { get; private set; } = string.Empty;

    [Sort(105)]
    [Column("Expired Time", Order =105)]
    public string ExpiredTime { get; private set; } = string.Empty;

    [Sort(106)]
    [Column("Server Handle", Order =106)]
    public string HandleServer { get; private set; } = string.Empty;
    //
}
