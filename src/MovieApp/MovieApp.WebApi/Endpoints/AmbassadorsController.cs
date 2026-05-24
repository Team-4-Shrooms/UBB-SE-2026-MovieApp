using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Interfaces.Services;


namespace MovieApp.WebApi.Endpoints
{
    [Authorize]
    [ApiController]
    [Route("/api/ambassadors")]
    public class AmbassadorsController : Controller
    {
        private readonly IAmbassadorService _ambassadorService;

        public AmbassadorsController(IAmbassadorService ambassadorService)
        {
            _ambassadorService = ambassadorService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllAmbassadors()
        {
            // NOTE: IAmbassadorService is missing this method!
            return Ok(new List<object>()); // Placeholder
        }

        [HttpGet("{userId:int}")]
        public async Task<IActionResult> GetAmbassador(int userId)
        {
            // NOTE: IAmbassadorService is missing this method!
            return Ok(new { Id = userId }); // Placeholder
        }
    }
}
