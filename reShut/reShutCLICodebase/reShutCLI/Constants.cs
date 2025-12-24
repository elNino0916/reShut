namespace reShutCLI
{
    internal static class Constants
    {
        // Exit Codes
        public const int ExitCodeSuccess = 0;

        // Registry Values
        public const string EulaNotAcceptedValue = "0";
        public const string SkipConfirmationEnabledValue = "1";
        public const string UpdateSearchEnabledValue = "1";

        // Registry Keys
        public const string RegistryValueEulaAccepted = "EULAAccepted";
        public const string RegistryValueSkipConfirmation = "SkipConfirmation";
        public const string RegistryValueEnableUpdateSearch = "EnableUpdateSearch";
        public const string RegistryValueReShutVersion = "reShutVersion";

        // Registry Value Types
        public const string RegistryValueTypeString = "STRING";

        // Registry Paths
        public const string RegistryPathBase = @"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI";
        public const string RegistryPathConfig = RegistryPathBase + @"\config";

        // UI Magic Numbers
        public const int BoxPaddingWidth = 2;
        public const int MinimumBoxWidth = 44;
        public const int MainMenuStartIndex = 1;
        public const int MenuItemPaddingWidth = 20;
        public const int UpdateDownloadMessageDelayMs = 2000;

        // Resource Names
        public const string ResourceAssemblyName = "reShutCLI.Resources.Strings";
    }
}
