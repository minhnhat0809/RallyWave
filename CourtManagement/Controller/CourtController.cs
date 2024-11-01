using CourtManagement.DTOs;
using CourtManagement.DTOs.CourtDto;
using CourtManagement.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CourtManagement.Controller
{
    [Route("api/courts")]
    [ApiController]
    public class CourtController(ICourtService courtService) : ControllerBase
    {

        [HttpGet]
        public async Task<ResponseDto> GetCourts(
            [FromQuery] string? filterField,
            [FromQuery] string? filterValue,
            [FromQuery] string? sortField,
            [FromQuery] string sortValue = "asc",
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 5
            )
        {
            var response = await courtService.GetCourts(filterField, filterValue, sortField, sortValue, pageNumber, pageSize);
            return response;
        }

        [HttpGet("{id:int}")]
        public async Task<ResponseDto> GetCourt(int id)
        {
            var response = await courtService.GetCourtById(id);
            return response;
        }

        [HttpPost]
        public async Task<ResponseDto> CreateCourt([FromForm] CourtCreateDto courtCreateDto)
        {
            var response = await courtService.CreateCourt(courtCreateDto);
            return response;
        }

        [HttpPut("{id:int}")]
        public async Task<ResponseDto> UpdateCourt( int id, [FromForm] CourtUpdateDto courtUpdateDto)
        {
            var response = await courtService.UpdateCourt(id, courtUpdateDto);
            return response;
        }
        
        [HttpDelete("{id:int}")]
        public async Task<ResponseDto> DeleteCourt( int id)
        {
            var response = await courtService.DeleteCourt(id);
            return response;
        }

        [HttpDelete("court-images/{imageId:int}")]
        public async Task<ResponseDto> DeleteCourtImages(int imageId)
        {
            var response = await courtService.DeleteCourtImages(imageId);
            return response;
        }
    }
}
