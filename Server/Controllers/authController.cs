using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using Trofi.io.Server.Repositories;
using Trofi.io.Shared.Auth;

namespace Trofi.io.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthRepository authRepository, ILogger<AuthController> logger)
        {
            _authRepository = authRepository;
            _logger = logger;
        }


        [HttpPost("login")]
        public async Task<ActionResult<UserManagerResponse>> Login(LoginRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _authRepository.LoginUserAsync(request);

                if (result.IsSuccess)
                {
                    // set the refresh token in the resopnse cookies
                    if (!string.IsNullOrEmpty(result.RefreshToken))
                    {
                        SetRefreshToken(result.RefreshToken, result.RefreshTokenExpiration);
                    }

                    return Ok(new ApiResponse<string>
                    {
                        Message = result.Message,
                        Body = result.JWT,
                        IsSuccess = true
                    });
                }
                else
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        ErrorMessage = result.Message
                    });
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _authRepository.RegisterUserAsync(request);

                if (result.IsSuccess)
                {
                    return Ok(new ApiResponse
                    {
                        Message = result.Message,
                        IsSuccess = true
                    });
                }
                else
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        ErrorMessage = result.Message
                    });
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPost("change-password")]
        public async Task<ActionResult> changepassword(ChangePasswordRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _authRepository.UpdatePasswordAsync(request);

                if (result.IsSuccess)
                {
                    return Ok(new ApiResponse
                    {
                        Message = result.Message,
                        IsSuccess = true
                    });
                }
                else
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        ErrorMessage = result.Message
                    });
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }


        /// <summary>
        /// Sets the refresh token in the response cookies and sets the expiration time for the cookie
        /// which is the same as the refresh token's expiration date
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <param name="expiresOn"></param>
        private void SetRefreshToken(string refreshToken, DateTime expiresOn)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expiresOn.ToLocalTime()
            };
            Response.Cookies.Append("Refresh_Token", refreshToken, cookieOptions);
        }
    }
}
