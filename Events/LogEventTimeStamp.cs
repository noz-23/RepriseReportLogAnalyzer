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
/// periodic timestamp
/// </summary>
[Sort(99)][Table("TbTimeStamp")]
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
        EventDateTime = DateTime.Parse(list_[0] + " " + list_[1]);
        LogFormat = LogFormat.NONE;
    }

    //periodic timestamp
    //mm/dd/yyyy hh:mm
    //0          1
    public bool SetCount(IDictionary<string, int> listCount_, IDictionary<string, int> listHave_, IDictionary<string, int> listOutIn_)
    {
        return true;
    }

}
