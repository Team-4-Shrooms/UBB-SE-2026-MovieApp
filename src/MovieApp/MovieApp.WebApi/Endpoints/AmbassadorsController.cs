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
            var ambassadors = await _ambassadorService.GetAllAmbassadorsAsync();
            return Ok(ambassadors);
        }

        [HttpGet("{userId:int}")]
        public async Task<IActionResult> GetAmbassador(int userId)
        {
            var ambassador = await _ambassadorService.GetAmbassadorByIdAsync(userId);
            if (ambassador == null) return NotFound();
            return Ok(ambassador);
        }
    }
}
