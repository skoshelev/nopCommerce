using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Api.Serializers
{
    public interface IJsonFieldsSerializer
    {
        string Serialize(object objectToSerialize, string fieldsToSerialize);
    }
}
