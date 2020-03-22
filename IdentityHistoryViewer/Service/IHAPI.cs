using BlazorWebAsmEasyAuth;
using IdentityHistoryFunctionApp.Entity;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace IdentityHistoryViewer.Service
{
    public class IHAPIConfig
    {
        public string BaseUri { get; set; }
    }

    public class IHAPI
    {
        private IHAPIConfig config;
        private HttpClient httpClient;
        private EasyAuthAuthenticationStateProvider easyAuthAuthenticationStateProvider;
        private async Task<HttpClient> ApiClient()
        {
            HttpClient authedClient = await easyAuthAuthenticationStateProvider.GetZumoAuthedClientAsync();
            return authedClient ?? httpClient;
        }

        public IHAPI(
            HttpClient http, 
            IHAPIConfig config, 
            EasyAuthAuthenticationStateProvider easyAuthAuthenticationStateProvider)
        {
            this.httpClient = http;
            this.config = config;
            this.easyAuthAuthenticationStateProvider = easyAuthAuthenticationStateProvider;
        }

        public async Task<IList<ListTeam>> ListTeams()
        {
            var res = await (await ApiClient()).GetJsonAsync<IList<ListTeam>>(
                $"{config.BaseUri}api/teams"
                );
            return res;
        }

        public async Task<IList<ListUser>> ListUser(string teamId)
        {
            var res = await (await ApiClient()).GetJsonAsync<IList<ListUser>>(
                $"{config.BaseUri}api/teams/{teamId}/users"
                );
            return res;
        }

        public async Task<SlackUserEntity> GetUser(string teamId, string userId)
        {
            var res = await (await ApiClient()).GetJsonAsync<SlackUserEntity>(
                $"{config.BaseUri}api/teams/{teamId}/users/{userId}"
                );
            return res;
        }
    }

    public class ListTeam
    {
        public string TeamId { get; set; }
    }

    public class ListUser
    {
        public string TeamId { get; set; }
        public string id { get; set; }
    }
}
