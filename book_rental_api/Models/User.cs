using System.Collections.Generic;

namespace book_rental_api.Models
{
    // https://www.pingidentity.com/en/resources/blog/post/encryption-vs-hashing-vs-salting.html

    public class User
    {
        public int Id { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public required string Email { get; set; }

        public required byte[] PasswordHash { get; set; }

        public required byte[] PasswordSalt { get; set; }
    }

    public class RegisterUserDTO
    {
        public required string Fullname { get; set; } 

        public required string Email { get; set; }

        public required string Password { get; set; }
    }

    public class LoginUserDTO
    {
        public required string Email { get; set; }

        public required string Password { get; set; }
    }

    public static class Users
    {
        public static List<User> AppUsers = new()
        {
            // Password - personal
            new User
            {
                Id = 1,
                FirstName = "Adhiraj",
                LastName = "Kinlekar",
                Email = "adhirajkkinlekar@gmail.com",
                PasswordHash = new byte[] { 139, 195, 88, 183, 106,69,132,6,161,240,50,241,222,67,219,109,89,49,218,242,35,130,27,124,116,235,98,121,13,40,8,240,110,69,56,133,241,13,189,146,107,9,104,33,56,90,96,189,79,124,229,241,172,118,164,84,207,102,92,181,60,251,196,158 },
                PasswordSalt = new byte[] { 68,31,85,2,30,215,248,64,148,156,31,232,150,81,242,33,191,27,159,62,163,19,12,166,249,190,13,177,58,114,55,216,174,37,236,15,224,248,86,213,173,169,173,44,55,245,224,129,140,48,69,255,43,152,100,196,77,104,131,100,177,247,211,194,250,137,3,187,150,95,51,221,141,195,123,61,169,56,43,246,137,248,166,157,22,243,196,7,41,189,156,94,181,152,15,125,85,142,109,117,92,232,46,178,32,149,121,103,72,117,45,208,15,138,0,176,30,17,97,165,129,98,84,19,187,25,117,112 }
            }
        };
    }
}