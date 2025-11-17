namespace Irisa.SpecialBonus.Api.Models.Auth
{
    public class LoginRequest
    {
        public string UserNameOrEmail { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
