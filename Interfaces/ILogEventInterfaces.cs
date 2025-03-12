/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
namespace RepriseReportLogAnalyzer.Interfaces;


public class ListStringLongPair : List<KeyValuePair<string, long>>;
public class ListStringStringPair : List<KeyValuePair<string, string>>;

interface ILogEventHost
{
    string Host { get; }
}
interface ILogEventUser
{
    string User { get; }
}

interface ILogEventUserHost : ILogEventUser, ILogEventHost
{
    string UserHost { get; }
}

interface ILogEventProduct
{
    string Product { get; }
    string Version { get; }
    string ProductVersion { get; }
}

interface ILogEventCountCurrent : ILogEventProduct
{
    int CountCurrent { get; }
}

interface ILogEventWhy
{
    int Why { get; }
}

interface IAnalysisOutputFile
{
    string Header(long select_);
    void WriteText(string path_, long select_);

    ListStringStringPair ListHeader(long select_);
    IEnumerable<List<string>> ListValue(long select_);

    //IEnumerable<KeyValuePair<string, long>> ListSelect { get; }
}
