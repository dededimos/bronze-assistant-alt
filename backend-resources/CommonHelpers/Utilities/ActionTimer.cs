using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CommonHelpers.Utilities
{
    public class ActionTimer : IDisposable
    {
        private System.Timers.Timer timer;
        private readonly double interval;
        private readonly Action action;
        private bool isRunning;

        /// <summary>
        /// Creates a timer that will execute an action after the interval passes
        /// </summary>
        /// <param name="intervalMilliseconds">Milliseconds</param>
        /// <param name="action">The Action to be Executed once the timer ends</param>
        public ActionTimer(double intervalMilliseconds, Action action)
        {
            this.interval = intervalMilliseconds;
            this.action = action;
            timer = new System.Timers.Timer(this.interval);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = false; // Ensure the timer only runs once per interval
        }

        /// <summary>
        /// Weather the timer is running
        /// </summary>
        public bool IsRunning { get => isRunning; }

        /// <summary>
        /// Starts running the timer;
        /// </summary>
        public void Start()
        {
            isRunning = true;
            timer.Start();
        }
        /// <summary>
        /// Resets the timer to start again
        /// </summary>
        public void Reset()
        {
            timer.Stop();
            timer.Start();
        }
        /// <summary>
        /// Stops the Timer from running
        /// </summary>
        public void Stop()
        {
            isRunning = false;
            timer.Stop();
        }

        private void OnTimedEvent(object? sender, ElapsedEventArgs e)
        {
            action.Invoke();
            isRunning = false;
            timer.Stop();
        }


        private bool _disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (timer != null)
                    {
                        timer.Elapsed -= OnTimedEvent;
                        timer.Dispose();
                        timer = null!;
                    }
                }
                _disposed = true;
            }
        }

        ~ActionTimer()
        {
            Dispose(false);
        }
    }
}
