using BookingManagement.DTOs.BookingDto;
using Microsoft.AspNetCore.Mvc;
using BookingManagement.Service;

namespace BookingManagement.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController(IBookingService bookingService) : ControllerBase
    {
        [HttpGet("bookings/user/{userId:int}")]
        public async Task<IActionResult> GetBookings(
            int userId,
            [FromQuery] string? filterField,
            [FromQuery] string? filterValue,
            [FromQuery] string? sortField,
            [FromQuery] string sortValue = "asc",
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 5
        )
        {
            var response = await bookingService.GetBookings(userId, filterField, filterValue, sortField, sortValue, pageNumber, pageSize);
            return StatusCode(response.StatusCode, response);
        }
        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var response = await bookingService.GetBookingById(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("bookings")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingCreateDto bookingCreateDto)
        {
            var response = await bookingService.CreateBooking(bookingCreateDto);
            return StatusCode(response.StatusCode, response);
        }
        
        [HttpPut("bookings/{id:int}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] BookingUpdateDto bookingUpdateDto)
        {
            var response = await bookingService.UpdateBooking(id, bookingUpdateDto);
            return StatusCode(response.StatusCode, response);
        }
        
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var response = await bookingService.DeleteBooking(id);
            return StatusCode(response.StatusCode, response);
        }
        
    }
}