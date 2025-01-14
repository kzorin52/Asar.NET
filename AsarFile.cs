﻿namespace Asar.NET;

public class AsarFile
{
    public AsarFile(string path, string filename, int size, int offset)
    {
        path = path.Replace(".files['", "/").Replace("['", "").Replace("']", "");
        path = path.Substring(0, path.Length - filename.Length);
        path = path.Replace(".files.", "/").Replace("files.", "");
        path += filename;
        Path = path;
        Size = size;
        Offset = offset;
    }

    public string Path { get; }
    public int Size { get; }
    public int Offset { get; }
}