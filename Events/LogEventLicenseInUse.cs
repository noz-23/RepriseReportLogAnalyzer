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
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

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
[Sort(15)]
[Table("TbLicenseInUse")]
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
        Product = list_[_INDEX_PRODUCT];
        Version = list_[_INDEX_VERSION];
        Pool = list_[_INDEX_POOL];
        User = list_[_INDEX_USER];
        Host = list_[_INDEX_HOST];
        IsvDef = list_[_INDEX_ISV_DEF];
        //
        Count = int.Parse(list_[_INDEX_COUNT], CultureInfo.InvariantCulture);
        HandleServer = list_[_INDEX_SERVER_HANDLE];
        HandleShare = list_[_INDEX_SHARE_HANDLE];
        ProcessId = list_[_INDEX_PROCESS_ID];

        EventDateTime = _GetDateTime(list_[_INDEX_DATE], list_[_INDEX_TIME]);
    }

    //license in use
    //INUSE product version pool# user host “isv_def” count server_handle share_handle process_id mm/dd hh:mm:ss
    //0     1       2       3     4    5     6          7     8             9            10         11    12
    private const int _INDEX_PRODUCT = 1;
    private const int _INDEX_VERSION = 2;
    private const int _INDEX_POOL = 3;
    private const int _INDEX_USER = 4;
    private const int _INDEX_HOST = 5;
    private const int _INDEX_ISV_DEF = 6;
    private const int _INDEX_COUNT = 7;
    private const int _INDEX_SERVER_HANDLE = 8;
    private const int _INDEX_SHARE_HANDLE = 9;
    private const int _INDEX_PROCESS_ID = 10;
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
    //
    [Column("Pool", Order = 101, TypeName = "TEXT")]
    public string Pool { get; private set; } = string.Empty;

    [Column("Isv Def", Order = 102, TypeName = "TEXT")]
    public string IsvDef { get; private set; } = string.Empty;

    [Column("Count", Order = 103, TypeName = "INTEGER")]
    public int Count { get; private set; } = -1;

    [Column("Server Handle", Order = 104, TypeName = "TEXT")]
    public string HandleServer { get; private set; } = string.Empty;

    [Column("Share Handle", Order = 105, TypeName = "TEXT")]
    public string HandleShare { get; private set; } = string.Empty;

    [Column("Process ID", Order = 106, TypeName = "TEXT")]
    public string ProcessId { get; private set; } = string.Empty;
    //

}
