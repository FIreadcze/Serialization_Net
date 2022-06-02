using System;
using System.IO;
using Serilog;
using Serilog.Events;
using Newtonsoft.Json;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using System.Xml;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Net.Http;
using ConsoleApp4.Models;
using ConsoleApp4.Utilities;
using ConsoleApp4.Logic;

namespace ConsoleApp4
{
   
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


            Input inputData = new Input();
            Output outputData = new Output();


            Document doc = new Document();

            //checking if the input is Http
            BusinessLogic.IsInputHttpOrFile(inputData, config["DirectoryAddress:Source"]);
          

            outputData.TargetPath = Path.Combine(Environment.CurrentDirectory, config["DirectoryAddress:Target"]);

            
            BusinessLogic.CheckDirOutputExistorCreate(outputData);
           

            try
            {

                if (!inputData.IsHttp) { BusinessLogic.CheckInputFilexist(inputData); }
                BusinessLogic.CheckOutputFilexist(outputData);
              
                if (!inputData.IsHttp)//in case data is from file
                {
                    switch (Path.GetExtension(config["DirectoryAddress:Source"]))
                    {
                        case Constants.xml:
                            doc = BusinessLogic.ReadInputXml(inputData.SourcePath, doc);
                            break;
                        case Constants.json:
                            doc = BusinessLogic.ReadInputJson(inputData.SourcePath, doc);
                            break;
                        default:
                            break;
                    }

                }
                if (inputData.IsHttp)//in case data is from http
                {
                    doc = BusinessLogic.SerializeInputFromHttp(inputData.SourcePath, doc);
                }



                BusinessLogic.SaveOutputToFile(config["DirectoryAddress:Target"],outputData,doc);

              
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
    }
}