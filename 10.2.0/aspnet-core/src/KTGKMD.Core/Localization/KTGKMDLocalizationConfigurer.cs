using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace KTGKMD.Localization;

public static class KTGKMDLocalizationConfigurer
{
    public static void Configure(ILocalizationConfiguration localizationConfiguration)
    {
        localizationConfiguration.Sources.Add(
            new DictionaryBasedLocalizationSource(KTGKMDConsts.LocalizationSourceName,
                new XmlEmbeddedFileLocalizationDictionaryProvider(
                    typeof(KTGKMDLocalizationConfigurer).GetAssembly(),
                    "KTGKMD.Localization.SourceFiles"
                )
            )
        );
    }
}
