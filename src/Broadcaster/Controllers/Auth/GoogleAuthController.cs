
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using Azure.Core;
using Broadcaster.Business;
using Broadcaster.Business.Models;
using Broadcaster.Business.Services;
using Broadcaster.Classes;
using Broadcaster.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/api/auth/google")]
public class GoogleAuthController : ControllerBase
{
    private readonly ILogger<GoogleAuthController> _logger;
    private readonly GoogleYouTubeTokenService _googleService;
    private readonly RefreshTokenService _refreshTokenService;
    private readonly JwtTokenService _jwtService;
    private readonly UserService _userService;
    private readonly IConfiguration _configuration;

    public GoogleAuthController(
        ILogger<GoogleAuthController> logger,
        GoogleYouTubeTokenService googleService,
        RefreshTokenService refreshTokenService,
        JwtTokenService jwtService,
        UserService userService,
        IConfiguration configuration
    )
    {
        _logger = logger;
        _googleService = googleService;
        _refreshTokenService = refreshTokenService;
        _jwtService = jwtService;
        _userService = userService;
        _configuration = configuration;
    }

    [HttpPost("callback")]
    [ProducesResponseType(typeof(AuthTokenResponse), 200)]
    public async Task<IActionResult> Callback([FromBody] GoogleCallbackRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Code))
            return BadRequest(new { error = "Code is required" });

        try
        {
            var (tokenResponse, payload) = await _googleService.ExchangeCodeAsync(request.Code);

            string googleSub = payload.Subject;
            string email = payload.Email ?? "";
            string name = payload.Name ?? email.Split('@')[0];
            string? givenName = payload.GivenName;
            string? familyName = payload.FamilyName;
            string? picture = payload.Picture;

            UserObject user = await _userService.GetOrCreateUserAsync(
                googleSub,
                email,
                name,
                givenName,
                familyName,
                picture,
                tokenResponse.RefreshToken
            );

            // Handle the middle-term refresh token stored as an http only cookie
            string token = TokenHelper.GenerateRefreshToken();
            string hash = TokenHelper.HashToken(token);

            RefreshTokenObject refreshToken = new RefreshTokenObject(user.GoogleSub, hash);
            await _refreshTokenService.InsertAsync(refreshToken);

            setRefreshTokenCookie(token);

            // Issue a jwt token for the API to use
            string jwt = _jwtService.GenerateToken(user);

            return Ok(new AuthTokenResponse(user, jwt));

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "");
            Console.Error.WriteLine($"Google callback error: {ex}");
            return BadRequest(new { error = "Failed to complete the Google sign-in. Please try again." });
        }
    }

    [HttpGet("refresh")]
    [ProducesResponseType(typeof(AuthTokenResponse), 200)]
    public async Task<ActionResult> Refresh()
    {
        if (!Request.Cookies.TryGetValue("refresh_token", out var rawToken))
            return Unauthorized("No refresh token present");

        var hash = TokenHelper.HashToken(rawToken);
        RefreshTokenObject? refreshToken = await _refreshTokenService.GetAsync(hash);

        if (refreshToken is null || refreshToken.TokenExpires < DateTime.Now)
        {
            expireRefreshTokenCookie();
            return Unauthorized("Refresh token invalid or expired");
        }

        UserObject? user = await _userService.GetAsync(refreshToken.GoogleSub);
        if (user is null)
        {
            expireRefreshTokenCookie();
            return Unauthorized("Refresh token produced an invalid user identifier");
        }

        string jwt = _jwtService.GenerateToken(user);
        return Ok(new AuthTokenResponse(user, jwt));
    }

    private void setRefreshTokenCookie(string rawToken)
    {
        Response.Cookies.Append("refresh_token", rawToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(30),
            MaxAge = TimeSpan.FromDays(30),
            Path = "/api/auth"
        });
    }

    private void expireRefreshTokenCookie()
    {
        Response.Cookies.Append("refresh_token", "", new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UnixEpoch,
            Path = "/api/auth"
        });
    }
}

// Request model from frontend
public class GoogleCallbackRequest
{
    [Required]
    public required string Code { get; set; }
}

// Response model sent back to frontend
public class AuthTokenResponse
{
    #region Properties

    public string AccessToken { get; set; }
    public UserObject User { get; set; }

    #endregion

    #region Constructor

    public AuthTokenResponse(UserObject user, string accessToken)
    {
        AccessToken = accessToken;
        User = user;
    }

    #endregion
}

public class RefreshResponse
{
    public string AccessToken { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    public RefreshResponse(string accessToken, UserObject user)
    {
        AccessToken = accessToken;
        Name = user.Name;
        Email = user.Email;
    }
}