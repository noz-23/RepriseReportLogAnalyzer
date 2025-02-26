using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Files;

namespace RepriseReportLogAnalyzer.Analyses
{
    class JoinEventStartShutdown
    {
        public JoinEventStartShutdown( LogEventStart start_, LogEventBase? shutdown_)
        {
            StartNumber = start_.EventNumber;
            if (shutdown_ is LogEventShutdown shutdown)
            {
                ShutdownNumber = shutdown.EventNumber;
            }
            else
            {
                LogFile.Instance.WriteLine($"{shutdown_?.EventNumber} {shutdown_?.GetType()}");
            }
        }

        public const string HEADER = "StartNumber,ShutdownNumber,IsSkip";

        public const long SKIP =1;
        public const long JOIN_SHUTDOWN =0;

        [ColumnSort(101)]
        public long StartNumber { get; private set; } = -1;

        [ColumnSort(102)]
        public long ShutdownNumber { get; private set; } = -1;

        /// <summary>
        /// -1  : Skip
        ///  0  : Shutdown
        /// >0  : Other Event
        /// </summary>
        [ColumnSort(102)]
        public long IsSkip { get; private set; } = JOIN_SHUTDOWN;

        public void SetSkip()
        {
            IsSkip = SKIP;
        }

        public override string ToString()
        {
            return $"{StartNumber},{ShutdownNumber},{IsSkip}";
        }
    }
}
