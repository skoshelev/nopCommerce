using System.Collections.Generic;

namespace Nop.Plugin.Api.Validators
{
    public interface IParametersValidator
    {
        IList<int> GetIdsAsListOfInts(string ids);
        string EnsurePublishedStatusIsValid(string publishedStatus);
    }
}