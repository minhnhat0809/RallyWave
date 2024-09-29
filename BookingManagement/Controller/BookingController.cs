using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {

        [HttpGet("/bookings")]
        public async Task<IActionResult> GetProducts(
            string? filterField,
            string? filterValue,
            string? sortField,
            string? sortValue,
            int? pageNumber = 1,
            int? pageSize = 5
            )
        {

            return null;
        }
    }
}
