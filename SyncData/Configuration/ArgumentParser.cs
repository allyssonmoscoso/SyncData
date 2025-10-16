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
                    case "-v":
                    case "-verbose":
                        config.Verbose = true;
                        break;
                    case "-log-file":
                        config.LogToFile = true;
                        break;
                    case var excludeArg when excludeArg.StartsWith("-exclude="):
                        config.Exclude = true;
                        config.ExcludePaths.AddRange(
                            excludeArg.Substring(9)
                                .Trim('{', '}')
                                .Split(',')
                                .Select(p => p.Trim())
                                .Where(p => !string.IsNullOrWhiteSpace(p))
                        );
                        break;
                    case "-ftp":
                        config.UseFtp = true;
                        break;
                    case "-preserve":
                        config.PreservePermissionsAndTimestamps = true;
                        break;
                    default:
                        if (arg.StartsWith("-source="))
                        {
                            config.SourcePath = arg.Substring(8);
                        }
                        else if (arg.StartsWith("-target="))
                        {
                            config.TargetPath = arg.Substring(8);
                        }
                        break;
                }
            }

            return config;
        }
    }
}
