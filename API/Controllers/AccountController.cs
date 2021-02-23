using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  public class AccountController : BaseApiController
  {
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    public AccountController(DataContext context, ITokenService tokenService)
    {
      _context = context;
      _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
      if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

      using var hash = new HMACSHA512();

      var newUser = new AppUser
      {
        UserName = registerDto.Username.ToLower(),
        PasswordHash = hash.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
        PasswordSalt = hash.Key
      };

      _context.Users.Add(newUser);

      await _context.SaveChangesAsync();

      var token = _tokenService.CreateToken(newUser);

      return new UserDto
      {
        Username = newUser.UserName,
        Token = token
      };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
      var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username);

      if (user == null) return Unauthorized("Invalid username");

      using var hmac = new HMACSHA512(user.PasswordSalt);

      var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

      for (int i = 0; i < computedHash.Length; i++)
      {
        if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
      }

      var token = _tokenService.CreateToken(user);

      return new UserDto
      {
        Username = user.UserName,
        Token = token
      };
    }

    public async Task<bool> UserExists(string username)
    {
      return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
    }
  }
}