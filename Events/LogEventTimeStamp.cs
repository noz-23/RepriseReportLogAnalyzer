/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Analyses;
using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace RepriseReportLogAnalyzer.Events;

/// <summary>
/// periodic timestamp
/// </summary>
[Sort(99)]
[Table("TbTimeStamp")]
internal sealed class LogEventTimeStamp : LogEventBase, ILicenseCount
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventTimeStamp(string[] list_) : base()
    {
        // small
        // std
        // detailed
        // 年の表示があるためそのまま時間へ変換
        EventDateTime = DateTime.Parse(list_[_INDEX_DATE] + " " + list_[_INDEX_TIME], CultureInfo.InvariantCulture);
    }

    //periodic timestamp
    //mm/dd/yyyy hh:mm
    //0          1
    private const int _INDEX_DATE = 0;
    private const int _INDEX_TIME = 1;
    //
    public bool SetCount(IDictionary<string, AnalysisLicenseCount.LicenseCount> listCount_)
    {
        return true;
    }

}
