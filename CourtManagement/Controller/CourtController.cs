using CourtManagement.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CourtManagement.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourtController(ICourtService courtService) : ControllerBase
    {

        [HttpGet("courts")]
        public async Task<IActionResult> GetCourts(
            [FromQuery] string? filterField,
            [FromQuery] string? filterValue,
            [FromQuery] string? sortField,
            [FromQuery] string sortValue = "asc",
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 5
            )
        {
            var response = await courtService.GetCourts(filterField, filterValue, sortField, sortValue, pageNumber, pageSize);
            return StatusCode(response.StatusCode, response);
        }
    }
}
