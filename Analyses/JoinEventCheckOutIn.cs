using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Files;
using System.Runtime.CompilerServices;

namespace RepriseReportLogAnalyzer.Analyses
{
    class JoinEventCheckOutIn
    {

        public JoinEventCheckOutIn(LogEventCheckOut checkOut_, LogEventBase? checkIn_)
        {
            CheckOutNumber = checkOut_.EventNumber;
            if (checkIn_ is LogEventCheckIn checkIn)
            {
                CheckInNumber = checkIn.EventNumber;
            }
            else if (checkIn_ is LogEventShutdown shutdown)
            {
                ShutdownNumber = shutdown.EventNumber;
            }
            else
            {
                LogFile.Instance.WriteLine($"{checkIn_?.EventNumber} {checkIn_?.GetType()}");
            }
            _checkIn = checkIn_;
        }

        public const string HEADER = "CheckOut,CheckIn,Shutdown,Duplication";

        public const long NO_DUPLICATION_ = 0;
        public const long DUPLICATION = -1;

        [ColumnSort(101)]
        public long CheckOutNumber { get; private set; } = -1;

        [ColumnSort(102)]
        public long CheckInNumber { get; private set; } = -1;

        [ColumnSort(103)]
        public long ShutdownNumber { get; private set; } = -1;


        /// <summary>
        /// -1  : Duplication
        ///  0  : Use CheckInNumber or ShutdownNumber;
        /// >0  : Use DuplicationNumber
        /// </summary>
        [ColumnSort(104)]
        public long DuplicationNumber { get; private set; } = NO_DUPLICATION_;

        private LogEventBase? _checkIn=null;
        public LogEventBase? CheckIn() => _checkIn;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetDuplication()
        {
            DuplicationNumber  = DUPLICATION;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetDuplication(LogEventBase? checkIn_)
        {
            _checkIn = checkIn_;
            DuplicationNumber = _checkIn?.EventNumber?? NO_DUPLICATION_;
        }

        public override string ToString()
        {
            return $"{CheckOutNumber},{CheckInNumber},{ShutdownNumber},{DuplicationNumber}";
        }
    }
}
