/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using Dapper;
using System.Data;

namespace RepriseReportLogAnalyzer.Databases;

/// <summary>
/// 文字列 → 時間変更
/// </summary>
internal sealed class DateTimeHandler : SqlMapper.TypeHandler<DateTimeOffset>
{
    private readonly TimeZoneInfo databaseTimeZone = TimeZoneInfo.Local;
    public static readonly DateTimeHandler Default = new DateTimeHandler();

    public DateTimeHandler()
    {

    }

    public override DateTimeOffset Parse(object value)
    {
        var storedDateTime = (value == null) ? (DateTime.MinValue) : ((DateTime)value);

        if (storedDateTime.ToUniversalTime() <= DateTimeOffset.MinValue.UtcDateTime)
        {
            return DateTime.MinValue;
        }
        else
        {
            return new DateTimeOffset(storedDateTime, databaseTimeZone.BaseUtcOffset);
        }
    }

    public override void SetValue(IDbDataParameter parameter, DateTimeOffset value)
    {
        DateTime paramVal = value.ToOffset(this.databaseTimeZone.BaseUtcOffset).DateTime;
        parameter.Value = paramVal;
    }
}
