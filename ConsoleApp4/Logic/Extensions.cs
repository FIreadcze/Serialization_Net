using ConsoleApp4.Models;
using ConsoleApp4.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ConsoleApp4.Logic
{
    public static class BusinessLogic
    {
        /// <summary>
        /// Serialize input from http request
        /// </summary>
        /// <param name="sourcePath">Http url for request data, type string</param>
        /// <param name="doc">Output document of type Document </param>
        /// <returns>Updated Document obj of Title prop and Text prop</returns>
        public static Document SerializeInputFromHttp(string sourcePath, Document doc)
        {

            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString(sourcePath);
                var contentType = wc.ResponseHeaders["Content-Type"];
                if (contentType.IndexOf(Constants.jsonContentType) >= 0)
                {
                    doc = JsonConvert.DeserializeObject<Document>(json);
                    return doc;
                }
                XmlRootAttribute xRoot = new XmlRootAttribute();
                xRoot.ElementName = "Data";
                xRoot.IsNullable = true;

                XmlSerializer serializer = new XmlSerializer(typeof(Document), xRoot);

                using (StringReader reader = new StringReader(json))
                {
                    doc = (Document)serializer.Deserialize(reader);
                }
            }

            return doc;
        }

        /// <summary>
        /// Checks whether input is Http or File
        /// </summary>
        /// <param name="inputData">Input object type Input</param>
        /// <param name="config">url/file path, type string </param>
        public static void IsInputHttpOrFile(Input inputData, string config)
        {
            if (Uri.IsWellFormedUriString(config, UriKind.Absolute))
            {
                inputData.IsHttp = true;
                inputData.SourcePath = config;
            }
            else
            {
                inputData.SourcePath = Path.Combine(Environment.CurrentDirectory, config);
                if (!Directory.Exists(inputData.SourcePath.Substring(0, inputData.SourcePath.LastIndexOf('\\'))))
                {
                    Directory.CreateDirectory(inputData.SourcePath.Substring(0, inputData.SourcePath.LastIndexOf('\\')));
                }
            }
        }

        private static readonly HttpClient client = new HttpClient();


        /// <summary>
        ///Checks if output directory exists, if not creates one
        /// </summary>
        /// <param name="outputData">Ouput object, type Output</param>
        public static void CheckDirOutputExistorCreate(Output outputData)
        {
            if (!Directory.Exists(outputData.TargetPath.Substring(0, outputData.TargetPath.LastIndexOf('\\'))))
            {
                Directory.CreateDirectory(outputData.TargetPath.Substring(0, outputData.TargetPath.LastIndexOf('\\')));
            }
        }
        /// <summary>
        ///Checks if input file exists, if not notes info to logger
        /// </summary>
        /// <param name="inputData">Input object, type Input</param>

        public static void CheckInputFilexist(Input inputData)
        {
            if (!File.Exists(inputData.SourcePath)) { Log.Error("Source file was not found"); }
        }

        /// <summary>
        ///Checks if output file exists, if not create one
        /// </summary>
        /// <param name="inputData">Input object, type Input</param>

        public static void CheckOutputFilexist(Output outputData)
        {
            if (!File.Exists(outputData.TargetPath))
            {
                using (var myFile = File.Create(outputData.TargetPath))
                {
                    myFile.Close();
                }
            }
        }

        /// <summary>
        /// Saves data of Document object to a file
        /// </summary>
        /// <param name="config">File path /url for input data, type string</param>
        /// <param name="outputData">Output object, type output </param>
        /// <param name="doc">Output document of type Document </param>
        /// <returns>Updated Document obj of Title prop and Text prop</returns>
        public static void SaveOutputToFile(string config, Output outputData, Document doc)
        {
            using (StreamWriter file = File.CreateText(outputData.TargetPath))
            {

                if (config.ToLower().IndexOf(".xml") >= 0)
                {
                    System.Xml.Serialization.XmlSerializer writer =
                        new System.Xml.Serialization.XmlSerializer(typeof(Document));
                    writer.Serialize(file, doc);
                    file.Close();
                }
                if (config.ToLower().IndexOf(".json") >= 0)
                {
                    var serializedDoc = JsonConvert.SerializeObject(doc, Newtonsoft.Json.Formatting.Indented);
                    var token = JToken.Parse(serializedDoc);
                    JsonSerializer se = new JsonSerializer();
                    se.Serialize(file, token);
                }
                else
                {
                }
            }
        }

        /// <summary>
        /// Reads and process input Xml in case it's from file
        /// </summary>
        /// <param name="sourceFileName">file path, type string</param>
        /// <param name="doc">Output document of type Document </param>
        /// <returns>Updated Document obj of Title prop and Text prop</returns>
        public static Document ReadInputXml(string sourceFileName, Document doc)
        {
            FileStream sourceStream = File.Open(sourceFileName, FileMode.Open);
            var reader = new StreamReader(sourceStream);
            string input = reader.ReadToEnd();
            sourceStream.Dispose();
            reader.Dispose();

            var xdoc = XDocument.Parse(input);
            doc.Title = xdoc.Root.Element("title").Value;
            doc.Text = xdoc.Root.Element("text").Value;
            return doc;
        }

        /// <summary>
        /// Reads and process input Json in case it's from file
        /// </summary>
        /// <param name="sourceFileName">file path, type string</param>
        /// <param name="doc">Output document of type Document </param>
        /// <returns>Updated Document obj of Title prop and Text prop</returns>
        public static Document ReadInputJson(string sourceFileName, Document doc)
        {

            FileStream sourceStream = File.Open(sourceFileName, FileMode.Open);
            var reader = new StreamReader(sourceStream);
            JsonSerializer serializer = new JsonSerializer();
            doc = (Document)serializer.Deserialize(reader, typeof(Document));
            sourceStream.Dispose();
            reader.Dispose();
            return doc;

        }
    }
}
