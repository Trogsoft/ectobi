using Trogsoft.Ectobi.Common;

namespace Trogsoft.Ectobi.Data
{
    public class WebHookEvent
    {
        public long Id { get; set; }
        public string? Url { get; set; }
        public long WebHookId { get; set; }
        public WebHook WebHook { get; set; }
        public bool Success { get; set; }
        public int Attempts { get; set; }
        public DateTime FirstAttempt { get; set; }
        public DateTime MostRecentAttempt { get; set; }
        public DateTime? NextAttempt { get; set; }
        public string? PostData { get; set; }
        public string? LastResponse { get; set; }
        public WebHookEventStatus Status { get; set; }
        public WebHookEventType EventType { get; set; }
    }
}