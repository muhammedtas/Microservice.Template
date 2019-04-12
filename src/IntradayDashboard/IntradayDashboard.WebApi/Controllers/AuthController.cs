using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using IntradayDashboard.Core.Model.Entities;
using IntradayDashboard.Core.Services.Interfaces;
using IntradayDashboard.WebApi.Dto;
using IntradayDashboard.WebApi.Hubs;
using IntradayDashboard.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace IntradayDashboard.WebApi.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger _logger;
        #region SignalRHub
        // This field may use for to list users connected to the system via Socket
        //public IHubContext<LoginHub> _hubContext;
        //private Dictionary<string, string> _loggedInUserList;
        #endregion

        private readonly IAuthHelper _authHelper;

        public AuthController(IUserService userService, IConfiguration config, IMapper mapper,
            UserManager<User> userManager,
            IAuthHelper authHelper,
            SignInManager<User> signInManager,
            IHubContext<LoginHub> hubContext,
            ILoggerFactory logger)
        {
            _userService = userService;
            _config = config;
            _mapper = mapper;
            _userManager = userManager;
            _authHelper = authHelper;
            _signInManager = signInManager;
            //_hubContext = hubContext;
            _logger = logger.CreateLogger("Engie.ApiPortal.Auth");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserRegisterDto userForRegisterDto)
        {
            try
            {

                var userToCreate = _mapper.Map<User>(userForRegisterDto);

                var result = await _userManager.CreateAsync(userToCreate, userForRegisterDto.Password);

                var userToReturn = _mapper.Map<UserDto>(userToCreate);

                if (result.Succeeded)
                {
                    // return CreatedAtRoute("GetUser",
                    //     new { controller = "Users", id = userToCreate.Id }, userToReturn);
                    return Ok(userToReturn);
                }

                return BadRequest(result.Errors);
            }
            catch (System.Exception ex)
            {   
                _logger.LogError("Unexpected Error while Registering user..."+ ex.Message);
                return BadRequest("Unexpected Error while Registering user...");
            }


        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]UserForLoginDto userForLoginDto)
        {

            try
            {
                var user = await _userManager.FindByNameAsync(userForLoginDto.UserName);

                var result = await _signInManager
                    .CheckPasswordSignInAsync(user, userForLoginDto.Password, false);

                if (result.Succeeded)
                {
                    var appUser = await _userManager.Users
                        .FirstOrDefaultAsync(u => u.NormalizedUserName == userForLoginDto.UserName.ToUpper(CultureInfo.InvariantCulture));


                    // Signal R login poppup

                    // var userFullName = appUser.Firstname + " " + appUser.Lastname;
                   
                    // if (!_loggedInUserList.ContainsKey(appUser.UserName))
                    // {
                    //      _loggedInUserList.Add(appUser.UserName, userFullName);
                    // }

                    return Ok(new
                    {
                        token = _authHelper.GenerateJwtToken(appUser).Result,
                    });
                }

                return Unauthorized();
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Unexpected Error has occured while logging the system..."+ ex.Message);
                return BadRequest("Unexpected Error has occured while logging the system... ");
            }

        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

    }
}