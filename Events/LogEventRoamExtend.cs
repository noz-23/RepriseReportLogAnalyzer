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

namespace RepriseReportLogAnalyzer.Events;

/// <summary>
/// 文字列 と イベントの紐づけ登録
/// </summary>
internal sealed partial class LogEventRegist
{
    private bool _logEventRoamExtend = Regist("ROAM_EXTEND", (l_) => new LogEventRoamExtend(l_));
}

/// <summary>
/// roam extend
/// </summary>
[Sort(73)]
[Table("TbRoamExtend")]
internal sealed class LogEventRoamExtend : LogEventBase, ILogEventUserHost, ILogEventProduct
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventRoamExtend(string[] list_) : base()
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
        DaysExtended = list_[_INDEX_DAYS_EXTENDED];
        HandleServer = list_[_INDEX_SERVER_HANDLE];
        ProcessId = list_[_INDEX_PROCESS_ID];
        //
        EventDateTime = _GetDateTime(list_[_INDEX_DATE], list_[_INDEX_DATE]);
    }

    //roam extend
    //ROAM_EXTEND product version pool# user host “isv_def” #days_extended server_handle process_id mm/dd hh:mm:ss
    //0           1       2       3     4    5     6          7              8             9          10    11
    private const int _INDEX_PRODUCT = 1;
    private const int _INDEX_VERSION = 2;
    private const int _INDEX_POOL = 3;
    private const int _INDEX_USER = 4;
    private const int _INDEX_HOST = 5;
    private const int _INDEX_ISV_DEF = 6;
    private const int _INDEX_DAYS_EXTENDED = 7;
    private const int _INDEX_SERVER_HANDLE = 8;
    private const int _INDEX_PROCESS_ID = 9;
    private const int _INDEX_DATE = 10;
    private const int _INDEX_TIME = 11;
    //
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
    //
    [Column("Pool", Order = 101)]
    public string Pool { get; private set; } = string.Empty;
    //
    [Column("Isv Def", Order = 102)]
    public string IsvDef { get; private set; } = string.Empty;

    [Column("Days Extended", Order = 103)]
    public string DaysExtended { get; private set; } = string.Empty;

    [Column("Server Handle", Order = 104)]
    public string HandleServer { get; private set; } = string.Empty;

    [Column("Process ID", Order = 105)]
    public string ProcessId { get; private set; } = string.Empty;
    //
}
