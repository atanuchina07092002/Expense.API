using ExpenseTracker.Dtos;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Data.Services
{
    public interface IAuthenticationService
    {
        public Task<User> SignUp(UserDto userDto);
        public Task<User> SignIn(UserDto userDto);
    }
    public class AuthenticationService(AppDbContext _context, PasswordHasher<User> passwordHasher) : IAuthenticationService
    {
        public async Task<User> SignIn(UserDto userDto)
        {
                User loginUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == userDto.Email);

            if (loginUser != null)
            {
                var hashPassword = passwordHasher.VerifyHashedPassword(loginUser, loginUser.Password, userDto.Password);

                if (hashPassword == PasswordVerificationResult.Success) {
                    return loginUser;
                }
            }
            return loginUser;
        }

        public async Task<User> SignUp(UserDto userDto)
        {
            User? newUser = null;
            var existingUser = await _context.Users.AnyAsync(u => u.Email == userDto.Email);

            if (existingUser )
            {
                throw new Exception("User with this email already exists.");
            }

            var hashPassword = passwordHasher.HashPassword(null, userDto.Password);
            newUser = new User
            {
              Email = userDto.Email,
              Password = hashPassword,
              CreatedAt = DateTime.Now,
            };
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            return newUser;
            
        }

    }
}
