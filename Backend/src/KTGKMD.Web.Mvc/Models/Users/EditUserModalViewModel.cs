using KTGKMD.Roles.Dto;
using KTGKMD.Users.Dto;
using System.Collections.Generic;
using System.Linq;

namespace KTGKMD.Web.Models.Users;

public class EditUserModalViewModel
{
    public UserDto User { get; set; }

    public IReadOnlyList<RoleDto> Roles { get; set; }

    public bool UserIsInRole(RoleDto role)
    {
        return User.RoleNames != null && User.RoleNames.Any(r => r == role.NormalizedName);
    }
}
