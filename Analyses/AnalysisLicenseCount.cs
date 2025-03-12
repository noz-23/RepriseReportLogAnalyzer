/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Data;
using RepriseReportLogAnalyzer.Events;

namespace RepriseReportLogAnalyzer.Analyses;

/// <summary>
/// ライセンスの利用数計算
/// </summary>
internal sealed class AnalysisLicenseCount:ToDataBase
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="eventBase_">イベント(時間)</param>
    /// <param name="countProduct_">利用数(アウト+1/イン-1)</param>
    /// <param name="maxProduct_">ライセンス数</param>
    /// <param name="outInProduct_">利用数(サーバ計算)</param>
    public AnalysisLicenseCount(LogEventBase eventBase_, SortedDictionary<string, int> countProduct_, SortedDictionary<string, int> maxProduct_, SortedDictionary<string, int> outInProduct_)
    {
        EventBase = eventBase_;
        CountProduct = new (countProduct_);
        MaxProduct = new (maxProduct_);
        OutInProduct = new (outInProduct_);
    }

    /// <summary>
    /// イベント(時間)
    /// </summary>
    public LogEventBase EventBase { get; private set; }

    /// <summary>
    /// 利用数(アウト+1/イン-1) Key:プロダクト
    /// </summary>
    public SortedDictionary<string, int> CountProduct { get; private set; }

    /// <summary>
    /// ライセンス数 Key:プロダクト
    /// </summary>
    public SortedDictionary<string, int> MaxProduct { get; private set; }

    /// <summary>
    /// 利用数(サーバ計算) Key:プロダクト
    /// </summary>
    public SortedDictionary<string, int> OutInProduct { get; private set; }


    public override string ToString() => string.Join(",", ListValue());
    public List<string> ListValue()
    {
        var rtn =new List<string>();

        rtn.Add($"{EventBase.EventDateTime.ToShortDateString()}");
        rtn.Add($"{EventBase.EventDateTime.ToShortTimeString()}");

        foreach (var key in CountProduct.Keys)
        {
            rtn.Add($"{CountProduct[key]}");
            rtn.Add($"{MaxProduct[key]}");
        }

        return rtn;

    }
}
