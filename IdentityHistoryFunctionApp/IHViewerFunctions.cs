using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using IdentityHistoryFunctionApp.Entity;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System.Linq;
using System.Collections.Generic;

namespace IdentityHistoryFunctionApp
{
    public static class IHViewerFunctions
    {

        [FunctionName("ListTeams")]
        public static async Task<ActionResult<IList<ListTeam>>> ListTeams(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "teams")] HttpRequest req,
             [CosmosDB(
                databaseName: "IdentityHistory",
                collectionName: "SlackUsers",
                ConnectionStringSetting = "CosmosDBConnection")] IDocumentClient client,
            ILogger log)
        {
            // XXX TeamId はパーティションキーのはずなんだが。
            // これかな？
            // https://github.com/Azure/azure-cosmos-dotnet-v2/issues/720
            /*
             * [2020/03/15 16:30:29] System.Private.CoreLib: Exception while executing function: ListTeams. 
             * Microsoft.Azure.WebJobs.Host: Exception binding parameter 'teams'. 
             * Microsoft.Azure.DocumentDB.Core: Distict query requires a matching order by in order to return a continuation token.
             * If you would like to serve this query through continuation tokens, 
             * then please rewrite the query in the form 
             * 'SELECT DISTINCT VALUE c.blah FROM c ORDER BY c.blah' 
             * and please make sure that there is a range index on 'c.blah'.
            */
            log.LogInformation("C# HTTP trigger function processed a request.");

            // XXX いい感じにパーティション問い合わせができない。。。
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("IdentityHistory", "SlackUsers");
            IDocumentQuery<ListTeam> query = client.CreateDocumentQuery<ListTeam>(collectionUri)
                .AsDocumentQuery();

            // XXX 1件しかワークスペースサポートしていないので1件だけ取っておく。。。ありえん。
            IList<ListTeam> responseList = new List<ListTeam>();
            if (query.HasMoreResults)
            {
                responseList.Add((await query.ExecuteNextAsync<ListTeam>()).First());
            }

            return new OkObjectResult(responseList);
        }
        public class ListTeam
        {
            public string TeamId { get; set; }
        }

        [FunctionName("ListUsers")]
        public static async Task<ActionResult<IList<ListUser>>> ListUsers(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "teams/{teamId}/users")] HttpRequest req,
            string teamId,
             [CosmosDB(
                databaseName: "IdentityHistory",
                collectionName: "SlackUsers",
                ConnectionStringSetting = "CosmosDBConnection")] IDocumentClient client,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // RU消費を避けるにはIDのみ問合せにすればいい？
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("IdentityHistory", "SlackUsers");
            IDocumentQuery<ListUser> query = client.CreateDocumentQuery<ListUser>(collectionUri)
                .Where(p => p.TeamId == teamId)
                .AsDocumentQuery();

            IList<ListUser> responseList = new List<ListUser>();
            while (query.HasMoreResults)
            {
                foreach (ListUser result in await query.ExecuteNextAsync())
                {
                    log.LogInformation(JsonConvert.SerializeObject(result));
                    responseList.Add(result);
                }
            }

            return new OkObjectResult(responseList);
        }

        public class ListUser
        {
            public string TeamId { get; set; }
            [JsonProperty(propertyName: "id")]
            public string UserId { get; set; }
        }

        [FunctionName("GetUser")]
        public static async Task<ActionResult<SlackUserEntity>> GetUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "teams/{teamId}/users/{userId}")] HttpRequest req,
            [CosmosDB(
                databaseName: "IdentityHistory",
                collectionName: "SlackUsers",
                ConnectionStringSetting = "CosmosDBConnection",
                Id = "{userId}",
                PartitionKey = "{teamId}")] SlackUserEntity userEntity,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult(userEntity);
        }
    }
}
