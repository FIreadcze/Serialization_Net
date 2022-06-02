using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConsoleApp4.Models
{
    /// <summary>
    /// Document object to store data to targetpath
    /// </summary>
    [XmlRoot(ElementName = "Data")]
    public class Document
    {
        [XmlElement(ElementName = "Title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "Text")]
        public string Text { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Document document &&
                   Title == document.Title &&
                   Text == document.Text;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Title, Text);
        }
    }

}
