using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class CustomSecurityTokenValidator : ISecurityTokenValidator
{
    private readonly IConfiguration _config;

    public bool CanValidateToken => true;

    public int MaximumTokenSizeInBytes { get; set; }

    public bool CanReadToken(string securityToken)
    {
        // This implementation assumes that the security token is a JWT token
        return new JwtSecurityTokenHandler().CanReadToken(securityToken);
    }

    public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
    {
        validatedToken = null;

        var tokenHandler = new JwtSecurityTokenHandler();
        validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _config["Jwt:Issuer"],
            ValidAudience = _config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]))
        };

        try
        {
            // Validate the token using the provided validation parameters
            var principal = tokenHandler.ValidateToken(securityToken, validationParameters, out validatedToken);
            return principal;
        }
        catch (SecurityTokenValidationException)
        {
            // Handle the exception and return null if the token validation fails
            return null;
        }
    }
}
