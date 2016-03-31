using System;
using System.Collections.Generic;

namespace Nop.Plugin.Api.Extensions
{
    public interface IStringExtensions
    {
        DateTime? ToDateTimeNullable(string value);
        int ToInt(string value);
        int? ToIntNullable(string value);
        IList<int> ToListOfInts(string value);
        bool? ToStatus(string value);
    }
}