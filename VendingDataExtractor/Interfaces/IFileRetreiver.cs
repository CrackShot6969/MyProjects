using VendingDataExtractor.Structures;

namespace VendingDataExtractor.Interfaces
{
    public interface IFileRetreiver<in T>
    {
        Result GetFiles(ILogger<string> logger );
        bool IsTargetFile(T file);
        Result GetFileContents(T file, ILogger<string> logger );
        bool MoveFile(T fileFrom, T fileTo, ILogger<string> logger);
    }
}