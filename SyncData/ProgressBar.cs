//crear base de class ProgressBar
using System;

namespace SyncData
{

    public class ProgressBar : IDisposable, IProgress<double>
    {
        private const int blockCount = 10;
        private readonly TimeSpan animationInterval = TimeSpan.FromSeconds(1.0 / 8);
        private const string animation = @"|/-\";
        private int animationIndex = 0;

        private double currentProgress = 0;
        private string currentText = string.Empty;
        private bool disposed = false;
        private Timer timer;

        public ProgressBar()
        {
            timer = new Timer(TimerHandler);
            timer.Change(animationInterval, animationInterval);
        }

        public void Report(double value)
        {
            value = Math.Max(0, Math.Min(1, value));
            Interlocked.Exchange(ref currentProgress, value);
            var progressBlockCount = (int)(value * blockCount);
            var percent = (int)(value * 100);
            var text = string.Format("[{0}{1}] {2,3}% {3}",
                new string('#', progressBlockCount), new string('-', blockCount - progressBlockCount),
                percent,
                animation[animationIndex++ % animation.Length]);
            UpdateText(text);
        }

        private void TimerHandler(object state)
        {
            lock (timer)
            {
                if (disposed) return;
                animationIndex++;
                UpdateText(currentText);
            }
        }

        private void UpdateText(string text)
        {
            currentText = text;
            Console.Write('\r' + text + new string(' ', Console.WindowWidth - text.Length));
        }

        public void Dispose()
        {
            lock (timer)
            {
                disposed = true;
                UpdateText(string.Empty);
            }
        }
    }

}

