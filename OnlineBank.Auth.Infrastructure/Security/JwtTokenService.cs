using System;
using System.Collections.Generic;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OnlineBank.Auth.Application.Security;
using OnlineBank.Auth.Domain.Entities;

namespace OnlineBank.Auth.Infrastructure.Security
{
    public class JwtTokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public JwtTokenService(IConfiguration config)
        {
            _config = config;
        }

        public string CreateAccessToken(User user)
        {
            var jwt = _config.GetSection("Jwt");

            var key = jwt["Key"]!;
            var issuer = jwt["Issuer"]!;
            var audience = jwt["Audience"]!;
            var expiresMinutes = int.Parse(jwt["ExpiresMinutes"]!);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}