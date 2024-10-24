using MatchManagement.DTOs;
using MatchManagement.DTOs.MatchDto;
using MatchManagement.Service;
using Microsoft.AspNetCore.Mvc;

namespace MatchManagement.Controller
{
    [Route("api/matches")]
    [ApiController]
    public class MatchController(IMatchService matchService) : ControllerBase
    {
        [HttpGet]
        public async Task<ResponseDto> GetMatches(
            [FromQuery] string? subject,
            [FromQuery] int? subjectId,
            [FromQuery] string? filterField,
            [FromQuery] string? filterValue,
            [FromQuery] string? sortField,
            [FromQuery] string sortValue = "asc",
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 5
            )
        {
            var response = await matchService.GetMatches(subject, subjectId, filterField, filterValue, sortField, sortValue, pageNumber, pageSize);
            
            return response;
        }

        [HttpGet("{id:int}")]
        public async Task<ResponseDto> GetMatch(int id)
        {
            var response = await matchService.GetMatchById(id);

            return response;
        }

        [HttpPost("user/{userId:int}")]
        public async Task<ResponseDto> CreateMatch(int userId, [FromBody] MatchCreateDto matchCreateDto)
        {
            var response = await matchService.CreateMatch(userId, matchCreateDto);

            return response;
        }

        [HttpPost("match/{matchId:int}/user/{userId:int}")]
        public async Task<ResponseDto> EnrollUser(int matchId, int userId)
        {
            var response = await matchService.EnrollInMatch(userId, matchId);

            return response;
        }
        
        [HttpPut("match/{matchId:int}/user/{userId:int}")]
        public async Task<ResponseDto> UnEnrollUser(int matchId, int userId)
        {
            var response = await matchService.UnEnrollFromMatch(userId, matchId);

            return response;
        }
        
        [HttpPut("{id:int}")]
        public async Task<ResponseDto> UpdateMatch(int id, [FromBody] MatchUpdateDto matchUpdateDto)
        {
            var response = await matchService.UpdateMatch(id, matchUpdateDto);

            return response;
        }

        [HttpDelete("{id:int}")]
        public async Task<ResponseDto> DeleteMatch(int id)
        {
            var response = await matchService.DeleteMatch(id);

            return response;
        }
    }
}
