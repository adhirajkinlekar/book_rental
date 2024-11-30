using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
// using book_rental_api.Helpers;
using book_rental_api.Models;
using book_rental_api.Data;
using book_rental_api.Exceptions;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using book_rental_api.Data.book_rental_db;
using Microsoft.EntityFrameworkCore;

namespace book_rental_api.Services
{
    public class TokenResponse
    {
        public required string Token { get; set; }
    }

    public interface IAuthService
    {

        public Task<Response<TokenResponse>> Login(LoginUserDTO loginUserDTO);

        public Task<Response<TokenResponse>> Register(RegisterUserDTO user);
    }

    class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;

        private readonly BookRentalDbContext dbContext;

        public AuthService(IConfiguration configuration, BookRentalDbContext bookRentalDbContext)
        {
            _configuration = configuration;
            dbContext = bookRentalDbContext;
        }

        public async Task<Response<TokenResponse>> Login(LoginUserDTO loginUserDTO)
        {
            book_rental_api.Data.book_rental_db.User? user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == loginUserDTO.Email); 

            if (user == null) throw new APIException("User is not registered.", 400);

            bool isAuthenticated = VerifyPasswordHash(loginUserDTO.Password, user.PasswordHash, user.PasswordSalt);

            if (!isAuthenticated) throw new APIException("Please check email or password", 401);

            string token = CreateToken(user.Id.ToString(), user.FullName);

            var response = new Response<TokenResponse>
            {
                Data = new TokenResponse { Token = token },
                Message = "OK Authenticated"
            };

            return response;
        }

        public async Task<Response<TokenResponse>> Register(RegisterUserDTO registeringUser)
        {

            if (await dbContext.Users.AnyAsync(x => x.Email == registeringUser.Email)) throw new APIException("User has already been registered.", 400);

            CreatePasswordHash(registeringUser.Password, out byte[] passwordHash, out byte[] passwordSalt);

            book_rental_api.Data.book_rental_db.User user = new()
            {
                Email = registeringUser.Email,
                FullName = registeringUser.Fullname, 
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            dbContext.Add(user);

            await dbContext.SaveChangesAsync();

            string token = CreateToken(user.Id.ToString(), user.FullName);

            var response = new Response<TokenResponse>
            {
                Data = new TokenResponse { Token = token },
                Message = "OK Authenticated"
            };

            return response;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {

            using var hmac = new HMACSHA512();

            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {

            using var hmac = new HMACSHA512(passwordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            return computedHash.SequenceEqual(passwordHash);
        }

        private string CreateToken(string userId, string userName)
        {

            List<Claim> claims = new List<Claim>()
            {
                new (ClaimTypes.NameIdentifier, userId),
                new (ClaimTypes.Name, userName)
            };

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_configuration.GetSection("appSettings:token").Value!));

            SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
