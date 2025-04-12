using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FssRedact.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Web;

namespace FssRedact.Services
{
    public partial class AddressService
    {
        private readonly HttpClient httpClient;
        private readonly NavigationManager? navigationManager;

        public AddressService(NavigationManager navigationManager, IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient("GetAddress");
            this.navigationManager = navigationManager;
        }

        partial void OnGetAddressList(HttpRequestMessage request);
        partial void OnGetAddressListResponse(HttpResponseMessage response);

        public async Task<IEnumerable<Address>> GetAddressList(string addr, string token)
        {
            var uri = new Uri(httpClient.BaseAddress, $"parse");

            var uriBuilder = new UriBuilder(uri);
            var qurString = HttpUtility.ParseQueryString(uriBuilder.Query);

            qurString.Add("addressText", $"{addr}");
            qurString.Add("token", $"{token}");

            uriBuilder.Query = qurString.ToString();
            uri = uriBuilder.Uri;

            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetAddressList(request);

            var response = await httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            OnGetAddressListResponse(response);

            return await response.Content.ReadFromJsonAsync<IEnumerable<Address>>();
        }
    }
}