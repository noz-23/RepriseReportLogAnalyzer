/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Extensions;
using RepriseReportLogAnalyzer.Interfaces;

namespace RepriseReportLogAnalyzer.Views;

/// <summary>
/// 出力系の表示
/// </summary>
sealed public class OutputView : BaseView
{

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="type_"></param>
    /// <param name="name_"></param>
    /// <param name="list_"></param>
    public OutputView(Type? type_, string name_ = "", ListStringLongPair? list_ = null) : this()
    {
        Name = (string.IsNullOrEmpty(name_) == true) ? type_?.Name ?? string.Empty : name_;
        ClassType = type_;

        list_?.ToList().ForEach(x_ => ListSelect.Add(x_));

        _NotifyPropertyChanged("SelectedIndex");
    }

    public OutputView() : base()
    {
        _isChecked = false;
        _name = string.Empty;
        _selectedIndex = -1;
        _selectedValue = -1;
        ClassType = null;
    }


    /// <summary>
    /// チェック状態
    /// </summary>
    public bool IsChecked
    {
        get => _isChecked;
        set => _SetValue(ref _isChecked, value);
    }
    private bool _isChecked;

    /// <summary>
    /// 基本の表示
    /// </summary>
    public string Name
    {
        get => _name;
        set => _SetValue(ref _name, value);
    }
    private string _name;

    /// <summary>
    /// コンボボックスの位置
    /// </summary>
    public int SelectedIndex
    {
        get => _selectedIndex;
        set => _SetValue(ref _selectedIndex, value);
    }
    private int _selectedIndex;

    /// <summary>
    /// コンボボックスの値
    /// </summary>
    public long SelectedValue
    {
        get => _selectedValue;
        set => _SetValue(ref _selectedValue, value);
    }
    private long _selectedValue = -1;

    /// <summary>
    /// クラスタイプ
    /// </summary>
    public Type? ClassType { get; private set; }

    /// <summary>
    /// コンボボックス表示内容
    /// </summary>
    public ListStringLongPair ListSelect { get; private set; } = new();

    /// <summary>
    /// 並び替え
    /// </summary>
    public int Sort { get => ClassType?.Sort() ?? -1; }

}
