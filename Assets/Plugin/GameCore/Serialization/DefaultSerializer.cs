using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Unity.Collections;
using Unity.Entities;
using System.Text;
namespace GameCore.Serialization
{
    public class DefaultSerializer
    {
        public static byte[] Serialize<T>(T data)
        {
            return ToByte(JsonConvert.SerializeObject(data));
        }
        public static T Deserialize<T>(byte[] data)
        {
            return JsonConvert.DeserializeObject<T>(FromByte(data));
        }
        public static byte[] ToByte(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }
        public static string FromByte(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }
    }


}