using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ServidorApiRestaurante.Models;
using System.Security.Claims;

namespace ServidorApiRestaurante.Controllers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ValidarTokenFilterController : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var identity = context.HttpContext.User.Identity as ClaimsIdentity;
            var resultado = Jwt.ValidarToken(identity);

            if (!resultado.success)
            {
                // Detiene la ejecución y devuelve directamente esto al cliente
                context.Result = new JsonResult(new { result = 0 });
                return;
            }

            base.OnActionExecuting(context);
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SoloAdminsFilterController : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var identity = context.HttpContext.User.Identity as ClaimsIdentity;
            var resultado = Jwt.ValidarRol(identity);

            if (!resultado.success)
            {
                context.Result = new JsonResult(new { result = 0 });
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
