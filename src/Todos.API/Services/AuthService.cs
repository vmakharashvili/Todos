using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Todos.Core.Dtos.Users;
using Todos.Core.Models;
using Todos.Core.Services.Abstract;

namespace Todos.API.Services;

public class AuthService : IAuthService
{
    private readonly Settings _settings;
    private int _userId = 0;

    public AuthService(Settings settings)
    {
        _settings = settings;
    }

    public void SetUser(int userId)
    {
        _userId = userId;
        
    }

    public int GetUserId()
    {
        return _userId;
    }

    public string GetUserToken(UserDto user)
    {
        if(user?.Username == null)
        {
            throw new ArgumentNullException(nameof(user), "User is null");
        }
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.GivenName, user.Username)
        };
        return new JwtSecurityTokenHandler().WriteToken(GetToken(claims));
    }

    private JwtSecurityToken GetToken(List<Claim> claims)
    {
        if(_settings?.Jwt?.Secret == null)
        {
            throw new ArgumentException("JWT Secret required");
        }
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Jwt.Secret));
        var token = new JwtSecurityToken(expires: DateTime.Now.AddDays(1), claims: claims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));
        return token;
    }
}