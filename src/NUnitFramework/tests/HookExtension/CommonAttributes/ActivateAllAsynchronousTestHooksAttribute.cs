// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension.CommonAttributes
{
    internal class ActivateAllAsynchronousTestHooksAttribute : NUnitAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context?.HookExtension?.BeforeAnySetUps.AddAsyncHandler(async (sender, eventArgs) =>
            {
                await Task.Delay(1000);
                TestLog.LogCurrentMethod(HookIdentifiers.BeforeAnySetUpsHook);
            });

            context?.HookExtension?.AfterAnySetUps.AddAsyncHandler(async (sender, eventArgs) =>
            {
                await Task.Delay(1000);
                TestLog.LogCurrentMethod(HookIdentifiers.AfterAnySetUpsHook);
            });

            context?.HookExtension?.BeforeTest.AddAsyncHandler(async (sender, eventArgs) =>
            {
                await Task.Delay(1000);
                TestLog.LogCurrentMethod(HookIdentifiers.BeforeTestHook);
            });

            context?.HookExtension?.AfterTest.AddAsyncHandler(async (sender, eventArgs) =>
            {
                await Task.Delay(1000);
                TestLog.LogCurrentMethod(HookIdentifiers.AfterTestHook);
            });

            context?.HookExtension?.BeforeAnyTearDowns.AddAsyncHandler(async (sender, eventArgs) =>
            {
                await Task.Delay(1000);
                TestLog.LogCurrentMethod(HookIdentifiers.BeforeAnyTearDownsHook);
            });

            context?.HookExtension?.AfterAnyTearDowns.AddAsyncHandler(async (sender, eventArgs) =>
            {
                await Task.Delay(1000);
                TestLog.LogCurrentMethod(HookIdentifiers.AfterAnyTearDownsHook);
            });
        }
    }
}
