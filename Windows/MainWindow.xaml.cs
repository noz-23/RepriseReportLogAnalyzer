using Microsoft.Win32;
using RepriseReportLogAnalyzer.Analyses;
using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Files;
using System.Windows;
using RepriseReportLogAnalyzer.Managers;


namespace RepriseReportLogAnalyzer.Windows
{
    public delegate void ProgressCountDelegate( int count_,int max_, string str_="");

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}

