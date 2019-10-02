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
            bool authenticated = await _signInManager.Login(username, password, "PRO");
            return RedirectToAction("Login", new {valid = !authenticated});
        }
        
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(bool error = false, string returnUrl = null)
        {
            return View(model: returnUrl);
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
        [Authorize(Roles = "TE,ST")]
        public IActionResult AuthenticatedTEST()
        {
            return Content("Okay");
        }
        [Authorize(Roles = "ST")]
        public IActionResult AuthenticatedST()
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
