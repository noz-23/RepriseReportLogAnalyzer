using Microsoft.Win32;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RepriseReportLogAnalyzer.Controls
{
    /// <summary>
    /// AnalysisControl.xaml の相互作用ロジック
    /// </summary>
    public partial class AnalysisControl : UserControl
    {
        public AnalysisControl()
        {
            InitializeComponent();

            AnalysisManager.Instance.SetProgressCount(_progressCount);
        }
        private const string _ANALYSIS = "[File Read]";

        private string _resultTitle =string.Empty;

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

        private async void _analysisClick(object sender_, RoutedEventArgs e_)
        {
            _buttonAnalysis.IsEnabled = false;
            var outFolder = _textBoxFolder.Text;
            await Task.Run(() =>
            {
                int count = 0;
                int max = _dataGrid.Items.Count;
                _progressCount(0, max, _ANALYSIS);

                var analysis = new AnalysisReportLog();
                foreach (string path_ in _dataGrid.Items)
                {
                    LogFile.Instance.WriteLine($"LogAnalysis: {path_}");
                    analysis.StartAnalysis(path_);
                    _progressCount(++count, max);
                }
                analysis.EndAnalysis();
                //_calendarShow(analysis.ListDate);

                AnalysisManager.Instance.Analysis(analysis);
            });

            _buttonAnalysis.IsEnabled = true;
        }


        private void _deleteClick(object sender_, RoutedEventArgs e_)
        {
            var select = _dataGrid.SelectedItem;

            _dataGrid.Items.Remove(select);
        }

        private void _progressCount(int count_, int max_, string str_ = "")
        {
            if (count_ == 0)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    _resultTitle = str_;
                    _progressBar.Maximum = max_;
                });
            }
            App.Current.Dispatcher.Invoke(() =>
            {
                _progressBar.Value = count_;
                _textBlock.Text = $"{count_} / {max_} {_resultTitle}".Trim();
            });
        }
    }
}
