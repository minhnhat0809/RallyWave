﻿namespace UserManagement.DTOs.UserDto;

public class UserCreateDto
{
    public string UserName { get; set; } = null!;

    public string? Email { get; set; }

    public int PhoneNumber { get; set; }

    public string Gender { get; set; } = null!;

    public DateOnly Dob { get; set; }

    public string Address { get; set; } = null!;

    public string Province { get; set; } = null!;

    public string? Avatar { get; set; }

    public sbyte Status { get; set; }
}