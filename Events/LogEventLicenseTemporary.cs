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
    private bool _logEventLicenseTemporary = Regist("TEMP", (l_) => new LogEventLicenseTemporary(l_));
}

/// <summary>
/// Temporary license creation/removal
/// </summary>
[Sort(74)]
[Table("TbLicenseTemporary")]
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
        switch (list_[_INDEX_LICENSE_TYPE])
        {
            default:
            case "create": LicenseTemporary = LicenseTemporaryType.CREATE; break;
            case "remove": LicenseTemporary = LicenseTemporaryType.REMOVE; break;
            case "restart": LicenseTemporary = LicenseTemporaryType.RESTART; break;
            case "expired": LicenseTemporary = LicenseTemporaryType.EXPIRED; break;
        }

        Product = list_[_INDEX_PRODUCT];
        Version = list_[_INDEX_VERSION];
        LicensePool = list_[_INDEX_LICENSE_POOL];
        User = list_[_INDEX_USER];
        Host = list_[_INDEX_HOST];
        IsvDef = list_[_INDEX_ISV_DEF];
        //
        ExpiredDate = list_[_INDEX_EXPIRED_DATE];
        ExpiredTime = list_[_INDEX_EXPIRED_TIME];
        HandleServer = list_[_INDEX_SERVER_HANDLE];

        EventDateTime = _GetDateTime(list_[_INDEX_DATE], list_[_INDEX_DATE]);
    }

    //Temporary license creation/removal
    //TEMP[create | remove | restart | expired] product version license-pool user host “isv_def” expdate exptime server_handle mm/dd hh:mm:ss
    //0    1                                    2       3       4            5    6     7          8       9       10            11    12
    private const int _INDEX_LICENSE_TYPE = 1;
    private const int _INDEX_PRODUCT = 2;
    private const int _INDEX_VERSION = 3;
    private const int _INDEX_LICENSE_POOL = 4;
    private const int _INDEX_USER = 5;
    private const int _INDEX_HOST = 6;
    private const int _INDEX_ISV_DEF = 7;
    private const int _INDEX_EXPIRED_DATE = 8;
    private const int _INDEX_EXPIRED_TIME = 9;
    private const int _INDEX_SERVER_HANDLE = 10;
    private const int _INDEX_DATE = 11;
    private const int _INDEX_TIME = 12;
    //

    [Column("Product", Order = 11, TypeName = "TEXT")]
    public string Product { get; private set; } = string.Empty;

    [Column("Version", Order = 12, TypeName = "TEXT")]
    public string Version { get; private set; } = string.Empty;

    [Column("Product Version", Order = 13, TypeName = "TEXT")]
    public string ProductVersion { get => Product + " " + Version; }
    //
    [Column("User", Order = 21, TypeName = "TEXT")]
    public string User { get; private set; } = string.Empty;

    [Column("Host", Order = 22, TypeName = "TEXT")]
    public string Host { get; private set; } = string.Empty;

    [Column("User@Host", Order = 23, TypeName = "TEXT")]
    public string UserHost { get => User + "@" + Host; }

    [Column("License Temporary", Order = 10, TypeName = "INTEGER")]
    public LicenseTemporaryType LicenseTemporary { get; private set; } = LicenseTemporaryType.CREATE;

    [Column("License Pool", Order = 102, TypeName = "TEXT")]
    public string LicensePool { get; private set; } = string.Empty;

    [Column("Isv Def", Order = 103, TypeName = "TEXT")]
    public string IsvDef { get; private set; } = string.Empty;
    //
    [Column("Expired Date", Order = 104, TypeName = "TEXT")]
    public string ExpiredDate { get; private set; } = string.Empty;

    [Column("Expired Time", Order = 105, TypeName = "TEXT")]
    public string ExpiredTime { get; private set; } = string.Empty;

    [Column("Server Handle", Order = 106, TypeName = "TEXT")]
    public string HandleServer { get; private set; } = string.Empty;
    //
}
