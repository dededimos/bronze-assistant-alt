using Microsoft.AspNetCore.Authentication;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.AuthenticationServices
{
    /// <summary>
    /// Provides Authentication to Microsoft Graph , By Adding the MicrosoftEntra (Azure AD access Token to the Http Graph Calls)
    /// </summary>
    public class GraphAuthProvider : IAuthenticationProvider
    {
        private readonly IAuthenticationServiceCustom authService;
        
        public GraphAuthProvider(IAuthenticationServiceCustom authService)
        {
            this.authService = authService;
        }

        public async Task AuthenticateRequestAsync(RequestInformation request,
                                             Dictionary<string, object>? additionalAuthenticationContext = null,
                                             CancellationToken cancellationToken = default)
        {
            // Get the access token
            var authResult = await authService.AuthenticateAsync(new List<string> { "https://graph.microsoft.com/.default" });
            var token = authResult.AccessToken;
            request.Headers.Add("Authorization", $"Bearer {token}");
        }
    }
}
