using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChattingManagement.DTOs;
using ChattingManagement.DTOs.ConservationDto;
using ChattingManagement.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entity;

namespace ChattingManagement.Controller
{
    [Route("api/conservation")]
    [ApiController]
    public class ConservationController : ControllerBase
    {
        private readonly IConservationService _conservationService;

        public ConservationController(IConservationService conservationService)
        {
            _conservationService = conservationService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ResponseDto>> GetConservations(
            [FromQuery] string? filterField,
            [FromQuery] string? filterValue)
        {
            var response = await _conservationService.GetConservationsByProperties(filterField, filterValue);
            return Ok(response);
        }

        [HttpGet("{conservationId:int}")]
        public async Task<ActionResult<ResponseDto>> GetConservationById(int conservationId)
        {
            var response = await _conservationService.GetConservationByProperties("ConservationId",conservationId.ToString());
            return response!.IsSucceed ? Ok(response) : BadRequest(response);
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDto>> CreateConservation([FromBody] ConservationCreateDto createDto)
        {
            var response = await _conservationService.CreateConservationAsync(createDto);

            return response.IsSucceed ? Ok(response) : BadRequest(response);
        }


        [HttpPut("{id:int}")]
        public async Task<ActionResult<ResponseDto>> UpdateConservation(int id, [FromBody] ConservationUpdateDto updateDto)
        {
            var response = await _conservationService.UpdateConservationAsync(updateDto);
            return response.IsSucceed ? Ok(response) : BadRequest(response);
        }
        
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ResponseDto>> DeleteConservation(int id)
        {
            var response = await _conservationService.DeleteConservationAsync(id);
            return response.IsSucceed ? Ok(response) : BadRequest(response);
        }
        
        [HttpPut("{conservationId:int}/user/{userId:int}")]
        public async Task<ActionResult<ResponseDto>> AddUserToConservation(int conservationId, int userId)
        {
            var response = await _conservationService.AddUserToConservation(conservationId, userId);
            return response.IsSucceed ? Ok(response) : BadRequest(response);
        }
        [HttpDelete("{conservationId:int}/user/{userId:int}")]
        public async Task<ActionResult<ResponseDto>>DeleteUserFromConservation(int conservationId, int userId)
        {
            var response = await _conservationService.DeleteUserOfConservation(conservationId, userId);
            return response.IsSucceed ? Ok(response) : BadRequest(response);
        }
    }
}
