// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension.CommonAttributes
{
    internal class ActivateBeforeTestHooksAttribute : NUnitAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context?.HookExtension?.BeforeTest.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod(HookIdentifiers.BeforeTestHook);
            });

            context?.HookExtension?.BeforeTest.AddHandler(async (sender, eventArgs) =>
            {
                await Task.Delay(100);
                TestLog.LogCurrentMethod(HookIdentifiers.BeforeTestHook);
            });
        }
    }
}
