using System.Collections.Generic;

namespace SyncData.Logging
{
    /// <summary>
    /// Composite logger that can log to multiple destinations
    /// </summary>
    public class CompositeLogger : Logger
    {
        private readonly List<Logger> _loggers = new List<Logger>();

        public void AddLogger(Logger logger)
        {
            _loggers.Add(logger);
        }

        public override void Log(string status, string message)
        {
            foreach (var logger in _loggers)
            {
                logger.Log(status, message);
            }
        }
    }
}
