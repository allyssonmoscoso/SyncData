using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace SyncData.Test
{
    public class TestUtility
    {
        [Fact]
        public void LogMessage_ShouldWriteToLogFile()
        {
            // Arrange
            string logFilePath = "syncData.log";
            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }

            string status = "Success";
            string message = "Test log message";
            bool verbose = false;
            bool logToFile = true;

            // Act
            Utility.LogMessage(status, message, verbose, logToFile);

            // Assert
            Assert.True(File.Exists(logFilePath));
            string logContent = File.ReadAllText(logFilePath);
            Assert.Contains("Test log message", logContent);

            // Cleanup
            File.Delete(logFilePath);
        }

        [Fact]
        public async Task SynchronizeDirectories_ShouldSynchronizeFilesAndDirectories()
        {
            // Arrange
            string sourceDir = Path.Combine(Path.GetTempPath(), "SourceDir");
            string targetDir = Path.Combine(Path.GetTempPath(), "TargetDir");

            Directory.CreateDirectory(sourceDir);
            Directory.CreateDirectory(targetDir);

            string sourceFile = Path.Combine(sourceDir, "testFile.txt");
            File.WriteAllText(sourceFile, "Test content");

            string excludeFile = Path.Combine(sourceDir, "excludeFile.txt");
            File.WriteAllText(excludeFile, "Exclude content");

            List<string> excludePaths = new List<string> { "excludeFile.txt" };

            bool verbose = false;
            bool logToFile = false;
            bool exclude = true;
            bool useFtp = false;
            bool preservePermissionsAndTimestamps = true;
            string userFtp = "user";
            string passwordFtp = "password";

            // Act
            await Utility.SynchronizeDirectories(
                sourceDir,
                targetDir,
                verbose,
                logToFile,
                exclude,
                excludePaths,
                useFtp,
                preservePermissionsAndTimestamps,
                userFtp,
                passwordFtp
            );

            // Assert
            string targetFile = Path.Combine(targetDir, "testFile.txt");
            Assert.True(File.Exists(targetFile));
            Assert.Equal("Test content", File.ReadAllText(targetFile));

            string excludedTargetFile = Path.Combine(targetDir, "excludeFile.txt");
            Assert.False(File.Exists(excludedTargetFile));

            // Cleanup
            Directory.Delete(sourceDir, true);
            Directory.Delete(targetDir, true);
        }

        [Fact]
        public async Task SynchronizeDirectories_ShouldCreateTargetDirectoryIfNotExists()
        {
            // Arrange
            string sourceDir = Path.Combine(Path.GetTempPath(), "SourceDir");
            string targetDir = Path.Combine(Path.GetTempPath(), "NonExistentTargetDir");

            Directory.CreateDirectory(sourceDir);

            string sourceFile = Path.Combine(sourceDir, "testFile.txt");
            File.WriteAllText(sourceFile, "Test content");

            List<string> excludePaths = new List<string>();

            bool verbose = false;
            bool logToFile = false;
            bool exclude = false;
            bool useFtp = false;
            bool preservePermissionsAndTimestamps = true;
            string userFtp = "user";
            string passwordFtp = "password";

            // Act
            await Utility.SynchronizeDirectories(
                sourceDir,
                targetDir,
                verbose,
                logToFile,
                exclude,
                excludePaths,
                useFtp,
                preservePermissionsAndTimestamps,
                userFtp,
                passwordFtp
            );

            // Assert
            Assert.True(Directory.Exists(targetDir));
            string targetFile = Path.Combine(targetDir, "testFile.txt");
            Assert.True(File.Exists(targetFile));
            Assert.Equal("Test content", File.ReadAllText(targetFile));

            // Cleanup
            Directory.Delete(sourceDir, true);
            Directory.Delete(targetDir, true);
        }
    }
}