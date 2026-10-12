using KTGKMD.Configuration.Dto;
using System.Threading.Tasks;

namespace KTGKMD.Configuration;

public interface IConfigurationAppService
{
    Task ChangeUiTheme(ChangeUiThemeInput input);
}
