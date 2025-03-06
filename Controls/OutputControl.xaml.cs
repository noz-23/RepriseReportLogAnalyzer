using Microsoft.Win32;
using RepriseReportLogAnalyzer.Managers;
using System.Windows;
using System.Windows.Controls;

namespace RepriseReportLogAnalyzer.Controls;

/// <summary>
/// OutputControl.xaml の相互作用ロジック
/// </summary>
public partial class OutputControl : UserControl
{
    public OutputControl()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 出力フォルダ 選択
    /// </summary>
    /// <param name="sender_"></param>
    /// <param name="e_"></param>
    private void _selectClick(object sender_, RoutedEventArgs e_)
    {
        var dlg = new OpenFolderDialog()
        {
            Title = "Please Select Output Folder",
            Multiselect = false
        };
        if (dlg.ShowDialog() == true)
        {
            _textBoxFolder.Text = dlg.FolderName;
        }
    }

    /// <summary>
    /// 出力 開始
    /// </summary>
    /// <param name="sender_"></param>
    /// <param name="e_"></param>
    private void _outputClick(object sender_, RoutedEventArgs e_)
    {
        if (string.IsNullOrEmpty(_textBoxFolder.Text) == false)
        {
            _textBoxFolder.IsEnabled = false;
            AnalysisManager.Instance.WriteText(_textBoxFolder.Text);
            _textBoxFolder.IsEnabled = true;
        }

    }
}
