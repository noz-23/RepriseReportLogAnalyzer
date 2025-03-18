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
using System.ComponentModel.DataAnnotations.Schema;

namespace RepriseReportLogAnalyzer.Events;

/// <summary>
/// 文字列 と イベントの紐づけ登録
/// </summary>
internal sealed partial class LogEventRegist
{
    private bool _logEventRlmReportLogFormat = Regist("RLM", (l_) => new LogEventRlmReportLogFormat(l_));
}

/// <summary>
/// log file start
/// </summary>
[Sort(82)][Table("TbRlmReportLogFormat")]
internal sealed class LogEventRlmReportLogFormat : LogEventBase
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventRlmReportLogFormat(string[] list_) : base()
    {
        // small
        // std
        // detailed
        Version = list_[6];

        EventDateTime = NowDateTime;
        LogFormat = LogFormat.NONE;
    }

    //RLM Report Log Format d, version x.y authentication flag
    //0   1      2   3      4  5       6
    [Column("Version", Order =101)]
    public string Version { get; private set; } = string.Empty;
}
