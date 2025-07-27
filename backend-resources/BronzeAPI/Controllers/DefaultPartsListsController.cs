using DataAccessLib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace BronzeAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class DefaultPartsListsController : ControllerBase
    {
        private readonly ILogger<DefaultPartsListsController> _logger;
        private readonly ICabinPartsListsRepository partsLists;

        public DefaultPartsListsController(
            ILogger<DefaultPartsListsController> logger,
            ICabinPartsListsRepository partsLists)
        {
            _logger = logger;
            this.partsLists = partsLists;
        }

        /// <summary>
        /// Returns the Default PartsLists for all the Different Models , Ver1
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetCabinDefaultPartsLists")]
        public async Task<IActionResult> GetCabinDefaultPartsLists()
        {
            try
            {
                var result = await partsLists.GetPartsListsDictionaryAsync();
                await Task.Delay(1);
                return new OkObjectResult(result.ToList());
            }
            catch (Exception ex)
            {
                string errorMessage = "An Error Occured while Getting Cabin Constraints";
                _logger.LogError(ex, "{errorMessage}", errorMessage);
                return new BadRequestObjectResult(errorMessage);
            }
        }
    }
}