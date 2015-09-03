using VendingDataExtractor.Interfaces;
using VendingDataExtractor.Structures;

namespace VendingDataExtractor.Consumer
{
    public class FileRetreiver
    {
        public IFileRetreiver<string> Retreiver;

        public FileRetreiver(IFileRetreiver<string> retreiver)
        {
            this.Retreiver = retreiver;
        }

        public Result GetFileData(string file, ILogger<string> logger )
        {
            return this.Retreiver.GetFileContents(file, logger);
        }

        public Result GetFileList(ILogger<string> logger)
        {
            return this.Retreiver.GetFiles(logger);
        }

        public bool IsTargetFile(string file)
        {
            return this.Retreiver.IsTargetFile(file);
        }

        public bool MoveFile(string fileFrom, string fileTo, ILogger<string> logger)
        {
            return this.Retreiver.MoveFile(fileFrom, fileTo, logger);
        }

    }
}
