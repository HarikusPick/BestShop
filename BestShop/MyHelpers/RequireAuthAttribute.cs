using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace BestShop.MyHelpers
{
    public class RequireAuthAttribute : Attribute, IPageFilter
    {
        public string RequiredRole { get; set; } = "";
        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {}

        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            string? role = context.HttpContext.Session.GetString("role");

            if (role == null)
            {
                //user rolü boşsa anasayfaya yönlendiriyoruz. RouteControl
                context.Result = new RedirectResult("/");
            }
            else
            {
                if (RequiredRole.Length > 0 && !RequiredRole.Equals(role))
                {
                    //user authenticated but the role is not authorized
                    context.Result = new RedirectResult("/");
                }
            }
        }

        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {}
    }
}
