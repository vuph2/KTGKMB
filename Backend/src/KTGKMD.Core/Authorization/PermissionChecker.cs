using Abp.Authorization;
using KTGKMD.Authorization.Roles;
using KTGKMD.Authorization.Users;

namespace KTGKMD.Authorization;

public class PermissionChecker : PermissionChecker<Role, User>
{
    public PermissionChecker(UserManager userManager)
        : base(userManager)
    {
    }
}
