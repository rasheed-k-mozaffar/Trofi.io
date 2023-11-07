using System.Text;
using Microsoft.AspNetCore.Routing.Template;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;

namespace Trofi.io.Server.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;
    public AuthRepository
    (
        UserManager<AppUser> userManager,
        IConfiguration configuration,
        AppDbContext context
    )
    {
        _userManager = userManager;
        _configuration = configuration;
        _context = context;
    }

    /// <summary>
    /// Logs in the user by creating a token if the provided credentials match that of the user
    /// </summary>
    /// <param name="request"></param>
    /// <returns>A success message, success status and the jwt token</returns>
    public async Task<UserManagerResponse> LoginUserAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email!);

        if (user is not null)
        {
            var passwordCheckResult = await _userManager.CheckPasswordAsync(user, request.Password!);

            if (passwordCheckResult)
            {
                // Create a JWT for the user
                string token = CreateJwtToken(user);

                // check to see if the user still has any active refresh tokens
                if (CheckActiveRefreshTokensForUser(user))
                {
                    // in this case, retrieve the still active refresh token, and give it back to the user
                    var activeRefreshToken = user.RefreshTokens!
                    .FirstOrDefault(rt => rt.IsActive == true);

                    return new UserManagerResponse
                    {
                        Message = "Logged in successfully",
                        JWT = token,
                        IsSuccess = true,
                        RefreshToken = activeRefreshToken!.Token,
                        RefreshTokenExpiration = activeRefreshToken.ExpiresOn
                    };
                }
                else
                {
                    // the user doesn't have any still active refresh tokensm
                    // so create a new one, and update the database to accommodate the new token
                    var refreshToken = GenerateRefreshToken();
                    user.RefreshTokens!.Add(refreshToken);

                    await _userManager.UpdateAsync(user);
                    return new UserManagerResponse
                    {
                        Message = "Logged in successfully",
                        JWT = token,
                        IsSuccess = true,
                        RefreshToken = refreshToken.Token,
                        RefreshTokenExpiration = refreshToken.ExpiresOn
                    };
                }
            }
            else
            {
                // the user entered incorrect credentials
                return new UserManagerResponse
                {
                    Message = "Incorrect email or password",
                    IsSuccess = false,
                };
            }
        }
        else
        {
            return new UserManagerResponse
            {
                Message = "No user exists with this email",
                IsSuccess = false,
            };
        }
    }

    /// <summary>
    /// Adds a new user to the database 
    /// </summary>
    /// <param name="request"></param>
    /// <returns>A success message and success status</returns>
    public async Task<UserManagerResponse> RegisterUserAsync(RegisterRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email!);

        if (user is not null)
        {
            return new UserManagerResponse
            {
                Message = "Email already used",
                IsSuccess = false,
            };
        }
        else
        {
            // Map the request to a new App User
            var appuser = new AppUser
            {
                UserName = request.Username,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Location = request.Location,
            };

            var result = await _userManager.CreateAsync(appuser, request.Password!);

            if (result.Succeeded)
            {
                var cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    AppUserId = appuser.Id,

                };
                appuser.CartId = cart.Id;

                await _context.Carts.AddAsync(cart);
                await _context.SaveChangesAsync();

                return new UserManagerResponse
                {
                    Message = "You've been successfully registered",
                    IsSuccess = true,
                };
            }
            return new UserManagerResponse
            {
                Message = "Something went wrong while registering new user",
                IsSuccess = false,
            };
        }
    }


    /// <summary>
    /// Allows the user to request chaning their current password to a new password
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<UserManagerResponse> UpdatePasswordAsync(ChangePasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email!);

        if (user is not null)
        {
            var result = await _userManager.CheckPasswordAsync(user, request.OldPassword!);

            if (result) // the user entered their correct password
            {
                var passwordChangingResult = await _userManager.ChangePasswordAsync(user, request.OldPassword!, request.NewPassword!);

                if (passwordChangingResult == IdentityResult.Success) // the change succeeded
                {
                    return new UserManagerResponse
                    {
                        IsSuccess = true,
                        Message = "Your password have been changed!"
                    };
                }
                else // the change failed
                {
                    return new UserManagerResponse
                    {
                        IsSuccess = true,
                        Message = "Something went wrong while attempting to change your password"
                    };
                }
            }
            else // the user entered incorrect password
            {
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "The password you entered is incorrect"
                };
            }
        }
        else // the email doesn't belong to the user
        {
            return new UserManagerResponse
            {
                IsSuccess = false,
                Message = "User not found"
            };
        }
    }



    public async Task<UserManagerResponse> RefreshTokenAsync(string token)
    {
        // check the validity of the received refresh token
        var user = await _userManager.Users
        .SingleOrDefaultAsync(u => u.RefreshTokens!.Any(rt => rt.Token == token));

        if (user is null)
        {
            return new UserManagerResponse
            {
                Message = "Invalid token",
                IsSuccess = false
            };
        }

        var refreshToken = user.RefreshTokens!.Single(t => t.Token == token);

        if (!refreshToken.IsActive)
        {
            return new UserManagerResponse
            {
                Message = "Inactive token",
                IsSuccess = false
            };
        }

        refreshToken.RevokedOn = DateTime.UtcNow;

        var newRefreshToken = GenerateRefreshToken();
        user.RefreshTokens!.Add(newRefreshToken);

        await _userManager.UpdateAsync(user);
        var jwtToken = CreateJwtToken(user);

        return new UserManagerResponse
        {
            Message = "Refreshed successfully",
            JWT = jwtToken,
            IsSuccess = true,
            RefreshToken = newRefreshToken.Token,
            RefreshTokenExpiration = newRefreshToken.ExpiresOn
        };
    }

    public async Task<bool> RevokeTokenAsync(string token)
    {
        // check the validity of the received refresh token
        var user = await _userManager.Users
        .SingleOrDefaultAsync(u => u.RefreshTokens!.Any(rt => rt.Token == token));

        if (user is null)
            return false;


        var refreshToken = user.RefreshTokens!.Single(t => t.Token == token);

        if (!refreshToken.IsActive)
            return false;


        refreshToken.RevokedOn = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);
        return true;
    }

    /// <summary>
    /// Generates a JWT for the user with the claims
    /// </summary>
    /// <param name="user"></param>
    /// <returns>The jwt for the provided user</returns>
    private string CreateJwtToken(AppUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
        var expiresIn = DateTime.UtcNow.AddSeconds(int.Parse(_configuration["JwtSettings:ExpiresIn"]!));

        List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.GivenName, user.FirstName!),
                new Claim(ClaimTypes.Surname, user.LastName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim("CartId", user.CartId.ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, expiresIn.ToString()),
                new Claim(JwtRegisteredClaimNames.Iss, _configuration["JwtSettings:Issuer"]!),
                new Claim(JwtRegisteredClaimNames.Aud, _configuration["JwtSettings:Audience"]!)
            };


        var token = new JwtSecurityToken
        (
            claims: claims,
            expires: expiresIn,
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Creates a new refresh token using a random string
    /// </summary>
    /// <returns></returns>
    private RefreshToken GenerateRefreshToken()
    {
        var text = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[0-9]{4}[A-Z]{2}" });
        string Token = text.Generate()!;

        RefreshToken refreshtoken = new RefreshToken
        {
            Token = Token,
            ExpiresOn = DateTime.UtcNow.AddDays(10),
            CreatedOn = DateTime.UtcNow,
        };

        return refreshtoken;
    }

    private bool CheckActiveRefreshTokensForUser(AppUser user)
    {
        if (user.RefreshTokens is not null)
        {
            return user.RefreshTokens.Any(rt => rt.IsActive == true);
        }
        else
        {
            return false;
        }
    }
}

