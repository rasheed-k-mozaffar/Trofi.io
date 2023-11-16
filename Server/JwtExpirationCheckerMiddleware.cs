using System.Text;
using Trofi.io.Server.Options;

namespace Trofi.io.Server;

public class JwtExpirationCheckerMiddleware : IMiddleware
{
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<JwtExpirationCheckerMiddleware> _logger;

    public JwtExpirationCheckerMiddleware(JwtOptions jwtOptions, ILogger<JwtExpirationCheckerMiddleware> logger)
    {
        _jwtOptions = jwtOptions;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // read the token, and see how much time is left for it to expire
        string? token = null;

        if (context.Request.Headers.Authorization.Any())
        {
            token = context.Request.Headers.Authorization.First()?.Split().Last();
        }

        if (!string.IsNullOrEmpty(token))
        {
            var secret = Encoding.UTF8.GetBytes(_jwtOptions.Secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = _jwtOptions.ValidateIssuer,
                ValidateAudience = _jwtOptions.ValidateAudience,
                ValidateLifetime = _jwtOptions.ValidateLifetime,
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(secret),
                RequireExpirationTime = _jwtOptions.RequireExpirationTime,
                ClockSkew = TimeSpan.Zero
            };

            // validate the token
            SecurityToken validatedToken = new JwtSecurityToken();
            var result = tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);

            // if the token's validity is over, log it and forward the request
            if (validatedToken.ValidTo <= DateTime.UtcNow)
            {
                _logger.LogWarning("The token is expired");
                await next.Invoke(context);
            }
            else
            {

                TimeSpan timeLeft = validatedToken.ValidTo - DateTime.UtcNow;
                _logger.LogWarning("The time left for this token is: {0}", timeLeft);

                await next.Invoke(context);
            }
        }
        else
        {
            await next.Invoke(context);
        }
    }
}
