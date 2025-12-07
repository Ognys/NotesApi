using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;


namespace NotesApi;

public static class ServiceApi
{
    static public string GenerJwt(string username, int id)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Super_srcurity_very_monster_hight_buny_key"));
        var creden = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("username",username),
            new Claim("id", id.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: "yourApp",
            audience: "yourApp",
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creden
        );

        return new JwtSecurityTokenHandler().WriteToken(token);

    }

    static public int GetIdJwt(HttpContext hc)
    {

        string header = hc.Request.Headers.Authorization.ToString();

        string token = header["Bearer ".Length..];

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        var id = jwt.Claims.First(i => i.Type == "id").Value;

        return Int32.Parse(id);
    }
}