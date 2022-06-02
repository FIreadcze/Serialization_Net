using ConsoleApp4.Models;
using DeepEqual.Syntax;
using Newtonsoft.Json;
using System;
using System.IO;
using Xunit;

namespace ConsoleApp4
{
    public class UnitTest1
    {


        [Fact]
        public void ReadInputJson_ReturnDeserializedDocument()
        {
            var expected = new Document()
            {
                Title = "titulek",
                Text = "text"
            };
            var inputFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\Source Files\\Document1.json");

            Document actual = ConsoleApp4.Logic.BusinessLogic.ReadInputJson(inputFilePath, new Document());

            expected.ShouldDeepEqual(actual);
        }

        [Fact]
        public void ReadInputXml_ReturnDeserializedDocument()
        {
            var expected = new Document()
            {
                Title = "titulek",
                Text = "text"
            };
            var inputFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\Source Files\\Document1.xml");

            Document actual = ConsoleApp4.Logic.BusinessLogic.ReadInputXml(inputFilePath, new Document());

            expected.ShouldDeepEqual(actual);
        }


        [Fact]
        public void Document_CloneObjShouldBeEqual()
        {

            Document expected = new Document()
            {
                Title = Guid.NewGuid().ToString(),
                Text = Guid.NewGuid().ToString()
            };
            Document actual = expected.DeepCopy();
            expected.ShouldDeepEqual(actual);
        }

        [Fact]
        public void Document_ShouldHaveSameHashCode()
        {

            Document expected = new Document()
            {
                Title = Guid.NewGuid().ToString(),
                Text = Guid.NewGuid().ToString()
            };
            Document actual = expected.DeepCopy();
            expected.ShouldDeepEqual(actual);//objects shoudl be same to compare hasch code
            expected.GetHashCode().ShouldDeepEqual(actual.GetHashCode());
        }
        [Fact]
        public void SerializeInputFromHttp_SerializedDataToDocumentObj()
        {
            var expected = new Document()
            {
                Title = "titulek",
                Text = "text"
            };
            var inputFilePath = "https://localhost:7283/Home/DataApi";

            Document actual = ConsoleApp4.Logic.BusinessLogic.SerializeInputFromHttp(inputFilePath, new Document());

            expected.ShouldDeepEqual(actual);
        }
    }
    public static class ExtensionMethods
    {
        public static T DeepCopy<T>(this T self)
        {
            var serialized = JsonConvert.SerializeObject(self);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}
