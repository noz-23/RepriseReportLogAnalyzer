using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepriseReportLogAnalyzer.Views
{
    class LicenseView
    {
        public LicenseView()
        {
        }

        public DateTime Date { get; set; } = DateTime.Now;
        public string Product { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public int Count { get; set; } = 0;

        public int Max { get; set; } = 0;
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;
    }
}
