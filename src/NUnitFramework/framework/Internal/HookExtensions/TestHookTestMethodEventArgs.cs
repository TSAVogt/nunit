// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Internal.HookExtensions
{
    /// <summary>
    /// Represents event arguments for test hook methods that involve test methods.
    /// </summary>
    public class TestHookTestMethodEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestHookTestMethodEventArgs"/> class.
        /// </summary>
        /// <param name="context">The test execution context.</param>
        /// <param name="exceptionContext">The exception context that was thrown during the method execution, if any.</param>
        public TestHookTestMethodEventArgs(TestExecutionContext context, Exception? exceptionContext = null) : base()
        {
            Context = context;
            ExceptionContext = exceptionContext;
        }

        /// <summary>
        /// Gets the test execution context.
        /// </summary>
        public TestExecutionContext Context { get; }

        /// <summary>
        /// Gets the exception context that was thrown during the method execution, if any.
        /// </summary>
        public Exception? ExceptionContext { get; }
    }
}
