using DomainLayer.Exceptions;
using DomainLayer.Exceptions.DomainLayer.Exceptions;
using DomainLayer.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServiceAbstraction;
using Shared.IdentityModule;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AuthenticationService(UserManager<ApplicationUser> _userManager, IConfiguration _configuration, ICloudinaryService _cloudinary) : IAuthenticationService
    {
        public async Task<ReturnUserDTO> RegisterAsync(RegisterDto _registerDto)
        {
            string? profileImagePath = null;

            string roleName = _registerDto.Role.ToString();
            string gender = _registerDto.Gender.ToString();
            string portfolioPath = null;

            try
            {
                var existingUser = await _userManager.FindByEmailAsync(_registerDto.Email);
                if (existingUser is not null)
                {
                    throw new UserAlreadyExistsException(_registerDto.Email);
                }
                profileImagePath = _registerDto.ProfileImage is not null ? await _cloudinary.UploadAsync(_registerDto.ProfileImage) : null;

                portfolioPath = await ExpertOption(_registerDto, portfolioPath);
                var user = CreatingUserName(_registerDto.Email);
                var newUser = new ApplicationUser
                {
                    UserName = user,
                    DisplayName = user,
                    Email = _registerDto.Email,
                    FirstName = _registerDto.FirstName,
                    SecondName = _registerDto.LastName,
                    NormalizedEmail = _registerDto.Email,
                    ProfileImage = profileImagePath,
                    Portfolio = portfolioPath,
                    Gender = (Gender)Enum.Parse(typeof(Gender), gender),
                    YearsOfExperience = _registerDto.YearsOfExperience

                };

                var result = await _userManager.CreateAsync(newUser, _registerDto.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, roleName);
                    return new ReturnUserDTO
                    {
                        Email = newUser.Email,
                        UserName = newUser.UserName,
                        Token = await CreateTokenAsync(newUser)
                    };
                }
                else
                {
                    throw new DomainLayer.Exceptions.InvalidOperationExceptionCustome(result.Errors.Select(e => e.Description).ToList());
                }

            }
            catch (Exception)
            {


                if (profileImagePath is not null)
                {
                    _cloudinary.DeleteAsync(profileImagePath);
                }

                if (portfolioPath is not null)
                {
                    _cloudinary.DeleteAsync(portfolioPath);
                }

                throw;

            }
        }
        public string CreatingUserName(string email)
        {
            return email.Split('@')[0].ToLower().Trim();
        }
        private async Task<string> ExpertOption(RegisterDto _registerDto, string portfolioPath)
        {
            if (_registerDto.Role == RoleType.Expert)
            {


                if (_registerDto.Portfolio is null)
                {
                    throw new InvalidOperationExceptionCustome(new List<string> { "Expert registration requires Portfolio." });
                }

                portfolioPath = await _cloudinary.UploadAsync(_registerDto.Portfolio);
            }

            return (portfolioPath);
        }
        private async Task<string> CreateTokenAsync(ApplicationUser user)
        {
            var claims = new List<Claim>()
            {

                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name,user.UserName!),
                new Claim(ClaimTypes.NameIdentifier,user.Id!),
            };
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)

                claims.Add(new Claim(ClaimTypes.Role, role));
            var secretKey = _configuration.GetSection("JWTOptions")["secretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWTOptions:issuer"],
                audience: _configuration["JWTOptions:audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
                );
            return new JwtSecurityTokenHandler().WriteToken(token);


        }

        public async Task<ReturnUserDTO> LoginAsync(LoginDTO loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user is null)
            {
                throw new UserNotFoundException(loginDto.Email);
            }
            //Check passwords match
            var checkPassword = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (checkPassword)
            {
                return new ReturnUserDTO
                {
                    Email = user.Email,
                    UserName = user.DisplayName,
                    Token = await CreateTokenAsync(user)

                };
            }
            throw new UnauthorizedAException();
        }

    }

}
