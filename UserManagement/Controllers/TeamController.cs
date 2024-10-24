using Microsoft.AspNetCore.Mvc;
using UserManagement.DTOs;
using UserManagement.DTOs.TeamDto;
using UserManagement.Service;
using UserManagement.Service.Impl;

namespace UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        /// <summary>
        /// Gets a list of teams with optional filtering and sorting.
        /// </summary>
        /// <param name="filterField">Field to filter by.</param>
        /// <param name="filterValue">Value to filter by.</param>
        /// <param name="sortField">Field to sort by.</param>
        /// <param name="sortValue">Sort order (asc or desc).</param>
        /// <param name="pageNumber">Page number for pagination.</param>
        /// <param name="pageSize">Number of teams per page.</param>
        /// <returns>A ResponseDto containing the list of teams.</returns>
        [HttpGet]
        public async Task<ActionResult<ResponseDto>> GetTeams(
            [FromQuery] string? filterField,
            [FromQuery] string? filterValue,
            [FromQuery] string? sortField,
            [FromQuery] string sortValue = "asc",
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var response = await _teamService.GetTeam(filterField, filterValue, sortField, sortValue, pageNumber, pageSize);
            return Ok(response);
        }

        /// <summary>
        /// Gets a team by their ID.
        /// </summary>
        /// <param name="teamId">The ID of the team.</param>
        /// <returns>A ResponseDto containing the team data.</returns>
        [HttpGet("{teamId:int}")]
        public async Task<ActionResult<ResponseDto>> GetTeamById(int teamId)
        {
            var response = await _teamService.GetTeamById(teamId);
            return response.IsSucceed ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Creates a new team.
        /// </summary>
        /// <param name="teamCreateDto">The team data to create.</param>
        /// <returns>A ResponseDto indicating the result of the operation.</returns>
        [HttpPost]
        public async Task<ActionResult<ResponseDto>> CreateTeam([FromBody] TeamCreateDto teamCreateDto)
        {
            var response = await _teamService.CreateTeam(teamCreateDto);

            if (response.IsSucceed)
            {
                // Assuming response.Result contains a TeamViewDto with a TeamId property
                var teamView = response.Result as TeamViewDto;
                if (teamView == null || teamView.TeamId == null)
                {
                    return BadRequest("Failed to retrieve team ID.");
                }

                return Ok(response);
            }

            return BadRequest(response);
        }


        /// <summary>
        /// Updates an existing team.
        /// </summary>
        /// <param name="id">The ID of the team to update.</param>
        /// <param name="teamUpdateDto">The updated team data.</param>
        /// <returns>A ResponseDto indicating the result of the operation.</returns>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ResponseDto>> UpdateTeam(int id, [FromBody] TeamUpdateDto teamUpdateDto)
        {
            var response = await _teamService.UpdateTeam(id, teamUpdateDto);
            return response.IsSucceed ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Deletes a team by their ID.
        /// </summary>
        /// <param name="id">The ID of the team to delete.</param>
        /// <returns>A ResponseDto indicating the result of the operation.</returns>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ResponseDto>> DeleteTeam(int id)
        {
            var response = await _teamService.DeleteTeam(id);
            return response.IsSucceed ? Ok(response) : BadRequest(response);
        }
    }
}
