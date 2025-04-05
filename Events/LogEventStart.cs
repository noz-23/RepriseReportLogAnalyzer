/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace RepriseReportLogAnalyzer.Events;

/// <summary>
/// 文字列 と イベントの紐づけ登録
/// </summary>
internal sealed partial class LogEventRegist
{
    private bool _logEventStart = Regist("START", (l_) => new LogEventStart(l_));
}

/// <summary>
/// log file start
/// </summary>

[Sort(1)]
[Table("TbStart")]
internal sealed class LogEventStart : LogEventBase
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventStart(string[] list_) : base()
    {
        // small
        // std
        // detailed
        HostName = list_[_INDEX_HOST_NAME];
        // 年の表示があるためそのまま時間へ変換
        EventDateTime = DateTime.Parse(list_[_INDEX_DATE] + " " + list_[_INDEX_TIME], CultureInfo.InvariantCulture);
    }


    //START hostname mm/dd/yyyy hh:mm
    //0     1        2          3
    private const int _INDEX_HOST_NAME = 1;
    private const int _INDEX_DATE = 2;
    private const int _INDEX_TIME = 3;
    //
    [Column("Host Name", Order = 101)]
    public string HostName { get; private set; } = string.Empty;
}
