using System.Collections.Generic;

namespace Nop.Plugin.Api.Extensions
{
    public interface IObjectExtensions
    {
        T ToObject<T>(ICollection<KeyValuePair<string, string>> source)
            where T : class, new();
    }
}