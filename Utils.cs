namespace Asar.NET;

internal static class Utils
{
    public static void CreateDir(this string path)
    {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    }

    public static async Task WriteToFile(byte[] bytes, string destination)
    {
        (Path.GetDirectoryName(destination) ?? string.Empty).CreateDir();
        using var fs = File.Create(destination);

        await fs.WriteAsync(bytes, 0, bytes.Length);
    }
}