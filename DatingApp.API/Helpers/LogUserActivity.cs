using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace DatingApp.API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            var userId = int.Parse(resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var usersRepository = resultContext.HttpContext.RequestServices.GetService<IUsersRepository>();

            var repository = resultContext.HttpContext.RequestServices.GetService<IRepository>();

            var user = await usersRepository.GetUser(userId, true);

            user.LastActive = DateTime.Now;

            await repository.SaveAll();
        }
    }
}