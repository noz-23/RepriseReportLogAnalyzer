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
[Sort(2)]
[Table("TbShutdown")]
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
    [Column("User", Order = 21, TypeName = "TEXT")]
    public string User { get; private set; } = string.Empty;

    [Column("Host", Order = 22, TypeName = "TEXT")]
    public string Host { get; private set; } = string.Empty;
    //

    [Column("User@Host", Order = 23, TypeName = "TEXT")]
    public string UserHost { get => User + "@" + Host; }
    //

    /// <summary>
    /// ライセンスカウント処理
    /// </summary>
    /// <param name="listCount_"></param>
    /// <returns></returns>
    public bool SetCount(IDictionary<string, AnalysisLicenseCount.LicenseCount> listCount_)
    {
        foreach (var product in listCount_.Keys.ToList())
        {
            var data = listCount_[product];
            data.Count = 0;
            data.ServerHave = 0;
            data.CheckOutInCurrent = 0;
            listCount_[product] = data;

        }

        return true;
    }

}
