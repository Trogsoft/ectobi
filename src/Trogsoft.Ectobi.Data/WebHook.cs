using System.Text;
using Trogsoft.Ectobi.Common;

namespace Trogsoft.Ectobi.Data
{
    public class WebHook
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public WebHookEventType Events { get; set; }
        public string Version { get; set; } = "1.0";
        public string? Url { get; set; }
    }
}