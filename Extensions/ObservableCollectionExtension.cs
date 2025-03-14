/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Views;
using System.Collections.ObjectModel;

namespace RepriseReportLogAnalyzer.Extensions;

internal static class ObservableCollectionExtension
{
    internal static void AddRange<T>(this ObservableCollection<T> src_, ICollection<T> collection_)
    {
        collection_?.ToList().ForEach(item => src_.Add(item));
    }

    internal static void SetView(this ObservableCollection<LicenseView> src_, LicenseView view_)
    {
        var find = src_.ToList().Find(x_ => x_.Name == view_.Name);
        if (find == null)
        {
            src_.Add(view_);
        }
        else
        {
            find.Count = view_.Count;
            find.Max = view_.Max;
        }
    }
}
