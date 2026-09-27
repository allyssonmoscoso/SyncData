using System.Collections.Generic;

namespace SyncData.Test.TestDoubles
{
    /// <summary>
    /// Captures every progress value reported during synchronization.
    /// </summary>
    public sealed class ProgressCollector : IProgress<double>
    {
        public List<double> Values { get; } = new List<double>();

        public void Report(double value)
        {
            Values.Add(value);
        }
    }
}
