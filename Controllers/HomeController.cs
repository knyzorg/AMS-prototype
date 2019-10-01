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
        public async Task<IActionResult> Login()
        {
            var claims = new List<Claim>
            {
                new Claim("Username", "The name"),
                new Claim(ClaimTypes.Role, "Test")
            };
            
            var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties();
            
            await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

            return View("Index");
        }

        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Json(new { Works = true });
        }

        [AllowAnonymous]
        public async Task<IActionResult> Check() {
            return Json(new { User = HttpContext.User.Claims });
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize(Roles = "Test")]
        public IActionResult Authenticated()
        {
            return Json(new {test = true});
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
