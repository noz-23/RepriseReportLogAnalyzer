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
    private bool _logEventLicenseDenial = Regist("DENY", (l_) => new LogEventLicenseDenial(l_));
}

/// <summary>
/// license denial
/// </summary>
[Sort(13)]
[Table("TbLicenseDenial")]
internal sealed class LogEventLicenseDenial : LogEventBase, ILogEventUserHost, ILogEventProduct, ILogEventWhy
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventLicenseDenial(string[] list_) : base()
    {
        // small
        // std
        // detailed
        Product = list_[_INDEX_PRODUCT];
        Version = list_[_INDEX_VERSION];
        User = list_[_INDEX_USER];
        Host = list_[_INDEX_HOST];
        IsvDef = list_[_INDEX_ISV_DEF];
        //
        Count = int.Parse(list_[_INDEX_COUNT], CultureInfo.InvariantCulture);
        Why = (StatusValue)int.Parse(list_[_INDEX_WHY], CultureInfo.InvariantCulture);
        LastAttempt = list_[_INDEX_LAST_ATTEMPT];
        ProcessId = list_[_INDEX_PROCESS_ID];

        EventDateTime = _GetDateTime(list_[_INDEX_DATE], list_[_INDEX_TIME]);
        LogFormat = (list_[_INDEX_TIME].Contains('.') == true) ? LogFormat.DETAILED : LogFormat.STANDARD;
    }

    //license denial
    //DENY product version user host “isv_def” count why last_attempt pid mm/dd hh:mm
    //DENY product version user host “isv_def” count why last_attempt pid mm/dd hh:mm:ss.tenths_of_msec
    //0    1       2       3    4     5          6     7   8            9   10    11
    private const int _INDEX_PRODUCT = 1;
    private const int _INDEX_VERSION = 2;
    private const int _INDEX_USER = 3;
    private const int _INDEX_HOST = 4;
    private const int _INDEX_ISV_DEF = 5;
    private const int _INDEX_COUNT = 6;
    private const int _INDEX_WHY = 7;
    private const int _INDEX_LAST_ATTEMPT = 8;
    private const int _INDEX_PROCESS_ID = 9;
    private const int _INDEX_DATE = 10;
    private const int _INDEX_TIME = 11;
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
    [Column("Isv Def", Order = 101, TypeName = "TEXT")]
    public string IsvDef { get; private set; } = string.Empty;

    [Column("Count", Order = 102, TypeName = "INTEGER")]
    public int Count { get; private set; } = -1;

    [Column("Why", Order = 103, TypeName = "INTEGER")]
    public StatusValue Why { get; private set; } = StatusValue.Success;

    [Column("Last Attempt", Order = 104, TypeName = "TEXT")]
    public string LastAttempt { get; private set; } = string.Empty;

    [Column("Process ID", Order = 105, TypeName = "TEXT")]
    public string ProcessId { get; private set; } = string.Empty;
    //

}
