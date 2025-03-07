using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace RepriseReportLogAnalyzer.Files;

/// <summary>
/// ログファイル
/// </summary>
class LogFile
{
    /// <summary>
    /// シングルトン
    /// </summary>
    public static LogFile Instance { get; private set; } = new LogFile();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    private LogFile()
    {
        if (File.Exists(_logFileName) == true)
        {
            File.Delete(_logFileName);
        }
    }

    /// <summary>
    /// ログファイル名
    /// </summary>
    private string _logFileName = Directory.GetCurrentDirectory() + @"\log.txt";

    /// <summary>
    /// 作成処理
    /// </summary>
    public void Create()
    {
        var stream = new StreamWriter(_logFileName);
        stream.AutoFlush = true;

        Trace.Listeners.Remove("Default");
        Trace.Listeners.Add(new TextWriterTraceListener(TextWriter.Synchronized(stream)));
    }
    /// <summary>
    /// ログの書き込み
    /// </summary>
    /// <param name="message_">実ログ</param>
    /// <param name="soruce_">ソース</param>
    /// <param name="line_">行</param>
    /// <param name="member_">関数</param>
    public void WriteLine(string message_, [CallerFilePath] string soruce_ = "", [CallerLineNumber] int line_ = -1, [CallerMemberName] string member_ = "")
    {
        Trace.WriteLine($"{DateTime.Now.ToString()} [{Path.GetFileName(soruce_)}({line_})][{member_}]\n{message_}\n");
        Trace.Flush();
    }
}
