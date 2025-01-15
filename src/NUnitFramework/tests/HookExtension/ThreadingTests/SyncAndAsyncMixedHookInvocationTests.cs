// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension.ThreadingTests
{
    internal class ActivateSyncAndAsyncHook : NUnitAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context?.HookExtension?.BeforeTest.AddHandler((sender, eventArgs) =>
            {
                TestExecutionContext.CurrentContext
                                    .CurrentTest.Properties
                                    .Add("BeforeTestSyncHook_ThreadId", Thread.CurrentThread.ManagedThreadId);
            });

            context?.HookExtension?.BeforeTest.AddHandler(async (sender, eventArgs) =>
            {
                TestExecutionContext.CurrentContext
                                    .CurrentTest.Properties
                                    .Add("BeforeTestAsyncHook_ThreadId", Thread.CurrentThread.ManagedThreadId);

                await Task.Delay(1);
            });
        }
    }

    internal class SyncAndAsyncMixedHookInvocationTests
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

            [Test, ActivateSyncAndAsyncHook]
            public void TestPasses_WithAssertPass()
            {
                Assert.Pass("Test passed.");
            }

            [Test, ActivateSyncAndAsyncHook]
            public void TestFails_WithAssertFail()
            {
                Assert.Fail("Test failed with Assert.Fail");
            }

            [Test, ActivateSyncAndAsyncHook]
            public void TestFails_WithException()
            {
                throw new Exception("Test failed with Exception");
            }
        }

        [Test]
        [NonParallelizable]
        [Explicit("Is it a valid requirement(from discussion thread) that:" +
                  " the hooks must be executing in the same thread as test itself because of usage of async-wait? ")]
        public void SyncAndAsyncMixedHookInvocation_HooksExecutesTest()
        {
            var testResult = TestsUnderTest.Execute();

            Assert.That(testResult.TestRunResult.Passed, Is.EqualTo(1));
            Assert.That(testResult.TestRunResult.Failed, Is.EqualTo(2));

            foreach (var testCase in testResult.TestRunResult.TestCases)
            {
                var testThreadId = int.Parse(testCase.Properties["TestThreadId"].First());
                var beforeTestSyncHookThreadId = int.Parse(testCase.Properties["BeforeTestSyncHook_ThreadId"].First());
                var beforeTestAsyncHookThreadId = int.Parse(testCase.Properties["BeforeTestAsyncHook_ThreadId"].First());

                Assert.That(testThreadId, Is.EqualTo(beforeTestSyncHookThreadId));
                Assert.That(testThreadId, !Is.EqualTo(beforeTestAsyncHookThreadId));
                Assert.That(beforeTestSyncHookThreadId, !Is.EqualTo(beforeTestAsyncHookThreadId));
            }
        }
    }
}
