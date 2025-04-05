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
    private bool _logEventLogFileEnd = Regist("END", (l_) => new LogEventLogFileEnd(l_));
}

/// <summary>
/// log file end
/// </summary>
[Sort(75)]
[Table("TbLogFileEnd")]
internal sealed class LogEventLogFileEnd : LogEventBase
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventLogFileEnd(string[] list_) : base()
    {
        // small
        // std
        // detailed
        // 年の表示があるためそのまま時間へ変換
        EventDateTime = DateTime.Parse(list_[_INDEX_DATE] + " " + list_[_INDEX_TIME], CultureInfo.InvariantCulture);
    }

    //END mm/dd/yyyy hh:mm
    //0   1          2
    private const int _INDEX_DATE = 1;
    private const int _INDEX_TIME = 2;
}
