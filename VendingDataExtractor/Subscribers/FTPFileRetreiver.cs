using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using VendingDataExtractor.Interfaces;
using VendingDataExtractor.Misc;
using VendingDataExtractor.Structures;

namespace VendingDataExtractor.Subscribers
{
    public class FtpFileRetreiver : IFileRetreiver<string>
    {
        private string _connectionString { get; set; }
        private string _port { get; set; }
        private string _directory { get; set; }
        private string _userName { get; set; }
        private string _passWord { get; set; }
        private string _targetFilePrefix { get; set; }

        public FtpFileRetreiver(string connectionString, string port, string directory, string userName, string password, string targetFilePrefix )
        {
            this._connectionString = connectionString;
            this._port = port;
            this._directory = directory;
            this._userName = userName;
            this._passWord = password;
            this._targetFilePrefix = targetFilePrefix;
        }

        public Result GetFiles(ILogger<string> logger )
        {
            return this.GetFtpDirectoryFiles(logger);
        }

        public Result GetFileContents(string file, ILogger<string> logger)
        {
            return this.GetFtpFile(file, logger);
            //return result != null ? Encoding.UTF8.GetString(result) : String.Empty;
        }

        public bool IsTargetFile(string fileName)
        {
            return fileName.IndexOf(this._targetFilePrefix, StringComparison.Ordinal) > -1;
        }

        public bool MoveFile(string fromFile, string toFile, ILogger<string> logger)
        {
            bool success;
            try
            {
                var reqFtp = (FtpWebRequest)WebRequest.Create(new Uri(IncludeNetworkPathDelimeter("ftp://" + this._connectionString + ":" + this._port) + IncludeNetworkPathDelimeter(this._directory) + fromFile));
				reqFtp.UseBinary = true;
				reqFtp.Method = WebRequestMethods.Ftp.Rename;
				reqFtp.Credentials = new NetworkCredential(this._userName, this._passWord);
				reqFtp.RenameTo = toFile ;
				var back = (FtpWebResponse)reqFtp.GetResponse();
                success = back.StatusCode == FtpStatusCode.CommandOK || back.StatusCode == FtpStatusCode.FileActionOK;
            }
            catch (Exception ex)
            {
                logger.LogEntry("Error during FTP file move : " + ex);
                success = false;
            }
            return success;
        }

        private FtpFileResult GetFtpFile(string fileName, ILogger<string> logger )
        {
            FtpWebResponse ftpResponse = null;
            Stream ftpStream = null;
            MemoryStream outputStream = null;
            var returnObject = new FtpFileResult();
            try
            {
                try
                {
                    outputStream = new MemoryStream();
                    var reqFtp = (FtpWebRequest)WebRequest.Create(new Uri(IncludeNetworkPathDelimeter("ftp://" + this._connectionString + ":" + this._port) + IncludeNetworkPathDelimeter(this._directory) + fileName));
                    reqFtp.Method = WebRequestMethods.Ftp.DownloadFile;
                    reqFtp.UseBinary = true;
                    reqFtp.Credentials = new NetworkCredential(this._userName, this._passWord);
                    ftpResponse = (FtpWebResponse)reqFtp.GetResponse();
                    ftpStream = ftpResponse.GetResponseStream();

                    const int bufferSize = 2048;
                    var buffer = new byte[bufferSize];

                    if (ftpStream != null)
                    {
                        int readCount = ftpStream.Read(buffer, 0, bufferSize);
                        while (readCount > 0)
                        {
                            outputStream.Write(buffer, 0, readCount);
                            readCount = ftpStream.Read(buffer, 0, bufferSize);
                        }
                        returnObject.File = Encoding.UTF8.GetString(outputStream.ToArray());
                        returnObject.SetStatus(new Status("", true));
                    }
                    else
                    {
                        logger.LogEntry(SolutionConstants.CONST_ERROR_RESPONSESTREAMNULL + " - GetFtpFile()");
                    }

                }
                catch (Exception ex)
                {
                    returnObject.SetStatus(new Status(SolutionConstants.CONST_ERROR_ERRORWHENDOWNLOADING + ex, false));
                    logger.LogEntry(SolutionConstants.CONST_ERROR_ERRORWHENDOWNLOADING + ex);

                }
            }
            finally
            {
                if (ftpStream != null) ftpStream.Close();
                if (outputStream != null) outputStream.Close();
                if (ftpResponse != null) ftpResponse.Close();
            }
            return returnObject;
        }

        public static string IncludeNetworkPathDelimeter(string filePath)
        {
            return filePath.EndsWith(@"/") ? filePath : filePath + "/";
        }

        public static string IncludePathDelimeter(string filePath)
        {
            return filePath.EndsWith(@"\") ? filePath : filePath + @"\";
        }

        private Result GetFtpDirectoryFiles(ILogger<string> logger)
        {
            var result = new StringBuilder();
            WebResponse ftpResponse = null;
            StreamReader ftpReader = null;
            var resultObject = new FtpFileListResult();
            try
            {
                var ftpReq = (FtpWebRequest)WebRequest.Create(new Uri(IncludeNetworkPathDelimeter("ftp://" + this._connectionString + ":" + this._port) + IncludeNetworkPathDelimeter(this._directory)));
                ftpReq.UseBinary = true;
                ftpReq.Credentials = new NetworkCredential(this._userName, this._passWord);
                ftpReq.Method = WebRequestMethods.Ftp.ListDirectory;
               
                try
                {
                    ftpResponse = ftpReq.GetResponse();
                    ftpReader = new StreamReader(ftpResponse.GetResponseStream());

                    var line = ftpReader.ReadLine();

                    while (line != null)
                    {
                        result.Append(line);
                        result.Append("\n");
                        line = ftpReader.ReadLine();
                    }
                    // to remove the trailing '\n'
                    result.Remove(result.ToString().LastIndexOf('\n'), 1);
                    resultObject.Items = result.ToString().Split('\n').ToList();
                    resultObject.SetStatus(new Status("", true));
                }
                catch (Exception e)
                {
                    logger.LogEntry(e.ToString());
                    resultObject.SetStatus(new Status("Exception in GetFtpDirectoryFiles : " + e, false));
                }
            }
            finally
            {
                if (ftpReader != null) ftpReader.Close();
                if (ftpResponse != null) ftpResponse.Close();
            }

            return resultObject;
        }
    }
}