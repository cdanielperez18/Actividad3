using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;
using TaskManagerAPI.Repositories;

namespace TaskManagerAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto)
        {
            // Verificar si el usuario ya existe
            if (await _userRepository.UsernameExistsAsync(registerDto.Username))
                return null;

            if (await _userRepository.EmailExistsAsync(registerDto.Email))
                return null;

            // Hash de la contraseña
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // Crear usuario
            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = passwordHash
            };

            var createdUser = await _userRepository.CreateAsync(user);

            // Generar token
            var token = GenerateJwtToken(createdUser.Id, createdUser.Username);

            return new AuthResponseDto
            {
                UserId = createdUser.Id,
                Username = createdUser.Username,
                Email = createdUser.Email,
                Token = token
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            // Buscar usuario
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
            if (user == null)
                return null;

            // Verificar contraseña
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                return null;

            // Generar token
            var token = GenerateJwtToken(user.Id, user.Username);

            return new AuthResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Token = token
            };
        }

        public string GenerateJwtToken(int userId, string username)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? "TaskManagerAPI";
            var jwtAudience = _configuration["Jwt:Audience"] ?? "TaskManagerApp";
            var jwtExpireMinutes = int.Parse(_configuration["Jwt:ExpireMinutes"] ?? "60");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(jwtExpireMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
