using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Meraki.Dashboard.Api.Helpers;
using Meraki.Dashboard.Api.Organizations.Models;
using Meraki.Dashboard.Api.Organizations.Queries;

namespace Meraki.Dashboard.Api.Organizations
{
    public class OrganizationsClient
    {
        private readonly HttpClient _client;

        public OrganizationsClient(HttpClient client)
        {
            _client = client;
        }
        
        public async Task<IList<Organization>> GetOrganizationsAsync()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "organizations");

                var response = await _client.SendRequestAsync(request);

                return await response.HandleResponse<IList<Organization>>();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Unable to get organizations. {e.Message}");
            }
        }

        public async Task<IList<Models.ApiRequests>> GetOrganizationApiRequests(string id, GetOrganizationApiRequestsQuery query)
        {
            try
            {
                return await _client.QueryWithPagination<GetOrganizationApiRequestsQuery, ApiRequests>(
                    $"organizations/{id}/apiRequests", query);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<ApiRequestOverview> GetOrganizationApiRequestOver(string id,
            GetOrganizationApiRequestOverviewQuery query)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "organizations".AppendParameters(query));
                
                var response = await _client.SendRequestAsync(request);

                return await response.HandleResponse<ApiRequestOverview>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}