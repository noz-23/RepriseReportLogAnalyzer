namespace RepriseReportLogAnalyzer.Attributes
{
    public class ColumnSortAttribute : Attribute
    {
        public int Sort =-1;

        public ColumnSortAttribute(int sort_)
        {
            Sort = sort_;
        }
    }
}
