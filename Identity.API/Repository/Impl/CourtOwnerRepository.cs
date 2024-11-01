using AutoMapper;
using Entity;
using Identity.API.BusinessObjects.CourtOwnerModel;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Repository.Impl;

public interface ICourtOwnerRepository : IRepositoryBase<CourtOwner>
{
    Task<List<CourtOwner>> GetUsers(string? filterField, string? filterValue);

    Task<CourtOwner?> GetUserById(int id);

    Task<CourtOwner> CreateCourtOwner(CourtOwner courtOwner);

    Task<CourtOwner> UpdateCourtOwner(CourtOwner courtOwner);

    Task<CourtOwner> DeleteCourtOwner(CourtOwner courtOwner);
    public Task<CourtOwner?> GetCourtOwnerByPropertyAndValue(string property, string value);
}
public class CourtOwnerRepository(RallyWaveContext repositoryContext) : RepositoryBase<CourtOwner>(repositoryContext), ICourtOwnerRepository
{
     // Get a list of court owners with optional filtering based on field and value
    public async Task<List<CourtOwner>> GetUsers(string? filterField, string? filterValue)
    {
        IQueryable<CourtOwner> query = repositoryContext.CourtOwners;

        if (!string.IsNullOrEmpty(filterField) && !string.IsNullOrEmpty(filterValue))
        {
            // Add filtering based on field and value
            query = filterField.ToLower() switch
            {
                "email" => query.Where(co => co.Email.Contains(filterValue)),
                "phone-number" => query.Where(co => co.PhoneNumber.ToString().Contains(filterValue)),
                "address" => query.Where(co => co.Address.Contains(filterValue)),
                "province" => query.Where(co => co.Province.Contains(filterValue)),
                _ => throw new ArgumentException($"Invalid filter field: {filterField}")
            };
        }

        return await query.ToListAsync();
    }

    // Get a specific court owner by their ID
    public async Task<CourtOwner?> GetUserById(int id)
    {
        var courtOwner = await repositoryContext.CourtOwners
            .Where(co => co.CourtOwnerId == id)
            .FirstOrDefaultAsync();

        return courtOwner;
    }

    // Create a new court owner
    public async Task<CourtOwner> CreateCourtOwner(CourtOwner courtOwner)
    {
        try
        {
            await repositoryContext.CourtOwners.AddAsync(courtOwner);
            await SaveChange();
            return courtOwner;
        }
        catch (Exception ex)
        {
            throw new Exception("Error creating CourtOwner", ex);
        }
    }

    // Update an existing court owner
    public async Task<CourtOwner> UpdateCourtOwner(CourtOwner courtOwner)
    {
        try
        {
            repositoryContext.CourtOwners.Update(courtOwner);
            await SaveChange();
            return courtOwner;
        }
        catch (Exception ex)
        {
            throw new Exception("Error updating CourtOwner", ex);
        }
    }

    // Delete an existing court owner
    public async Task<CourtOwner> DeleteCourtOwner(CourtOwner courtOwner)
    {
        try
        {
            repositoryContext.CourtOwners.Remove(courtOwner);
            await SaveChange();
            return courtOwner;
        }
        catch (Exception ex)
        {
            throw new Exception("Error deleting CourtOwner", ex);
        }
    }

    // Get court owner by a specific property and value (email, phone number, address, etc.)
    public async Task<CourtOwner?> GetCourtOwnerByPropertyAndValue(string property, string value)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(property) || string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Property and value must not be null or empty.");
            }

            property = property.ToLower();

            return property switch
            {
                "email" => await repositoryContext.CourtOwners.FirstOrDefaultAsync(co => co.Email == value),
                "phone-number" => await repositoryContext.CourtOwners.FirstOrDefaultAsync(co => co.PhoneNumber.ToString() == value),
                "address" => await repositoryContext.CourtOwners.FirstOrDefaultAsync(co => co.Address == value),
                "province" => await repositoryContext.CourtOwners.FirstOrDefaultAsync(co => co.Province == value),
                "firebase-uid" => await repositoryContext.CourtOwners.FirstOrDefaultAsync(co => co.FirebaseUid == value),
                _ => throw new ArgumentException($"Invalid property name: {property}")
            };
        }
        catch (ArgumentException ex)
        {
            throw new Exception(ex.Message);
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while retrieving the user: {ex.Message}");
        }
    }
}