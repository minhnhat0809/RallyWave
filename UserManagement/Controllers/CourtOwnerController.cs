using Microsoft.AspNetCore.Mvc;
using UserManagement.DTOs;
using UserManagement.Repository.Impl;
using UserManagement.Service;
using UserManagement.Service.Impl;

namespace UserManagement.Controllers;


    [Route("api/court-owner")]
    [ApiController]
    public class CourtOwnerController : ControllerBase
    {
        private readonly ICourtOwnerService _service;

        public CourtOwnerController(ICourtOwnerService service)
        {
            _service = service;
        }


        /// <summary>
        /// Gets a list of users with optional filtering and sorting.
        /// </summary>
        /// <param name="filterField">Field to filter by.</param>
        /// <param name="filterValue">Value to filter by.</param>
        /// <param name="sortField">Field to sort by.</param>
        /// <param name="sortValue">Sort order (asc or desc).</param>
        /// <param name="pageNumber">Page number for pagination.</param>
        /// <param name="pageSize">Number of users per page.</param>
        /// <returns>A ResponseDto containing the list of users.</returns>
        [HttpGet]
        public async Task<ActionResult<ResponseDto>> GetCourtOwners(
            [FromQuery] string? filterField,
            [FromQuery] string? filterValue,
            [FromQuery] string? sortField,
            [FromQuery] string sortValue = "asc",
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var response =
                await _service.GetAllCourtOwnersAsync(filterField, filterValue, sortField, sortValue, pageNumber, pageSize);
            return Ok(response);
        }
        
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ResponseDto>> GetCourtOwnerById(int id)
        {
            var response = await _service.GetCourtOwnerByIdAsync(id);
            return response.IsSucceed ? Ok(response) : BadRequest(response);
        }

    }
