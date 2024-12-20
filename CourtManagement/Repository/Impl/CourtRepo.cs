﻿using CourtManagement.DTOs.CourtDto.ViewDto;
using Entity;

namespace CourtManagement.Repository.Impl;

public class CourtRepo(RallyWaveContext repositoryContext) : RepositoryBase<Court>(repositoryContext), ICourtRepo
{
    public async Task<List<CourtsViewDto>> GetCourts(string? filterField, string? filterValue)
    {
        var courts = new List<CourtsViewDto>();
        try
        {
            if (string.IsNullOrEmpty(filterField) || string.IsNullOrEmpty(filterValue))
            {
                courts =  await FindAllAsync(c => new CourtsViewDto(c.CourtId, 
                    c.CourtName, 
                    c.Address, 
                    c.Province, 
                    c.MaxPlayers, 
                    c.Status, 
                    c.Sport!.SportName, 
                    c.CourtImages.Count > 0 ? c.CourtImages.FirstOrDefault()!.ImageUrl : "No images"));
            }
            else
            {
                switch (filterValue.ToLower())
                {
                    case "courtname":
                        courts =  await FindByConditionAsync(c => c.CourtName.Contains(filterValue),
                            c => new CourtsViewDto(c.CourtId, 
                                c.CourtName, 
                                c.Address, 
                                c.Province, 
                                c.MaxPlayers, 
                                c.Status, 
                                c.Sport!.SportName, 
                                c.CourtImages.Count > 0 ? c.CourtImages.FirstOrDefault()!.ImageUrl : "No images"));
                        break;
                    case "address":
                        courts =  await FindByConditionAsync(c => c.Address.Contains(filterValue),
                            c => new CourtsViewDto(c.CourtId, 
                                c.CourtName, 
                                c.Address, 
                                c.Province, 
                                c.MaxPlayers, 
                                c.Status, 
                                c.Sport!.SportName, 
                                c.CourtImages.Count > 0 ? c.CourtImages.FirstOrDefault()!.ImageUrl : "No images"));
                        break;
                    case "province":
                        courts =  await FindByConditionAsync(c => c.Province.Contains(filterValue),
                            c => new CourtsViewDto(c.CourtId, 
                                c.CourtName, 
                                c.Address, 
                                c.Province, 
                                c.MaxPlayers, 
                                c.Status, 
                                c.Sport!.SportName, 
                                c.CourtImages.Count > 0 ? c.CourtImages.FirstOrDefault()!.ImageUrl : "No images"));
                        break;
                    case "sport":
                        if (sbyte.TryParse(filterValue, out var sport))
                        {
                            courts = await FindByConditionAsync(c => c.Sport!.SportId == sport,
                                c => new CourtsViewDto(c.CourtId, 
                                    c.CourtName, 
                                    c.Address, 
                                    c.Province, 
                                    c.MaxPlayers, 
                                    c.Status, 
                                    c.Sport!.SportName, 
                                    c.CourtImages.Count > 0 ? c.CourtImages.FirstOrDefault()!.ImageUrl : "No images"));
                        }
                        break;
                    case "status":
                        if (sbyte.TryParse(filterValue, out var status))
                        {
                            courts =  await FindByConditionAsync(c => c.Status.Equals(status),
                                c => new CourtsViewDto(c.CourtId, 
                                    c.CourtName, 
                                    c.Address, 
                                    c.Province, 
                                    c.MaxPlayers, 
                                    c.Status, 
                                    c.Sport!.SportName, 
                                    c.CourtImages.Count > 0 ? c.CourtImages.FirstOrDefault()!.ImageUrl : "No images"));
                        }
                        break;
                    case "maxplayers":
                        var number = sbyte.Parse(filterValue);
                        courts =  await FindByConditionAsync(c => c.MaxPlayers.Equals(number),
                            c => new CourtsViewDto(c.CourtId, 
                                c.CourtName, 
                                c.Address, 
                                c.Province, 
                                c.MaxPlayers, 
                                c.Status, 
                                c.Sport!.SportName, 
                                c.CourtImages.Count > 0 ? c.CourtImages.FirstOrDefault()!.ImageUrl : "No images"));
                        break;
                }
            }
            
            return courts;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}