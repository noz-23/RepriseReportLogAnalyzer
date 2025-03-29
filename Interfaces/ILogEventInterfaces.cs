/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Analyses;
using RepriseReportLogAnalyzer.Enums;

namespace RepriseReportLogAnalyzer.Interfaces;


public class ListStringLongPair : List<KeyValuePair<string, long>>;
public class ListStringStringPair : List<KeyValuePair<string, string>>;

internal interface ILogEventHost
{
    string Host { get; }
}
internal interface ILogEventUser
{
    string User { get; }
}

internal interface ILogEventUserHost : ILogEventUser, ILogEventHost
{
    string UserHost { get; }
}

internal interface ILogEventProduct
{
    string Product { get; }
    string Version { get; }
    string ProductVersion { get; }
}

//interface ILogEventCountCurrent : ILogEventProduct
//{
//    int CountCurrent { get; }
//}

internal interface ILogEventWhy
{
    StatusValue Why { get; }
}

internal interface IAnalysisOutputFile
{
    /// <summary>
    /// 書き出しヘッダー
    /// </summary>
    /// <param name="select_"></param>
    /// <returns></returns>
    string Header(long select_);
    /// <summary>
    /// テキストの書き出し
    /// </summary>
    /// <param name="path_"></param>
    /// <param name="select_"></param>
    Task WriteText(string path_, long select_);
    /// <summary>
    /// リスト化したヘッダー
    /// </summary>
    /// <param name="select_"></param>
    /// <returns></returns>
    ListStringStringPair ListHeader(long select_);
    /// <summary>
    /// リスト化したデータ
    /// </summary>
    /// <param name="select_"></param>
    /// <returns></returns>
    IEnumerable<List<string>> ListValue(long select_);
}

internal interface ILicenseCount
{
    /// <summary>
    /// ライセンスのカウント処理をするイベント
    /// </summary>
    /// <param name="listCount_"></param>
    /// <returns></returns>
    bool SetCount(IDictionary<string, AnalysisLicenseCount.LicenseCount> listCount_);
}