using RepriseReportLogAnalyzer.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepriseReportLogAnalyzer.Analyses
{
    /// <summary>
    /// ライセンスの利用数計算
    /// </summary>
    class AnalysisLicenseCount
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="eventBase_">イベント(時間)</param>
        /// <param name="countProduct_">利用数(アウト+1/イン-1)</param>
        /// <param name="maxProduct_">ライセンス数</param>
        /// <param name="outInProduct_">利用数(サーバ計算)</param>
        public AnalysisLicenseCount(LogEventBase eventBase_, Dictionary<string, int> countProduct_, Dictionary<string, int> maxProduct_, Dictionary<string, int> outInProduct_)
        {
            EventBase = eventBase_;
            CountProduct = new Dictionary<string, int>(countProduct_);
            MaxProduct = new Dictionary<string, int>(maxProduct_);
            OutInProduct = new Dictionary<string, int>(outInProduct_);
        }

        /// <summary>
        /// イベント(時間)
        /// </summary>
        public LogEventBase EventBase{get;private set;}

        /// <summary>
        /// 利用数(アウト+1/イン-1) Key:プロダクト
        /// </summary>
        public Dictionary<string, int> CountProduct { get; private set; }

        /// <summary>
        /// ライセンス数 Key:プロダクト
        /// </summary>
        public Dictionary<string, int> MaxProduct { get; private set; }

        /// <summary>
        /// 利用数(サーバ計算) Key:プロダクト
        /// </summary>
        public Dictionary<string, int> OutInProduct { get; private set; }
    }
}
