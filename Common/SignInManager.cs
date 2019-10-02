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
using ServiceReference;
using Microsoft.AspNetCore.Http;

namespace vkAMS_prototype.Common {
    public class SignInManager {
        private readonly IHttpContextAccessor _httpContext;
        public SignInManager(IHttpContextAccessor httpContext) => _httpContext = httpContext;
        public ClaimsPrincipal User => _httpContext.HttpContext.User;
        public bool IsAuthenticated => User.Identity.IsAuthenticated;
        public IEnumerable<string> Roles => User.Claims
                                                .Where(claim => claim.Type == ClaimTypes.Role)
                                                .Select(claim => claim.Value);
        public async Task<bool> Logout() {
            await _httpContext.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return IsAuthenticated;
        }
        public async Task<LoginResult> Login(string authUsername, string authPassword, string appCode)
        {
 
            LoginSoapClient.EndpointConfiguration endpointConfiguration = LoginSoapClient.EndpointConfiguration.LoginSoap;
            LoginSoapClient login = new LoginSoapClient(endpointConfiguration );
            AuthenticateResponse authentication = await login.AuthenticateAsync(authUsername, authPassword);
            AuthorizeResponse authorization = await login.AuthorizeAsync(authUsername, appCode);

            bool valid = authentication.Body.AuthenticateResult && !authorization.Body.AuthorizeResult.HasError;
            if (valid)
            {
                
                var employeeId = authorization.Body.AuthorizeResult.EmployeeId;
                var userId = authorization.Body.AuthorizeResult.UserId;
                var studentId = authorization.Body.AuthorizeResult.StudentId;
                var firstName = authorization.Body.AuthorizeResult.FirstName;
                var lastName = authorization.Body.AuthorizeResult.LastName;
                var username = authorization.Body.AuthorizeResult.Username;
                var roleList = authorization.Body.AuthorizeResult.RoleList;


                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Surname, lastName ?? ""),
                    new Claim(ClaimTypes.GivenName, firstName ?? ""),
                    new Claim("userId", userId.ToString())
                };

                // roles have a trailing space. Trimming is necessary.
                claims.AddRange(roleList.Select(role => new Claim(ClaimTypes.Role, role.Code.Trim())));
                if (employeeId != null)
                    claims.Append(new Claim("employeeId", employeeId.ToString()));

                if (studentId != null)
                    claims.Append(new Claim("studentId", studentId.ToString()));
                
                
                var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties();
                
                await _httpContext.HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties
                );
            }
            return new LoginResult { Success = valid, ErrorMessage = authorization.Body.AuthorizeResult.ErrorMessage };
        }
    }
}