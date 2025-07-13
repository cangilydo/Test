using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ApiService.Authorize
{
    public class InnerAuthorize : Attribute, IAuthorizationFilter
    {
        public InnerAuthorize()
        {
            
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            //authorize
        }
    }
}
