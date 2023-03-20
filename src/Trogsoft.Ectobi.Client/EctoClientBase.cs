using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Trogsoft.Ectobi.Common;

namespace Trogsoft.Ectobi.Client
{
    public class EctoClientBase
    {
        protected HttpClient client;

        public EctoClientBase(HttpClient client)
        {
            this.client = client;
        }

        protected Success PostJson(string requestUri, object data)
        {

            StringContent sc = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            request.Content = sc;

            var send = client.Send(request);

            var result = send.Content.ReadFromJsonAsync<Success>().Result;
            if (result == null) return Success.Error("Response was empty.");
            return result;

        }

        /// <summary>
        /// Post JSON to an endpoint.
        /// </summary>
        /// <param name="requestUri">The endpoint</param>
        /// <param name="data">The data to encode and send.</param>
        /// <returns>A Success result indicating success or failure.</returns>
        protected async Task<Success> PostJsonAsync(string requestUri, object data)
        {

            StringContent sc = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            request.Content = sc;

            var send = await client.SendAsync(request);

            var result = await send.Content.ReadFromJsonAsync<Success>();
            if (result == null) return Success.Error("Response was empty.");
            return result;

        }

        protected async Task<Success> DeleteAsync(string requestUri) {
            var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
            var send = await client.SendAsync(request);
            var result = await send.Content.ReadFromJsonAsync<Success>();
            if (result == null) return Success.Error("Response was empty.");
            return result;
        }

        protected async Task<Success> GetAsync(string requestUri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var send = await client.SendAsync(request);
            var result = await send.Content.ReadFromJsonAsync<Success>();
            if (result == null) return Success.Error("Response was empty.");
            return result;
        }


        /// <summary>
        /// Post JSON to an endpoint and receive a JSON result of the specified type.
        /// </summary>
        /// <typeparam name="TDestination">The type of the result item.</typeparam>
        /// <param name="requestUri">The endpoint</param>
        /// <param name="data">The data to encode and send.</param>
        /// <returns>A result object of the type <cref>TDestination</cref></returns>
        protected async Task<Success<TDestination>> PostJsonAsync<TDestination>(string requestUri, object data)
        {
            
            StringContent sc = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            request.Content = sc;

            var send = await client.SendAsync(request);

            var result = await send.Content.ReadFromJsonAsync<Success<TDestination>>();
            if (result == null) return Success<TDestination>.Error("Response was empty.");
            return result;

        }

        protected async Task<Success<T>> GetAsync<T>(string requestUri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var send = await client.SendAsync(request);
            var result = await send.Content.ReadFromJsonAsync<Success<T>>();
            if (result == null) return Success<T>.Error("Response was empty.");
            return result;
        }

    }
}