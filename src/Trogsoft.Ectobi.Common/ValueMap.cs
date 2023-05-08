using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common
{
    public class ValueMap 
    {
        public List<string> Headings { get; set; } = new List<string>();
        public List<ValueMapRow> Rows { get; set; } = new();
    }

    public class ValueMapRow : List<string>
    {
    }

    public class ValueMapWithMetadata : ValueMap
    {
        public TimeSpan TimeTaken { get; set; }
        public int TotalRowsForQuery { get; set; }
    }

}
