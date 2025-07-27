using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System;
using BronzeArtWebApplication.Shared.Models;
using AKSoftware.Localization.MultiLanguages;
using BathAccessoriesModelsLibrary;
using Microsoft.Extensions.Options;
using CommonInterfacesBronze;
using System.Text;
using BathAccessoriesModelsLibrary.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using YamlDotNet.Core.Tokens;
using Blazored.LocalStorage;
using BronzeArtWebApplication.Pages;

namespace BronzeArtWebApplication.Shared.Services
{
    public class APICallService
    {
        private readonly APICallServiceOptions options;
        private readonly HttpClient authedClient;
        private readonly HttpClient anonymousClient;

        /// <summary>
        /// The Current Two Letter Iso Language Prefix
        /// </summary>
        private readonly string isoLangPrefix = "";
        private readonly ILocalStorageService localStorage;
        private readonly BronzeUser user;

        public APICallService(
            IHttpClientFactory httpFactory,
            ILanguageContainerService lc,
            ILocalStorageService localStorage,
            BronzeUser user,
            IOptions<APICallServiceOptions> options)
        {
            isoLangPrefix = lc.CurrentCulture.TwoLetterISOLanguageName;
            this.authedClient = httpFactory.CreateClient(Program.AuthedUserClient);
            this.anonymousClient = httpFactory.CreateClient(Program.AnonymousUserClient);
            this.options = options.Value;
            this.localStorage = localStorage;
            this.user = user;
        }

        /// <summary>
        /// Retrieves the Accessories Stash
        /// </summary>
        /// <returns></returns>
        public async Task<AccessoriesDtoStash> GetAccessoriesStash(bool ignoreCache = false)
        {
            // formulate the Call with its parameters
            var call = options.BronzeApiGetAccessoriesStashCall;
            AccessoriesDtoStash dtoStash;
            // execute it

            //Construct the Parameters and make the Call
            QueryParameterObject lngParam = new(options.LanguageParameterName, LocalizedString.GetFourLetterIsoIdentifier(isoLangPrefix));
            call = GetCallWithParameters(call, lngParam);
            if (ignoreCache)
            {
                QueryParameterObject ignoreCacheParam = new(options.IgnoreCacheParameterName, ignoreCache.ToString());
                call = AddParamToCall(call, ignoreCacheParam);
            }
            
            //Check if power user and Machine is Registered pass the machines number into the call as param to retrieve more info and pricing
            if (user.IsPowerUser)
            {
                var containesRegMachine = await localStorage.ContainKeyAsync(Pages.Index.MachineRegistryStorageKey);
                if (containesRegMachine)
                {
                    var rm = await localStorage.GetItemAsync<string>(Pages.Index.MachineRegistryStorageKey);
                    QueryParameterObject rmParam = new(options.RegisteredMachineParameterName, rm);
                    call = AddParamToCall(call, rmParam);
                }
            }

            //Choose an Http client based on the state of the User
            HttpClient chosenClient;
            if (user.IsAuthenticated)
            {
                chosenClient = authedClient;
            }
            else
            {
                chosenClient = anonymousClient;
            }

            try
            {
                dtoStash = await chosenClient.GetFromJsonAsync<AccessoriesDtoStash>(call);
                return dtoStash;
            }
            //If the user appears as authenticated then the message handler will throw the below exception if the token has expired or is not there for some reason
            //It will prompt the user to log in again , otherwise the user will receive nothing and wont know why
            catch (AccessTokenNotAvailableException tokenEx)
            {
                tokenEx.Redirect();
                return default;
            }
        }

        /// <summary>
        /// Returns the Sas Download Uri of the requested Blob
        /// </summary>
        /// <param name="blobName">The Name of the blob (url without container)</param>
        /// <returns></returns>
        public async Task<string> GetAccessoriesBlobDownloadUri(string blobName)
        {
            var call = options.BronzeApiGetDownloadUri;
            QueryParameterObject blobNameParam = new(options.BlobNameParameterName, blobName);
            call = GetCallWithParameters(call, blobNameParam);
            var sasUri = await authedClient.GetFromJsonAsync<Uri>(call);
            return sasUri.AbsoluteUri;
        }
        private static string GetCallWithParameters(string apiCall, params QueryParameterObject[] parameters)
        {
            StringBuilder builder = new(apiCall);
            // the ? char is the identifier that the params start
            if (parameters.Length > 0)
            {
                builder.Append('?');
            }

            // Build the Parameters
            // after each subsequent param add &
            foreach (var parameter in parameters)
            {
                builder.Append(parameter.QueryParameterName)
                       .Append('=')
                       .Append(parameter.QueryParameterValue)
                       .Append('&');
            }
            // remove the last '&'
            return builder.ToString().TrimEnd('&');
        }

        /// <summary>
        /// Adds a parameter to a certain call string
        /// </summary>
        /// <param name="callString">The String of the call with or without any parameters</param>
        /// <param name="parameter">The Parameter to add</param>
        /// <returns>the New Call String containing the added parameter</returns>
        private static string AddParamToCall(string callString ,QueryParameterObject parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter.QueryParameterName) || string.IsNullOrWhiteSpace(parameter.QueryParameterValue))
            {
                return callString;
            }
            StringBuilder builder = new(callString);
            if (callString.Contains('?'))
            {
                builder.Append('&');
                    
            }
            else
            {
                builder.Append('?');
            }
            builder.Append(parameter.QueryParameterName)
                    .Append('=')
                    .Append(parameter.QueryParameterValue);
            return builder.ToString();
        }
    }


    public class APICallServiceOptions
    {
        /// <summary>
        /// The Base Url to Call BronzeApi
        /// </summary>
        public string BronzeApiBaseUrl { get; set; } = "api";
        public string BronzeApiGetDownloadUri { get; set; } = "api/Blobs/GetBlobSasDownloadUri";
        /// <summary>
        /// The Call to get the Accessories Stash
        /// </summary>
        public string BronzeApiGetAccessoriesStashCall { get; set; } = "api/Accessories/GetAccessoriesDtoStash";
        public string LanguageParameterName { get; set; } = "lng";
        public string BlobNameParameterName { get; set; } = "blobName";
        public string IgnoreCacheParameterName { get; set; } = "ignoreCache";
        public string RegisteredMachineParameterName { get; set; } = "rm";
        /// <summary>
        /// Weather the Current API Call Service is in a Development Environment
        /// </summary>
        public bool IsInDevelopment { get; set; } = false;
    }

    /// <summary>
    /// An Object representing a Query Parameter
    /// </summary>
    public class QueryParameterObject
    {
        /// <summary>
        /// The Name of the Query Parameter
        /// </summary>
        public string QueryParameterName { get; set; }
        /// <summary>
        /// The Value of the Query Parameter
        /// </summary>
        public string QueryParameterValue { get; set; }
        public QueryParameterObject(string queryParameterName, string queryParameterValue)
        {
            QueryParameterName = queryParameterName;
            QueryParameterValue = queryParameterValue;
        }
    }
}
