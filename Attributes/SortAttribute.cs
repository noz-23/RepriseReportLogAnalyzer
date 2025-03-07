/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
namespace RepriseReportLogAnalyzer.Attributes;

/// <summary>
/// リフレクションの順序用
/// </summary>

internal sealed class SortAttribute : Attribute
{
    /// <summary>
    /// 順序
    /// </summary>
    public int Sort = -1;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="sort_">順序</param>
    public SortAttribute(int sort_)
    {
        Sort = sort_;
    }
}
