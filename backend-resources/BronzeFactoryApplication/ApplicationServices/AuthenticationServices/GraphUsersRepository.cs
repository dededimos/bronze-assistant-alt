using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.AuthenticationServices
{
    /// <summary>
    /// A Repository for the Microsofts Graphs Users
    /// </summary>
    public class GraphUsersRepository
    {
        private readonly GraphServiceClient client;
        private readonly ILogger<GraphUsersRepository> logger;

        public GraphUsersRepository(GraphServiceClient client, ILogger<GraphUsersRepository> logger)
        {
            this.client = client;
            this.logger = logger;
        }

        public async Task<IEnumerable<GraphUser>> GetAllUsers()
        {
            List<GraphUser> allUsers = new();

            var usersResponse = await client.Users.GetAsync();

            if (usersResponse == null)
            {
                return allUsers;
            }
            // Creates an iterator for all the pages that must be received from graph
            var pageIterator = PageIterator<User, UserCollectionResponse>.CreatePageIterator(
                    client,
                    usersResponse,
                    user =>
                    {
                        allUsers.Add(new GraphUser() { Id = user.Id ?? "Undefined Id", DisplayName = user.DisplayName ?? "Undefined Display Name" });
                        return true;
                    });

            await pageIterator.IterateAsync();

            return allUsers;
        }

    }

    public class GraphUser
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
    }
}
