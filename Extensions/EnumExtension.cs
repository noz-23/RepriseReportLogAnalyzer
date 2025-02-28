using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepriseReportLogAnalyzer.Extensions
{
    public static class EnumExtension
    {
        // https://www.sejuku.net/blog/42539
        public static string Description<T>(this T src_) where T : Enum
        {
            string rtn = string.Empty;

            var fieldInfo = typeof(T).GetField(src_.ToString());
            var attr = Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute));
            if (attr != null)
            {
                var descAttr =attr as DescriptionAttribute;
                rtn = descAttr?.Description ??string.Empty;
            }
            return rtn;
        }
    }
}
