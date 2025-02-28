using Dapper;
using RepriseReportLogAnalyzer.Analyses;
using RepriseReportLogAnalyzer.Databases;
using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Files;
using RLMLogReader.Extensions;
using System.Data.SQLite;
using System.IO;

namespace RepriseReportLogAnalyzer.Managers
{
    public class SQLiteManager
    {
        public static SQLiteManager Instance = new SQLiteManager();
        private SQLiteManager()
        {

        }

        public void Create()
        {
            var path = @"report.db";
            if (File.Exists(path) == true)
            {
                File.Delete(path);
            }

            var config = new SQLiteConnectionStringBuilder()
            {
                DataSource =path
                //DataSource = @":memory:"
            };
            _connection = new SQLiteConnection(config.ToString());
            _connection.Open();
            //
            _connection.CreateTable<LogEventStart>();
            _connection.CreateTable<LogEventShutdown>();
            _connection.CreateTable<LogEventCheckOut>();
            _connection.CreateTable<LogEventCheckIn>();
            _connection.CreateTable<LogEventLicenseDenial>();
            _connection.CreateTable<LogEventLicenseInUse>();
            //
            _connection.CreateTable<LogEventProduct>();
            _connection.CreateTable<LogEventLicenseReread>();
            _connection.CreateTable<LogEventLicenseFile>();
            //
            _connection.CreateTable<LogEventQueue>();
            _connection.CreateTable<LogEventRoamExtend>();
            _connection.CreateTable<LogEventMeterDecrement>();
            _connection.CreateTable<LogEventDynamicReservation>();
            _connection.CreateTable<LogEventAuthentication>();
            //
            _connection.CreateTable<AnalysisStartShutdown>();
            _connection.CreateTable<AnalysisCheckOutIn>();
            //
            SqlMapper.AddTypeHandler(DateTimeHandler.Default);

            LogFile.Instance.WriteLine("Create");
        }

        ~SQLiteManager()
        {
            //_connection?.Close();
            //_connection?.Dispose();
        }

        private SQLiteConnection? _connection = null;

        public void Insert<T>(ICollection<T> list_)
        {
            using (var tran = _connection?.BeginTransaction())
            {
                try
                {
                    _connection?.Insert(list_, tran);
                }
                catch(Exception ex_)
                {
                    LogFile.Instance.WriteLine(ex_.Message);
                }
                tran?.Commit();
            }
        }

        //public List<LogViewStartShutdown> ListLogViewStartShutdown()
        //{
        //    var query = "SELECT"
        //        + " LogEventStart.EventDateTime as StartDateTime,"
        //        + " LogEventShutdown.EventDateTime as EndDateTime,"
        //        + " LogEventStart.HostName as HostName,"
        //        + " LogEventShutdown.User as User,"
        //        + " LogEventShutdown.Host as Host,"
        //        + " LogEventShutdown.UserHost as UserHost "
        //        + "FROM LogEventStart "
        //        + "LEFT JOIN LogEventShutdown "
        //        + "ON"
        //        + " LogEventShutdown.EventNumber = (SELECT Min(LogEventShutdown.EventNumber) FROM LogEventStart LEFT JOIN LogEventShutdown ON LogEventShutdown.EventNumber > LogEventStart.EventNumber GROUP BY LogEventStart.EventNumber);";

        //    return _connection.Query<LogViewStartShutdown>(query).ToList();
        //}
    }
}
