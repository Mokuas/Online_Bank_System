using OnlineBank.Auth.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineBank.Auth.Application.Security
{
    public interface ITokenService
    {
        string CreateAccessToken(User user);
    }
}
