using System.Collections.Generic;
using System.Linq;
using SyncData.Logging;

namespace SyncData.Test.TestDoubles
{
    /// <summary>
    /// Test double for <see cref="Logger"/> that records every logged entry.
    /// </summary>
    public sealed class FakeLogger : Logger
    {
        public List<(string Status, string Message)> Entries { get; } = new List<(string, string)>();

        public override void Log(string status, string message)
        {
            Entries.Add((status, message));
        }

        public bool HasStatus(string status) => Entries.Any(e => e.Status == status);

        public bool Contains(string text) => Entries.Any(e => e.Message.Contains(text));
    }
}
