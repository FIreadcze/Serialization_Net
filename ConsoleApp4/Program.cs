using System;
using System.IO;
using Serilog;
using Serilog.Events;
using Newtonsoft.Json;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace ConsoleApp4
{
    /// <summary>
    /// Document object
    /// </summary>
    ///
    public class Document
    {
        public string Title { get; set; }
        public string Text { get; set; }

    }
    public class Program
    {
        static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Debug().WriteTo
                        .File("log.txt").WriteTo
                        .Console(restrictedToMinimumLevel: LogEventLevel.Information)
                        .CreateLogger();
            var builder = new ConfigurationBuilder()
           .AddJsonFile($"appsettings.json", true, true);

            var config = builder.Build();

            Log.Information("App has started");

            //1 missing + -possible error while reading white spaces
            var sourceFileName = Path.Combine(Environment.CurrentDirectory, config["DirectoryAddress:Source"]);
            //XmlTextReader reader1 = new XmlTextReader(config["DirectoryAddress:Source"]);
            //string m = reader1.readto
            var targetFileName = Path.Combine(Environment.CurrentDirectory, config["DirectoryAddress:Target"]);


            if (!Directory.Exists(sourceFileName.Substring(0, sourceFileName.LastIndexOf('\\'))))
            {
                Directory.CreateDirectory(sourceFileName.Substring(0, sourceFileName.LastIndexOf('\\')));
            }
            if (!Directory.Exists(targetFileName.Substring(0, targetFileName.LastIndexOf('\\'))))
            {
              Directory.CreateDirectory(targetFileName.Substring(0, targetFileName.LastIndexOf('\\')));
            }

            try
            {
                if (!File.Exists(sourceFileName)) { Log.Error("Source file was not found"); }
                if (!File.Exists(targetFileName)) {

                    using (var myFile = File.Create(targetFileName))
                    {
                    myFile.Close();
                    }
                    //var myFile = File.Create(targetFileName);

                }
                Document doc = new Document() ;
                switch (Path.GetExtension(config["DirectoryAddress:Source"]))
                {
                    case ".xml":
                        doc=ReadInputXml(sourceFileName,doc);
                        break;
                    case ".json":
                        doc=ReadInputJson(sourceFileName, doc);
                        break;
                    default:
                        break;
                }

                using (StreamWriter file = File.CreateText(targetFileName))
                {
                var serializedDoc = JsonConvert.SerializeObject(doc, Newtonsoft.Json.Formatting.Indented);
                 var token =   JToken.Parse(serializedDoc);
                JsonSerializer se = new JsonSerializer();
                se.Serialize(file, token);

                }


                //var serializedDoc = JsonConvert.SerializeObject(doc);
                //var targetStream = File.Open(targetFileName, FileMode.Create, FileAccess.Write);
                //var sw = new StreamWriter(targetStream);

                //sw.Write(serializedDoc);
                //better to dispose obj
                //targetStream.Dispose();
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message);
                Log.Error(ex, " An error occured, the reason was logged ");
            }
            finally
            {
                Log.CloseAndFlush();
                Log.Information("App has finished");
            }
        }

        public static Document ReadInputXml(string sourceFileName,Document doc)
        {
            FileStream sourceStream = File.Open(sourceFileName, FileMode.Open);
            var reader = new StreamReader(sourceStream);
            string input = reader.ReadToEnd();
            //better to dispose object
            sourceStream.Dispose();
            reader.Dispose();
            //moved to catch setion
            var xdoc = XDocument.Parse(input);
            doc.Title = xdoc.Root.Element("title").Value;
                doc.Text = xdoc.Root.Element("text").Value;
            return doc;
        }
        public static Document ReadInputJson(string sourceFileName, Document doc)
        {

           // StreamReader file = File.OpenText(@"c:\movie.json")
            FileStream sourceStream = File.Open(sourceFileName, FileMode.Open);
            var reader = new StreamReader(sourceStream);
            //string input = reader.ReadToEnd();
            //better to dispose object
            JsonSerializer serializer = new JsonSerializer();
           doc =  (Document)serializer.Deserialize(reader, typeof(Document));
            sourceStream.Dispose();
            reader.Dispose();
            return doc;
            //moved to catch setion
            //doc.Title = xdoc.Root.Element("title").Value;
            //doc.Text = xdoc.Root.Element("text").Value;
            //return doc;
        }
    }
}