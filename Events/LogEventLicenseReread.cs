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
    private bool _logEventLicenseReread = Regist("REREAD", (l_) => new LogEventLicenseReread(l_));
}

/// <summary>
/// server reread of license/option file
/// </summary>
[Sort(32)][Table("TbLicenseReread")]
internal sealed class LogEventLicenseReread : LogEventBase, ILogEventUserHost
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventLicenseReread(string[] list_) : base()
    {
        // small
        // std
        // detailed
        User = list_[1];
        Host = list_[2];

        EventDateTime = _GetDateTime(list_[3], list_[4]);
        LogFormat = LogFormat.NONE;
    }

    //server reread of license/option file
    //REREAD user host mm/dd hh:mm:ss
    //0      1    2    3     4
    //
    [Column("Host", Order =21)]
    public string User { get; private set; } = string.Empty;

    [Column("Host", Order =22)]
    public string Host { get; private set; } = string.Empty;

    [Column("User@Host", Order =23)]
    public string UserHost { get => User + "@" + Host; }
    //
}
