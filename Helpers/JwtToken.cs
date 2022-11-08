﻿using CarDealer.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CarDealer.Helpers
{
    public class JwtToken : IJwtToken
    {
        private readonly IConfiguration _configuration;

        public JwtToken(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ClaimsPrincipal GetPrincipal(string token)
        {
            //var TokenValidationParameters = new TokenValidationParameters
            //{
            //    ValidateIssuerSigningKey = true,
            //    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            //    _configuration.GetSection("AppSettings:Token").Value)),
            //    ValidateIssuer = false,
            //    ValidateAudience = false,
            //    ValidateLifetime = false,
            //    ClockSkew = TimeSpan.Zero
            //};
            //var tokenHandler = new JwtSecurityTokenHandler();
            //var principal = tokenHandler.ValidateToken(token, TokenValidationParameters, out SecurityToken securityToken);
            //JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;
            //if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
            //{
            //    throw new SecurityTokenException("Invalid Token");
            //}
            //return principal;
            throw new Exception();
        }
        public Token CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var expiry = DateTime.Now.AddMinutes(20);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: expiry,
                signingCredentials: creds);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = GenerateRefreshToken();
            user.TokenCreated = DateTime.Now;
            user.TokenExpires = expiry;


            return new Token { AccessToken = accessToken, RefreshToken = refreshToken };
        }
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
