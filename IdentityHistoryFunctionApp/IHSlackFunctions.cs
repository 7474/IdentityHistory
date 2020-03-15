using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityHistoryFunctionApp.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SlackAPI;

namespace IdentityHistoryFunctionApp
{
    public static class IHSlackFunctions
    {
        private static SlackTaskClient slackClient = new SlackTaskClient(Environment.GetEnvironmentVariable("SlackBotToken"));

        [FunctionName("PullSlackUser")]
        [return: Queue("receive-changed-user")]
        public static async Task<IdentityHistoryFunctionApp.SlackAPI.User> PullSlackUser(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "PullSlackUser")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string user = req.Query["user"];
            log.LogInformation($"user: {user}");

            var res = await slackClient.APIRequestWithTokenAsync<MyUserInfoResponse>(new Tuple<string, string>("user", user));
            var slackUser = res.user;
            log.LogInformation($"slackUser: {JsonConvert.SerializeObject(slackUser)}");

            return slackUser;
        }

        [FunctionName("PullAllSlackUsers")]

        public static async Task PullAllSlackUsers(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "PullAllSlackUsers")] HttpRequest req,
            [Queue("receive-changed-user")]   IAsyncCollector<IdentityHistoryFunctionApp.SlackAPI.User> slackUsersOut,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string next_cursor = "";
            do
            {
                var res = await slackClient.APIRequestWithTokenAsync<MyUsersInfoResponse>(new Tuple<string, string>("cursor", next_cursor));
                foreach (var slackUser in res.members)
                {
                    await slackUsersOut.AddAsync(slackUser);
                    log.LogInformation($"slackUser: {JsonConvert.SerializeObject(slackUser)}");
                }
                next_cursor = res?.response_metadata?.next_cursor;
            } while (!string.IsNullOrEmpty(next_cursor));
        }

        [RequestPath("users.info", true)]
        public class MyUserInfoResponse : Response
        {
            public IdentityHistoryFunctionApp.SlackAPI.User user;
        }

        [RequestPath("users.list", true)]
        public class MyUsersInfoResponse : Response
        {
            public ICollection<IdentityHistoryFunctionApp.SlackAPI.User> members;

            public long cache_ts;
            public ResponseMetadata response_metadata;

            public class ResponseMetadata
            {
                public string next_cursor;
            }
        }

        [FunctionName("ReceiveSlackUser")]
        public static async Task ReceiveSlackUser(
            [QueueTrigger("receive-changed-user", Connection = "")]UserInfo changedUser,
            [CosmosDB(
                databaseName: "IdentityHistory",
                collectionName: "SlackUsers",
                ConnectionStringSetting = "CosmosDBConnection",
                Id = "{Id}",
                PartitionKey = "{TeamId}")] SlackUserEntity userEntity,
            [CosmosDB(
                databaseName: "IdentityHistory",
                collectionName: "SlackUsers",
                ConnectionStringSetting = "CosmosDBConnection",
                Id = "{Id}",
                PartitionKey = "{TeamId}")] IAsyncCollector<SlackUserEntity> userEntityOut,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {JsonConvert.SerializeObject(changedUser)}");

            if (userEntity == null)
            {
                userEntity = new SlackUserEntity()
                {
                    TeamId = changedUser.team_id,
                    Id = changedUser.id,
                };
            }

            var newProfie = JsonConvert.DeserializeObject<UserProfileInfo>(JsonConvert.SerializeObject(changedUser.profile));
            newProfie.Timestamp = DateTimeOffset.UtcNow;

            var existProfile = userEntity.RecentProfile.OrderByDescending(x => x.Timestamp).FirstOrDefault();
            if (existProfile == null || existProfile.hasChange(newProfie))
            {
                log.LogInformation("Profile changed.");

                userEntity.CurrentUser = changedUser;
                userEntity.RecentProfile.Add(newProfie);
                await userEntityOut.AddAsync(userEntity);
            }
        }
    }
}