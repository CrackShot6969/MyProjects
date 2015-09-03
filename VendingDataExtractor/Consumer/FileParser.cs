using System;
using VendingDataExtractor.Structures;

namespace VendingDataExtractor.Consumer
{
    public interface IParser
    {
        Result CanParse(Object file);
        Result Parse(Object file);
    }

    public class Parser
    {
        readonly IParser _proxy;

        public Parser(IParser injection)
        {
            this._proxy = injection;
        }

        public Result TryParse(object objFile)
        {
            return this._proxy.CanParse(objFile);
        }

        public Result ParseText(Object file)
        {
            return this._proxy.Parse(file);
        }
    }
}
