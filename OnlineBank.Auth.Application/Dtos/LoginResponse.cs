using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineBank.Auth.Application.Dtos
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = null!;
    }
}
