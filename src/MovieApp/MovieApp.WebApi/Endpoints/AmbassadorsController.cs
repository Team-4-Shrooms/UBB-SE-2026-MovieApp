using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.WebDTOs.DTOs.RequestDTOs;


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
            if (ambassador == null)
            {
                return NotFound();
            }
            return Ok(ambassador);
        }

        [HttpPost("{userId:int}/profile")]
        public async Task<IActionResult> CreateAmbassadorProfile(int userId, [FromBody] CreateAmbassadorProfileRequestBody request)
        {
            await _ambassadorService.CreateAmbassadorProfileAsync(userId, request.Code);
            return Ok();
        }

        [HttpGet("referral/validate")]
        public async Task<IActionResult> ValidateReferralCode([FromQuery] string code)
        {
            var isValid = await _ambassadorService.IsReferralCodeValidAsync(code);
            return Ok(isValid);
        }

        [HttpPost("referral/process")]
        public async Task<IActionResult> ProcessReferral([FromBody] ProcessReferralRequestBody request)
        {
            await _ambassadorService.ProcessReferralAsync(request.ReferralCode, request.FriendId, request.EventId);
            return Ok();
        }

        [HttpGet("{userId:int}/rewards/balance")]
        public async Task<IActionResult> GetRewardBalance(int userId)
        {
            var balance = await _ambassadorService.GetRewardBalanceAsync(userId);
            return Ok(balance);
        }

        [HttpPost("{userId:int}/rewards/redeem")]
        public async Task<IActionResult> RedeemReward(int userId)
        {
            await _ambassadorService.RedeemRewardAsync(userId);
            return Ok();
        }

        [HttpGet("resolve")]
        public async Task<IActionResult> ResolveCodeToUserId([FromQuery] string code)
        {
            var userId = await _ambassadorService.ResolveCodeToUserIdAsync(code);
            return Ok(userId);
        }

        [HttpGet("{ambassadorId:int}/referral-log/exists")]
        public async Task<IActionResult> ReferralLogExists(int ambassadorId, [FromQuery] int friendId, [FromQuery] int eventId)
        {
            var exists = await _ambassadorService.ReferralLogExistsAsync(ambassadorId, friendId, eventId);
            return Ok(exists);
        }

        [HttpPost("referral-log")]
        public async Task<IActionResult> LogReferralByAmbassadorId([FromBody] AddReferralLogRequestBody request)
        {
            await _ambassadorService.LogReferralByAmbassadorIdAsync(request.AmbassadorId, request.FriendId, request.EventId);
            return Ok();
        }

        [HttpPost("{userId:int}/rewards/decrement")]
        public async Task<IActionResult> DecrementRewardBalance(int userId)
        {
            await _ambassadorService.DecrementRewardBalanceAsync(userId);
            return Ok();
        }
    }
}
