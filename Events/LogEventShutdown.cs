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
    private bool _logEventShutdown = Regist("SHUTDOWN", (l_) => new LogEventShutdown(l_));
}

/// <summary>
/// server shutdown
/// </summary>
[Sort(2)][Table("TbShutdown")]
internal sealed class LogEventShutdown : LogEventBase, ILogEventUserHost, ILicenseCount
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventShutdown(string[] list_) : base()
    {
        // small
        // std
        // detailed
        User = list_[1];
        Host = list_[2];
        //
        EventDateTime = _GetDateTime(list_[3], list_[4]);
        LogFormat = LogFormat.NONE;
    }

    /// <summary>
    /// コンストラクタ
    /// ログ終了
    /// </summary>
    public LogEventShutdown()
    {
        EventNumber = NowEventNumber;
        EventDateTime = NowDateTime;
    }

    //server shutdown
    //SHUTDOWN user host mm/dd hh:mm:ss
    //0        1    2    3     4
    [Column("User", Order =21)]
    public string User { get; private set; } = string.Empty;

    [Column("Host", Order =22)]
    public string Host { get; private set; } = string.Empty;
    //

    [Column("User@Host", Order =23)]
    public string UserHost { get => User + "@" + Host; }
    //

    public bool SetCount(IDictionary<string, int> listCount_, IDictionary<string, int> listHave_, IDictionary<string, int> listOutIn_)
    {
        foreach (var product in listCount_.Keys.ToList())
        {
            listCount_[product] = 0;
        }
        foreach (var product in listHave_.Keys.ToList())
        {
            listHave_[product] = 0;
        }
        foreach (var product in listOutIn_.Keys.ToList())
        {
            listOutIn_[product] = 0;
        }
        return true;
    }

}
