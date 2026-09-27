using System;
using System.IO;

namespace SyncData.Test.TestDoubles
{
    /// <summary>
    /// Creates an isolated temporary directory and removes it on dispose.
    /// </summary>
    public sealed class TempDirectory : IDisposable
    {
        public string Path { get; }

        public TempDirectory()
        {
            Path = GetUniquePath();
            Directory.CreateDirectory(Path);
        }

        /// <summary>
        /// Returns a path under the temp folder that is guaranteed not to exist yet.
        /// </summary>
        public static string GetUniquePath()
        {
            return System.IO.Path.Combine(
                System.IO.Path.GetTempPath(),
                "SyncDataTests_" + Guid.NewGuid().ToString("N"));
        }

        public string CreateFile(string relativePath, string content)
        {
            var fullPath = System.IO.Path.Combine(Path, relativePath);
            var directory = System.IO.Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(fullPath, content);
            return fullPath;
        }

        public string CreateDirectory(string relativePath)
        {
            var fullPath = System.IO.Path.Combine(Path, relativePath);
            Directory.CreateDirectory(fullPath);
            return fullPath;
        }

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(Path))
                {
                    Directory.Delete(Path, true);
                }
            }
            catch
            {
                // Best-effort cleanup
            }
        }
    }
}
