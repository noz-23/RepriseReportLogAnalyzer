using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Managers;
using System.Windows;

namespace RepriseReportLogAnalyzer;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private void _startup(object sender, StartupEventArgs e)
    {
        LogFile.Instance.Create();
        LogEventRegist.Instance.Create();
        AnalysisManager.Instance.Create();
        //SQLiteManager.Instance.Create();
    }
}

