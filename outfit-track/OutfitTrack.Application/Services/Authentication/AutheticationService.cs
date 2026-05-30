using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using OutfitTrack.Application.Interfaces;
using OutfitTrack.Application.Security;
using OutfitTrack.Arguments;
using OutfitTrack.Domain.Entities;
using OutfitTrack.Domain.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OutfitTrack.Application.Services;

public class AuthenticationService(IHttpContextAccessor httpContext, IUserRepository userRepository, IUserService userService) : BaseService_0, IAuthenticationService
{
    private readonly HttpContext _httpContext = httpContext.HttpContext!;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserService _userService = userService;

    public OutputAuthentication? Authenticate(InputAuthentication inputAuthentication)
    {
        var user = GetUserByEmail(inputAuthentication.Email!);
        ValidateUserPassword(inputAuthentication.Password!, user.Password!);

        var token = GenerateJwtToken(user.Id.ToString()!, user.Email!);
        _userService.UpdateTokenExpirationDate(user.Id!.Value);

        return new OutputAuthentication(token, DateTime.UtcNow.AddDays(7));
    }

    #region Private Methods
    private User GetUserByEmail(string email)
    {
        var user = _userRepository.Get(x => x.Email == email)
            ?? throw new InvalidOperationException("Usuário não existe. Cadastre seu usuário no endpoint aberto POST '/api/User'");
        return user;
    }

    private static void ValidateUserPassword(string inputPassword, string storedPassword)
    {
        if (!PasswordEncryption.Verify(inputPassword, storedPassword))
            throw new InvalidOperationException("Usuário não autorizado. Senha incorreta.");
    }

    private string GenerateJwtToken(string userId, string userName)
    {
        var claims = CreateTokenClaims(userName);
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKeyJwt.Key));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var expireDate = DateTime.UtcNow.AddDays(7);

        var token = new JwtSecurityToken(
            issuer: _httpContext.Request.Host.Value,
            audience: userId,
            claims: claims,
            expires: expireDate,
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private List<Claim> CreateTokenClaims(string userName)
    {
        return
        [
            new Claim(JwtRegisteredClaimNames.Iss, _httpContext.Request.Host.Value),
            new Claim(JwtRegisteredClaimNames.Sub, userName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];
    }
    #endregion
}