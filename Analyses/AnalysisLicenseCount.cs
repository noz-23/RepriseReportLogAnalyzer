﻿/*
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
/// 
internal sealed class AnalysisLicenseCount : BaseToData
{
    /// <summary>
    /// ライセンスのカウント処理
    /// </summary>
    internal struct LicenseCount : ICloneable
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LicenseCount()
        {
            Count = 0;
            ServerHave = 0;
            CheckOutInCurrent = 0;
        }

        public LicenseCount(LicenseCount c_):this()
        {
            Count = c_.Count;
            ServerHave = c_.ServerHave;
            CheckOutInCurrent = c_.CheckOutInCurrent;
        }

        /// <summary>
        /// 集計カウント
        /// </summary>
        public int Count;
        /// <summary>
        /// サーバーの保有数
        /// </summary>
        public int ServerHave;
        /// <summary>
        /// サーバの貸出数(=集計カウント)
        /// </summary>
        public int CheckOutInCurrent;

        /// <summary>
        ///コピー処理
        /// </summary>
        /// <returns></returns>
        public object Clone() => new LicenseCount(this);
    }


    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="eventBase_">イベント(時間)</param>
    /// <param name="countProduct_">利用数(アウト+1/イン-1)</param>
    /// <param name="maxProduct_">ライセンス数</param>
    /// <param name="outInProduct_">利用数(サーバ計算)</param>
    public AnalysisLicenseCount(LogEventBase eventBase_, IDictionary<string, LicenseCount> listCount_)
    {
        EventBase = eventBase_;
        ListCount = new(listCount_);
    }

    /// <summary>
    /// イベント(時間)
    /// </summary>
    public LogEventBase EventBase { get; private set; }


    /// <summary>
    /// ライセンスカウント情報
    /// </summary>
    public SortedList<string, LicenseCount> ListCount { get; private set; }

    /// <summary>
    /// 文字列化
    /// </summary>
    /// <returns></returns>
    public override string ToString() => string.Join(",", ListValue());

    /// <summary>
    /// リスト化したデータ
    /// </summary>
    /// <returns></returns>
    public List<string> ListValue()
    {
        var rtn = new List<string>();

        rtn.Add($"{EventBase.EventDateTime.ToShortDateString()}");
        rtn.Add($"{EventBase.EventDateTime.ToShortTimeString()}");

        foreach (var key in ListCount.Keys)
        {
            rtn.Add($"{ListCount[key].Count}");
            rtn.Add($"{ListCount[key].ServerHave}");
            //rtn.Add($"{ListCount[key].CheckOutInCurrent}");
        }

        return rtn;
    }
}
