using KTGKMD.Roles.Dto;
using System.Collections.Generic;

namespace KTGKMD.Web.Models.Common;

public interface IPermissionsEditViewModel
{
    List<FlatPermissionDto> Permissions { get; set; }
}