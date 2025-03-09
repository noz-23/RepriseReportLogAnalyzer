/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using Dapper;
using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Files;
using System.Data.Common;
using System.Reflection;

namespace RLMLogReader.Extensions;

/// <summary>
/// DbConnection の Extension
/// </summary>
public static class DbConnectionExtension
{
    public static void CreateTable<T>(this DbConnection src_)
    {
        try
        {
            src_.Query(_createTabel<T>());
        }
        catch (Exception ex_)
        {
            LogFile.Instance.WriteLine(ex_.Message);
        }
    }

    public static void Insert<T>(this DbConnection src_, ICollection<T> list_, DbTransaction? tran_ = null)
    {

        LogFile.Instance.WriteLine($"{typeof(T).Name} [{list_.Count}]");

        try
        {
            src_.Execute(_insert<T>(), list_, tran_);
        }
        catch (Exception ex_)
        {
            LogFile.Instance.WriteLine(ex_.Message);
        }
    }


    private static string _createTabel<T>()
    {
        var listColunm = new List<string>();
        var listPropetyInfo = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)?.OrderBy(s_ => (Attribute.GetCustomAttribute(s_, typeof(SortAttribute)) as SortAttribute)?.Sort);

        listPropetyInfo?.ToList().ForEach(prop =>
        {
            listColunm.Add($"{prop.Name} {_getType(prop.PropertyType)}");
        });
        //
        var rtn = $"CREATE TABLE {typeof(T).Name} ({string.Join(",", listColunm)});";
        LogFile.Instance.WriteLine(rtn);

        return rtn;
    }

    private static string _getType(Type type_)
    {
        if (type_ == typeof(string))
        {
            return "TEXT";
        }
        if (type_ == typeof(int))
        {
            return "INTEGER";
        }
        if (type_ == typeof(long))
        {
            return "INTEGER";
        }
        if (type_ == typeof(float))
        {
            return "REAL";
        }
        if (type_ == typeof(double))
        {
            return "REAL";
        }
        return "TEXT";
    }

    private static string _insert<T>()
    {
        var listColunm = new List<string>();
        var listValue = new List<string>();
        var listPropetyInfo = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);


        listPropetyInfo?.ToList().ForEach(prop =>
        {
            listColunm.Add($"{prop.Name}");
            listValue.Add($"@{prop.Name}");
        });

        var rtn = $"INSERT INTO {typeof(T).Name} ({string.Join(",", listColunm)}) VALUES({string.Join(",", listValue)});";
        LogFile.Instance.WriteLine(rtn);
        return rtn;
    }

}
