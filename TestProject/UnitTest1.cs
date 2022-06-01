using ConsoleApp4;
using DeepEqual.Syntax;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using Xunit;

namespace ConsoleApp4
{
    public class UnitTest1
    {
        [Fact]
        public void ReadInputXml_ReturnDeserializedDocument()
        {



            var expected = new Document()
            {
                Title = "neco",
                Text = "no to to"
            };
            var inputFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\Source Files\\Document1.xml");
            //var inputFilePath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\Source Files\\Document1.xml");

            Document actual = ConsoleApp4.Program.ReadInputXml(inputFilePath,new Document());

            expected.ShouldDeepEqual(actual);
        }

        [Fact]
        public void ReadInputJson_ReturnDeserializedDocument()
        {



            var expected = new Document()
            {
                Title = "neco",
                Text = "no to to"
            };
            var inputFilePath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\Source Files\\Document1.json");

            Document actual = ConsoleApp4.Program.ReadInputJson(inputFilePath, new Document());

            expected.ShouldDeepEqual(actual);
        }


    }
}
