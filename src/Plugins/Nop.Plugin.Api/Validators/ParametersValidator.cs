using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Plugin.Api.MVC;

namespace Nop.Plugin.Api.Validators
{
    public class ParametersValidator : IParametersValidator
    {
        // TODO: remove this method because it is available in the string extensions.
        public IList<int> GetIdsAsListOfInts(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                List<string> stringIds = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                List<int> intIds = new List<int>();

                foreach (var id in stringIds)
                {
                    int intId;
                    if (int.TryParse(id, out intId))
                    {
                        intIds.Add(intId);
                    }
                }

                intIds = intIds.Distinct().ToList();
                return intIds.Count > 0 ? intIds : null;
            }

            return null;
        }

        // TODO: remove this method because it is not used anymore.
        public string EnsurePublishedStatusIsValid(string publishedStatus)
        {
            if (publishedStatus != Configurations.PublishedStatus && publishedStatus != Configurations.UnpublishedStatus &&
                publishedStatus != Configurations.AnyStatus)
            {
                publishedStatus = Configurations.AnyStatus;
            }

            return publishedStatus;
        }
    }
}