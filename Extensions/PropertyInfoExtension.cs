/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RepriseReportLogAnalyzer.Extensions
{
    internal static class PropertyInfoExtension
    {
        public static T? GetAttribute<T>(this PropertyInfo src_) where T : Attribute
        {
            return (Attribute.GetCustomAttribute(src_, typeof(T)) as T);

        }
    }
}
