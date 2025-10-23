using System;

namespace SyncData.Logging
{
    /// <summary>
    /// Abstract base class for logging operations
    /// </summary>
    public abstract class Logger
    {
        public abstract void Log(string status, string message);
        
        public void LogError(string message) => Log("Error", message);
        public void LogSuccess(string message) => Log("Success", message);
        public void LogInfo(string message) => Log("Info", message);
    }
}
