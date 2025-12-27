namespace OnlineBank.Auth.Application.Dtos
{
    public class RegisterRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
