// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension.CommonAttributes
{
    internal class ActivateAllSynchronousTestHooksAttribute : NUnitAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context?.HookExtension?.BeforeAnySetUps.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod(HookIdentifiers.BeforeAnySetUpsHook);
            });

            context?.HookExtension?.AfterAnySetUps.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod(HookIdentifiers.AfterAnySetUpsHook);
            });

            context?.HookExtension?.BeforeTest.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod(HookIdentifiers.BeforeTestHook);
            });

            context?.HookExtension?.AfterTest.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod(HookIdentifiers.AfterTestHook);
            });

            context?.HookExtension?.BeforeAnyTearDowns.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod(HookIdentifiers.BeforeAnyTearDownsHook);
            });

            context?.HookExtension?.AfterAnyTearDowns.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod(HookIdentifiers.AfterAnyTearDownsHook);
            });
        }
    }
}
