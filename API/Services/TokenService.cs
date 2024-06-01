using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;
namespace API.Services;

public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _key;
    public TokenService(IConfiguration config)
    {
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));

    }
    public string CreateToken(Entities.AppUser user)
    {
        var claims = new List<Claim>{
            new Claim(JwtRegisteredClaimNames.NameId,user.UserName)
        };
        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

        var toeknDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(toeknDescriptor);
        return tokenHandler.WriteToken(token);


    }

}
