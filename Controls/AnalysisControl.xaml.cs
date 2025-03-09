/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using Microsoft.Win32;
using RepriseReportLogAnalyzer.Managers;
using RepriseReportLogAnalyzer.Windows;
using System.Windows;
using System.Windows.Controls;

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
        //var outFolder = _textBoxFolder.Text;
        await Task.Run(() =>
        {
            //int count = 0;
            //int max = _dataGrid.Items.Count;
            //_progressCount(0, max, _ANALYSIS);

            //var analysis = new AnalysisReportLog();
            //foreach (string path_ in _dataGrid.Items)
            //{
            //    LogFile.Instance.WriteLine($"LogAnalysis: {path_}");
            //    analysis.StartAnalysis(path_);
            //    _progressCount(++count, max);
            //}
            //analysis.EndAnalysis();
            ////_calendarShow(analysis.ListDate);

            //AnalysisManager.Instance.Analysis(analysis);
            var list =new List<string>();
            foreach (string path_ in _dataGrid.Items)
            {
                list.Add(path_);
            }
            AnalysisManager.Instance.Analysis(list);
        });

        _buttonAnalysis.IsEnabled = true;

        if (App.Current.MainWindow is MainWindow mainWindow)
        {
            mainWindow._resultControl.SetDate();
            //mainWindow._tabControl._resultControl.SetData();
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
            _progressBar.Value = count_;
            _textBlock.Text = $"{count_} / {max_} {_resultTitle} [{(DateTime.Now- _startDateTime):hh\\:mm\\:ss}]".Trim();
        });
    }
}
