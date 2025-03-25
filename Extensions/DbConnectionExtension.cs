/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using Dapper;
using RepriseReportLogAnalyzer.Data;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Interfaces;
using ScottPlot.MultiplotLayouts;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;

namespace RepriseReportLogAnalyzer.Extensions;

/// <summary>
/// DbConnection の Extension
/// </summary>
public static class DbConnectionExtension
{
    /// <summary>
    /// データベースにテーブル作成(単一)
    /// </summary>
    /// <param name="src_"></param>
    /// <param name="name_"></param>
    public static void CreateTable(this DbConnection src_, string name_)
    {
        var table ="Tb"+ name_;
        string query = $"CREATE TABLE '{table}' ('{name_}' TEXT);";

        LogFile.Instance.WriteLine($"[{query}]");

        try
        {
            src_.Execute(query);
        }
        catch (Exception ex_)
        {
            LogFile.Instance.WriteLine(ex_.Message);
        }
    }

    /// <summary>
    /// データベースにテーブル作成
    /// </summary>
    /// <param name="src_"></param>
    /// <param name="classType_"></param>
    /// <param name="list_"></param>
    public static void CreateTable(this DbConnection src_, Type classType_, ListStringStringPair ?list_ =null)
    {
        list_ ??= ToDataBase.ListHeader(classType_);

        LogFile.Instance.WriteLine($"[{classType_.Name}] [{list_.Count()}]");

        try
        {
            src_.Execute(_createTabel(classType_, list_));
        }
        catch (Exception ex_)
        {
            LogFile.Instance.WriteLine(ex_.Message);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="src_"></param>
    /// <param name="listValue_"></param>
    /// <param name="tran_"></param>
    public static void Insert(this DbConnection src_, string name_, IEnumerable<string> listValue_)
    {
        var table = "Tb" + name_;
        var query = $"INSERT INTO '{table}' ('{name_}') VALUES('{string.Join("') ('", listValue_)}');";

        LogFile.Instance.WriteLine($"[{query}]");

        if (listValue_.Any() == false)
        {
            return;
        }

        foreach (var lv in listValue_)
        {
            try
            {
                src_.Execute(query, null);
            }
            catch (Exception ex_)
            {
                LogFile.Instance.WriteLine(ex_.Message);
            }
        }
    }

    /// <summary>
    /// データベースにデータ挿入
    /// </summary>
    /// <param name="src_"></param>
    /// <param name="classType_"></param>
    /// <param name="header_"></param>
    /// <param name="listValue_"></param>
    /// <param name="tran_"></param>
    public static void Insert(this DbConnection src_, Type classType_, string header_, IEnumerable<List<string>> listValue_, DbTransaction? tran_ = null)
    {
        LogFile.Instance.WriteLine($"[{classType_.Name}] [{header_}] [{listValue_.Count()}]");

        if (listValue_.Any() == false)
        {
            return;
        }

        foreach (var lv in listValue_)
        {
            try
            {
                src_.Execute( _insert(classType_,header_, lv), null,tran_);
            }
            catch (Exception ex_)
            {
                LogFile.Instance.WriteLine(ex_.Message);
            }
        }
    }

    private static string _tableName(Type classType_) =>classType_.GetAttribute<TableAttribute>()?.Name?? classType_.Name;

    /// <summary>
    /// テーブル作成処理クエリ
    /// </summary>
    /// <param name="classType_"></param>
    /// <param name="list_"></param>
    /// <returns></returns>
    private static string _createTabel(Type classType_, ListStringStringPair list_)
    {
        var listColunm = new List<string>();
        list_.ForEach(column_ => listColunm.Add($"'{column_.Key}' {column_.Value}"));


        var rtn = $"CREATE TABLE {_tableName(classType_)} ({string.Join(",", listColunm)});";
        LogFile.Instance.WriteLine(rtn);
        return rtn;
    }

    /// <summary>
    /// データ挿入処理クエリ
    /// </summary>
    /// <param name="classType_"></param>
    /// <param name="header_"></param>
    /// <param name="listValue"></param>
    /// <returns></returns>
    private static string _insert(Type classType_, string header_, List<string> listValue)
    {
        var rtn = $"INSERT INTO {_tableName(classType_)} ({header_}) VALUES('{string.Join("','", listValue)}');";
        return rtn;
    }
}
