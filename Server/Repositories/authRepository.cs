

using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;

namespace Trofi.io.Server.Repositories
{
    public class authRepository : IauthRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        public authRepository( UserManager<AppUser> UserManager, IConfiguration configuration, AppDbContext AppDbContext)
        {
            _userManager = UserManager;
            _configuration = configuration;
            _context = AppDbContext;
        }

        public async Task<UserManagerResponse> LoginUserAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.email);
            if (user != null) 
            {
                var result1 = await _userManager.CheckPasswordAsync(user, request.password);
                Console.WriteLine(result1);
                if (result1)
                {
                    string token = CreateToken(user);
                    Console.WriteLine("We are logged in  :  " + token);
                    return new UserManagerResponse
                    {
                        JWT = token,
                        IsSuccess = true,
                        message = "Logged in",
                    };
                }
                else
                {
                    return new UserManagerResponse
                    {
                        IsSuccess = false,
                        message = "Wrong password",
                    };
                }
            }
            else
            {
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    message = "User does not exist",
                };
            }
        }
        public async Task<UserManagerResponse> RegisterUserAsync(RegisterRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.email);
            Console.WriteLine("entered registerUser");
            if (user != null)
            {
                Console.WriteLine("User already exists");
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    message = "Email already used",
                };
            }
            else 
            {
                Console.WriteLine("User being registered");
                Console.WriteLine(request.lastName);
                Console.WriteLine(request.firstName);
                Console.WriteLine(request.username);

                var appuser = new AppUser
                {
                    UserName = request.username,
                    Email = request.email,
                    FirstName = request.firstName,
                    LastName = request.lastName,
                    PhoneNumber = request.phoneNumber,
                    Location = request.Location,
                };
                var result = await _userManager.CreateAsync(appuser , request.password);

                Console.WriteLine(result);
                if (result.Succeeded)
                {
                Console.WriteLine("User created");
                    var cart = new Cart
                    {
                        Id = Guid.NewGuid(),
                        AppUserId=appuser.Id,
                        
                    };
                    appuser.CartId = cart.Id;

                    await _context.Carts.AddAsync(cart);
                    await _context.SaveChangesAsync();
                    return new UserManagerResponse
                    {
                        IsSuccess = true,
                        message = "user registered",
                    };
                }
                return new UserManagerResponse
                {
                    IsSuccess= false,
                    message="registeration failed",
                };
            }
        }

        public async Task<UserManagerResponse> UpdatePasswordAsync(ChangePasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.email);
            if (user != null) {
                var result = await _userManager.CheckPasswordAsync(user, request.oldPassword);
                Console.WriteLine("Wrong password");
                if (result) {
                    var wow = await _userManager.ChangePasswordAsync(user, request.oldPassword, request.newPassword);
                    if (wow == IdentityResult.Success)
                    {
                        return new UserManagerResponse
                        {
                            IsSuccess = true,
                            message = "Password changed"
                        };
                    }                    
                    else
                    {
                        return new UserManagerResponse
                        {
                            IsSuccess = true,
                            message = "change failed"
                        };
                    }
                }
                else
                {
                    return new UserManagerResponse
                    {
                        IsSuccess = false,
                        message = "Wrong information"
                    };
                }
            }
            else
            {
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    message = "User not found"
                };
            }
        }

    private string CreateToken(AppUser user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                new Claim("CardId",user.CartId.ToString()),
                new Claim("User Location",user.Location ),
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Key").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMilliseconds(10000),
                signingCredentials: creds
                ) ;
            var jwt = new JwtSecurityTokenHandler().WriteToken(token) ;
            return jwt;
        }
    public static RefreshToken GenerateRefreshToken()
    {
        var text = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[0-9]{4}[A-Z]{2}" });
        string Token = text.Generate();
        RefreshToken refreshtoken = new RefreshToken
        {
            Token = Token,
            ExpiresOn = DateTime.UtcNow.AddDays(1),
            CreatedOn = DateTime.UtcNow,
        };
        return refreshtoken;
    }
    }
}
