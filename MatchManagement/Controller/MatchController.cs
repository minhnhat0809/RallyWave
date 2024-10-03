using MatchManagement.DTOs;
using MatchManagement.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MatchManagement.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController(IMatchService matchService) : ControllerBase
    {
        [HttpGet("matches")]
        public async Task<ResponseDto> GetMatches(
            [FromQuery] string? filterField,
            [FromQuery] string? filterValue,
            [FromQuery] string? sortField,
            [FromQuery] string sortValue = "asc",
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 5
            )
        {
            var response = await matchService.GetMatches(filterField, filterValue, sortField, sortValue, pageNumber, pageSize);
            
            return response;
        }
    }
}
