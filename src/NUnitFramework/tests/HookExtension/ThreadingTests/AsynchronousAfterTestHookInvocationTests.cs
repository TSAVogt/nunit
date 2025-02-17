// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Legacy;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension.ThreadingTests
{
    internal class ActivateMultipleAsynchronousHooks : NUnitAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context?.HookExtension?.AfterTest.AddHandler(async (sender, eventArgs) =>
            {
                TestExecutionContext.CurrentContext
                                    .CurrentTest.Properties
                                    .Add("AfterTestHook_1_ThreadId", Thread.CurrentThread.ManagedThreadId);

                await Task.Delay(1);
            });

            context?.HookExtension?.AfterTest.AddHandler(async (sender, eventArgs) =>
            {
                TestExecutionContext.CurrentContext
                                    .CurrentTest.Properties
                                    .Add("AfterTestHook_2_ThreadId", Thread.CurrentThread.ManagedThreadId);

                await Task.Delay(1);
            });
        }
    }

    internal class AsynchronousAfterHookInvocationTests
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

            [Test, ActivateMultipleAsynchronousHooks]
            public void TestPasses_WithAssertPass()
            {
                Assert.Pass("Test passed.");
            }

            [Test, ActivateMultipleAsynchronousHooks]
            public void TestFails_WithAssertFail()
            {
                Assert.Fail("Test failed with Assert.Fail");
            }

            [Test, ActivateMultipleAsynchronousHooks]
            public void TestFails_WithException()
            {
                throw new Exception("Test failed with Exception");
            }
        }

        [Test]
        [Parallelizable]
        [Explicit("Thread IDs is not a good way to test. Are we testing dot net framework here??")]
        public void AsynchronousHookInvocation_HookExecutesInSeparateThreads()
        {
            var testResult = TestsUnderTest.Execute();

            Assert.That(testResult.TestRunResult.Passed, Is.EqualTo(1));
            Assert.That(testResult.TestRunResult.Failed, Is.EqualTo(2));

            foreach (var testCase in testResult.TestRunResult.TestCases)
            {
                var testThreadId = int.Parse(testCase.Properties["TestThreadId"].First());

                var afterTestHook1ThreadId = int.Parse(testCase.Properties["AfterTestHook_1_ThreadId"].First());
                var afterTestHook2ThreadId = int.Parse(testCase.Properties["AfterTestHook_2_ThreadId"].First());

                CollectionAssert.AllItemsAreUnique(new List<int>()
                {
                    testThreadId,
                    
                    afterTestHook1ThreadId,
                    afterTestHook2ThreadId
                });
            }
        }
    }
}
