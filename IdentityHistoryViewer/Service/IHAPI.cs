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

        public IHAPI(HttpClient http, IHAPIConfig config)
        {
            this.httpClient = http;
            this.config = config;
        }

        public async Task<IList<ListTeam>> ListTeams()
        {
            var res = await httpClient.GetJsonAsync<IList<ListTeam>>(
                $"{config.BaseUri}api/teams"
                );
            return res;
        }

        public async Task<IList<ListUser>> ListUser(string teamId)
        {
            var res = await httpClient.GetJsonAsync<IList<ListUser>>(
                $"{config.BaseUri}api/teams/{teamId}/users"
                );
            return res;
        }

        public async Task<SlackUserEntity> GetUser(string teamId, string userId)
        {
            var res = await httpClient.GetJsonAsync<SlackUserEntity>(
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
