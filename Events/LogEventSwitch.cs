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
internal sealed class LogEventSwitch : LogEventBase
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventSwitch(string[] list_) : base()
    {
        Switch = (list_[1] == "to") ? SwitchType.TO : SwitchType.FROM;
        OldReportLogName = list_[2];
        EventDateTime = NowDateTime;
    }

    //log file end
    //SWITCH to filename(if an rlmswitch was done)
    //0      1  2

    //SWITCH from old-reportlog-name (new in v14.0, not authenticated)
    //0      1    2                  4
    [Sort(101)]
    public SwitchType Switch { get; private set; } = SwitchType.FROM;

    [Sort(102)]
    public string OldReportLogName { get; private set; } = string.Empty;
}
