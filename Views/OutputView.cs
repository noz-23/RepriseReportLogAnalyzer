using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepriseReportLogAnalyzer.Views
{
    public class OutputView:BaseView
    {

        public OutputView(Type type_, string name_="")
        {
            Name =(string.IsNullOrEmpty(name_)==true) ? type_.Name:name_;
            ClassType=type_;
        }


        public bool IsChecked
        {
            get => _isChecked;
            set => _SetValue(ref _isChecked, value);
        }
        private bool _isChecked = false;

        public string Name
        {
            get => _name;
            set => _SetValue(ref _name, value);
        }
        public string _name = string.Empty;

        public Type ClassType { get; private set; }

    }
}
