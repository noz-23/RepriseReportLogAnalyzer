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
        if (list_.Length <= _INDEX_SML_TIME)
        {
            _initSmall(list_);
        }
        else
        {
            _initStandardDetail(list_);
        }
    }

    private void _initSmall(string[] list_)
    {
        // small
        Product = list_[_INDEX_PRODUCT];
        Version = list_[_INDEX_VERSION];
        User = list_[_INDEX_USER];
        Host = list_[_INDEX_HOST];
        IsvDef = list_[_INDEX_ISV_DEF];
        Count = int.Parse(list_[_INDEX_COUNT], CultureInfo.InvariantCulture);
        //
        HandleServer = list_[_INDEX_SERVER_HANDLE];
        //
        //EventDateTime = DateTime.Parse(_NowDate + " " + list_[_INDEX_SML_TIME], CultureInfo.InvariantCulture);
        EventDateTime = _GetDateTime(list_[_INDEX_SML_TIME]);
        LogFormat = LogFormat.SMALL;
    }

    private void _initStandardDetail(string[] list_)
    {
        // std
        // detailed
        Product = list_[_INDEX_PRODUCT];
        Version = list_[_INDEX_VERSION];
        User = list_[_INDEX_USER];
        Host = list_[_INDEX_HOST];
        IsvDef = list_[_INDEX_ISV_DEF];
        //
        Count = int.Parse(list_[_INDEX_COUNT], CultureInfo.InvariantCulture);
        //
        HandleServer = list_[_INDEX_SERVER_HANDLE];
        //
        Project = list_[_INDEX_STD_PROJECT];
        RequestedProduct = list_[_INDEX_STD_REQUEST_PRODUCT];
        RequestedVersion = list_[_INDEX_STD_REQUEST_VERSION];
        //
        EventDateTime = _GetDateTime(list_[_INDEX_STD_DATE], list_[_INDEX_STD_TIME]);
        LogFormat = (list_[_INDEX_STD_TIME].Contains('.') == true) ? LogFormat.DETAILED : LogFormat.STANDARD;
    }

    //queue
    //QUE product version user host “isv_def” count server_handle hh:mm
    //QUE product version user host “isv_def” count server_handle “project” “requested product” “requested version” mm/dd hh:mm:ss
    //QUE product version user host “isv_def” count server_handle “project” “requested product” “requested version” mm/dd hh:mm:ss.tenths_of_msec
    //0   1       2       3    4     5          6     7             8            9                     10                   11    12
    private const int _INDEX_PRODUCT = 1;
    private const int _INDEX_VERSION = 2;
    private const int _INDEX_USER = 3;
    private const int _INDEX_HOST = 4;
    private const int _INDEX_ISV_DEF = 5;
    private const int _INDEX_COUNT = 6;
    private const int _INDEX_SERVER_HANDLE = 7;
    //
    private const int _INDEX_SML_TIME = 8;
    //
    private const int _INDEX_STD_PROJECT = 8;
    private const int _INDEX_STD_REQUEST_PRODUCT = 9;
    private const int _INDEX_STD_REQUEST_VERSION = 10;

    private const int _INDEX_STD_DATE = 11;
    private const int _INDEX_STD_TIME = 12;
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
