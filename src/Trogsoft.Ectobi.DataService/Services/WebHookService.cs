using Hangfire;
using Newtonsoft.Json;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;
using Trogsoft.Ectobi.Data;

namespace Trogsoft.Ectobi.DataService.Services
{
    public class WebHookService : IWebHookService
    {
        private const int MAX_RETRIES = 5;
        private readonly EctoDb db;
        private readonly ILogger<WebHookService> logger;
        private readonly IHttpClientFactory ihcf;

        public WebHookService(EctoDb db, ILogger<WebHookService> logger, IHttpClientFactory ihcf)
        {
            this.db = db;
            this.logger = logger;
            this.ihcf = ihcf;
        }

        public async Task Dispatch<TEvent>(WebHookEventType type, TEvent webHookEvent)
        {
            List<WebHookEvent> events = new List<WebHookEvent>();

            // todo: this will need restricting to the relevant user/schema/whatever at some point
            foreach (var wh in db.WebHooks.Where(x => x.Events.HasFlag(type)))
            {

                events.Add(new WebHookEvent
                {
                    Attempts = 0,
                    FirstAttempt = DateTime.Now,
                    PostData = JsonConvert.SerializeObject(webHookEvent, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }),
                    Status = WebHookEventStatus.Pending,
                    Success = false,
                    WebHookId = wh.Id,
                    Url = wh.Url,
                    EventType = type
                });

            }

            db.WebHookEvents.AddRange(events);
            await db.SaveChangesAsync();

            events.ForEach(x => BackgroundJob.Enqueue<IWebHookService>(y => y.ExecuteTask(x.Id)));

        }

        public void ExecuteTask(long id)
        {

            var task = db.WebHookEvents.SingleOrDefault(x => x.Id == id);
            if (task == null) return;

            if (task.Status == WebHookEventStatus.RetryPending || task.Status == WebHookEventStatus.Pending)
            {

                task.Attempts += 1;
                task.MostRecentAttempt = DateTime.Now;

                try
                {
                    var httpClient = ihcf.CreateClient("webhook");
                    var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, task.Url);

                    var wrappedPostData = new WebHookPostWrapper();
                    wrappedPostData.EventType = task.EventType.ToString();
                    wrappedPostData.Attempt = task.Attempts;
                    wrappedPostData.Created = task.FirstAttempt;
                    if (!string.IsNullOrWhiteSpace(task.PostData))
                    {
                        var obj = JsonConvert.DeserializeObject<dynamic>(task.PostData);
                        wrappedPostData.Data = obj;
                    }

                    var jsonPostData = JsonConvert.SerializeObject(wrappedPostData);
                    StringContent sc = new StringContent(jsonPostData, System.Text.Encoding.UTF8);
                    httpRequestMessage.Content = sc;
                    var result = httpClient.Send(httpRequestMessage);
                    task.LastResponse = result.Content.ReadAsStringAsync().Result;

                    if (result.IsSuccessStatusCode)
                    {
                        task.Success = true;
                        task.Status = WebHookEventStatus.Success;
                        task.Attempts = 0;
                    }
                    else
                    {
                        RetryOrFail(id, task);
                    }
                }
                catch (Exception ex)
                {
                    RetryOrFail(id, task);
                }

                db.SaveChanges();

            }

        }

        private void RetryOrFail(long id, WebHookEvent? task)
        {

            if (task == null)
            {
                logger.LogWarning("Unable to execute IWebHookService.RetryOrFail because task is null.");
                return;
            }

            if (id == 0)
            {
                logger.LogWarning("Unable to execute IWebHookService.RetryOrFail because id is zero.");
                return;
            }

            task.Status = WebHookEventStatus.RetryPending;
            task.Success = false;
            if (task.Attempts > MAX_RETRIES)
            {
                task.Status = WebHookEventStatus.Failed;
            }
            else
            {
                var ts = TimeSpan.FromMinutes(task.Attempts * 2);
                task.NextAttempt = DateTime.Now.Add(ts);
                BackgroundJob.Schedule<IWebHookService>(x => x.ExecuteTask(id), ts);
            }
        }
    }
}
