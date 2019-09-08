using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Unity.Collections;
using Unity.Entities;
using System.Text;
namespace GameCore
{
    public class DefaultSerializer
    {
        public byte[] Serialize<T>(T data)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
        }
        public T Deserialize<T>(byte[] data)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data));
        }
    }

}