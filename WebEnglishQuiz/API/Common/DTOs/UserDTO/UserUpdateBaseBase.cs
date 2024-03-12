namespace API.Common.DTOs.UserDTO
{
    public class UserUpdateBaseBase
    {
        public string Username { get; set; } = null!;
        public bool? Role { get; set; }
        public bool? Status { get; set; }
    }
}
