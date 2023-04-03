using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common
{
    public class FileHandlerAttribute : Attribute
    {
        public FileHandlerAttribute(string name)
        {
            this.Name = name;
        }
        public string[] Extensions { get; set; } 
        public string Name { get; }
    }
}
