using Auth.Models;

namespace Auth.Utils;

static class RolePolicies
{
    public static readonly string[] AdminRoles = [DefaultUserRoles.SuperAdmin.ToString(), DefaultUserRoles.Admin.ToString()];
    public static readonly string SuperAdmin = DefaultUserRoles.SuperAdmin.ToString();
    public static readonly string Admin = DefaultUserRoles.Admin.ToString();
    public static readonly string User = DefaultUserRoles.User.ToString();
}