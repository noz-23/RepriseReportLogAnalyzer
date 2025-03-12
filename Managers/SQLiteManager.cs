/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Interfaces;
using RLMLogReader.Extensions;
using System.Data.SQLite;
using System.IO;
using System.Windows;

namespace RepriseReportLogAnalyzer.Managers
{
    /// <summary>
    /// SQL 操作処理
    /// </summary>
    internal class SQLiteManager
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="path_"></param>
        public SQLiteManager(string path_)
        {
            if (File.Exists(path_) == true)
            {
                MessageBox.Show("Sqlite Data Override!", "Attention");
                LogFile.Instance.WriteLine($"Delete [{path_}]");

                File.Delete(path_);
            }
            Open(path_);
        }

        private SQLiteConnection? _connection = null;

        public void Close()
        {
            _connection?.Close();
            _connection?.Dispose();
        }

        public void Open(string path_)
        {
            var config = new SQLiteConnectionStringBuilder()
            {
                DataSource = path_
                //DataSource = @":memory:"
            };

            _connection = new SQLiteConnection(config.ToString());
            _connection.Open();
        }

        /// <summary>
        /// テーブル作成処理
        /// </summary>
        /// <param name="classType_"></param>
        public void Create(Type classType_)=>_connection?.CreateTable(classType_);
        public void Create(Type classType_, ListStringStringPair list_) => _connection?.CreateTable(classType_, list_);

        /// <summary>
        /// データ挿入処理
        /// </summary>
        /// <param name="classType_"></param>
        /// <param name="header_"></param>
        /// <param name="list_"></param>
        public void Insert(Type classType_, string header_, IEnumerable<List<string>> list_)
        {
            LogFile.Instance.WriteLine($"Insert [{header_}] [{list_.Count()}]");

            using (var tran = _connection?.BeginTransaction())
            {
                try
                {
                    _connection?.Insert(classType_,header_, list_, tran);
                }
                catch (Exception ex_)
                {
                    LogFile.Instance.WriteLine(ex_.Message);
                }
                tran?.Commit();
            }
        }
    }
}
