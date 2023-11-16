using Microsoft.Extensions.Options;
using Trofi.io.Server.Options;

namespace Trofi.io.Server;

public class JwtOptionsSetup : IConfigureOptions<JwtOptions>
{
    private const string ConfigSectionName = "JwtSettings";
    private readonly IConfiguration _configuration;

    public JwtOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(JwtOptions options)
    {
        _configuration.GetSection(ConfigSectionName).Bind(options);
    }
}
