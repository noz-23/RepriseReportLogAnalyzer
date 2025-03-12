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
using RepriseReportLogAnalyzer.Data;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Interfaces;
using System.Data.Common;
using System.Reflection;
using System.Windows.Input;

namespace RLMLogReader.Extensions;

/// <summary>
/// DbConnection の Extension
/// </summary>
public static class DbConnectionExtension
{
    //public static void CreateTable<T>(this DbConnection src_) => CreateTable(src_, typeof(T));
    //{
    //    try
    //    {
    //        src_.Query(_createTabel<T>());
    //    }
    //    catch (Exception ex_)
    //    {
    //        LogFile.Instance.WriteLine(ex_.Message);
    //    }
    //}

    public static void CreateTable(this DbConnection src_, Type classType_, ListStringStringPair ?list_ =null)
    {
        try
        {
            list_ ??= ToDataBase.ListHeader(classType_);
            src_.Execute(_createTabel(classType_, list_));
        }
        catch (Exception ex_)
        {
            LogFile.Instance.WriteLine(ex_.Message);
        }
    }

    public static void Insert(this DbConnection src_, Type classType_, string header_, IEnumerable<List<string>> listValue_, DbTransaction? tran_ = null)
    {

        //LogFile.Instance.WriteLine($"{header_} [{listValue_.Count()}]");

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

    //public static void Insert(this DbConnection src_, Type classType_, IEnumerable<string> listValue_, DbTransaction? tran_ = null)
    //{

    //    LogFile.Instance.WriteLine($"{classType_} [{listValue_.Count()}]");

    //    foreach (var v in listValue_)
    //    {
    //        try
    //        {
    //            src_.Execute(_insert(classType_, v), null, tran_);
    //        }
    //        catch (Exception ex_)
    //        {
    //            LogFile.Instance.WriteLine(ex_.Message);
    //        }
    //    }
    //}

    //public static void Insert<T>(this DbConnection src_, ICollection<T> list_, DbTransaction? tran_ = null)
    //{

    //    LogFile.Instance.WriteLine($"{typeof(T).Name} [{list_.Count}]");

    //    try
    //    {
    //        src_.Execute(_insert<T>(), list_, tran_);
    //    }
    //    catch (Exception ex_)
    //    {
    //        LogFile.Instance.WriteLine(ex_.Message);
    //    }
    //}


    //private static string _createTabel<T>()=> _createTabel(typeof(T));

    private static string _createTabel(Type classType_, ListStringStringPair list_)
    {
        var listColunm = new List<string>();
        //var listPropetyInfo = classType_.GetProperties(BindingFlags.Instance | BindingFlags.Public)?.OrderBy(s_ => (Attribute.GetCustomAttribute(s_, typeof(SortAttribute)) as SortAttribute)?.Sort);

        //listPropetyInfo?.ToList().ForEach(prop =>
        //{
        //    listColunm.Add($"{prop.Name} {_getType(prop.PropertyType)}");
        //});
        //

        list_.ForEach(column_ => listColunm.Add($"{column_.Key} {column_.Value}"));

        var rtn = $"CREATE TABLE {classType_.Name} ({string.Join(",", listColunm)});";
        LogFile.Instance.WriteLine(rtn);

        return rtn;
    }

    //private static string _insert<T>() => _insert(typeof(T));
    //{
    //    var listColunm = new List<string>();
    //    var listValue = new List<string>();
    //    var listPropetyInfo = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);


    //    listPropetyInfo?.ToList().ForEach(prop =>
    //    {
    //        listColunm.Add($"{prop.Name}");
    //        listValue.Add($"@{prop.Name}");
    //    });

    //    var rtn = $"INSERT INTO {typeof(T).Name} ({string.Join(",", listColunm)}) VALUES({string.Join(",", listValue)});";
    //    LogFile.Instance.WriteLine(rtn);
    //    return rtn;
    //}

    //private static string _insert(Type classType_)
    //{
    //    var listColunm = new List<string>();
    //    var listValue = new List<string>();
    //    var listPropetyInfo = classType_.GetProperties(BindingFlags.Instance | BindingFlags.Public);


    //    listPropetyInfo?.ToList().ForEach(prop =>
    //    {
    //        listColunm.Add($"{prop.Name}");
    //        listValue.Add($"@{prop.Name}");
    //    });

    //    var rtn = $"INSERT INTO {classType_.Name} ({string.Join(",", listColunm)}) VALUES({string.Join(",", listValue)});";
    //    LogFile.Instance.WriteLine(rtn);
    //    return rtn;
    //}

    //private static string _insert(Type classType_, string listValue)
    //{
    //    var listColunm = new List<string>();
    //    //var listValue = new List<string>();
    //    var listPropetyInfo = classType_.GetProperties(BindingFlags.Instance | BindingFlags.Public);


    //    listPropetyInfo?.ToList().ForEach(prop =>
    //    {
    //        listColunm.Add($"{prop.Name}");
    //        //listValue.Add($"@{prop.Name}");
    //    });

    //    var rtn = $"INSERT INTO {classType_.Name} ({string.Join(",", listColunm)}) VALUES({listValue});";
    //    LogFile.Instance.WriteLine(rtn);
    //    return rtn;
    //}

    private static string _insert(Type classType_, string header_, List<string> listValue)
    {
        //var listColunm = new List<string>();
        ////var listValue = new List<string>();
        //var listPropetyInfo = classType_.GetProperties(BindingFlags.Instance | BindingFlags.Public);


        //listPropetyInfo?.ToList().ForEach(prop =>
        //{
        //    listColunm.Add($"{prop.Name}");
        //    //listValue.Add($"@{prop.Name}");
        //});

        var rtn = $"INSERT INTO {classType_.Name} ({header_}) VALUES('{string.Join("','", listValue)}');";
        //LogFile.Instance.WriteLine(rtn);
        return rtn;
    }
}
