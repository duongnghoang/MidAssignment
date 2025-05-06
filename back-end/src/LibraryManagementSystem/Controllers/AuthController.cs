using Application.Services.Authentication;
using Contract.Dtos.Authentication.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LibraryManagementSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("sign-in")]
    public async Task<IActionResult> SignInUser([FromBody] SignInRequest request)
    {
        var result = await authService.LoginAsync(request);

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUpUser([FromBody] SignUpRequest request)
    {
        await authService.RegisterAsync(request);

        return Created();
    }

    [Authorize("NormalUser")]
    [HttpGet("test")]
    public IActionResult Get()
    {
        return Ok("Ok");
    }

    [Authorize("SuperUser")]
    [HttpGet("test2")]
    public IActionResult Get2()
    {
        return Ok("Ok");
    }
}