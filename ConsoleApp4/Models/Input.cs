using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp4.Models
{
    /// <summary>
    /// Input object for recognize info about input
    /// </summary>
    public class Input
    {
        public string Type { get; set; }
        public bool IsHttp { get; set; }
        public string SourcePath { get; set; }
    }
}
