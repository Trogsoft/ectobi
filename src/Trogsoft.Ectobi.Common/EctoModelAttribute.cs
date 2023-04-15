using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common
{
    public class EctoModelAttribute : Attribute
    {
        public EctoModelAttribute(string title)
        {
            Title = title;
        }

        public string Title { get; }
        public string? Description { get; set; }
    }
}
