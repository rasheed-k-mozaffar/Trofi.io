using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using Trofi.io.Server.Repositories;
using Trofi.io.Shared.Auth;
using Trofi.io.Shared.DTOs;

namespace Trofi.io.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class authController : ControllerBase
    {
        private readonly IauthRepository _IauthRepository;
        public authController( IauthRepository IauthRepository)
        {
            _IauthRepository = IauthRepository;
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserManagerResponse>> Login( LoginRequest request)
        {
            if(request is null)
            {
                return BadRequest("Login information empty");
            }
            else
            {
                var response = await _IauthRepository.LoginUserAsync(request);
                if (response.IsSuccess == true)
                {
                    RefreshToken refresh = authRepository.GenerateRefreshToken();
                    SetRefreshToken(refresh);
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
            }
        }
        [HttpPost("register")]
        public async Task<ActionResult> Register( RegisterRequest request)
        {

            if (string.IsNullOrEmpty(request.username))
            {
            Console.WriteLine(request.username);
                return BadRequest("Login information empty");
            }
            else
            {
                var response = await _IauthRepository.RegisterUserAsync(request);
                if (response.IsSuccess)
                {
                    return Ok(response);
                }                
                else
                {
                    return BadRequest(response);
                }
            }
        }        
        [HttpPost("changepassword")]
        public async Task<ActionResult> changepassword( ChangePasswordRequest request)
        {

            if (request is null)
            {
                return BadRequest("Login information empty");
            }
            else
            {
                var response = await _IauthRepository.UpdatePasswordAsync(request);
                if (response.IsSuccess)
                {
                    return Ok(response);
                }                
                else
                {
                    return BadRequest(response);
                }
            }
        }
        private void SetRefreshToken(RefreshToken refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddDays(1),
            };
            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
        }
    }
}
