using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Meraki.Dashboard.Api.Helpers;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace Merak.Dashboard.Api.Tetsts.Features.Helpers
{
    [TestFixture]
    public class HttpRequestHelpersTests
    {
        private HttpClient _client;
        private MockHttpMessageHandler _messageHandler;
        
        [SetUp]
        public void Setup()
        {
            _messageHandler = new MockHttpMessageHandler();
            _client = new HttpClient(_messageHandler);
        }

        [Test]
        public async Task WhenHttpException_RetryOccurs()
        {
            var counter = 0;

            _messageHandler.When(HttpMethod.Get, "http://test.local/clients")
                .Respond(HttpStatusCode.TooManyRequests, new Dictionary<string, string>
                {
                    {"Retry-After", "1"}
                }, message =>
                {
                    counter++;
                    return message.Content = null;
                });

            _client.BaseAddress = new Uri("http://test.local");
            var request = new HttpRequestMessage(HttpMethod.Get, "clients");
            await _client.SendRequestAsync(request);

            Assert.AreEqual(4, counter);
        }
    }
}