using DataAccessLib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace BronzeAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class CabinSettingsController : ControllerBase
    {
        private readonly ILogger<CabinSettingsController> _logger;
        private readonly ICabinSettingsRepository cabinSettings;

        public CabinSettingsController(
            ILogger<CabinSettingsController> logger,
            ICabinSettingsRepository cabinSettings)
        {
            _logger = logger;
            this.cabinSettings = cabinSettings;
        }

        /// <summary>
        /// Returns the Default Cabin Settings for all the Different Models , Ver1
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetCabinSettings")]
        public async Task<IActionResult> GetCabinSettings()
        {
            try
            {
                var result = await cabinSettings.GetCabinSettingsDictionaryAsync();
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