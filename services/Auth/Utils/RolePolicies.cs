using Auth.Models;
using Microsoft.AspNetCore.Authorization;

namespace Auth.Utils;

static class RolePolicies
{
    public static readonly string[] AdminRoles = [DefaultUserRoles.SuperAdmin.ToString(), DefaultUserRoles.Admin.ToString()];
    public static readonly string SuperAdmin = DefaultUserRoles.SuperAdmin.ToString();
    public static readonly string Admin = DefaultUserRoles.Admin.ToString();
    public static readonly string User = DefaultUserRoles.User.ToString();
    
    // default Restaurants roles are normalized here
    public static readonly string RestaurantOwner = DefaultRestaurantUserRoles.Owner.ToString().ToUpperInvariant();
    public static readonly string RestaurantCustomer = DefaultRestaurantUserRoles.Customer.ToString().ToUpperInvariant();
    public static readonly string RestaurantAdmin = DefaultRestaurantUserRoles.Admin.ToString().ToUpperInvariant();
    public static readonly Action<AuthorizationPolicyBuilder> AdminPolicy = (pol) => pol.RequireRole(Admin);
    public static readonly Action<AuthorizationPolicyBuilder> UserPolicy = (pol) => pol.RequireRole(User);
    public static readonly Action<AuthorizationPolicyBuilder> SuperAdminPolicy = (pol) => pol.RequireRole(SuperAdmin);
    public static readonly Action<AuthorizationPolicyBuilder> AdminsRolesPolicy = (pol) => pol.RequireRole(Admin, SuperAdmin);
    public static readonly Action<AuthorizationPolicyBuilder> RestaurantAdminPolicy = (pol) => pol.RequireRole(RestaurantAdmin);
    public static readonly Action<AuthorizationPolicyBuilder> RestaurantCustomerPolicy = (pol) => pol.RequireRole(RestaurantCustomer);
    public static readonly Action<AuthorizationPolicyBuilder> RestaurantOwnerPolicy = (pol) => pol.RequireRole(RestaurantOwner);
    public static readonly Action<AuthorizationPolicyBuilder> RestaurantAdminsRolesPolicy = (pol) => pol.RequireRole(RestaurantAdmin, RestaurantOwner);
}