using System.Security.Claims;
using ASP.NET_Razor_Final.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Asn1.Cmp;

namespace MyApp.Security.Requirements
{
    public class AppAuthorizationHandle : IAuthorizationHandler
    {
        private readonly ILogger<AppAuthorizationHandle> _logger;
        private readonly UserManager<AppUser> _userManager;
        public AppAuthorizationHandle(ILogger<AppAuthorizationHandle> logger, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            var requirements = context.PendingRequirements.ToList();
            foreach (var require in requirements)
            {
                if (require is GenZRequirement)
                {
                    //code xu li kiem tra user dam bao requirement/genzrequirement
                    if (IsGenZ(context.User, (GenZRequirement)require))
                    {
                        context.Succeed(require);
                    }
                }
                if (require is ArticleUpdateRequirement)
                {
                    if (CanUpdateArticle(context.User, context.Resource, (ArticleUpdateRequirement)require))
                    {
                        context.Succeed(require);
                    }
                }
            }
            return Task.CompletedTask;
        }

        private bool CanUpdateArticle(ClaimsPrincipal user, object? resource, ArticleUpdateRequirement require)
        {
            if (user.IsInRole("Admin"))
                return true;
            var article = resource as Article;
            var dateCreate = article?.Created;
            var dateCanUpdate = new DateTime(require.Year, require.Month, require.Day);
            if (dateCreate < dateCanUpdate)
            {
                _logger.LogInformation("Qua ngày cập nhật được cho phép ");
                return false;
            }
            return true;
        }

        private bool IsGenZ(ClaimsPrincipal user, GenZRequirement require)
        {
            var appUserTask = _userManager.GetUserAsync(user);
            Task.WaitAll(appUserTask);
            var appUser = appUserTask.Result;
            if (appUser == null) return false;
            if (appUser.BirthDate == null)
            {
                _logger.LogInformation($"{appUser.UserName} co ngay sinh khong thoa GenZ");
                return false;
            }
            int? year = appUser.BirthDate.Value.Year;
            var success = (year >= require.FromYear && year <= require.ToYear);
            if (success) _logger.LogInformation($"{appUser.UserName} thoa man Genz");
            else  _logger.LogInformation($"{appUser.UserName} khong thoa man Genz");
            return success;
        }
    }
}