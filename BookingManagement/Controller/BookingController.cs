using Microsoft.AspNetCore.Mvc;
using BookingManagement.Service;

namespace BookingManagement.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        
        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetBookings(
            [FromQuery] string? filterField,
            [FromQuery] string? filterValue,
            [FromQuery] string? sortField,
            [FromQuery] string sortValue = "asc",
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 5
        )
        {
            var responseDto = await _bookingService.GetBookings(filterField, filterValue, sortField, sortValue, pageNumber, pageSize);
            return StatusCode(responseDto.StatusCode, responseDto);
        }
    }
}