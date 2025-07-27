using AccessoriesRepoMongoDB;
using AccessoriesRepoMongoDB.Repositories;
using BathAccessoriesModelsLibrary.AccessoriesUserOptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static AccessoriesRepoMongoDB.Repositories.IMongoAccessoriesDTORepository;

namespace BronzeAPI.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccessoriesController : ControllerBase
    {
        private readonly ILogger<AccessoriesController> _logger;
        private readonly IMongoAccessoriesDTORepository repo;

        public AccessoriesController(ILogger<AccessoriesController> logger,
            IMongoAccessoriesDTORepository repo)
        {
            this._logger = logger;
            this.repo = repo;
        }

        /// <summary>
        /// Returns the Accessories Stash (Contains all Accessories and Trait Classes)
        /// </summary>
        /// <param name="lng">The Language Identifier  (ex. el-GR)</param>
        /// <param name="ignoreCache">Weather to Ignore the Cache of the Repo and directly get the Items from the Database</param>
        /// <param name="rm">The Registered machine if any  (ex. el-GR)</param>
        /// <returns></returns>
        [HttpGet("GetAccessoriesDtoStash", Name = "GetAccessoriesDtoStash")]
        public async Task<IActionResult> GetAccessoriesEntitiesStash([FromQuery] string lng, [FromQuery] bool ignoreCache, [FromQuery] string? rm)
        {
            try
            {
                // Get the Stash without any Options
                var stash = await repo.GetStashAsync(lng,ignoreCache);
                
                // Get the User that logged In
                var user = GetUser(stash.Users);

                //Pass the user to the Stash to get the Stash only for the Specified User or a power users stash for a specific registered machine
                var stashToReturn = stash.GetUsersStash(user,IsPowerUser(),rm ?? "");
                
                return new OkObjectResult(stashToReturn);
            }
            catch (Exception ex)
            {
                string errorMessage = "An Error Occured while Geting the Accessories Stash";
                _logger.LogError(ex, "{errorMessage}", errorMessage);
                return new BadRequestObjectResult(errorMessage);
            }
        }

        /// <summary>
        /// Returns the CustomUser Options from MonogoDB
        /// </summary>
        /// <param name="ignoreCache"></param>
        /// <returns></returns>
        private UserInfoDTO GetUser(IEnumerable<UserInfoDTO> users)
        {
            // If the User is Authenticated return his Options
            if (User?.Identity?.IsAuthenticated == true)
            {
                var objectIdClaim = User.FindFirst(c => c.Type == ObjectIdClaimType);
                UserInfoDTO foundUser = users.FirstOrDefault(u => objectIdClaim?.Value == u.GObjectId) ?? users.FirstOrDefault(u => u.UserName == DefaultUserName) ?? UserInfoDTO.Undefined();
                return foundUser;
            }
            else
            {
                return users.FirstOrDefault(u => u.UserName == DefaultUserName) ?? UserInfoDTO.Undefined();
            }
        }
        
        /// <summary>
        /// Returns weather the User calling the API is a power User
        /// </summary>
        /// <returns></returns>
        private bool IsPowerUser()
        {
            var isPowerUserClaim = User.FindFirst(c => c.Type == "extension_IsPowerUser")?.Value;
            return bool.TryParse(isPowerUserClaim, out bool isPowerUserParsed) && isPowerUserParsed;
        }

    }
}
