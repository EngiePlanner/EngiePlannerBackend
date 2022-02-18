using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace EngiePlanner
{
    public class NtlmAndAnonymousSetupMiddleware
    {
        private readonly RequestDelegate next;

        public NtlmAndAnonymousSetupMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated || context.Request.Path.ToString().StartsWith("/Anonymous"))
            {
                await next(context);
                return;
            }

            await context.ChallengeAsync("Windows");
        }
    }
}
