// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NUnit.Framework.Internal.HookExtensions
{
    /// <summary>
    /// Event that supports both synchronous and asynchronous handlers.
    /// </summary>
    public sealed class AsyncEvent<TEventArgs>
    {
        private readonly List<Delegate> _handlers = new();

        /// <summary>
        /// Constructor that initializes the event and provides an event handler to invoke it.
        /// </summary>
        /// <param name="invoke">
        /// The event handler to invoke the event.
        /// </param>
        public AsyncEvent(out AsyncEventHandler<TEventArgs> invoke)
        {
            invoke = Invoke;
        }

        /// <summary>
        /// Adds a synchronous handler to the event.
        /// </summary>
        /// <param name="handler">The event handler to be attached to the event.</param>
        public void AddHandler(EventHandler<TEventArgs> handler)
        {
            lock (_handlers)
                _handlers.Add(handler);
        }

        /// <summary>
        /// Adds an asynchronous handler to the event.
        /// </summary>
        /// <param name="asyncHandler">The event handler to be attached to the event.</param>
        public void AddAsyncHandler(AsyncEventHandler<TEventArgs> asyncHandler)
        {
            lock (_handlers)
                _handlers.Add(asyncHandler);
        }

        private async Task Invoke(object? sender, TEventArgs e)
        {
            if (!_handlers.Any())
            {
                return;
            }

            Delegate[] handlers;
            lock (_handlers)
                handlers = _handlers.ToArray();

            var tasks = new List<Task>(handlers.Length);

            foreach (var handler in handlers)
            {
                if (handler is EventHandler<TEventArgs> syncHandler)
                {
                    try
                    {
                        syncHandler(sender, e);
                    }
                    catch (Exception ex)
                    {
                        (e as TestHookTestMethodEventArgs)?.Context.CurrentResult.RecordException(ex);
                    }
                }
                else if (handler is AsyncEventHandler<TEventArgs> asyncHandler)
                {
                    // In order to ensure that all exceptions are logged while keeping the method properly async:
                    // await Task.WhenAll: Avoids blocking the thread and unwrapping AggregateException.
                    // Each asyncHandler is inside a try-catch: Ensures each handler's exception is logged.
                    tasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            await asyncHandler(sender, e);
                        }
                        catch (Exception ex)
                        {
                            (e as TestHookTestMethodEventArgs)?.Context.CurrentResult.RecordException(ex);
                        }
                    }));
                }
            }

            await Task.WhenAll(tasks);
        }
    }
}
