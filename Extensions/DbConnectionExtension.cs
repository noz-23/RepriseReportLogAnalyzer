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
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;

namespace RepriseReportLogAnalyzer.Extensions;

/// <summary>
/// DbConnection の Extension
/// </summary>
public static class DbConnectionExtension
{
    /// <summary>
    /// 単一/Summy テーブル
    /// </summary>
    private static readonly string _TABLE_SUMMY = "TbSummy";

    /// <summary>
    /// データベースにテーブル作成(単一/Summy)
    /// </summary>
    /// <param name="src_"></param>
    /// <param name="name_"></param>
    public static void CreateTable(this DbConnection src, string name)
    {
        var table = _TABLE_SUMMY + name;
        string query = $"CREATE TABLE '{table}' ('{name}' TEXT);";

        LogFile.Instance.WriteLine($"[{query}]");

        try
        {
            src.Execute(query);
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
    public static void CreateTable(this DbConnection src, Type classType, ListStringStringPair? list = null)
    {
        list ??= ToDataBase.ListHeader(classType);

        LogFile.Instance.WriteLine($"[{classType.Name}] [{list.Count}]");

        try
        {
            src.Execute(_createTabel(classType, list));
        }
        catch (Exception ex_)
        {
            LogFile.Instance.WriteLine(ex_.Message);
        }
    }

    /// <summary>
    /// ータベースにデータ挿入(単一/Summy)
    /// </summary>
    /// <param name="src_"></param>
    /// <param name="listValue_"></param>
    /// <param name="tran_"></param>
    public static void Insert(this DbConnection src, string name, IEnumerable<string> listValue)
    {
        var table = _TABLE_SUMMY + name;
        var query = $"INSERT INTO '{table}' ('{name}') VALUES('{string.Join("'),('", listValue)}');";

        LogFile.Instance.WriteLine($"[{query}]");

        if (listValue.Any() == false)
        {
            return;
        }

        foreach (var lv in listValue)
        {
            try
            {
                src.Execute(query, null);
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
    public static void Insert(this DbConnection src, Type classType, string header, IEnumerable<List<string>> listValue, DbTransaction? tran = null)
    {
        LogFile.Instance.WriteLine($"[{classType.Name}] [{header}] [{listValue.Count()}]");

        if (listValue.Any() == false)
        {
            return;
        }

        foreach (var lv in listValue)
        {
            try
            {
                src.Execute(_insert(classType, header, lv), null, tran);
            }
            catch (Exception ex_)
            {
                LogFile.Instance.WriteLine(ex_.Message);
            }
        }
    }

    private static string _tableName(Type classType_) => classType_.GetAttribute<TableAttribute>()?.Name ?? classType_.Name;

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
