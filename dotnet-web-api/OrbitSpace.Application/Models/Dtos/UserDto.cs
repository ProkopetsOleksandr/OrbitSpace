﻿namespace OrbitSpace.Application.Models.Dtos;

public class UserDto
{
    public string? Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}