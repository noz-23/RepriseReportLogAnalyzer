using Microsoft.Win32;
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
    /// OutputControl.xaml の相互作用ロジック
    /// </summary>
    public partial class OutputControl : UserControl
    {
        public OutputControl()
        {
            InitializeComponent();
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

    }
}
