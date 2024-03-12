namespace API.JWTAuth
{
    public class JwtResponse
    {
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Token {  get; set; }
        public string Role {  get; set; }
    }
}
