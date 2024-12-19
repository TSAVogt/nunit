// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Internal.HookExtensions
{
    internal class TestHookTestMethodEventArgs(TestExecutionContext context) : EventArgs
    {
        public TestExecutionContext Context { get; } = context;
    }
}