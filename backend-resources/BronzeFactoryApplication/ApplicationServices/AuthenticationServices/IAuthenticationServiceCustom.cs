using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.AuthenticationServices
{
    public interface IAuthenticationServiceCustom
    {
        /// <summary>
        /// Authenticates User with the Requested Scopes
        /// </summary>
        /// <param name="scopes"></param>
        /// <returns></returns>
        Task<AuthenticationResult> AuthenticateAsync(IEnumerable<string> scopes);
        /// <summary>
        /// Authenticates User with the Default Scopes (Usually taken from Configuration)
        /// </summary>
        /// <returns></returns>
        Task<AuthenticationResult> AuthenticateAsync();
    }

    public class AuthenticationServiceCustom : IAuthenticationServiceCustom
    {
        private readonly IPublicClientApplication publicClientApp;
        /// <summary>
        /// The Default Scopes of authentication as declared in the service (ex. User.Read , User.Write e.t.c.)
        /// </summary>
        private readonly string[] defaultScopes;

        public AuthenticationServiceCustom(IPublicClientApplication publicClientApp, string[] defaultScopes)
        {
            this.publicClientApp = publicClientApp;
            this.defaultScopes = defaultScopes;
        }

        public async Task<AuthenticationResult> AuthenticateAsync(IEnumerable<string> scopes)
        {
            try
            {
                //Aquire the Access token Silently without any User Interaction
                var accounts = await publicClientApp.GetAccountsAsync();
                return await publicClientApp.AcquireTokenSilent(scopes, accounts.FirstOrDefault()).ExecuteAsync();
            }
            catch (MsalUiRequiredException)
            {
                // If User Interaction is Needed , Aquire the Token with User Interaction
                return await publicClientApp.AcquireTokenInteractive(scopes).ExecuteAsync();
            }
        }

        public async Task<AuthenticationResult> AuthenticateAsync()
        {
            return await AuthenticateAsync(defaultScopes);
        }
    }
}
