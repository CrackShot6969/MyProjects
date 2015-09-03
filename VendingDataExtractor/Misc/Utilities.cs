using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using VendingDataExtractor.Interfaces;

namespace VendingDataExtractor.Misc
{
    class Utilities
    {
        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

        public static string PathAddBackslash(string path)
        {
            var separator1 = Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture);
            var separator2 = Path.AltDirectorySeparatorChar.ToString(CultureInfo.InvariantCulture);

            path = path.Trim();

            if (path.EndsWith(separator1) || path.EndsWith(separator2))
                return path;

            if (path.Contains(separator2))
                return path + separator2;

            return path + separator1;
        }

        public static string XMLSerialize(object MyObject)
        {
            var xsSubmit = new XmlSerializer(MyObject.GetType());
            var settings = new XmlWriterSettings {Encoding = Encoding.UTF8};
            var sww = new StringWriterWithEncoding(new UTF8Encoding());
            var writer = XmlWriter.Create(sww, settings );
            xsSubmit.Serialize(writer, MyObject);
            return sww.ToString();
          
        }

        public static string JSONSerialize(object MyObject)
        {
            return JsonConvert.SerializeObject(MyObject);
        }

        public static object DeserializeObject(Type typeToDeserialize, String fileLocation, ILogger<string> _logger)
        {
            var ser = new XmlSerializer(typeToDeserialize);
            var returnObj = new object();
            try
            {
                using (var xmlreader = XmlReader.Create(fileLocation))
                {
                    returnObj = ser.Deserialize(xmlreader);
                }

            }
            catch (Exception e)
            {
                _logger.LogEntry("Error deserializing(): " + e);
            }

            return returnObj;
        }

        public static object DeserializeObjectByString(Type typeToDeserialize, String xmlString, ILogger<string> _logger)
        {
            var ser = new XmlSerializer(typeToDeserialize);
            var returnObj = new object();
            try
            {
                using (var xmlreader = XmlReader.Create(GenerateStreamFromString(xmlString)))
                {
                    returnObj = ser.Deserialize(xmlreader);
                }

            }
            catch (Exception e)
            {
                _logger.LogEntry("Error deserializing(): " + e);
            }

            return returnObj;
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }

    public class StringWriterWithEncoding : StringWriter
    {
        private readonly Encoding _encoding;

        public StringWriterWithEncoding()
        {
        }

        public StringWriterWithEncoding(IFormatProvider formatProvider)
            : base(formatProvider)
        {
        }

        public StringWriterWithEncoding(StringBuilder sb)
            : base(sb)
        {
        }

        public StringWriterWithEncoding(StringBuilder sb, IFormatProvider formatProvider)
            : base(sb, formatProvider)
        {
        }


        public StringWriterWithEncoding(Encoding encoding)
        {
            _encoding = encoding;
        }

        public StringWriterWithEncoding(IFormatProvider formatProvider, Encoding encoding)
            : base(formatProvider)
        {
            _encoding = encoding;
        }

        public StringWriterWithEncoding(StringBuilder sb, Encoding encoding)
            : base(sb)
        {
            _encoding = encoding;
        }

        public StringWriterWithEncoding(StringBuilder sb, IFormatProvider formatProvider, Encoding encoding)
            : base(sb, formatProvider)
        {
            _encoding = encoding;
        }

        public override Encoding Encoding
        {
            get { return (null == _encoding) ? base.Encoding : _encoding; }
        }
    }
}
