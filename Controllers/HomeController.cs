using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using vkAMS_prototype.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace vkAMS_prototype.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public async Task<IActionResult> Login(string roles = "", bool valid = false, string returnUrl = null)
        {
            if (valid) {
                var claims = new List<Claim>
                {
                    new Claim("Username", "The name"),
                    new Claim(ClaimTypes.Role, roles)
                };
                
                var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties();
                
                await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
            }

            return View(model: returnUrl);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Json(new { LoggedIn = HttpContext.User.Identity.IsAuthenticated });
        }

        [AllowAnonymous]
        public async Task<IActionResult> Check() {
            return Json(new { 
                        Username = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Username")?.Value,
                        IsAuthenticated = HttpContext.User.Identity.IsAuthenticated,
                        Roles = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value
                    });
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize(Roles = "Foo")]
        public IActionResult AuthenticatedFoo()
        {
            return Json(new {Welcome = true});
        }
        [Authorize(Roles = "Bar")]
        public IActionResult AuthenticatedBar()
        {
            return Json(new {Welcome = true});
        }
        [Authorize(Roles = "Foo,Bar")]
        public IActionResult AuthenticatedFooBar()
        {
            return Json(new {Welcome = true});
        }

        [AllowAnonymous]
        public IActionResult Denied(string returnUrl = null)
        {
            return View(model: returnUrl);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
