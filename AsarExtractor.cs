using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace Asar.NET;

public class AsarExtractor
{
    private bool _emptyDir;

    private List<AsarFile>? _filesToExtract;

    public event EventHandler<AsarExtractEvent>? FileExtracted;
    public event EventHandler<bool>? Finished;

    public async Task Extract(AsarArchive archive, string path, string destination)
    {
        var pathArr = path.Split('/');

        var token = pathArr.Aggregate(archive.Header.Json, (current, t) => (JObject?) current["files"]?[t]!);

        var size = token.Value<int>("size");
        var offset = archive.BaseOffset + token.Value<int>("offset");

        var fileBytes = archive.Bytes.Skip(offset).Take(size).ToArray();

        await Utils.WriteToFile(fileBytes, destination);
    }

    public async Task ExtractAll(AsarArchive archive, string destination, bool emptyDir = false)
    {
        _filesToExtract = new List<AsarFile>();
        _emptyDir = emptyDir;

        var jObject = archive.Header.Json;
        if (jObject.HasValues) TokenIterator(jObject.First);

        var bytes = archive.Bytes;

        var progress = 0;

        foreach (var asarFile in _filesToExtract)
        {
            progress++;
            var size = asarFile.Size;
            var offset = archive.BaseOffset + asarFile.Offset;

            if (size > -1)
            {
                var fileBytes = new byte[size];

                Buffer.BlockCopy(bytes, offset, fileBytes, 0, size);
                var filePath = $"{destination}{asarFile.Path}";

                await Utils.WriteToFile(fileBytes, filePath);

                FileExtracted?.Invoke(this, new AsarExtractEvent(asarFile, progress, _filesToExtract.Count));
            }
            else
            {
                if (_emptyDir)
                    Path.Combine(destination, asarFile.Path).CreateDir();
            }
        }

        Finished?.Invoke(this, true);
    }

    private void TokenIterator(JToken? token)
    {
        if (token is not JProperty property) return;

        foreach (var jToken in property.Value.Children())
        {
            var prop = (JProperty) jToken;
            var size = -1;
            var offset = -1;
            foreach (var jToken1 in prop.Value.Children())
            {
                var nextProp = (JProperty) jToken1;
                switch (nextProp.Name)
                {
                    case "files":
                    {
                        if (_emptyDir)
                        {
                            Debug.WriteLine($"PROP PATH: {prop.Path}");
                            var afile = new AsarFile(prop.Path, "", size, offset);
                            _filesToExtract?.Add(afile);
                        }

                        TokenIterator(nextProp);
                        break;
                    }
                    case "size":
                        size = int.Parse(nextProp.Value.ToString());
                        break;
                    case "offset":
                        offset = int.Parse(nextProp.Value.ToString());
                        break;
                }
            }

            if (size <= -1 || offset <= -1) continue;
            {
                var afile = new AsarFile(prop.Path, prop.Name, size, offset);
                _filesToExtract?.Add(afile);
            }
        }
    }
}