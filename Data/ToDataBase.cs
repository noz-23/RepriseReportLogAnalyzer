/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Enums;
using RepriseReportLogAnalyzer.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace RepriseReportLogAnalyzer.Data
{

    /// <summary>
    /// 保存する関係ベース
    /// </summary>
    internal class ToDataBase
    {
        /// <summary>
        /// ヘッダー
        /// </summary>
        /// <param name="classType_">子のクラス</param>
        /// <returns></returns>
        public static string Header(Type classType_) => string.Join(",", ListHeader(classType_).Select(x_=>x_.Key));

        /// <summary>
        /// リスト化したヘッダー
        /// </summary>
        /// <param name="classType_"></param>
        /// <returns></returns>
        public static ListStringStringPair ListHeader(Type classType_)
        {
            var rtn = new ListStringStringPair();
            var listPropetyInfo = classType_.GetProperties(BindingFlags.Instance | BindingFlags.Public)?.Where(s_ => (Attribute.GetCustomAttribute(s_, typeof(ColumnAttribute)) as ColumnAttribute)?.Order != 999).OrderBy(s_ => (Attribute.GetCustomAttribute(s_, typeof(ColumnAttribute)) as ColumnAttribute)?.Order);

            listPropetyInfo?.ToList().ForEach(prop =>
            {
                var column = Attribute.GetCustomAttribute(prop, typeof(ColumnAttribute)) as ColumnAttribute;

                //rtn.Add(new ($"{prop.Name}",$"{GetDatabaseType(prop.PropertyType)}"));
                rtn.Add(new($"{column.Name}", $"{GetDatabaseType(prop.PropertyType)}"));
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
        public virtual List<string> ListValue(Type? classType_ = null)
        {
            classType_ ??= this.GetType();
            //
            var rtn = new List<string>();
            var listPropetyInfo = classType_.GetProperties(BindingFlags.Instance | BindingFlags.Public)?.Where(s_ => (Attribute.GetCustomAttribute(s_, typeof(ColumnAttribute)) as ColumnAttribute)?.Order != 999).OrderBy(s_ => (Attribute.GetCustomAttribute(s_, typeof(ColumnAttribute)) as ColumnAttribute)?.Order);

            listPropetyInfo?.ToList().ForEach(prop =>
            {
                var classType = prop.PropertyType;
                if (classType == typeof(TimeSpan))
                {
                    rtn.Add($"{prop.GetValue(this)}:d\\.hh\\:mm\\:ss");
                }
                if (classType==typeof(StatusValue))
                //if (classType.FullName == typeof(StatusValue).FullName)
                {
                    StatusValue val = (StatusValue)Enum.Parse(typeof(StatusValue), prop.GetValue(this).ToString());
                    rtn.Add($"{(long)val}");
                }
                else
                {
                    rtn.Add($"{prop.GetValue(this)}");
                }
            });
            return rtn;
        }

        /// <summary>
        /// データベース(SQL)に変換する場合のデータ型
        /// </summary>
        /// <param name="type_"></param>
        /// <returns></returns>
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
            if (type_ == typeof(StatusValue))
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
