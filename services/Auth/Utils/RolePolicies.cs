using Auth.Models;
using Microsoft.AspNetCore.Authorization;

namespace Auth.Utils;

static class RolePolicies
{
    public static readonly string[] AdminRoles = [DefaultUserRoles.SuperAdmin.ToString(), DefaultUserRoles.Admin.ToString()];
    public static readonly string SuperAdmin = DefaultUserRoles.SuperAdmin.ToString();
    public static readonly string Admin = DefaultUserRoles.Admin.ToString();
    public static readonly string User = DefaultUserRoles.User.ToString();
    public static readonly Action<AuthorizationPolicyBuilder> AdminPolicy = (pol) => pol.RequireRole(Admin);
    public static readonly Action<AuthorizationPolicyBuilder> UserPolicy = (pol) => pol.RequireRole(User);
    public static readonly Action<AuthorizationPolicyBuilder> SuperAdminPolicy = (pol) => pol.RequireRole(SuperAdmin);
    public static readonly Action<AuthorizationPolicyBuilder> AdminsRolesPolicy = (pol) => pol.RequireRole(Admin, SuperAdmin);
}