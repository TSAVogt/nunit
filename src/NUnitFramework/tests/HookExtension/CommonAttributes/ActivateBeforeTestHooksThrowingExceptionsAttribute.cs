// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension.CommonAttributes
{
    internal class ActivateBeforeTestHooksThrowingExceptionsAttribute : NUnitAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context?.HookExtension?.BeforeTest.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod(HookIdentifiers.BeforeTestHook);
                throw new Exception("After test hook crashed");
            });

            context?.HookExtension?.BeforeTest.AddHandler(async (sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod(HookIdentifiers.BeforeTestHook);
                await Task.Delay(100);
                throw new Exception("After test hook crashed");
            });
        }
    }
}
