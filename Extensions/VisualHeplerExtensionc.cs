/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2024 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using System.Windows;
using System.Windows.Media;

namespace RepriseReportLogAnalyzer.Extensions;

/// <summary>
/// ドラッグ&ドロップ で利用(表示位置からからアイテムの選択)
/// </summary>
public static class VisualHeplerExtensionc
{
    public static T? GetParentOfType<T>(this DependencyObject src) where T : DependencyObject
    {
        while (src != null)
        {
            if (src is T rtn)
            {
                return rtn;
            }
            src = VisualTreeHelper.GetParent(src);
        }

        return null;
    }
}
