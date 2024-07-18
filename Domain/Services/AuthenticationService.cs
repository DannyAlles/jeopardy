using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Data.Models;
using Domain.Exceptions;
using Data.Repositories;
using System.Security.Cryptography;

namespace Domain.Services
{
    public interface IAuthenticationService
    {
        Task<Data.Models.AuthenticationToken> Authenticate(string login, string password);
        Guid? ValidateToken(string token);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _configuration;
        private readonly IProfessorRepository _professorRepository;

        public AuthenticationService(IConfiguration configuration,
                                    IProfessorRepository professorRepository)
        {
            _configuration = configuration;
            _professorRepository = professorRepository;
        }

        public async Task<Data.Models.AuthenticationToken> Authenticate(string login, string password)
        {
            var professor = await _professorRepository.GetProfessorByLogin(login).ConfigureAwait(false);
            if (professor == null)
                throw new ProfessorNotFoundException();

            if (!VerifyHashedPassword(professor.Password, password))
                throw new PasswordsNotEqualsException();

            return CreateAuthenticationToken(professor);
        }

        public Guid? ValidateToken(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:Key"]);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.FromDays(93),
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var inspectorId = Guid.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                // return user id from JWT token if validation successful
                return inspectorId;
            }
            catch
            {
                // return null if validation fails
                return null;
            }
        }

        private Data.Models.AuthenticationToken CreateAuthenticationToken(Professor professor)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new("id", professor.Id.ToString()),
                }),

                Expires = DateTime.UtcNow.AddMonths(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new Data.Models.AuthenticationToken()
            {
                Token = tokenHandler.WriteToken(token),
            };
        }

        private static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            byte[] buffer4;
            if (hashedPassword == null)
            {
                return false;
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            byte[] src = Convert.FromBase64String(hashedPassword);
            if ((src.Length != 0x31) || (src[0] != 0))
            {
                return false;
            }
            byte[] dst = new byte[0x10];
            Buffer.BlockCopy(src, 1, dst, 0, 0x10);
            byte[] buffer3 = new byte[0x20];
            Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
            {
                buffer4 = bytes.GetBytes(0x20);
            }
            return ByteArraysEqual(buffer3, buffer4);
        }
        private static bool ByteArraysEqual(byte[] b1, byte[] b2)
        {
            if (b1 == b2) return true;
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return false;
            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i]) return false;
            }
            return true;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // skip authorization if action is decorated with [AllowAnonymous] attribute
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
                return;

            // authorization
            var user = (Professor)context.HttpContext.Items["User"];
            if (user == null)
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }
}