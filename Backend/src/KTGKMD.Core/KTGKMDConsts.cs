using KTGKMD.Debugging;

namespace KTGKMD;

public class KTGKMDConsts
{
    public const string LocalizationSourceName = "KTGKMD";

    public const string ConnectionStringName = "Default";

    public const bool MultiTenancyEnabled = true;


    /// <summary>
    /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
    /// </summary>
    public static readonly string DefaultPassPhrase =
        DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "09ea9e56dcee4bf3b991f79cc3979f1c";
}
