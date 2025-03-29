﻿/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Extensions;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Interfaces;
using System.Data.SQLite;
using System.IO;
using System.Windows;

namespace RepriseReportLogAnalyzer.Managers
{
    /// <summary>
    /// SQL 操作処理
    /// </summary>
    internal sealed class SQLiteManager : IDisposable
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

        private SQLiteConnection? _connection;

        public void Close()
        {
            _connection?.Close();
            _connection?.Dispose();
            _connection = null;
        }

        public void Open(string path_)
        {
            LogFile.Instance.WriteLine($"Open [{path_}]");

            var config = new SQLiteConnectionStringBuilder()
            {
                DataSource = path_
                //DataSource = @":memory:"
            };

            _connection = new SQLiteConnection(config.ToString());
            _connection.Open();
        }

        /// <summary>
        /// テーブル作成処理(単一)
        /// </summary>
        /// <param name="name_"></param>
        public void CreateTable(string name_) => _connection?.CreateTable(name_);

        public void Insert(string name_, IEnumerable<string> listValue_) => _connection?.Insert(name_, listValue_);


        /// <summary>
        /// テーブル作成処理
        /// </summary>
        /// <param name="classType_"></param>
        public void CreateTable(Type classType_) => _connection?.CreateTable(classType_);
        public void CreateTable(Type classType_, ListStringStringPair list_) => _connection?.CreateTable(classType_, list_);

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
                // トランザクションしないと遅い
                try
                {
                    _connection?.Insert(classType_, header_, list_, tran);
                }
                catch (Exception ex_)
                {
                    LogFile.Instance.WriteLine(ex_.Message);
                }
                tran?.Commit();
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
