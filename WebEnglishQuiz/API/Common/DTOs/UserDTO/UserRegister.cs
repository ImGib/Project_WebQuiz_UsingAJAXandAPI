namespace API.Common.DTOs.UserDTO
{
    public class UserRegister
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phonenumber { get; set; }
    }
}
