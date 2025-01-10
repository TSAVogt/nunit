// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension
{
    internal class ActivateAsynchronousHook : NUnitAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context?.HookExtension?.BeforeTest.AddHandler(async (sender, eventArgs) =>
            {
                TestExecutionContext.CurrentContext
                                    .CurrentTest.Properties
                                    .Add("BeforeTestHook_ThreadId", Thread.CurrentThread.ManagedThreadId);

                await Task.Delay(1);
            });

            context?.HookExtension?.AfterTest.AddHandler(async (sender, eventArgs) =>
            {
                TestExecutionContext.CurrentContext
                                    .CurrentTest.Properties
                                    .Add("AfterTestHook_ThreadId", Thread.CurrentThread.ManagedThreadId);

                await Task.Delay(1);
            });
        }
    }

    internal class AsynchronousHookInvocationTests
    {
        [TestSetupUnderTest]
        public class TestUnderTest
        {
            [SetUp]
            public void Setup()
            {
                TestExecutionContext.CurrentContext
                                    .CurrentTest.Properties
                                    .Add("TestThreadId", Thread.CurrentThread.ManagedThreadId);
            }

            [Test, ActivateAsynchronousHook]
            public void TestPasses_WithAssertPass()
            {
                Assert.Pass("Test passed.");
            }

            [Test, ActivateAsynchronousHook]
            public void TestFails_WithAssertFail()
            {
                Assert.Fail("Test failed with Assert.Fail");
            }

            [Test, ActivateAsynchronousHook]
            public void TestFails_WithException()
            {
                throw new Exception("Test failed with Exception");
            }
        }

        [Test]
        [NonParallelizable]
        [Explicit("Is it a valid requirement(from discussion thread) that:" +
                  " the hooks must be executing in the same thread as test itself because of usage of async-wait? ")]
        public void AsynchronousHookInvocation_HookExecutesInSeparateThread()
        {
            var testResult = TestsUnderTest.Execute();

            Assert.That(testResult.TestRunResult.Passed, Is.EqualTo(1));
            Assert.That(testResult.TestRunResult.Failed, Is.EqualTo(2));

            foreach (var testCase in testResult.TestRunResult.TestCases)
            {
                var testThreadId = int.Parse(testCase.Properties["TestThreadId"].First());
                var beforeTestHookThreadId = int.Parse(testCase.Properties["BeforeTestHook_ThreadId"].First());
                var afterTestHookThreadId = int.Parse(testCase.Properties["AfterTestHook_ThreadId"].First());

                Assert.That(testThreadId, !Is.EqualTo(beforeTestHookThreadId));
                Assert.That(testThreadId, !Is.EqualTo(afterTestHookThreadId));
            }
        }

        // todo: Additional tests to be added
        // test if the exception from Test is known by the Hook.
        // test if the exception from Hook is known by the Test.
        // test if there are two hooks and exception happens in 1 Hook. What happens to the other hook?
        // test for order of hooks
    }
}
