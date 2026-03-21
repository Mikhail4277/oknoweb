using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace api.Services.Auth;

public class AuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IConfig Config;

    public AuthenticationHandler(
        IConfig config,
        IOptionsMonitor<AuthenticationSchemeOptions> options, 
        ILoggerFactory logger, 
        UrlEncoder encoder, 
        ISystemClock clock) 
        : base(options, logger, encoder, clock)
    {
        Config = config;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return AuthenticateResult.Fail("Unauthorized");
        }

        string authorizationHeader = Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authorizationHeader))
        {
            return AuthenticateResult.Fail("Unauthorized");
        }

        if (!authorizationHeader.StartsWith("basic ", StringComparison.OrdinalIgnoreCase))
        {
            return AuthenticateResult.Fail("Unauthorized");
        }

        var token = authorizationHeader.Substring(6);
        var credentialAsString = Encoding.UTF8.GetString(Convert.FromBase64String(token));

        var credentials = credentialAsString.Split(":");
        if (credentials?.Length != 2)
        {
            return AuthenticateResult.Fail("Unauthorized");
        }
        
        var username = credentials[0];
        var password = credentials[1];

        
        return AuthenticateResult.Success(null);
    }
}