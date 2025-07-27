using DataAccessLib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace BronzeAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class CabinPartsController : ControllerBase
    {
        private readonly ILogger<CabinPartsController> _logger;
        private readonly ICabinPartsRepository partsRepo;

        public CabinPartsController(
            ILogger<CabinPartsController> logger,
            ICabinPartsRepository partsRepo)
        {
            _logger = logger;
            this.partsRepo = partsRepo;
        }

        /// <summary>
        /// Returns the Default PartsLists for all the Different Models , Ver1
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetCabinParts")]
        public async Task<IActionResult> GetCabinParts(string? language)
        {
            try
            {
                var result = await partsRepo.GetAllPartsObjectsAsync(language ?? "EN");
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