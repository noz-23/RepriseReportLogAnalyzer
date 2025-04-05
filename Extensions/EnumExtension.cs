/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using System.ComponentModel;

namespace RepriseReportLogAnalyzer.Extensions;

public static class EnumExtension
{
    /// <summary>
    /// https://www.sejuku.net/blog/42539
    /// 説明の取得
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="src"></param>
    /// <returns></returns>
    public static string Description<T>(this T src) where T : Enum
    {
        var fieldInfo = typeof(T).GetField(src.ToString());
        if (fieldInfo == null)
        {
            return src.ToString();
        }
        return fieldInfo.GetAttribute<DescriptionAttribute>()?.Description ?? string.Empty;
    }
}
