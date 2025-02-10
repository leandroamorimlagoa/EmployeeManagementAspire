using System.Text;
using Newtonsoft.Json;

namespace IntegrationTests.Extensions;

public static class ObjectSerializerContentExtensions
{
    public static StringContent ToJsonContent(this object obj)
    {
        var json = JsonConvert.SerializeObject(obj);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
