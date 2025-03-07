using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Interfaces;

namespace RepriseReportLogAnalyzer.Events;

/// <summary>
/// 文字列 と イベントの紐づけ登録
/// </summary>
internal sealed partial class LogEventRegist
{
    private bool _logEventProduct = Regist("PRODUCT", (l_) => new LogEventProduct(l_));
}

/// <summary>
/// support for a product
/// </summary>
[Sort(30)]
internal sealed class LogEventProduct : LogEventBase, ILogEventProduct
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>

    public LogEventProduct(string[] list_) : base()
    {
        Product = list_[1];
        Version = list_[2];
        Pool = list_[3];
        //
        Count = int.Parse(list_[4]);
        Reservations = int.Parse(list_[5]);
        LimitSoft = int.Parse(list_[6]);
        //
        HostId = list_[7];
        Contract = list_[8];
        Customer = list_[9];
        Issuer = list_[10];
        LineItem = list_[11];
        Options = list_[12];
        //
        Share = int.Parse(list_[13]);
        MaxShare = int.Parse(list_[14]);
        TypeShare = int.Parse(list_[15]);
        CountNamedUser = int.Parse(list_[16]);
        MeterType = int.Parse(list_[17]);
        MeterCounter = int.Parse(list_[18]);
        MeterInitialDecrement = int.Parse(list_[19]);
        MeterPeriod = int.Parse(list_[20]);
        MeterPeriodDecrement = int.Parse(list_[21]);

        EventDateTime = NowDateTime;
    }

    //support for a product
    //PRODUCT name     version pool# count #reservations soft_limit “hostid” “contract” “customer” “issuer” “line_item” “options”        share max_share type named_user_count meter_type meter_counter meter_initial_decrement meter_period meter_period_decrement
    //0       1        2       3     4     5             6           7          8            9            10         11            12                13    14        15   16               17         18            19                      20           21
    //PRODUCT AbcName  25      1     1     0             1           ""         "A-B-C-D-E"  ""           ""         ""            "LA:xx_XX TY:XXX" 3     0         0    0                0          0             0                       0            0
    [Sort(11)]
    public string Product { get; private set; } = string.Empty;
    [Sort(12)]
    public string Version { get; private set; } = string.Empty;
    [Sort(13)]
    public string ProductVersion { get => Product + " " + Version; }
    //
    [Sort(101)]
    public string Pool { get; private set; } = string.Empty;
    //
    [Sort(102)]
    public int Count { get; private set; } = -1;
    [Sort(103)]
    public int Reservations { get; private set; } = -1;
    [Sort(104)]
    public int LimitSoft { get; private set; } = -1;
    //
    [Sort(105)]
    public string HostId { get; private set; } = string.Empty;
    //
    [Sort(106)]
    public string Contract { get; private set; } = string.Empty;
    [Sort(107)]
    public string Customer { get; private set; } = string.Empty;
    [Sort(108)]
    public string Issuer { get; private set; } = string.Empty;
    [Sort(109)]
    public string LineItem { get; private set; } = string.Empty;
    [Sort(110)]
    public string Options { get; private set; } = string.Empty;

    [Sort(111)]
    public int Share { get; private set; } = -1;
    [Sort(112)]
    public int MaxShare { get; private set; } = -1;
    [Sort(113)]
    public int TypeShare { get; private set; } = -1;
    [Sort(114)]
    public int CountNamedUser { get; private set; } = -1;
    [Sort(115)]
    public int MeterType { get; private set; } = -1;
    [Sort(116)]
    public int MeterCounter { get; private set; } = -1;
    [Sort(117)]
    public int MeterInitialDecrement { get; private set; } = -1;
    [Sort(118)]
    public int MeterPeriod { get; private set; } = -1;
    [Sort(119)]
    public int MeterPeriodDecrement { get; private set; } = -1;
    //
}
