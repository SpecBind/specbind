// <copyright file="Waiter.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Helpers
{
    using System;

    /// <summary>
    /// Provides a handy wait helper when you need to wait until a condition is met.
    /// </summary>
    public class Waiter
    {
        /// <summary>
        /// Creates a waiter with default timeout and waitInterval.
        /// </summary>
        public Waiter()
            : this(timeout: TimeSpan.FromSeconds(30), waitInterval: TimeSpan.FromMilliseconds(200))
        {
        }

        /// <summary>
        /// Creates a waiter with the specified timeout and default waitInterval.
        /// </summary>
        /// <param name="timeout">The duration after which to stop waiting.</param>
        public Waiter(TimeSpan timeout)
            : this(timeout: timeout, waitInterval: TimeSpan.FromMilliseconds(200))
        {
        }

        /// <summary>
        /// Creates a waiter with the specified timeout and waitInterval.
        /// </summary>
        /// <param name="timeout">The duration after which to stop waiting.</param>
        /// <param name="waitInterval">The length of time to wait between retries.</param>
        public Waiter(TimeSpan timeout, TimeSpan waitInterval)
        {
            this.Timeout = timeout;
            this.WaitInterval = waitInterval;
        }

        /// <summary>
        /// Gets or sets the Timeout.
        /// </summary>
        public TimeSpan Timeout { get; protected set; }

        /// <summary>
        /// Gets or sets the WaitInterval.
        /// </summary>
        public TimeSpan WaitInterval { get; protected set; }

        /// <summary>
        /// Waits for the given condition function to return true, or until the timeout is reached.
        /// </summary>
        /// <param name="conditionChecker">The condition function to check.</param>
        /// <exception cref="TimeoutException">The condition was not met before the timeout was reached.</exception>
        public void WaitFor(Func<bool> conditionChecker)
        {
            var startTime = DateTime.Now;
            var waitIntervalMilliseconds = Convert.ToInt32(this.WaitInterval.TotalMilliseconds);

            // We must wait for conditions in the same thread to support Microsfot Coded UI tests
            // because they're run in Single Thread Apartment (STA) mode of COM
            // See https://github.com/dpiessens/specbind/issues/111
            do
            {
                if (conditionChecker())
                {
                    return;
                }

                System.Threading.Thread.Sleep(waitIntervalMilliseconds);
            }
            while (DateTime.Now - startTime < this.Timeout);

            throw new TimeoutException("Timed out after " + this.Timeout);
        }
    }

    /// <summary>
    /// Provides a handy wait helper when you need to wait until a condition is met.
    /// </summary>
    /// <typeparam name="T">The type of the element being waited for.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "SA1402", Justification = "Generic and Non-generic versions of same class")]
    public class Waiter<T>
    {
        /// <summary>
        /// Creates a waiter with default timeout and waitInterval.
        /// </summary>
        public Waiter() : this(timeout: TimeSpan.FromSeconds(30), waitInterval: TimeSpan.FromMilliseconds(200))
        {
        }

        /// <summary>
        /// Creates a waiter with the specified timeout and default waitInterval.
        /// </summary>
        /// <param name="timeout">The duration after which to stop waiting.</param>
        public Waiter(TimeSpan timeout) : this(timeout: timeout, waitInterval: TimeSpan.FromMilliseconds(200))
        {
        }

        /// <summary>
        /// Creates a waiter with the specified timeout and waitInterval.
        /// </summary>
        /// <param name="timeout">The duration after which to stop waiting.</param>
        /// <param name="waitInterval">The length of time to wait between retries.</param>
        public Waiter(TimeSpan timeout, TimeSpan waitInterval)
        {
            this.Timeout = timeout;
            this.WaitInterval = waitInterval;
        }

        /// <summary>
        /// Gets or sets the Timeout.
        /// </summary>
        public TimeSpan Timeout { get; protected set; }

        /// <summary>
        /// Gets or sets the WaitInterval.
        /// </summary>
        public TimeSpan WaitInterval { get; protected set; }

        /// <summary>
        /// Waits for the given condition function to return true, or until the timeout is reached.
        /// </summary>
        /// <param name="element">The object being checked, to pass to the condition function.</param>
        /// <param name="conditionChecker">The condition function to check.</param>
        /// <exception cref="TimeoutException">The condition was not met before the timeout was reached.</exception>
        public void WaitFor(T element, Func<T, bool> conditionChecker)
        {
            var startTime = DateTime.Now;
            var waitIntervalMilliseconds = Convert.ToInt32(this.WaitInterval.TotalMilliseconds);

            // We must wait for conditions in the same thread to support Microsfot Coded UI tests
            // because they're run in Single Thread Apartment (STA) mode of COM
            // See https://github.com/dpiessens/specbind/issues/111
            do
            {
                if (conditionChecker(element))
                {
                    return;
                }

                System.Threading.Thread.Sleep(waitIntervalMilliseconds);
            }
            while (DateTime.Now - startTime < this.Timeout);

            throw new TimeoutException("Timed out after " + this.Timeout);
        }
    }
}