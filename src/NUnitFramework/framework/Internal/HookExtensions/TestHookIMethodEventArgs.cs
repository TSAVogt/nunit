// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.HookExtensions
{
    /// <summary>
    /// Represents event arguments for test hook methods.
    /// </summary>
    public class TestHookIMethodEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestHookIMethodEventArgs"/> class.
        /// </summary>
        /// <param name="context">The test execution context.</param>
        /// <param name="method">The method information.</param>
        /// <param name="exceptionContext">The exception context that was thrown during the method execution, if any.</param>
        public TestHookIMethodEventArgs(TestExecutionContext context, IMethodInfo method, Exception? exceptionContext = null)
        {
            Context = context;
            Method = method;
            ExceptionContext = exceptionContext;
        }

        /// <summary>
        /// Gets the test execution context.
        /// </summary>
        public TestExecutionContext Context { get; }

        /// <summary>
        /// Gets the method information.
        /// </summary>
        public IMethodInfo Method { get; }

        /// <summary>
        /// Gets the exception context that was thrown during the method execution, if any.
        /// </summary>
        public Exception? ExceptionContext { get; }
    }
}
