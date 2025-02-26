using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Windows;
using System.IO;
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
            var listSkipNumber = new SortedSet<long>();
            foreach (var end in log_.ListEnd)
            {
                // ログのの切り替えの次のスタートはスキップ
                var start = log_.ListStart.Find(x_ => x_.EventNumber > end.EventNumber);
                if (start != null)
                {
                    listSkipNumber.Add(start.EventNumber);
                }
            }

            AnalysisStartShutdown? last = null;
            foreach(var start in log_.ListStart)
            {
                LogEventBase? shutdown = log_.ListShutdown.ToList().Find(down_ => down_.EventNumber > start.EventNumber);

                var startShutdown = new AnalysisStartShutdown(start, shutdown);
                this.Add(startShutdown);
                //
                if (listSkipNumber.Contains(start.EventNumber) == true)
                {
                    startShutdown.JoinEvent().SetSkip();
                    last = startShutdown;
                    continue;
                }

                if (last != null)
                {
                    // スタートが2回続いた場合(シャットダウンログ等がない)
                    if (shutdown?.EventNumber == last.ShutdownNumber)
                    {
                        startShutdown.JoinEvent().SetSkip();
                    }
                }
                last = startShutdown;
            }
        }

        public IEnumerable<AnalysisStartShutdown> ListWithoutSkip()
        {
            return this.Where(x_ => x_.JoinEvent().IsSkip != JoinEventStartShutdown.SKIP);
        }

        public void WriteText(string path_, bool withoutSkip_ =false)
        {
            var list = new List<string>();
            list.Add(AnalysisStartShutdown.HEADER);

            var listData = (withoutSkip_==false) ? this :ListWithoutSkip();

            list.AddRange(listData.Select(x_ => x_.ToString()));
            File.WriteAllLines(path_, list, Encoding.UTF8);
        }
    }
}
