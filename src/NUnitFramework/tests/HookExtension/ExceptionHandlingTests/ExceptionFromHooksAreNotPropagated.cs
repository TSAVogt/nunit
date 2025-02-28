// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.HookExtension.CommonAttributes;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension.ExceptionHandlingTests
{
    internal class ExceptionFromHooksAreNotPropagated
    {
        [TestSetupUnderTest]
        public class TestUnderTest
        {
            [Test]
            [ActivateBeforeTestHooksThrowingExceptions]
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
        public void ExceptionsFromHooks_AreDigested()
        {
            var testResult = TestsUnderTest.Execute();
            Assert.That(testResult.TestRunResult.Passed, Is.EqualTo(1));

            Assert.That(TestContext.CurrentContext.Result.Message, Is.Empty);
            Assert.That(TestContext.CurrentContext.Result.StackTrace, Is.Null);

            Assert.That(testResult.Logs, Is.EqualTo([
                HookIdentifiers.BeforeTestHook,
                HookIdentifiers.BeforeTestHook,

                nameof(TestUnderTest.TestPasses),

                HookIdentifiers.AfterTestHook,
                HookIdentifiers.AfterTestHook,
                
                nameof(TestUnderTest.TearDown),
                nameof(TestUnderTest.OneTimeTearDown)
            ]));

            foreach (var testCase in testResult.TestRunResult.TestCases)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(testCase.Result, Is.EqualTo("Passed"));
                });
            }
        }
    }
}
