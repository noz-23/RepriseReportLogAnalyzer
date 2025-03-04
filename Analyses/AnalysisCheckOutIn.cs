using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Enums;
using RepriseReportLogAnalyzer.Events;
using System.Runtime.CompilerServices;

namespace RepriseReportLogAnalyzer.Analyses
{
    /// <summary>
    /// CheckOutとCheckInを結合
    /// </summary>
    class AnalysisCheckOutIn
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="checkOut_"></param>
        /// <param name="checkIn_"></param>
        public AnalysisCheckOutIn(LogEventCheckOut checkOut_, LogEventBase? checkIn_)
        {
            _checkOut = checkOut_;
            _checkIn = checkIn_;

            _joinEvent = new JoinEventCheckOutIn(checkOut_, checkIn_);
        }

        /// <summary>
        /// 保存時のヘッダー
        /// </summary>
        public const string HEADER = "CheckOut Date Time,CheckIn Date Time,Duration,Product,Version,Product Version,User,Host,User@Host";

        /// <summary>
        /// チェックアウト時間
        /// </summary>
        [ColumnSort(101)]
        public DateTime CheckOutDateTime { get => _checkOut.EventDateTime; }

        /// <summary>
        /// チェックイン時間
        /// </summary>
        [ColumnSort(102)]
        public DateTime CheckInDateTime { get => _checkIn?.EventDateTime?? LogEventBase.NowDateTime; }

        /// <summary>
        /// 利用時間
        /// </summary>
        [ColumnSort(103)]
        public TimeSpan Duration { get => CheckInDateTime - CheckOutDateTime; }

        /// <summary>
        /// プロダクト
        /// </summary>
        [ColumnSort(111)]
        public string Product { get => _checkOut.Product; }
        /// <summary>
        /// バージョン
        /// </summary>
        [ColumnSort(112)]
        public string Version { get => _checkOut.Version; }
        /// <summary>
        /// プロダクト バージョン
        /// </summary>
        [ColumnSort(113)]
        public string ProductVersion { get => _checkOut.ProductVersion; }
        /// <summary>
        /// ユーザー
        /// </summary>
        [ColumnSort(121)]
        public string User { get => _checkOut.User; }
        /// <summary>
        /// ホスト
        /// </summary>
        [ColumnSort(121)]
        public string Host { get => _checkOut.Host; }
        /// <summary>
        /// ユーザー@ホスト
        /// </summary>
        [ColumnSort(121)]
        public string UserHost { get => _checkOut.UserHost; }

        /// <summary>
        /// チェックアウト
        /// </summary>
        private readonly LogEventCheckOut _checkOut;
        /// <summary>
        /// チェックイン(シャットダウン)
        /// </summary>
        private readonly LogEventBase? _checkIn=null;
        /// <summary>
        /// 結合情報
        /// </summary>
        private readonly JoinEventCheckOutIn _joinEvent;

        /// <summary>
        /// チェックアウト(リファクタリングで呼び出さないため関数化)
        /// </summary>
        /// <returns></returns>
        public LogEventCheckOut CheckOut() => _checkOut;
        /// <summary>
        /// チェックイン(リファクタリングで呼び出さないため関数化)
        /// </summary>
        /// <returns></returns>
        public LogEventBase? CheckIn() => _checkIn;
        /// <summary>
        /// 結合情報(リファクタリングで呼び出さないため関数化)
        /// </summary>
        /// <returns></returns>
        public JoinEventCheckOutIn JoinEvent() => _joinEvent;

        /// <summary>
        /// チェックアウトのイベント番号
        /// </summary>
        /// <returns></returns>
        public long CheckOutNumber() => _checkOut.EventNumber;
        /// <summary>
        /// チェックインのイベント番号
        /// </summary>
        public long CheckInNumber() => _checkIn?.EventNumber ?? LogEventBase.NowEventNumber;


        /// <summary>
        /// 同一のチェックインのか？
        /// </summary>
        /// <param name="checkIn_"></param>
        /// <returns></returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSame(LogEventCheckIn checkIn_)
        {
            return _checkIn == checkIn_;
        }

        /// <summary>
        /// チェックアウトとチェックインの間のイベントか？
        /// </summary>
        /// <param name="number_"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsWithInRange(long number_)
        {
            return (number_ > CheckOutNumber()) && (number_ < CheckInNumber());
        }

        public bool IsWithInRange(DateTime dateTime_)
        {
            return (dateTime_ > CheckOutDateTime) && (dateTime_ < CheckInDateTime);
        }

        /// <summary>
        /// 重複を取り除いたチェックインの時間
        /// </summary>
        /// <returns></returns>
        public DateTime JointDateTime() => _joinEvent.CheckIn()?.EventDateTime ?? LogEventBase.NowDateTime;

        /// <summary>
        /// 重複を取り除いた利用時間
        /// </summary>
        /// <returns></returns>
        public TimeSpan DurationDuplication() 
        {
            return JointDateTime() - CheckOutDateTime;
        }

        /// <summary>
        /// グループ集計する名称
        /// </summary>
        /// <param name="group_"></param>
        /// <returns></returns>
        public string GroupName(ANALYSIS_GROUP group_)
        {
            switch (group_)
            {
                case ANALYSIS_GROUP.USER:return User;
                case ANALYSIS_GROUP.HOST:return Host;
                case ANALYSIS_GROUP.USER_HOST:return UserHost;
                default:
                    break;
            }return string.Empty;
        }

        /// <summary>
        /// 文字列化
        /// </summary>
        /// <param name="duplication_">重複除去</param>
        /// <returns></returns>
        public string ToString( bool duplication_)
        {
            if (duplication_ == true)
            {
                return $"{CheckOutDateTime.ToString()},{JointDateTime().ToString()},{DurationDuplication().ToString(@"d\.hh\:mm\:ss")},{Product},{Version},{ProductVersion},{User},{Host},{UserHost}";
            }

            return ToString();
        }

        /// <summary>
        /// 文字列化
        /// </summary>
        /// <returns></returns>

        public override string ToString()
        {
            return $"{CheckOutDateTime.ToString()},{CheckInDateTime.ToString()},{Duration.ToString(@"d\.hh\:mm\:ss")},{Product},{Version},{ProductVersion},{User},{Host},{UserHost}";
        }


    }
}
