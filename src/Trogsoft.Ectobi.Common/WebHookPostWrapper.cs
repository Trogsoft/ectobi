using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common
{
    public class WebHookPostWrapper
    {
        public object Data { get; set; }
        public string EventType { get; set; }
        public int Attempt { get; set; }
        public DateTime Created { get; set; }
    }
}
