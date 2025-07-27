using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace BronzeArtWebApplication.Shared.Helpers
{
    /// <summary>
    /// An Http Message Handler that Attaches the Access Token also to Calls that are NOT Part of the Base Address
    /// To be USED ONLY DURING DEVELOPMENT In order to Attach Tokens to API Calls where the API Runs Locally in a different Port than the Application
    /// (ex. Application Runs on 5001 whereas API runs on 7065)
    /// </summary>
    public class CustomAuthorizationMessageHandler : DelegatingHandler
    {
        private readonly IAccessTokenProvider tokenProvider;

        public CustomAuthorizationMessageHandler(IAccessTokenProvider tokenProvider)
        {
            this.tokenProvider = tokenProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //Whenever there is a request from the Http Client where this handler is attached to : 

            //Try get the Token
            var tokenResult = await tokenProvider.RequestAccessToken();
            Console.WriteLine("Tok");
            
            // Add it to the Headers of the http Request , to the 'Bearer' Value
            if (tokenResult.TryGetToken(out var token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
                Console.WriteLine("Tik");
            }
            Console.WriteLine("Tokagain");
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
