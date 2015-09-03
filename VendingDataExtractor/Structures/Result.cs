using System;

namespace VendingDataExtractor.Structures
{
    [Serializable]
    public class Result
    {
        public Status Status { get; set; }

        public Result()
        {
            this.Status = new Status();
        }

        public void SetStatus(Status status)
        {
            this.Status = status;
        }

    }
}