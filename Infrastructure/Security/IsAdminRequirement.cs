using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security;

public class IsAdminRequirement : IAuthorizationRequirement
{
    
}

public class IsAdminRequirementHandler : AuthorizationHandler<IsAdminRequirement>
{
    private readonly DataContext _context;
    private readonly IHttpContextAccessor _contextAccessor;

    public IsAdminRequirementHandler(DataContext context, IHttpContextAccessor contextAccessor)
    {
        _context = context;
        _contextAccessor = contextAccessor;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsAdminRequirement requirement)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
            return;

        var user = await _context.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId);
        
        if (user is null)
            return;
        
        if (user.IsAdmin)
            context.Succeed(requirement);
    }
}