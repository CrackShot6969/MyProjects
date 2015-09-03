using System.Collections.Generic;

namespace VendingDataExtractor.Structures
{
    public class FtpFileListResult : Result
    {
        public List<string> Items { get; set; }

        public FtpFileListResult()
        {
            this.Items = new List<string>();
        }

        public void Add(string file)
        {
            this.Items.Add(file);
        }
    }
}