namespace Asar.NET;

public enum EAsarException
{
    AsarFileCantFind,
    AsarFileCantRead,
    AsarInvalidDescriptor,
    AsarInvalidFileSize
}

public class AsarException : Exception
{
    public AsarException(EAsarException ex, string message = "")
    {
        ExceptionCode = ex;
        if (message.Length > 0)
        {
            ExceptionMessage = message;
            return;
        }

        ExceptionMessage = GetMessage(ex);
    }

    public EAsarException ExceptionCode { get; }

    public string ExceptionMessage { get; }

    private static string GetMessage(EAsarException ex)
    {
        return ex switch
        {
            EAsarException.AsarFileCantFind => "Error: The specified file couldn't be found.",
            EAsarException.AsarFileCantRead => "Error: File can't be read.",
            EAsarException.AsarInvalidDescriptor => "Error: File's header size is not defined on 4 or 8 bytes.",
            EAsarException.AsarInvalidFileSize =>
                "Error: Data table size shorter than the size specified in in the header.",
            _ => "Error: Unhandled exception!"
        };
    }

    public override string ToString()
    {
        return $"[{ExceptionCode}] {ExceptionMessage}";
    }
}