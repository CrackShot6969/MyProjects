using System;
using VendingDataExtractor.Structures;

namespace VendingDataExtractor.Consumer
{
    public abstract class RestHelper
    {
        public Method ApiMethod { get; set; }
        public ContentType ContentType { get; set; }
        public String ApiAddress { get; set; }

        protected RestHelper()
        {
            this.ApiMethod = Method.Get;
            this.ContentType = ContentType.ApplicationJson;
            this.ApiAddress = "";
        }

        abstract public object Get(Type serializeType);

        abstract public object Post(object data);

        abstract public object Put(object data);

        abstract public string GetContentTypeString();

        abstract public string GetEndPointFuntionString();

        public abstract string GetEndPointFunctionArgumentString();

        abstract public string GetEndPointFunctionParametersString();

    }
}
