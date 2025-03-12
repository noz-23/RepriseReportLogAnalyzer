using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Interfaces;
using System.Reflection;

namespace RepriseReportLogAnalyzer.Data
{
    internal class ToDataBase
    {
        /// <summary>
        /// 文字列化のヘッダー
        /// </summary>
        /// <param name="classType_">子のクラス</param>
        /// <returns></returns>
        //public static string Header(Type classType_ ) =>string.Join(",", ListHeader(classType_));
        public static string Header(Type classType_) => string.Join(",", ListHeader(classType_).Select(x_=>x_.Key));

        /// <summary>
        /// リスト化したヘッダーの文字列
        /// </summary>
        /// <param name="classType_"></param>
        /// <returns></returns>
        //public static List<string> ListHeader(Type classType_)
        //{
        //    var rtn = new List<string>();
        //    var listPropetyInfo = classType_.GetProperties(BindingFlags.Instance | BindingFlags.Public)?.OrderBy(s_ => (Attribute.GetCustomAttribute(s_, typeof(SortAttribute)) as SortAttribute)?.Sort);

        //    listPropetyInfo?.ToList().ForEach(prop =>
        //    {
        //        rtn.Add($"{prop.Name}");
        //    });

        //    return rtn;
        //}

        public static ListStringStringPair ListHeader(Type classType_)
        {
            var rtn = new ListStringStringPair();
            var listPropetyInfo = classType_.GetProperties(BindingFlags.Instance | BindingFlags.Public)?.OrderBy(s_ => (Attribute.GetCustomAttribute(s_, typeof(SortAttribute)) as SortAttribute)?.Sort);

            listPropetyInfo?.ToList().ForEach(prop =>
            {
                rtn.Add(new ($"{prop.Name}",$"{GetDatabaseType(prop.PropertyType)}"));
            });

            return rtn;
        }

        /// <summary>
        /// 文字列化
        /// </summary>
        /// <returns></returns>
        public override string ToString() => string.Join(",", ListValue(this.GetType()));

        /// <summary>
        /// リスト化したデータ(文字)
        /// </summary>
        /// <param name="classTyep_"></param>
        /// <returns></returns>
        public virtual List<string> ListValue(Type? classTyep_ =null)
        {
            classTyep_ ??= this.GetType();
            //
            var rtn = new List<string>();
            var listPropetyInfo = classTyep_.GetProperties(BindingFlags.Instance | BindingFlags.Public)?.OrderBy(s_ => (Attribute.GetCustomAttribute(s_, typeof(SortAttribute)) as SortAttribute)?.Sort);

            listPropetyInfo?.ToList().ForEach(prop =>
            {
                if (prop.GetType() == typeof(TimeSpan))
                {

                    rtn.Add($"{prop.GetValue(this)}:d\\.hh\\:mm\\:ss");
                }
                else 
                {
                    rtn.Add($"{prop.GetValue(this)}");
                }
            });
            return rtn;
        }

        public static string GetDatabaseType(Type type_)
        {
            if (type_ == typeof(string))
            {
                return "TEXT";
            }
            if (type_ == typeof(Enum))
            {
                return "INTEGER";
            }
            if (type_ == typeof(int))
            {
                return "INTEGER";
            }
            if (type_ == typeof(long))
            {
                return "INTEGER";
            }
            if (type_ == typeof(float))
            {
                return "REAL";
            }
            if (type_ == typeof(double))
            {
                return "REAL";
            }
            return "TEXT";
        }
    }
}
