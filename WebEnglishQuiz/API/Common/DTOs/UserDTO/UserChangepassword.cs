﻿namespace API.Common.DTOs.UserDTO
{
    public class UserChangepassword
    {
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }
}
