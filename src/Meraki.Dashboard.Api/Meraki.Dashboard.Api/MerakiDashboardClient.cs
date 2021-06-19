using System;
using System.Net.Http;
using Meraki.Dashboard.Api.Organizations;

namespace Meraki.Dashboard.Api
{
    public class MerakiDashboardClient
    {
        private readonly string _baseUrl = "https://api.meraki.com/api/v1/";
        public readonly OrganizationsClient Organizations;
        public MerakiDashboardClient(HttpClient client)
        {
            client.BaseAddress = new Uri(_baseUrl);
            Organizations = new OrganizationsClient(client);
        }
    }
}