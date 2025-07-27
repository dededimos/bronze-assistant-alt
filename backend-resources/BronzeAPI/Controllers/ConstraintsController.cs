using DataAccessLib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace BronzeAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class ConstraintsController : ControllerBase
    {
        private readonly ILogger<ConstraintsController> _logger;
        private readonly ICabinConstraintsRepository constraints;

        public ConstraintsController(
            ILogger<ConstraintsController> logger,
            ICabinConstraintsRepository constraints)
        {
            _logger = logger;
            this.constraints = constraints;
        }

        /// <summary>
        /// Returns the Cabin Constraints for all the Different Models , Ver1
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetCabinConstraints")]
        public async Task<IActionResult> GetCabinConstraints()
        {
            try
            {
                var result = await constraints.GetConstraintsDictionaryAsync();
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