﻿/*
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
    private bool _logEventSwitch = Regist("SWITCH", (l_) => new LogEventSwitch(l_));
}

/// <summary>
/// log file start
/// log file end
/// </summary>
[Sort(81)]
[Table("TbSwitch")]
internal sealed class LogEventSwitch : LogEventBase
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventSwitch(string[] list_) : base()
    {
        // small
        // std
        // detailed
        Switch = (list_[_INDEX_SWITCH] == "to") ? SwitchType.TO : SwitchType.FROM;
        OldReportLogName = list_[_INDEX_OLD_NAME];
    }

    //log file end
    //SWITCH to filename(if an rlmswitch was done)
    //0      1  2

    //SWITCH from old-reportlog-name (new in v14.0, not authenticated)
    //0      1    2                  4
    private const int _INDEX_SWITCH = 1;
    private const int _INDEX_OLD_NAME = 2;
    //

    [Column("Switch", Order = 101, TypeName = "INTEGER")]
    public SwitchType Switch { get; private set; } = SwitchType.FROM;

    [Column("Old Report Log Name", Order = 102, TypeName = "TEXT")]
    public string OldReportLogName { get; private set; } = string.Empty;
}
