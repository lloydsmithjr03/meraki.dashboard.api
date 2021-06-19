using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Polly;
using Polly.Retry;

namespace Meraki.Dashboard.Api.Helpers
{
    public static class HttpRequestHelpers
    {

        public static async Task<HttpRequestMessage> CloneAsync(this HttpRequestMessage request)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri)
            {
                Content = await request.Content.CloneAsync().ConfigureAwait(false),
                Version = request.Version
            };
            foreach (var prop in request.Properties) clone.Properties.Add(prop);
            foreach (var header in request.Headers) clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

            return clone;
        }

        public static async Task<HttpContent> CloneAsync(this HttpContent content)
        {
            if (content == null) return null;

            var ms = new MemoryStream();
            await content.CopyToAsync(ms).ConfigureAwait(false);
            ms.Position = 0;

            var clone = new StreamContent(ms);
            foreach (var header in content.Headers) clone.Headers.Add(header.Key, header.Value);
            return clone;
        }

        public static async Task<T> HandleResponse<T>(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(),
                    new JsonSerializerOptions
                    {
                        IgnoreNullValues = true
                    });
            
            throw new InvalidOperationException(response.ReasonPhrase);
        }

        public static async Task<HttpResponseMessage> SendRequestAsync(this HttpClient httpClient,
            HttpRequestMessage requestMessage)
        {
            return await GetRetryPolicy()
                .ExecuteAsync(async () => await httpClient.SendAsync(await requestMessage.CloneAsync()));
        }

        public static string AppendParameters(this string endPoint, object parameters)
        {
            if (parameters == null) return endPoint;

            foreach (var parameter in DictionaryFromAnonymousObject(parameters))
            {
                if (parameter.Value == null)
                    continue;

                if (!endPoint.Contains("?")) endPoint += "?";

                if (!endPoint.EndsWith("?")) endPoint += "&";

                endPoint += $"{char.ToLowerInvariant(parameter.Key[0]) + parameter.Key.Substring(1)}={parameter.Value}";
            }

            return endPoint;
        }
        
        public static async Task<IList<TResult>> QueryWithPagination<TQuery, TResult>(this HttpClient httpClient, string uri,
            TQuery query)
        {
            var result = new List<TResult>();

            var request = new HttpRequestMessage(HttpMethod.Get, uri.AppendParameters(query));

            var response = await httpClient.SendRequestAsync(request);

            var link = GetNextLink(response.Headers);

            var devices = await response.HandleResponse<IList<TResult>>();

            if (!devices.Any())
                return result;

            result.AddRange(devices);

            if (link == string.Empty) return result;

            var dict = HttpUtility.ParseQueryString(new Uri(link).Query);

            var json = JsonSerializer.Serialize(dict.AllKeys.ToDictionary(k => k, k => dict[k]));

            result.AddRange(await httpClient.QueryWithPagination<TQuery, TResult>(uri,
                JsonSerializer.Deserialize<TQuery>(json)));

            return result;
        }

        private static IEnumerable<KeyValuePair<string, object>> DictionaryFromAnonymousObject(object o)
        {
            var values = new List<KeyValuePair<string, object>>();
            var properties = o.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (Nullable.GetUnderlyingType(property.PropertyType) != null && property.GetValue(o) == null) continue;

                if (property.PropertyType == typeof(string) &&
                    string.IsNullOrWhiteSpace(property.GetValue(o) as string))
                    continue;

                if (property.GetValue(o, null) is IEnumerable enumerable && !(property.GetValue(o, null) is string))
                    foreach (var value in enumerable)
                        values.Add(new KeyValuePair<string, object>(property.Name, value));
                else
                    values.Add(new KeyValuePair<string, object>(property.Name, property.GetValue(o, null)));
            }

            return values;
        }
        private static bool HandleRequest(HttpResponseMessage message)
        {
            return !message.IsSuccessStatusCode;
        }
        
        private static string GetNextLink(HttpResponseHeaders responseHeaders)
        {
            var linkHeader = responseHeaders.FirstOrDefault(x => x.Key == "Link");

            if (linkHeader.Key == null) return string.Empty;

            if (!linkHeader.Value.First().Contains("rel=next")) return string.Empty;

            var nextLink = linkHeader.Value.First().Split(',').First(x => x.Contains("rel=next"))
                .Split(';')[0].Replace("<", string.Empty).Replace(">", string.Empty);

            return nextLink;
        }
        
        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int numRetries = 3, int delayInMilliseconds = 10000)
        {
            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.TooManyRequests ||
                                                    r.StatusCode == HttpStatusCode.ServiceUnavailable)
                .WaitAndRetryAsync(numRetries,
                    (retryCount, response, context) =>
                    {
                        var delay = TimeSpan.FromSeconds(0);
 
                        // if an exception was thrown, this will be null
                        if (response.Result != null)
                        {
                            if (!response.Result.Headers.TryGetValues("Retry-After", out IEnumerable<string> values))
                                return delay;
 
                            if (int.TryParse(values.First(), out int delayInSeconds))
                                delay = TimeSpan.FromSeconds(delayInSeconds);
                        }
                        else
                        {
                            var exponentialBackoff = Math.Pow(2, retryCount);
                            var delayInSeconds = exponentialBackoff * delayInMilliseconds;
                            delay = TimeSpan.FromMilliseconds(delayInSeconds);
                        }
 
                        return delay;
                    },
                    async (response, timespan, retryCount, context) =>
                    {
                        // add your logging and what you want to do
                    }
                );
 
            return retryPolicy;
        }
        
    }
}