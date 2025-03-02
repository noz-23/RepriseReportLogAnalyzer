using RepriseReportLogAnalyzer.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepriseReportLogAnalyzer.Analyses
{
    class AnalysisLicenseCount
    {
        public AnalysisLicenseCount(LogEventBase eventBase_, Dictionary<string, int> countProduct_, Dictionary<string, int> maxProduct_, Dictionary<string, int> outInProduct_)
        {
            EventBase = eventBase_;
            CountProduct = new Dictionary<string, int>(countProduct_);
            MaxProduct = new Dictionary<string, int>(maxProduct_);
            OutInProduct = new Dictionary<string, int>(outInProduct_);
        }

        public LogEventBase EventBase{get;private set;}
        public Dictionary<string, int> CountProduct { get; private set; }
        public Dictionary<string, int> MaxProduct { get; private set; }
        public Dictionary<string, int> OutInProduct { get; private set; }
    }
}
