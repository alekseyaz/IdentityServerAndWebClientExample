using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebClient.Controllers
{
    //[Authorize(AuthenticationSchemes = "oidc")]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Authorize]
        public async Task<IActionResult> SignIn(string returnUrl = null)
        {

            //string appNamespace = typeof(Program).Namespace;
            ////string appName = appNamespace.Substring(appNamespace.LastIndexOf('.', appNamespace.LastIndexOf('.') - 1) + 1);

            //var user = User as ClaimsPrincipal;
            //var token = await HttpContext.GetTokenAsync("access_token");

            //_logger.LogInformation("----- User {@User} authenticated into {AppName}", user, appNamespace);

            //if (token != null)
            //{
            //    ViewData["access_token"] = token;
            //}

            return Redirect("~/");

        }


        public async Task<IActionResult> Signout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

            // "Catalog" because UrlHelper doesn't support nameof() for controllers
            // https://github.com/aspnet/Mvc/issues/5853
            //var homeUrl = Url.Action(nameof(CatalogController.Index), "Catalog");
            var homeUrl = Url.Page("/Index");
            return new SignOutResult(OpenIdConnectDefaults.AuthenticationScheme,
                new AuthenticationProperties { RedirectUri = homeUrl });
        }
    }
}
