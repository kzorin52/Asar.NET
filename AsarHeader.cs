using Newtonsoft.Json.Linq;

namespace Asar.NET;

public class AsarHeader
{
    public AsarHeader(byte[] info, int length, byte[] data, JObject json)
    {
        Info = info;
        Length = length;
        Data = data;
        Json = json;
    }

    public byte[] Info { get; }
    public int Length { get; }

    public byte[] Data { get; }
    public JObject Json { get; }
}