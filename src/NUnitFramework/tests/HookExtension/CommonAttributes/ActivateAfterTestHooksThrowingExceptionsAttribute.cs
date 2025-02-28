// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension.CommonAttributes
{
    internal class ActivateAfterTestHooksThrowingExceptionsAttribute : NUnitAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context?.HookExtension?.AfterTest.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod(HookIdentifiers.AfterTestHook);
                throw new Exception("After test hook crashed");
            });

            context?.HookExtension?.AfterTest.AddHandler(async (sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod(HookIdentifiers.AfterTestHook);
                await Task.Delay(100);
                throw new Exception("After test hook crashed");
            });
        }
    }
}
