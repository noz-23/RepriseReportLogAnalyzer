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
    private bool _logEventRepProcessed = Regist("REPROCESSED", (l_) => new LogEventRepProcessed(l_));
}

/// <summary>
/// support for a product
/// </summary>
[Sort(72)]
[Table("TbRepProcessed")]
internal sealed class LogEventRepProcessed : LogEventBase
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventRepProcessed(string[] list_) : base()
    {
        // small
        // std
        // detailed
        Version = list_[3];

        EventDateTime = NowDateTime;
        LogFormat = LogFormat.NONE;
    }

    //REPROCESSED with rlmanon vx.y
    //0           1    2       3
    [Column("Version", Order = 101)]
    public string Version { get; private set; } = string.Empty;
}
