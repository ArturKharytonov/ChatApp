﻿namespace ChatApp.Domain.DTOs.UserDto
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
