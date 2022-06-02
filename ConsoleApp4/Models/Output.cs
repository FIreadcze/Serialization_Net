using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp4.Models
{
    /// <summary>
    /// Output object for identifying info about output
    /// </summary>
    public class Output
    {
        public string Type { get; set; }
        public bool IsHttp { get; set; }
        public string TargetPath { get; set; }
    }
}
