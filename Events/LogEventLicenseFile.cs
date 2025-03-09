/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Attributes;

namespace RepriseReportLogAnalyzer.Events;

/// <summary>
/// 文字列 と イベントの紐づけ登録
/// </summary>

internal sealed partial class LogEventRegist
{
    private bool _logEventLicenseFile = Regist("LICENSE", (l_) => new LogEventLicenseFile(l_));
}

/// <summary>
/// log file start
/// </summary>
[Sort(31)]
internal sealed class LogEventLicenseFile : LogEventBase
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventLicenseFile(string[] list_) : base()
    {
        FileName = list_[2];
        EventDateTime = NowDateTime;
    }

    //LICENSE FILE filename
    //0       1    2
    [Sort(101)]
    public string FileName { get; private set; } = string.Empty;
}
