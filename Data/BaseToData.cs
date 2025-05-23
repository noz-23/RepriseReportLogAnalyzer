﻿/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Enums;
using RepriseReportLogAnalyzer.Extensions;
using RepriseReportLogAnalyzer.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Reflection;

namespace RepriseReportLogAnalyzer.Data;


/// <summary>
/// 保存する関係ベース
/// </summary>
internal class BaseToData
{
    /// <summary>
    /// 出力しないデータ
    /// </summary>
    //public const int NO_OUPUT_DATA = 999;

    /// <summary>
    /// データの文字列指定変換デリゲード
    /// </summary>
    /// <param name="prop"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    private delegate string ToStringCallBack(PropertyInfo prop, object obj);

    // Column属性がないのは出力しない
    private static IEnumerable<PropertyInfo> _listPropetyInfo(Type classType_)=> classType_.GetProperties(BindingFlags.Instance | BindingFlags.Public)?.Where(s_ => s_.GetAttribute<ColumnAttribute>() != null).OrderBy(s_ => s_.GetAttribute<ColumnAttribute>()?.Order);
    
    /// <summary>
    /// ヘッダー
    /// </summary>
    /// <param name="classType_">子のクラス</param>
    /// <returns></returns>
    //public static string Header(Type classType_) => "'" + string.Join("','", ListHeader(classType_).Select(x_ => x_.Key)) + "'";
    public static string CsvHeader(Type classType_) => string.Join("','", ListCsvHeader(classType_));

    public static List<string> ListCsvHeader(Type classType_)
    {
        var rtn = new List<string>();
        foreach (var prop in _listPropetyInfo(classType_))
        {
            var name = prop.GetAttribute<ColumnAttribute>()?.Name ?? prop.Name;

            rtn.Add($"{name}");
        }

        return rtn;
    }
    /// <summary>
    /// リスト化したヘッダー
    /// </summary>
    /// <param name="classType_"></param>
    /// <returns></returns>
    //public static ListStringStringPair ListHeader(Type classType_)
    //{
    //    var rtn = new ListStringStringPair();
    //    // Column属性がないのは出力しない
    //    var listPropetyInfo = classType_.GetProperties(BindingFlags.Instance | BindingFlags.Public)?.Where(s_ => s_.GetAttribute<ColumnAttribute>()!=null).OrderBy(s_ => s_.GetAttribute<ColumnAttribute>()?.Order);

    //    if (listPropetyInfo != null)
    //    {
    //        foreach (var prop in listPropetyInfo)
    //        {
    //            var name = prop.GetAttribute<ColumnAttribute>()?.Name ?? prop.Name;
    //            var type = prop.GetAttribute<ColumnAttribute>()?.TypeName ?? "TEXT";

    //            rtn.Add(new($"{name}", $"{type}"));
    //        }
    //    }

    //    return rtn;
    //}
    public static List<string> ListCreateHeader(Type classType_)
    {
        var rtn = new List<string>();
        foreach (var prop in _listPropetyInfo(classType_))
        {
            var name = prop.GetAttribute<ColumnAttribute>()?.Name ?? prop.Name;
            var type = prop.GetAttribute<ColumnAttribute>()?.TypeName ?? "TEXT";

            rtn.Add($"'{name}' {type}");
        }

        return rtn;
    }

    public static List<string> ListInsertHeader(Type classType_)
    {
        var rtn = new List<string>();
        foreach (var prop in _listPropetyInfo(classType_))
        {
            var name = prop.GetAttribute<ColumnAttribute>()?.Name ?? prop.Name;

            rtn.Add($"'{name}'");
        }

        return rtn;
    }

    /// <summary>
    /// リスト化したデータ(文字)
    /// </summary>
    /// <param name="classTyep_"></param>
    /// <returns></returns>
    public virtual List<string> ListValue(Type? classType_ = null)
    {
        classType_ ??= GetType();
        //
        var rtn = new List<string>();
        //var listPropetyInfo = classType_.GetProperties(BindingFlags.Instance | BindingFlags.Public)?.Where(s_ => s_.GetAttribute<ColumnAttribute>()?.Order != NO_OUPUT_DATA).OrderBy(s_ => s_.GetAttribute<ColumnAttribute>()?.Order);
        //var listPropetyInfo = classType_.GetProperties(BindingFlags.Instance | BindingFlags.Public)?.Where(s_ => s_.GetAttribute<ColumnAttribute>()!=null).OrderBy(s_ => s_.GetAttribute<ColumnAttribute>()?.Order);

        //if ((listPropetyInfo?.Any() ?? false) == false)
        //{
        //    return rtn;
        //}

        //foreach (var prop in listPropetyInfo)
        foreach (var prop in _listPropetyInfo(classType_))
        {
            rtn.Add((_listToStringCallBack.TryGetValue(prop.PropertyType, out var callBack) == true) ? callBack(prop, this) : prop.GetValue(this)?.ToString() ?? "0");
        }
        return rtn;
    }

    /// <summary>
    /// データの型から文字列変換
    /// </summary>    
    private static Dictionary<Type, ToStringCallBack> _listToStringCallBack = new()
    {
        { typeof(DateTime),(prop_,obj_)=>( prop_.GetValue(obj_) is DateTime dateTime) ? dateTime.ToString(Properties.Settings.Default.FORMAT_DATE_TIME, CultureInfo.InvariantCulture) : prop_.GetValue(obj_).ToString() },
        { typeof(TimeSpan),(prop_,obj_)=>( prop_.GetValue(obj_) is TimeSpan timeSpan) ? timeSpan.ToString(Properties.Settings.Default.FORMAT_TIME_SPAN, CultureInfo.InvariantCulture) : prop_.GetValue(obj_).ToString()},
        { typeof(StatusValue),(prop_,obj_)=>
            {
                var val = prop_.GetValue(obj_)?.ToString() ?? "0";
                var num = Enum.Parse<StatusValue>(val);
                return $"{(long)num}";
            }
        }
    };

    /// <summary>
    /// データベース(SQL)に変換する場合のデータ型
    /// </summary>
    /// <param name="type_"></param>
    /// <returns></returns>
    //public static string GetDatabaseType(Type type_) => (_listDatabaseType.TryGetValue(type_, out var rtn) == true) ? rtn : "TEXT";

    /// <summary>
    /// データベース(SQL)に変換する場合のデータ型
    /// </summary>
    //private static Dictionary<Type, string> _listDatabaseType = new()
    //{
    //    { typeof(string), "TEXT" },
    //    { typeof(Enum), "INTEGER" },
    //    { typeof(StatusValue), "INTEGER" },
    //    { typeof(int), "INTEGER" },
    //    { typeof(short), "INTEGER" },
    //    { typeof(long), "INTEGER" },
    //    { typeof(float), "REAL" },
    //    { typeof(double), "REAL" },
    //};

    /// <summary>
    /// 文字列化
    /// </summary>
    /// <returns></returns>
    public override string ToString() => string.Join(",", ListValue(this.GetType()));

}
