// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension.CommonAttributes
{
    internal class ActivateLongRunningBeforeTestHooksAttribute : NUnitAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            TestExecutionContext.CurrentContext?.HookExtension?.BeforeTest.AddAsyncHandler(async (sender, eventArgs) =>
            {
                // Delay to ensure that handlers run longer than the test case
                ThreadUtility.BlockingDelay(1000);
                TestLog.LogCurrentMethod(HookIdentifiers.BeforeTestHook);
            });
            TestExecutionContext.CurrentContext?.HookExtension?.BeforeTest.AddAsyncHandler(async (sender, eventArgs) =>
            {
                // Delay to ensure that handlers run longer than the test case
                ThreadUtility.BlockingDelay(1000);
                TestLog.LogCurrentMethod(HookIdentifiers.BeforeTestHook);
            });
        }
    }
}
