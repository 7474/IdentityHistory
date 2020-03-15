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
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "teams")] HttpRequest req,
            [CosmosDB(
                databaseName: "IdentityHistory",
                collectionName: "SlackUsers",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "SELECT DISTINCT c.TeamId FROM c ORDER BY c.TeamId")]
                IEnumerable<ListTeam> teams,
            ILogger log)
        {
            // XXX TeamId はパーティションキーのはずなんだが。
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

            var responseList = teams.ToList();

            return new OkObjectResult(responseList);
        }
        public class ListTeam
        {
            public string TeamId { get; set; }
        }

        [FunctionName("ListUsers")]
        public static async Task<ActionResult<IList<ListUser>>> ListUsers(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "teams/{teamId}/users")] HttpRequest req,
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
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "teams/{teamId}/users/{userId}")] HttpRequest req,
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
