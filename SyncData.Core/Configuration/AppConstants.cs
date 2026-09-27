namespace SyncData.Configuration
{
    /// <summary>
    /// Constants used throughout the application
    /// </summary>
    public static class AppConstants
    {
        public const string DefaultLogFileName = "syncData.log";
        
        // Command-line argument prefixes
        public const string SourceArgPrefix = "-source=";
        public const string TargetArgPrefix = "-target=";
        public const string ExcludeArgPrefix = "-exclude=";
        
        // Command-line flags
        public const string VerboseFlag = "-v";
        public const string VerboseFlagLong = "-verbose";
        public const string LogFileFlag = "-log-file";
        public const string FtpFlag = "-ftp";
        public const string PreserveFlag = "-preserve";
    }
}
