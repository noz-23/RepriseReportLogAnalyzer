/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Attributes;
using System.Reflection;

namespace RepriseReportLogAnalyzer.Extensions
{
    internal static class MemberInfoExtension
    {
        public static T? GetAttribute<T>(this MemberInfo src_) where T : Attribute => (Attribute.GetCustomAttribute(src_, typeof(T)) as T);
        public static int Sort(this MemberInfo src_) => src_?.GetAttribute<SortAttribute>()?.Sort ?? -1;
    }



}
