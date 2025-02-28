// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.HookExtension.CommonAttributes;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension.ExecutionSequence;

public class ExecutionProceedsOnlyAfterAllBeforeTestHooksExecute
{
    [TestSetupUnderTest]
    public class TestUnderTest
    {
        [Test]
        [ActivateBeforeTestHooks]
        [ActivateLongRunningBeforeTestHooks]
        public void SomeTest()
        {
            TestLog.LogCurrentMethod();
        }
    }

    [Test]
    [NonParallelizable]
    public void CheckThatLongRunningBeforeTestHooksCompleteBeforeTest()
    {
        var testResult = TestsUnderTest.Execute();

        Assert.That(testResult.Logs, Is.EqualTo([
            HookIdentifiers.BeforeTestHook,
            HookIdentifiers.BeforeTestHook,
            HookIdentifiers.BeforeTestHook,
            HookIdentifiers.BeforeTestHook,
            nameof(TestUnderTest.SomeTest)
        ]));
    }
}
