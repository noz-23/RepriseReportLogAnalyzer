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
using System.Globalization;
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

    private DateTime _startDateTime = DateTime.Now;

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

        LogFile.Instance.WriteLine($"Analysis Start");

        var win = new WaitWindow()
        {
            Run = async () => await AnalysisManager.Instance.Analysis(_dataGirdItems()),
            Owner = Application.Current.MainWindow
        };
        win.ShowDialog();

        _buttonAnalysis.IsEnabled = true;

        if (App.Current.MainWindow is MainWindow mainWindow)
        {
            await mainWindow._resultControl.SetDate();
        }
        LogFile.Instance.WriteLine($"Analysis End");
    }

    private List<string> _dataGirdItems()
    {
        var rtn = new List<string>();
        if (_dataGrid.Items.Count > 0)
        {
            foreach (string path_ in _dataGrid.Items)
            {
                rtn.Add(path_);
            }
        }
        return rtn;
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
        LogFile.Instance.WriteLine($"{select.ToString()}");
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
            str.Append(CultureInfo.InvariantCulture,$"{_resultTitle} [{(DateTime.Now - _startDateTime).ToString(Properties.Settings.Default.FORMAT_TIME_SPAN, CultureInfo.InvariantCulture)}]");
            _textLabel.Text = str.ToString();

            _progressBar.Value = count_;
            _textProgress.Text = $"{count_} / {max_}";
        });
    }


    /// <summary>
    /// ドラッグアンドドロップ開始位置
    /// </summary>
    private System.Windows.Point _startPoint;
    /// <summary>
    /// マウスドラッグ
    /// </summary>
    /// <param name="sender_"></param>
    /// <param name="e_"></param>
    private void _mouseDown(object sender_, MouseButtonEventArgs e_)
    {
        _startPoint = e_.GetPosition(null);
        LogFile.Instance.WriteLine($"{_startPoint.ToString()}");
    }

    /// <summary>
    /// 移動
    /// </summary>
    /// <param name="sender_"></param>
    /// <param name="e_"></param>
    private void _mouseMove(object sender_, System.Windows.Input.MouseEventArgs e_)
    {
        var nowPoint = e_.GetPosition(null);

        if ( e_.LeftButton == MouseButtonState.Released == true
          || Math.Abs(nowPoint.X - _startPoint.X) < SystemParameters.MinimumHorizontalDragDistance
          || Math.Abs(nowPoint.Y - _startPoint.Y) < SystemParameters.MinimumVerticalDragDistance)
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
                // 最終行の文字列
                var str = _dataGridLasItem();

                // ドロップ先のViewを取得
                var dropView = (dropItem.DataContext as string) ?? str;

                var listItem = new List<string>();
                foreach (var item in _dataGrid.Items)
                {
                    if (item is string add)
                    {
                        listItem.Add(add);
                    }
                }

                var oldIndex = listItem.IndexOf(dragView);
                var newIndex = (string.IsNullOrEmpty(dropView) == false) ? listItem.IndexOf(dropView) : _dataGrid.Items.Count;

                var temp = _dataGrid.Items[newIndex];
                _dataGrid.Items[newIndex] = _dataGrid.Items[oldIndex];
                _dataGrid.Items[oldIndex] = temp;


                LogFile.Instance.WriteLine($"{oldIndex} {newIndex}");
            }
        }
    }

    private string _dataGridLasItem()=> (_dataGrid.Items.Count > 0) ? _dataGrid.Items[_dataGrid.Items.Count - 1]?.ToString()??string.Empty : string.Empty;

}
