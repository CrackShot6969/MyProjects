using System;

namespace VendingDataExtractor.Structures
{
    [Serializable]
    public class Status
    {
        public string Message
        {
            get;
            set;
        }
           
        public bool Success {get; set;}

        public Status()
        {
            this.Message = "";
            this.Success = false;
        }

        public Status(string message, bool success)
        {
            this.Message = message;
            this.Success = success;
        }

        public void setStatus(string message, bool success)
        {
            this.Message = message;
            this.Success = success;
        }
    }
}