using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common
{
    public class WebHookModel
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public WebHookEventType Events { get; set; }
        public string Version { get; set; } = "1.0";
        public string? Url { get; set; }
    }
}
