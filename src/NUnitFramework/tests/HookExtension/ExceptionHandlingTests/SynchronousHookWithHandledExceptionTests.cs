// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension.ExceptionHandlingTests
{
    internal class ActivateSynchronousHookThrowingException : NUnitAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context?.HookExtension?.AfterTest.AddHandler((sender, eventArgs) =>
            {
                // If exception is thrown without being handled then a new thread will be picked up from thread pool.
                // In order to simulate that the hook is running in the same thread and not another one picked from thread pool try-catch is used.
                // ToDo: Understand why if no try-catch is used then the thread IDs are different!!
                try
                {
                    throw new Exception("BeforeTestHook crashed!!");
                }
                catch
                {
                    TestExecutionContext.CurrentContext
                                        .CurrentTest.Properties
                                        .Add("BeforeTestHook_ThreadId", Thread.CurrentThread.ManagedThreadId);
                }

            });
        }
    }

    internal class SynchronousHookWithHandledExceptionTests
    {
        [TestSetupUnderTest]
        public class TestUnderTest
        {
            private void CacheThreadId()
            {
                TestExecutionContext.CurrentContext
                                    .CurrentTest.Properties
                                    .Add("TestThreadId", Thread.CurrentThread.ManagedThreadId);
            }

            [Test, ActivateSynchronousHookThrowingException]
            public void TestPasses_WithSimpleAssert()
            {
                CacheThreadId();
                Assert.That(1, Is.EqualTo(1));
            }

            [Test, ActivateSynchronousHookThrowingException]
            public void TestPasses_WithAssertPass()
            {
                CacheThreadId();
                Assert.Pass("Another test passed.");
            }

            [Test, ActivateSynchronousHookThrowingException]
            public void TestFails_WithAssertFail()
            {
                CacheThreadId();
                Assert.Fail("Test failed with Assert.Fail");
            }

            [Test, ActivateSynchronousHookThrowingException]
            public void TestFails_WithException()
            {
                CacheThreadId();
                throw new Exception("Test failed with Exception");
            }
        }

        [Test]
        [NonParallelizable]
        public void SynchronousHookInvocation_ExceptionInHook_DoesNotStopTestExecution()
        {
            var testResult = TestsUnderTest.Execute();

            Assert.That(testResult.TestRunResult.Passed, Is.EqualTo(2));
            Assert.That(testResult.TestRunResult.Failed, Is.EqualTo(2));

            foreach (var testCase in testResult.TestRunResult.TestCases)
            {
                var testThreadId = int.Parse(testCase.Properties["TestThreadId"].First());
                var beforeTestHookThreadId = int.Parse(testCase.Properties["BeforeTestHook_ThreadId"].First());
                Assert.That(testThreadId, Is.EqualTo(beforeTestHookThreadId));
            }
        }
    }
}
