// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension.ExceptionHandlingTests
{
    internal class ActivateAfterTestHookThrowingException : NUnitAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context?.HookExtension?.BeforeTest.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod("BeforeTestHook");
                throw new Exception("BeforeTest hook crashed!!");
            });

            context?.HookExtension?.BeforeTest.AddHandler(async (sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod("BeforeTestHook");
                await Task.Delay(100);
                throw new Exception("AsyncBeforeTest hook crashed!!");
            });

            context?.HookExtension?.AfterTest.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod("AfterTestHook");
                throw new Exception("AfterTestHook crashed!!");
            });

            context?.HookExtension?.AfterTest.AddHandler(async (sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod("AfterTestHook");
                await Task.Delay(100);
                throw new Exception("AsyncAfterTestHook crashed!!");
            });
        }
    }

    internal class ExceptionFromHooksAreNotPropagated
    {
        [TestSetupUnderTest]
        public class TestUnderTest
        {
            [Test, ActivateAfterTestHookThrowingException]
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
                "BeforeTestHook",
                "BeforeTestHook",
                "TestPasses",
                "AfterTestHook",
                "AfterTestHook",
                "TearDown",
                "OneTimeTearDown"
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
