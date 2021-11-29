using System.Text;
using Newtonsoft.Json.Linq;

namespace Asar.NET;

public class AsarArchive
{
    private const int SizeUint = 4;
    private const int SizeLong = 2 * SizeUint;
    private const int SizeInfo = 2 * SizeLong;
    private readonly byte[] _bytes;

    public AsarArchive(string filepath)
    {
        FilePath = filepath;

        if (!File.Exists(FilePath)) throw new AsarException(EAsarException.AsarFileCantFind);

        try
        {
            _bytes = File.ReadAllBytes(FilePath);
        }
        catch (Exception ex)
        {
            throw new AsarException(EAsarException.AsarFileCantRead, ex.ToString());
        }

        Header = ReadHeader(ref _bytes);
        BaseOffset = Header.Length;
    }

    public int BaseOffset { get; }
    public byte[] Bytes => _bytes;
    private string FilePath { get; }
    public AsarHeader Header { get; }

    private static AsarHeader ReadHeader(ref byte[] bytes)
    {
        var headerInfo = bytes.Take(SizeInfo).ToArray();

        if (headerInfo.Length < SizeInfo) throw new AsarException(EAsarException.AsarInvalidFileSize);

        var asarFileDescriptor = headerInfo.Take(SizeLong).ToArray();
        var asarPayloadSize = asarFileDescriptor.Take(SizeUint).ToArray();

        var payloadSize = BitConverter.ToInt32(asarPayloadSize, 0);
        var payloadOffset = asarFileDescriptor.Length - payloadSize;

        if (payloadSize != SizeUint && payloadSize != SizeLong)
            throw new AsarException(EAsarException.AsarInvalidDescriptor);

        var asarHeaderLength = asarFileDescriptor.Skip(payloadOffset).Take(SizeUint).ToArray();

        var headerLength = BitConverter.ToInt32(asarHeaderLength, 0);

        var asarFileHeader = headerInfo.Skip(SizeLong).Take(SizeLong).ToArray();
        var asarHeaderPayloadSize = asarFileHeader.Take(SizeUint).ToArray();

        var headerPayloadSize = BitConverter.ToInt32(asarHeaderPayloadSize, 0);
        var headerPayloadOffset = headerLength - headerPayloadSize;

        var dataTableLength = asarFileHeader.Skip(headerPayloadOffset).Take(SizeUint).ToArray();
        var dataTableSize = BitConverter.ToInt32(dataTableLength, 0);

        var hdata = bytes.Skip(SizeInfo).Take(dataTableSize).ToArray();

        if (hdata.Length != dataTableSize) throw new AsarException(EAsarException.AsarInvalidFileSize);

        return new AsarHeader(headerInfo, asarFileDescriptor.Length + headerLength, hdata,
            JObject.Parse(Encoding.Default.GetString(hdata)));
    }
}