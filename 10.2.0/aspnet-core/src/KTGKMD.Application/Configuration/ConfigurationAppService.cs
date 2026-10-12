using Abp.Authorization;
using Abp.Runtime.Session;
using KTGKMD.Configuration.Dto;
using System.Threading.Tasks;

namespace KTGKMD.Configuration;

[AbpAuthorize]
public class ConfigurationAppService : KTGKMDAppServiceBase, IConfigurationAppService
{
    public async Task ChangeUiTheme(ChangeUiThemeInput input)
    {
        await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
    }
}
