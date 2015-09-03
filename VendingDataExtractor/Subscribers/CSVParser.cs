using System;
using System.IO;
using System.Linq;
using CsvHelper;
using VendingDataExtractor.Consumer;
using VendingDataExtractor.Interfaces;
using VendingDataExtractor.Misc;
using VendingDataExtractor.Structures;

namespace VendingDataExtractor.Subscribers
{
    public class CsvParser : IParser
    {
        
        private readonly ILogger<string> _logger ;

        public CsvParser(ILogger<string> logger)
        {
            this._logger = logger;
        }

        public Result CanParse(object file)
        {
            var resultObject = new VendingDataResult();
            try
            {
                var s = file as string;
                if (s != null)
                {
                    var reader = new CsvReader(new StringReader(s));
                    var tempList = reader.GetRecords<VendingDataObject>().ToList();
                    resultObject.SetStatus(new Status(tempList.Count > 0 ? "" : SolutionConstants.CONST_ERROR_NOTHING_RETURNED , tempList.Count > 0));
                }
                else
                {
                    resultObject.SetStatus(new Status(SolutionConstants.CONST_ERROR_PARAMETER_FORMAT_INCORRECT, false));
                }
            }
            catch (Exception e)
            {
                _logger.LogEntry("Error in CanParse() :" + e);
                resultObject.SetStatus(new Status(e.ToString(),false));
            }
            return resultObject;
        }

        public Result Parse(object file)
        {
            
            var returnObject = new VendingDataResult();
            
            try
            {
                var reader = new CsvReader(new StringReader((string)file));
                var tempList = reader.GetRecords<VendingDataObject>().ToList();
                if (tempList.Count == 0)
                {
                    returnObject.SetStatus(new Status(SolutionConstants.CONST_ERROR_NOTHING_RETURNED, true));
                }
                else
                {
                    tempList.ForEach(returnObject.Add);
                    returnObject.SetStatus(new Status("", true));
                }
            }
            catch (Exception e)
            {
                _logger.LogEntry("Error in Parse() :" + e);
                returnObject.SetStatus(new Status(e.ToString(), false));
            }
            return returnObject;
        }
    }


}