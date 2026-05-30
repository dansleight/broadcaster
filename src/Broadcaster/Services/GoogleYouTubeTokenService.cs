using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System.Security.Claims;

namespace Broadcaster.Services;

public class GoogleYouTubeTokenService
{
    private readonly ClientSecrets _clientSecrets;
    private readonly string _redirectUri;

    public GoogleYouTubeTokenService(IConfiguration configuration)
    {
        var clientId = configuration["Google:ClientId"] ?? throw new InvalidOperationException("Google:ClientId is missing");
        var clientSecret = configuration["Google:ClientSecret"] ?? throw new InvalidOperationException("Google:ClientSecret is missing");
        _clientSecrets = new ClientSecrets
        {
            ClientId = clientId,
            ClientSecret = clientSecret
        };
        _redirectUri = configuration["Google:RedirectUri"] ?? throw new InvalidOperationException("Google:RedirectUri is missing");
    }

    public async Task<(TokenResponse TokenResponse, GoogleJsonWebSignature.Payload UserPayload)> ExchangeCodeAsync(string code)
    {
        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = _clientSecrets,
            Scopes = new[] { "openid", "email", "profile", "https://www.googleapis.com/auth/youtube" }
        });

        var tokenResponse = await flow.ExchangeCodeForTokenAsync(
            userId: "temp-user",
            code: code,
            redirectUri: _redirectUri,
            CancellationToken.None);

        GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(tokenResponse.IdToken);

        return (tokenResponse, payload);
    }

    public async Task<string> GetAccessTokenAsync(string refreshToken)
    {
        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = _clientSecrets
        });

        var credential = new UserCredential(flow, "user", new TokenResponse
        {
            RefreshToken = refreshToken
        });

        await credential.RefreshTokenAsync(CancellationToken.None);

        return credential.Token.AccessToken ?? throw new InvalidOperationException("Failed to obtain access token");
    }

    public YouTubeService CreateYouTubeService(string accessToken)
    {
        var initializer = new BaseClientService.Initializer
        {
            HttpClientInitializer = GoogleCredential.FromAccessToken(accessToken)
        };
        return new YouTubeService(initializer);
    }
}
