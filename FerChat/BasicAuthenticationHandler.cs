using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Text;

namespace FerChat {
    public class BasicAuthenticationAuthorizationFilter : IAuthorizationFilter {
        public void OnAuthorization(AuthorizationFilterContext context) {
            string expected = $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Startup.ApiCredentials.UserName}:{Startup.ApiCredentials.Password}"))}";
            if (!context.HttpContext.Request.Headers["Authorization"].Contains(expected)) {
                context.Result = new UnauthorizedResult();
                context.HttpContext.Response.Headers.Add("WWW-Authenticate", $"Basic realm=\"{nameof(FerChat)} API\"");
            }
        }
    }

    public class BasicAuthenticationAttribute : TypeFilterAttribute {
        public BasicAuthenticationAttribute() : base(typeof(BasicAuthenticationAuthorizationFilter)) { }
    }
}
