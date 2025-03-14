/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
namespace RepriseReportLogAnalyzer.Extensions;

/// <summary>
/// ICollectionのExtension
/// </summary>
internal static class CollectionExtension
{
    /// <summary>
    /// 範囲追加
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="src_"></param>
    /// <param name="items_"></param>
    public static void AddRange<T>(this ICollection<T> src_, IEnumerable<T> items_)
    {
        foreach (var item in items_)
        {
            src_.Add(item);
        }
    }
}
}
