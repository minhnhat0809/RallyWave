using BookingManagement.DTOs.BookingDto;
using BookingManagement.DTOs.BookingDto.ViewDto;
using Microsoft.AspNetCore.Mvc;
using BookingManagement.Service;

namespace BookingManagement.Controller
{
    [Route("api/bookings")]
    [ApiController]
    public class BookingController(IBookingService bookingService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetBookings(
            [FromQuery] string? subject,
            [FromQuery] int? subjectId,
            [FromQuery] BookingFilterDto? bookingFilterDto,
            [FromQuery] string? sortField,
            [FromQuery] string sortValue = "asc",
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 5
        )
        {
            var response = await bookingService.GetBookings(subject, subjectId, bookingFilterDto, sortField, sortValue, pageNumber, pageSize);
            return StatusCode(response.StatusCode, response);
        }
        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var response = await bookingService.GetBookingById(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingCreateDto bookingCreateDto)
        {
            var response = await bookingService.CreateBooking(bookingCreateDto);
            return StatusCode(response.StatusCode, response);
        }
        
        [HttpPut("{id:int}")]
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