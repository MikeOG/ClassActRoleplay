using MsgPack.Serialization;
using System.IO;
using System.Linq;
using Roleplay.Client.Helpers;
using Roleplay.Client.Enums;

namespace Roleplay.Client.Helpers
{
    public static class MsgPack
    {
        public static byte[] BinarySerialize<T>(T obj)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                MessagePackSerializer.Get<T>().Pack((Stream)memoryStream, obj);
                return memoryStream.ToArray();
            }
        }

        public static T BinaryDeserialize<T>(byte[] data)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
                return MessagePackSerializer.Get<T>().Unpack((Stream)memoryStream);
        }

        public static string Serialize<T>(T obj)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                MessagePackSerializer.Get<T>().Pack((Stream)memoryStream, obj);
                return StringExtensions.BytesToString(memoryStream.ToArray());
            }
        }

        public static T Deserialize<T>(string data)
        {
            using (MemoryStream memoryStream = new MemoryStream(StringExtensions.StringToBytes(data)))
                return MessagePackSerializer.Get<T>().Unpack((Stream)memoryStream);
        }
    }
}