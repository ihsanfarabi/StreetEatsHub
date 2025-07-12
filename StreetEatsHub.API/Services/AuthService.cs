using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using StreetEatsHub.API.Data;
using StreetEatsHub.API.DTOs;
using StreetEatsHub.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StreetEatsHub.API.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterVendorAsync(RegisterVendorDto registerDto);
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
    }

    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            AppDbContext context,
            IMapper mapper,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto?> RegisterVendorAsync(RegisterVendorDto registerDto)
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return null; // User already exists
            }

            // Create Identity User
            var user = new IdentityUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return null; // Failed to create user
            }

            // Create Vendor profile
            var vendor = _mapper.Map<Vendor>(registerDto);
            vendor.UserId = user.Id;

            _context.Vendors.Add(vendor);
            await _context.SaveChangesAsync();

            // Generate JWT token
            var token = GenerateJwtToken(user, vendor.Id);
            var vendorDto = _mapper.Map<VendorListDto>(vendor);

            return new AuthResponseDto
            {
                Token = token.Token,
                Expires = token.Expires,
                Vendor = vendorDto
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return null; // User not found
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
            {
                return null; // Invalid password
            }

            // Get vendor profile
            var vendor = _context.Vendors.FirstOrDefault(v => v.UserId == user.Id);
            if (vendor == null)
            {
                return null; // Vendor profile not found
            }

            // Generate JWT token
            var token = GenerateJwtToken(user, vendor.Id);
            var vendorDto = _mapper.Map<VendorListDto>(vendor);

            return new AuthResponseDto
            {
                Token = token.Token,
                Expires = token.Expires,
                Vendor = vendorDto
            };
        }

        private (string Token, DateTime Expires) GenerateJwtToken(IdentityUser user, int vendorId)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var secretKey = jwtSettings["SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"]!));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim("VendorId", vendorId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expires);
        }
    }
}