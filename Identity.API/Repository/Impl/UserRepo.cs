using Entity;
using Identity.API.BusinessObjects.UserViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Identity.API.Repository.Impl;

public class UserRepo(RallyWaveContext repositoryContext) : RepositoryBase<User>(repositoryContext), IUserRepo
{
    private readonly RallyWaveContext _repositoryContext = repositoryContext;

    public async Task<List<UserViewDto>> GetUsers(string? filterField, string? filterValue)
    {
        try
        {
            var users = new List<UserViewDto>();
            switch (filterField?.ToLower())
            {
                case "username":
                    users = await FindByConditionAsync(
                        u => u.UserName.Contains(filterValue!),
                        u => new UserViewDto(
                            u.UserId,
                            u.UserName,
                            u.Email,
                            u.PhoneNumber,
                            u.Gender,
                            u.Dob,
                            u.Address,
                            u.Province,
                            u.Avatar,
                            u.Status,
                            u.CreatedDate));
                    break;

                case "email":
                    users = await FindByConditionAsync(
                        u => u.Email != null && u.Email.Equals(filterValue!),
                        u => new UserViewDto(
                            u.UserId,
                            u.UserName,
                            u.Email,
                            u.PhoneNumber,
                            u.Gender,
                            u.Dob,
                            u.Address,
                            u.Province,
                            u.Avatar,
                            u.Status,
                            u.CreatedDate));
                    break;

                case "phonenumber":
                    if (int.TryParse(filterValue, out var phoneNumber))
                    {
                        users = await FindByConditionAsync(
                            u => u.PhoneNumber == phoneNumber,
                            u => new UserViewDto(
                                u.UserId,
                                u.UserName,
                                u.Email,
                                u.PhoneNumber,
                                u.Gender,
                                u.Dob,
                                u.Address,
                                u.Province,
                                u.Avatar,
                                u.Status,
                                u.CreatedDate));
                    }
                    break;

                case "status":
                    if (sbyte.TryParse(filterValue, out var status))
                    {
                        users = await FindByConditionAsync(
                            u => u.Status == status,
                            u => new UserViewDto(
                                u.UserId,
                                u.UserName,
                                u.Email,
                                u.PhoneNumber,
                                u.Gender,
                                u.Dob,
                                u.Address,
                                u.Province,
                                u.Avatar,
                                u.Status,
                                u.CreatedDate));
                    }
                    break;
            }

            return users;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<UserViewDto?> GetUserById(int userId)
    {
        try
        {
            return await GetByIdAsync(
                userId,
                u => new UserViewDto(
                    u.UserId,
                    u.UserName,
                    u.Email,
                    u.PhoneNumber,
                    u.Gender,
                    u.Dob,
                    u.Address,
                    u.Province,
                    u.Avatar,
                    u.Status,
                    u.CreatedDate));
        }
        catch (Exception e)
        {
            throw new Exception("An error occurred while retrieving the user: " + e.Message);
        }
    }

    public async Task<User> CreateUser(User user)
    {
        try
        {
            await CreateAsync(user);
            return user;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<User> UpdateUser(User user)
    {
        try
        {
            await UpdateAsync(user);
            return user;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<User> DeleteUser(User user)
    {
        try
        {
            await DeleteAsync(user);
            return user;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<User?> GetUserByPropertyAndValue(string property, string value)
    {
        try
        {
            // Validate input parameters
            if (string.IsNullOrWhiteSpace(property) || string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Property and value must not be null or empty.");
            }

            // Normalize the property to lowercase to avoid case-sensitivity issues
            property = property.ToLower();

            // Fetch user based on the property name and value
            return property switch
            {
                
                "email" => await _repositoryContext.Users
                    .FirstOrDefaultAsync(u => u.Email == value),
            
                "phone-number" => await _repositoryContext.Users
                    .FirstOrDefaultAsync(u => u.PhoneNumber.ToString() == value),

                "address" => await _repositoryContext.Users
                    .FirstOrDefaultAsync(u => u.Address == value),

                "province" => await _repositoryContext.Users
                    .FirstOrDefaultAsync(u => u.Province == value),

                "firebase-uid" => await _repositoryContext.Users
                    .FirstOrDefaultAsync(u => u.FirebaseUid == value),

                _ => throw new ArgumentException($"Invalid property name: {property}")
            };
        }
        catch (ArgumentException ex)
        {
            // Handle invalid arguments
            throw new Exception(ex.Message);
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            throw new Exception($"An error occurred while retrieving the user: {ex.Message}");
        }
    }


}




