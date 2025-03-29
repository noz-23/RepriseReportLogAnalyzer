﻿/*
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
    // https://www.sejuku.net/blog/42539
    public static string Description<T>(this T src) where T : Enum
    {
        //string rtn = string.Empty;

        var fieldInfo = typeof(T).GetField(src.ToString());
        if (fieldInfo == null)
        {
            return src.ToString();
        }

        //var attr = Attribute.GetCustomAttribute(fieldInfo, typeof());
        //if (attr != null)
        //{
        //    var descAttr = attr as DescriptionAttribute;
        //    rtn = descAttr?.Description ?? string.Empty;
        //}
        //return rtn;
        return fieldInfo.GetAttribute<DescriptionAttribute>()?.Description ?? string.Empty;
    }
}
