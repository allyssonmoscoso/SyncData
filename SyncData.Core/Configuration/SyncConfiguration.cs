using System.Collections.Generic;

namespace SyncData.Configuration
{
    /// <summary>
    /// Encapsulates all configuration settings for the sync operation
    /// </summary>
    public class SyncConfiguration
    {
        public string SourcePath { get; set; } = string.Empty;
        public string TargetPath { get; set; } = string.Empty;
        public bool Verbose { get; set; }
        public bool LogToFile { get; set; }
        public bool Exclude { get; set; }
        public List<string> ExcludePaths { get; set; } = new List<string>();
        public bool UseFtp { get; set; }
        public bool PreservePermissionsAndTimestamps { get; set; }
        public string? FtpUsername { get; set; }
        public string? FtpPassword { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(SourcePath) && !string.IsNullOrEmpty(TargetPath);
        }

        public bool HasSamePaths()
        {
            return string.Equals(SourcePath, TargetPath, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
