using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using vkAMS_prototype.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using vkAMS_prototype.Common;

namespace vkAMS_prototype.Controllers
{
    public class IdentityController : Controller
    {
        private readonly SignInManager _signInManager;

        public IdentityController(SignInManager signInManager) => _signInManager = signInManager;

        [AllowAnonymous]
        public IActionResult Index() => View();


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExecuteLogin(string username, string password, string returnUrl = null)
        {
            LoginResult authenticationResult = await _signInManager.Login(username, password, "PRO");
            return RedirectToAction("Login", authenticationResult);
        }
        
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login([Bind]LoginResult loginResult, string returnUrl = null)
        {
            return View(model: loginResult);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout() {
            await _signInManager.Logout();
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public IActionResult Denied(string returnUrl = null)
        {
            return View(model: returnUrl);
        }

        [Authorize(Roles = "TE")]
        public IActionResult AuthenticatedTE()
        {
            return Content("Okay");
        }
        [Authorize(Roles = "TE,AD")]
        public IActionResult AuthenticatedTEAD()
        {
            return Content("Okay");
        }
        [Authorize(Roles = "AD")]
        public IActionResult AuthenticatedAD()
        {
            return Content("Okay");
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
