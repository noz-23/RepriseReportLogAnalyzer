/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using Microsoft.Win32;
using RepriseReportLogAnalyzer.Extensions;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Managers;
using RepriseReportLogAnalyzer.Windows;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RepriseReportLogAnalyzer.Controls;

/// <summary>
/// AnalysisControl.xaml の相互作用ロジック
/// </summary>
public partial class AnalysisControl : UserControl
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public AnalysisControl()
    {
        InitializeComponent();

        AnalysisManager.Instance.SetProgressCount(_progressCount);
    }

    /// <summary>
    /// 処理内容
    /// </summary>
    private string _resultTitle = string.Empty;

    private DateTime _startDateTime =DateTime.Now;

    /// <summary>
    /// レポートログ を開く
    /// </summary>
    /// <param name="sender_"></param>
    /// <param name="e_"></param>
    private void _openClick(object sender_, RoutedEventArgs e_)
    {
        var dlg = new OpenFileDialog()
        {
            Title = "Please Select Reprise Report Log Files",
            Filter = "Reprise Report Log|*.*",
            Multiselect = true
        };
        if (dlg.ShowDialog() == true)
        {
            var list = dlg.FileNames.ToList();
            list.Sort();
            list.ForEach(path_ => _dataGrid.Items.Add(path_));
        }
    }

    /// <summary>
    /// 解析開始
    /// </summary>
    /// <param name="sender_"></param>
    /// <param name="e_"></param>
    private async void _analysisClick(object sender_, RoutedEventArgs e_)
    {
        _buttonAnalysis.IsEnabled = false;
        _startDateTime = DateTime.Now;

        if (_dataGrid.Items.Count ==0)
        {
            return;
        }

        var list = new List<string>();
        foreach (string path_ in _dataGrid.Items)
        {
            list.Add(path_);
        }

        var win = new WaitWindow()
        {
            Run = async () => await AnalysisManager.Instance.Analysis(list),
            Owner = Application.Current.MainWindow
        };
        win.ShowDialog();

        _buttonAnalysis.IsEnabled = true;

        if (App.Current.MainWindow is MainWindow mainWindow)
        {
            await mainWindow._resultControl.SetDate();
            _textLabel.Text = $"Runing [{(DateTime.Now - _startDateTime):hh\\:mm\\:ss}]".Trim();
        }
    }

    /// <summary>
    /// ファイル項目の削除
    /// </summary>
    /// <param name="sender_"></param>
    /// <param name="e_"></param>
    private void _deleteClick(object sender_, RoutedEventArgs e_)
    {
        var select = _dataGrid.SelectedItem;

        _dataGrid.Items.Remove(select);
    }

    /// <summary>
    /// プログレスバーの情報更新
    /// </summary>
    /// <param name="count_">カウント</param>
    /// <param name="max_">最大数</param>
    /// <param name="str_">文字列</param>
    private void _progressCount(int count_, int max_, string str_ = "")
    {
        App.Current.Dispatcher.Invoke(() =>
        {
            if (count_ == 0)
            {
                // カウントが0の場合に最大数と文字列を更新
                _resultTitle = str_;
                _progressBar.Maximum = max_;
            }

            StringBuilder str = new StringBuilder("Runing ");
            str.Append($"{ _resultTitle} [{ (DateTime.Now - _startDateTime):hh\\:mm\\:ss}]");
            _textLabel.Text = str.ToString();

            _progressBar.Value = count_;
            _textProgress.Text = $"{count_} / {max_}";
        });
    }


    /// <summary>
    /// ドラッグアンドドロップ開始位置
    /// </summary>
    private System.Windows.Point _startPoint = new System.Windows.Point();
    /// <summary>
    /// マウスドラッグ
    /// </summary>
    /// <param name="sender_"></param>
    /// <param name="e_"></param>
    private void _mouseDown(object sender_, MouseButtonEventArgs e_)
    {
        _startPoint = e_.GetPosition(null);
    }

    /// <summary>
    /// 移動
    /// </summary>
    /// <param name="sender_"></param>
    /// <param name="e_"></param>
    private void _mouseMove(object sender_, System.Windows.Input.MouseEventArgs e_)
    {
        var nowPoint = e_.GetPosition(null);
        if (e_.LeftButton == MouseButtonState.Released == true)
        {
            return;
        }
        if (Math.Abs(nowPoint.X - _startPoint.X) < SystemParameters.MinimumHorizontalDragDistance)
        {
            return;
        }
        if (Math.Abs(nowPoint.Y - _startPoint.Y) < SystemParameters.MinimumVerticalDragDistance)
        {
            return;
        }

        if (_dataGrid.SelectedItem is string selectView)
        {
            LogFile.Instance.WriteLine($"selectView {selectView}");

            // 選択されたViewをセット
            DragDrop.DoDragDrop(_dataGrid, selectView, System.Windows.DragDropEffects.Move);
        }
    }

    /// <summary>
    /// ドロップ
    /// </summary>
    /// <param name="sender_"></param>
    /// <param name="e_"></param>
    private void _drop(object sender_, System.Windows.DragEventArgs e_)
    {
        if (e_.Data.GetData(typeof(string)) is string dragView)
        {
            // 選択したViewの取得
            var dropPositon = e_.GetPosition(_dataGrid);
            var hit = VisualTreeHelper.HitTest(_dataGrid, dropPositon);
            if (hit.VisualHit.GetParentOfType<ItemsControl>() is ItemsControl dropItem)
            {
                // ドロップ先のViewを取得
                var dropView = (dropItem.DataContext as string)?? _dataGrid.Items[_dataGrid.Items.Count-1].ToString();

                var listItem = new List<string>();
                foreach (var item in _dataGrid.Items)
                {
                    listItem.Add(item.ToString());
                }

                var oldIndex = listItem.IndexOf(dragView);
                var newIndex = listItem.IndexOf(dropView);

                var temp = _dataGrid.Items[newIndex];
                _dataGrid.Items[newIndex] = _dataGrid.Items[oldIndex];
                _dataGrid.Items[oldIndex] = temp;
            }
        }
    }

}
