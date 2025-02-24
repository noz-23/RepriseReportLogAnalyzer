using Microsoft.Win32;
using RepriseReportLogAnalyzer.Files;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Analyses;


namespace RepriseReportLogAnalyzer.Windows
{
    public delegate void ProgressCountDelegate( int count_,int max_, string str_);

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

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
                list.ForEach(path_ => 
                {
                    _listBox.Items.Add(path_);
                });
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
                var analysis = new ReportLogAnalysis();
                foreach (string path_ in _listBox.Items)
                {
                    LogFile.Instance.WriteLine($"LogAnalysis: {path_}");
                    analysis.StartAnalysis(path_);
                }
                analysis.EndAnalysis();


                analysis.WriteSummy( @"\Summy.txt");

                analysis.WriteText<LogEventStart>(outFolder + @"\LogEventStart.csv");
                analysis.WriteText<LogEventLogFileEnd>(outFolder + @"\LogEventLogFileEnd.csv");
                analysis.WriteText<LogEventShutdown>(outFolder + @"\LogEventShutdown.csv");

                analysis.WriteText<LogEventCheckOut>(outFolder + @"\LogEventCheckOut.csv");
                analysis.WriteText<LogEventCheckIn>(outFolder + @"\LogEventCheckIn.csv");
                analysis.WriteText<LogEventLicenseDenial>(outFolder + @"\LogEventLicenseDenial.csv");
                analysis.WriteText<LogEventLicenseInUse>(outFolder + @"\LogEventLicenseInUse.csv");

                //analysis.ListRunning.WriteText(outFolder + @"\Runing.csv");
                var listAnalysisStartShutdown =new ListAnalysisStartShutdown();
                listAnalysisStartShutdown.ProgressCount = _progressCount;
                listAnalysisStartShutdown.Analysis(analysis);
                listAnalysisStartShutdown.WriteText(outFolder + @"\ListAnalysisStartShutdown.csv");

                //analysis.ListAnalysisCheckOutIn.WriteText(outFolder + @"\CheckOutIn.csv");
                var listAnalysisCheckOutIn = new ListAnalysisCheckOutIn();
                listAnalysisCheckOutIn.ProgressCount = _progressCount;
                listAnalysisCheckOutIn.Analysis(analysis, listAnalysisStartShutdown);
                listAnalysisCheckOutIn.WriteText(outFolder + @"\ListAnalysisCheckOutIn.csv");

                listAnalysisCheckOutIn.WriteText(outFolder + @"\ListAnalysisCheckOutInDuplication.csv", true);

                listAnalysisCheckOutIn.WriteDuplicationText(outFolder + @"\ListJoinEventCheckOutIn.csv");

                //analysis.ListAnalysisLicenseCount.WriteText(outFolder + @"\LicenseCount.csv");

                var listAnalysisLicenseCount = new ListAnalysisLicenseCount();
                listAnalysisLicenseCount.ProgressCount = _progressCount;
                listAnalysisLicenseCount.Analysis(analysis, listAnalysisCheckOutIn);
                listAnalysisLicenseCount.WriteText(outFolder + @"\ListAnalysisLicenseCount.csv");

                //analysis.ListAnalysisLicenseCount.WriteTimeSpanText(outFolder + @"\LicenseCountDate.csv", TimeSpan.TicksPerDay);
                //analysis.ListAnalysisLicenseCount.WriteTimeSpanText(outFolder + @"\LicenseCountHour.csv", TimeSpan.TicksPerHour);
                //analysis.ListAnalysisLicenseCount.WriteTimeSpanText(outFolder + @"\LicenseCount15Minute.csv", TimeSpan.TicksPerMinute * 15);

                /*
                var listUser = new ListAnalysisLicenseGroupDuration(ANALYSIS_GROUP.USER);
                listUser.ProgressCount = _progressCount;
                listUser.Analysis(analysis.ListUser, listAnalysisCheckOutIn);
                listUser.WriteText(outFolder + @"\DurationUser.csv");

                var listHost = new ListAnalysisLicenseGroupDuration(ANALYSIS_GROUP.HOST);
                listHost.ProgressCount = _progressCount;
                listHost.Analysis(analysis.ListHost, listAnalysisCheckOutIn);
                listHost.WriteText(outFolder + @"\DurationHost.csv");

                var listUserHost = new ListAnalysisLicenseGroupDuration(ANALYSIS_GROUP.USER_HOST);
                listUserHost.ProgressCount = _progressCount;
                listUserHost.Analysis( analysis.ListUserHost, listAnalysisCheckOutIn);
                listUserHost.WriteText(outFolder + @"\DurationUserHost.csv");
                */

            });

            _buttonAnalysis.IsEnabled = true;
        }


        private void _progressCount(int count_, int max_, string str_="")
        {
            if (count_ == 0)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    _progressBar.Maximum = max_;
                });
            }
            App.Current.Dispatcher.Invoke(() =>
            {
                _progressBar.Value = count_;
                _textBlock.Text = $"{count_} / {max_} {str_}".Trim();
            });
        }
    }
}

