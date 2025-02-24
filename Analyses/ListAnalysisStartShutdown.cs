using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Windows;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace RepriseReportLogAnalyzer.Analyses
{
    class ListAnalysisStartShutdown: List<AnalysisStartShutdown>
    {
        public ProgressCountDelegate? ProgressCount = null;


        public ListAnalysisStartShutdown()
        { 
        }

        public void Analysis(ReportLogAnalysis log_)
        {
            var listSkipNumber = new List<long>();
            foreach (var end in log_.ListEnd)
            {
                // ログのの切り替えの次のスタートはスキップ
                var start = log_.ListStart.Find(x_ => x_.EventNumber > end.EventNumber);
                if (start != null)
                {
                    listSkipNumber.Add(start.EventNumber);
                }
            }

            for (int i =0;i < log_.ListStart.Count;i++)
            {
                var start = log_.ListStart[i];
                if (listSkipNumber.Contains(start.EventNumber) == true)
                {
                    // スキップ
                    continue;
                }

                var shutdown = log_.ListShutdown.ToList().Find(down_ => down_.EventNumber > start.EventNumber);
                if (shutdown != null && (i<(log_.ListStart.Count-1)))
                {
                    var nextStart = log_.ListStart[i + 1];
                    if (shutdown.EventNumber>nextStart.EventNumber)
                    {
                        shutdown = new LogEventShutdown(nextStart);
                    }
                }


                this.Add(new AnalysisStartShutdown(start, shutdown));
            }
            /*
            */

        }

        public void WriteText(string path_)
        {
            var list = new List<string>();
            list.Add(AnalysisStartShutdown.HEADER);
            list.AddRange(this.Select(x_ => x_.ToString()));
            File.WriteAllLines(path_, list, Encoding.UTF8);
        }
    }
}
