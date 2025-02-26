using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Events;
using System.Runtime.CompilerServices;

namespace RepriseReportLogAnalyzer.Analyses
{
    internal class AnalysisStartShutdown
    {
        public AnalysisStartShutdown(LogEventStart start_, LogEventBase? shutdown_)
        {
            _start = start_;
            _shutdown = shutdown_;

            _joinEvent = new JoinEventStartShutdown(start_, shutdown_);
        }
        public const string HEADER = "Start Date Time,End Date Time,Duration";

        [ColumnSort(101)]
        public long StartNumber { get => _start.EventNumber; }
        [ColumnSort(102)]
        public long ShutdownNumber { get => _shutdown?.EventNumber ?? LogEventBase.NowEventNumber; }
        [ColumnSort(111)]
        public DateTime StartDateTime { get => _start.EventDateTime; }
        [ColumnSort(112)]
        public DateTime ShutdownDateTime { get => _shutdown?.EventDateTime ?? LogEventBase.NowDateTime; }
        [ColumnSort(113)]
        public TimeSpan Duration { get => (ShutdownDateTime - StartDateTime); }

        private readonly LogEventStart _start;
        private LogEventBase? _shutdown;
        private readonly JoinEventStartShutdown _joinEvent;

        public LogEventStart EventStart() => _start;
        public LogEventBase? EventShutdown() => _shutdown;
        public JoinEventStartShutdown JoinEvent() => _joinEvent;

        public long ShudownNumber() => _shutdown?.EventNumber ?? LogEventBase.NowEventNumber;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsWithInRange(long number_)
        {
            if (number_ < _start.EventNumber)
            {
                return false;
            }

            if (number_ > ShudownNumber())
            {
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            return $"{StartDateTime.ToString()},{ShutdownDateTime.ToString()},{Duration.ToString(@"d\.hh\:mm\:ss")}";
        }

    }
}
