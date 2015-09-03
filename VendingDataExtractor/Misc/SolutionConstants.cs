namespace VendingDataExtractor.Misc
{
    public class SolutionConstants
    {
        public static readonly string CONST_ERROR_NOTENOUGHINFO = "Session does not have enough information to begin";
        public static readonly string CONST_ERROR_NOFILESFOUND = "No files were found for processing";
        public static readonly string CONST_ERROR_RESPONSESTREAMNULL = "Response stream was null";
        public static readonly string CONST_ERROR_ERRORWHENDOWNLOADING = "Exception when downloading file : ";
        public static readonly string CONST_ERROR_FILELISTEMPTY = "Filelist is null : ";
        public static readonly string CONST_ERROR_FILEISEMPTY = "Contents of file {0} is empty.";
        public static readonly string CONST_ERROR_ERRORWHENDATALOGGING = "An exception occurred when logging data : ";
        public static readonly string CONST_ERROR_REMOTEMOVE = "An error occurred when remotely moving file. ";
        public static readonly string CONST_ERROR_UNABLETOLOGDATA ="Able to upload entry {0} but was unable to log data.";
        public static readonly string CONST_ERROR_NOTHING_RETURNED = "The object can be parsed but yields no results";
        public static readonly string CONST_ERROR_PARAMETER_FORMAT_INCORRECT = "The file passed in was not of type 'string'";


        public static readonly string CONST_SUCCESS_SENDING = "Send successful, Logging Data...";
        public static readonly string CONST_ERROR_SENDING = "Error sending file contents : ";
        public static readonly string CONST_STATUS_SENDING = "Sending file contents...";

        public static readonly string CONST_ERROR_MAPPING = "Error mapping file contents : ";
        public static readonly string CONST_STATUS_MAPPING = "Mapping file contents...";

        public static readonly string CONST_ERROR_PARSING = "Error parsing file contents: ";
        public static readonly string CONST_STATUS_PARSING = "Mapping file contents...";

        public static readonly string CONST_STATUS_SAVING = "Saving file...";
        public static readonly string CONST_STATUS_PROCESSING = "Processing file : {0}";

        public static readonly string CONST_STATUS_FILESTOPROCESS = "There are {0} files to process..";

        public static readonly string CONST_STATUS_RETREIVINGFILES = "Retreiving file list to process...";

        public static readonly string CONST_STATUS_STARTINGSESSION = "Starting Session...";

        public static readonly string CONST_STATUS_EMAILHEADER =
            "<b>The following files failed to parse and be processed</b><br>";

        public static readonly string CONST_TAB = "\t";
    }
}
