using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Events;
using System;
using System.Runtime.CompilerServices;

namespace RepriseReportLogAnalyzer.Analyses
{
    internal class AnalysisStartShutdown
    {
        public AnalysisStartShutdown(LogEventStart start_, LogEventShutdown shutdown_)
        {
            _start = start_;
            _shutdown = shutdown_;
        }

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


        public LogEventStart EventStart() => _start;
        private readonly LogEventStart _start;

        public LogEventShutdown EventShutdown() => _shutdown;
        private readonly LogEventShutdown _shutdown;

        public static string HEADER { get => "Start Date Time,End Date Time,Duration"; }
        public override string ToString()
        {
            return $"{StartDateTime.ToString()},{ShutdownDateTime.ToString()},{Duration.ToString(@"d\.hh\:mm\:ss")}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsWithInRange(long number_)
        {
            if (number_ < _start.EventNumber)
            {
                return false;
            }

            if (number_ > (_shutdown?.EventNumber ?? LogEventBase.NowEventNumber))
            {
                return false;
            }
            return true;
        }
    }
}
