using BackgroundJobs.Jobs;
using Business.Abstract;
using Business.DTOs.UserDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public ActionResult Login(UserForLoginDto userForLoginDto)
        {
            var result = _authService.Login(userForLoginDto);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("register")]
        public ActionResult Register(UserForRegisterDto userForRegisterDto)
        {
            var result = _authService.Register(userForRegisterDto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            FireAndForgetJobs.SendMailRegisterJobs(userForRegisterDto.Email);
            return Ok(result);
        }

        [HttpPut("refresh")]
        public ActionResult RefreshToken(string existingRefreshToken)
        {
            var result = _authService.RefreshToken(existingRefreshToken);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
