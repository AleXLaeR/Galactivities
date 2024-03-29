﻿using System.Security.Claims;
using Domain.DTOs;
using Domain.Entities;
using Domain.Entities.Users;
using Infrastructure.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[AllowAnonymous]
public class AccountController : BaseApiController
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ITokenService _tokenService;

    public AccountController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }
    
    // TODO fix unwanted redirection on login
    [HttpPost("Login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto loginDto)
    {
        var currentUser = await _userManager.Users
            .Include(u => u.Images)
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

        if (currentUser is null)
            return Unauthorized($"Sorry, but E-Mail {loginDto.Email} does not exist in our database");

        var result = await _signInManager.CheckPasswordSignInAsync(currentUser, loginDto.Password, default);

        if (result.Succeeded)
        {
            return CreateUserDtoFrom(currentUser);
        }

        return Unauthorized("Sorry, bad password");
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
    {
        if (await _userManager.Users.AnyAsync(u => u.Email == registerDto.Email))
        {
            ModelState.AddModelError("email", "E-Mail taken.");
        }
        if (await _userManager.Users.AnyAsync(u => u.UserName == registerDto.Username))
        {
            ModelState.AddModelError("userName", "Username taken.");
        }

        if (ModelState.ErrorCount != default)
        {
            return ValidationProblem();
        }

        var user = new User
        {
            DisplayName = registerDto.DisplayName,
            Email = registerDto.Email,
            UserName = registerDto.Username,
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (result.Succeeded)
        {
            return CreateUserDtoFrom(user);
        }

        return BadRequest("Sorry, There was a problem creating a user.");
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<UserDto?>> GetCurrentUser()
    {
        var user = await _userManager.Users
            .Include(u => u.Images)
            .FirstOrDefaultAsync(u => u.Email == User.FindFirstValue(ClaimTypes.Email));
        
        return (user is null) ? null : CreateUserDtoFrom(user);
    }

    [NonAction]
    private UserDto CreateUserDtoFrom(User user)
    {
        return new UserDto
        {
            DisplayName = user.DisplayName,
            ImageUri = user.Images.FirstOrDefault(i => i.IsMain)?.Uri,
            Token = _tokenService.GetJwtToken(user),
            Username = user.UserName,
            IsAdmin = user.IsAdmin,
        };
    }
}