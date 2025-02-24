namespace RepriseReportLogAnalyzer.Interfaces
{
    interface ILogEventHost
    {
        string Host { get; }
    }
    interface ILogEventUser
    {
        string User { get; }
    }

    interface ILogEventUserHost : ILogEventUser, ILogEventHost
    {
        string UserHost { get; }
    }

    interface ILogEventProduct
    {
        string Product { get; }
        string Version { get; }
        string ProductVersion { get; }
    }

    interface ILogEventCountCurrent:ILogEventProduct
    {
        int CountCurrent { get; }
    }
}
