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
            context?.HookExtension?.BeforeAnySetUps.AddHandler(async (sender, eventArgs) =>
            {
                await Task.Delay(1000);
                TestLog.LogCurrentMethod(HookIdentifiers.BeforeAnySetUpsHook);
            });

            context?.HookExtension?.AfterAnySetUps.AddHandler(async (sender, eventArgs) =>
            {
                await Task.Delay(1000);
                TestLog.LogCurrentMethod(HookIdentifiers.AfterAnySetUpsHook);
            });

            context?.HookExtension?.BeforeTest.AddHandler(async (sender, eventArgs) =>
            {
                await Task.Delay(1000);
                TestLog.LogCurrentMethod(HookIdentifiers.BeforeTestHook);
            });

            context?.HookExtension?.AfterTest.AddHandler(async (sender, eventArgs) =>
            {
                await Task.Delay(1000);
                TestLog.LogCurrentMethod(HookIdentifiers.AfterTestHook);
            });

            context?.HookExtension?.BeforeAnyTearDowns.AddHandler(async (sender, eventArgs) =>
            {
                await Task.Delay(1000);
                TestLog.LogCurrentMethod(HookIdentifiers.BeforeAnyTearDownsHook);
            });

            context?.HookExtension?.AfterAnyTearDowns.AddHandler(async (sender, eventArgs) =>
            {
                await Task.Delay(1000);
                TestLog.LogCurrentMethod(HookIdentifiers.AfterAnyTearDownsHook);
            });
        }
    }
}
