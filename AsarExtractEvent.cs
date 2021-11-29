namespace Asar.NET;

public class AsarExtractEvent : EventArgs
{
    public AsarExtractEvent(AsarFile file, double index, double total)
    {
        File = file;
        Index = index;
        Total = total;

        Progress = Math.Round(index / total * 100, 2);
    }

    public AsarFile File { get; }
    public double Index { get; }
    public double Total { get; }
    public double Progress { get; }
}