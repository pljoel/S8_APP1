using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Server.Services
{
    // Contient les options d'authentification
    // https://geeklearning.io/how-to-migrate-your-authentication-middleware-to-asp-net-core-2-0/
    public class CustomAuthenticationOptions : AuthenticationSchemeOptions
    {
        public ClaimsIdentity Identity { get; set; }
        // ... Other authentication options properties
    }


    // Point d'entree pour le processus d'authentification
    public class CustomAuthenticationHandler : AuthenticationHandler<CustomAuthenticationOptions>
    {
        readonly string API_KEY = "A2D3-HTDG-MLU2-3AM5";

        public CustomAuthenticationHandler(IOptionsMonitor<CustomAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock) { }

        // Logique d'authentification
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                if(this.Request.Headers["Authorization"] == API_KEY)
                {
                    return Task.FromResult(
                        AuthenticateResult.Success(
                            new AuthenticationTicket(
                                new ClaimsPrincipal(new ClaimsIdentity("S8APP1Scheme")),
                                this.Scheme.Name)));
                }
                else
                {
                    return Task.FromResult(AuthenticateResult.Fail("Authentication failed."));
                }
                
            }
            catch
            {
                return Task.FromResult(AuthenticateResult.Fail("Exception while Handling authentication key"));
            }
            
        }
    }

    // Enregistre le "scheme" d'authentification personnalise
    public static class CustomAuthenticationExtensions
    {
        public static AuthenticationBuilder AddCustomAuthentication(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<CustomAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<CustomAuthenticationOptions, CustomAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}
