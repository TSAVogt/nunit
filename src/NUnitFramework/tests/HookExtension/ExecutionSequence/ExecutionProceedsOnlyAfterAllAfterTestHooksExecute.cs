// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.HookExtension.CommonAttributes;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension.ExecutionSequence
{
    internal class ExecutionProceedsOnlyAfterAllAfterTestHooksExecute
    {
        [TestSetupUnderTest]
        public class TestUnderTest
        {
            [Test]
            [ActivateAfterTestHooks]
            [ActivateAfterTestHooksThrowingExceptions]
            public void TestPasses()
            {
                TestLog.LogCurrentMethod();
            }

            [TearDown]
            public void TearDown()
            {
                TestLog.LogCurrentMethod();
            }

            [OneTimeTearDown]
            public void OneTimeTearDown()
            {
                TestLog.LogCurrentMethod();
            }
        }

        [Test]
        [NonParallelizable]
        public void TestProceedsAfterAllAfterTestHooksExecute()
        {
            var testResult = TestsUnderTest.Execute();

            Assert.That(testResult.Logs, Is.EqualTo([
                nameof(TestUnderTest.TestPasses),

                HookIdentifiers.AfterTestHook,
                HookIdentifiers.AfterTestHook,
                HookIdentifiers.AfterTestHook,
                HookIdentifiers.AfterTestHook,
                
                nameof(TestUnderTest.TearDown),
                nameof(TestUnderTest.OneTimeTearDown)
            ]));
        }
    }
}
