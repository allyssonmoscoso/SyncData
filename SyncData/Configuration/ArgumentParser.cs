using System;
using System.Linq;

namespace SyncData.Configuration
{
    /// <summary>
    /// Parses command-line arguments and creates a SyncConfiguration object
    /// </summary>
    public class ArgumentParser
    {
        public SyncConfiguration Parse(string[] args)
        {
            var config = new SyncConfiguration();

            foreach (var arg in args)
            {
                switch (arg)
                {
                    case AppConstants.VerboseFlag:
                    case AppConstants.VerboseFlagLong:
                        config.Verbose = true;
                        break;
                    case AppConstants.LogFileFlag:
                        config.LogToFile = true;
                        break;
                    case var excludeArg when excludeArg.StartsWith(AppConstants.ExcludeArgPrefix):
                        config.Exclude = true;
                        config.ExcludePaths.AddRange(
                            excludeArg.Substring(AppConstants.ExcludeArgPrefix.Length)
                                .Trim('{', '}')
                                .Split(',')
                                .Select(p => p.Trim())
                                .Where(p => !string.IsNullOrWhiteSpace(p))
                        );
                        break;
                    case AppConstants.FtpFlag:
                        config.UseFtp = true;
                        break;
                    case AppConstants.PreserveFlag:
                        config.PreservePermissionsAndTimestamps = true;
                        break;
                    default:
                        if (arg.StartsWith(AppConstants.SourceArgPrefix))
                        {
                            config.SourcePath = arg.Substring(AppConstants.SourceArgPrefix.Length);
                        }
                        else if (arg.StartsWith(AppConstants.TargetArgPrefix))
                        {
                            config.TargetPath = arg.Substring(AppConstants.TargetArgPrefix.Length);
                        }
                        break;
                }
            }

            return config;
        }
    }
}
