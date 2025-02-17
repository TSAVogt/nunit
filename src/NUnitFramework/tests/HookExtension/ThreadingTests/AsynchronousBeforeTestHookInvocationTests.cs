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
    internal class ActivateMultipleAsynchronousBeforeTestHooks : NUnitAttribute, IApplyToContext
    {
        public static TaskCompletionSource<bool> StartSignal = new();

        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context?.HookExtension?.BeforeTest.AddHandler(async (sender, eventArgs) =>
            {
                TestExecutionContext.CurrentContext
                                    .CurrentTest.Properties
                                    .Add("BeforeTestHook_1_ThreadId", Thread.CurrentThread.ManagedThreadId);

                // Wait for all tasks to be scheduled before proceeding
                await StartSignal.Task;

                // Some work
                await Task.Delay(100);
            });

            context?.HookExtension?.BeforeTest.AddHandler(async (sender, eventArgs) =>
            {
                TestExecutionContext.CurrentContext
                                    .CurrentTest.Properties
                                    .Add("BeforeTestHook_2_ThreadId", Thread.CurrentThread.ManagedThreadId);

                // Wait for all tasks to be scheduled before proceeding
                await StartSignal.Task;

                // Some work
                await Task.Delay(100);
            });
        }
    }

    internal class AsynchronousBeforeTestHookInvocationTests
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

            [Test, ActivateMultipleAsynchronousBeforeTestHooks]
            public void TestPasses_WithAssertPass()
            {
                Assert.Pass("Test passed.");
            }

            [Test, ActivateMultipleAsynchronousBeforeTestHooks]
            public void TestFails_WithAssertFail()
            {
                Assert.Fail("Test failed with Assert.Fail");
            }

            [Test, ActivateMultipleAsynchronousBeforeTestHooks]
            public void TestFails_WithException()
            {
                throw new Exception("Test failed with Exception");
            }
        }

        [Test]
        [NonParallelizable]
        public void AsynchronousHookInvocation_HookExecutesInSeparateThreads()
        {
            ActivateMultipleAsynchronousBeforeTestHooks.StartSignal.SetResult(true);

            var testResult = TestsUnderTest.Execute();

            Assert.That(testResult.TestRunResult.Passed, Is.EqualTo(1));
            Assert.That(testResult.TestRunResult.Failed, Is.EqualTo(2));
            
            foreach (var testCase in testResult.TestRunResult.TestCases)
            {
                var testThreadId = int.Parse(testCase.Properties["TestThreadId"].First());

                var beforeTestHook1ThreadId = int.Parse(testCase.Properties["BeforeTestHook_1_ThreadId"].First());
                var beforeTestHook2ThreadId = int.Parse(testCase.Properties["BeforeTestHook_2_ThreadId"].First());

                CollectionAssert.AllItemsAreUnique(new List<int>()
                {
                    testThreadId,
                    
                    beforeTestHook1ThreadId,
                    beforeTestHook2ThreadId
                });
            }
        }
    }
}
