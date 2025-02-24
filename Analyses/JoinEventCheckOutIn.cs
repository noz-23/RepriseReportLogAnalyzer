using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Files;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RepriseReportLogAnalyzer.Analyses
{
    class JoinEventCheckOutIn
    {
        public const long NO_DUPLICATION_ =0;
        public const long DUPLICATION = -1;

        //public JoinEventCheckOutIn(LogEventCheckOut checkOut_, LogEventCheckIn checkIn_)
        //{
        //    CheckOutNumber=checkOut_.EventNumber;
        //    CheckInNumber = checkIn_.EventNumber;
        //}

        //public JoinEventCheckOutIn(LogEventCheckOut checkOut_, LogEventShutdown shutdown_)
        //{
        //    CheckOutNumber = checkOut_.EventNumber;
        //    ShutdownNumber = shutdown_.EventNumber;
        //}

        public JoinEventCheckOutIn(LogEventCheckOut checkOut_, LogEventBase checkIn_)
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
                LogFile.Instance.WriteLine($"{checkIn_.EventNumber} {checkIn_.GetType()}");
            }
            _checkIn = checkIn_;
        }

        [ColumnSort(101)]
        public long CheckOutNumber { get; private set; } = -1;

        [ColumnSort(102)]
        public long CheckInNumber { get; private set; } = -1;
        //public long CheckInNumber {get => (_checkIn is LogEventCheckIn checkIn) ? checkIn.EventNumber : -1; }
        //

        [ColumnSort(103)]
        public long ShutdownNumber { get; private set; } = -1;
        //public long ShutdownNumber { get => (_checkIn is LogEventShutdown shutdown) ? shutdown.EventNumber : -1; }


        /// <summary>
        /// -1  : Duplication
        ///  0  : Use CheckInNumber or ShutdownNumber;
        /// >0  : Use DuplicationNumber
        /// </summary>
        [ColumnSort(104)]
        public long DuplicationNumber { get; private set; } = NO_DUPLICATION_;

        private LogEventBase _checkIn;
        public LogEventBase CheckIn() => _checkIn;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetDuplication()
        {
            DuplicationNumber  = DUPLICATION;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetDuplication(LogEventBase checkIn_)
        {
            _checkIn = checkIn_;
            DuplicationNumber = _checkIn.EventNumber;
        }


        public static string HEADER { get => "CheckOut,CheckIn,Shutdown,Duplication"; }
        public override string ToString()
        {
            return $"{CheckOutNumber},{CheckInNumber},{ShutdownNumber},{DuplicationNumber}";
        }
    }
}
