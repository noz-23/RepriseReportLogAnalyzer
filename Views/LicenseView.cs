/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
namespace RepriseReportLogAnalyzer.Views;

/// <summary>
/// カウント系の表示
/// </summary>
internal class LicenseView :BaseView
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public LicenseView()
    {
    }

    /// <summary>
    /// チェック状態
    /// </summary>
    public bool IsChecked
    {
        get => _isChecked;
        set => _SetValue(ref _isChecked, value);
    }
    private bool _isChecked = true;

    /// <summary>
    /// メインの名称
    /// </summary>
    public string Name
    {
        get => _name;
        set => _SetValue(ref _name, value);
    }
    public string _name = string.Empty;

    /// <summary>
    /// カウント
    /// </summary>
    public int Count
    {
        get => _count;
        set => _SetValue(ref _count, value);
    }
    public int _count = 0;

    /// <summary>
    /// 最大値
    /// </summary>
    public int Have
    {
        get => _have;
        set => _SetValue(ref _have, value);
    }
    public int _have = 0;

    /// <summary>
    /// 期間
    /// </summary>
    public TimeSpan Duration
    {
        get => _duration;
        set => _SetValue(ref _duration, value);
    }
    public TimeSpan _duration = TimeSpan.Zero;

}
