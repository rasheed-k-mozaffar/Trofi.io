using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["Refresh_Token"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(new ApiErrorResponse
                {
                    ErrorMessage = "The refresh token is required"
                });
            }

            var result = await _authRepository.RefreshTokenAsync(refreshToken);

            if (!result.IsSuccess)
            {
                return BadRequest(new ApiErrorResponse
                {
                    ErrorMessage = result.Message
                });
            }

            SetRefreshToken(result.RefreshToken!, result.RefreshTokenExpiration);

            return Ok(new ApiResponse<string>
            {
                Message = result.Message,

            });
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken(RevokeTokenRequest revokeTokenRequest)
        {
            // getting the token from either place works
            var refreshToken = revokeTokenRequest.Token ?? Request.Cookies["Refresh_Token"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(new ApiErrorResponse
                {
                    ErrorMessage = "Token is required"
                });
            }

            var result = await _authRepository.RevokeTokenAsync(refreshToken);

            if (!result)
            {
                return BadRequest(new ApiErrorResponse
                {
                    ErrorMessage = "The token is invalid"
                });
            }

            // remove token from cookies
            RemoveTokenFromCookies();

            return Ok(new ApiResponse
            {
                Message = "Session revoked successfully",
                IsSuccess = true
            });
        }


        /// <summary>
        /// Allows the user to request a change to their current password into sometihng else
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword(ChangePasswordRequest request)
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

        /// <summary>
        /// Removes the Refresh token from the cookies after the token in the cookies is revoked.
        /// </summary>
        private void RemoveTokenFromCookies()
        {
            Response.Cookies.Delete("Refresh_Token");
        }
    }
}
