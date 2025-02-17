// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension.ThreadingTests
{
    internal class ActivateSynchronousHook : NUnitAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context?.HookExtension?.BeforeTest.AddHandler((sender, eventArgs) =>
            {
                TestExecutionContext.CurrentContext
                                    .CurrentTest.Properties
                                    .Add("BeforeTestHook_ThreadId", Thread.CurrentThread.ManagedThreadId);
            });

            context?.HookExtension?.AfterTest.AddHandler((sender, eventArgs) =>
            {
                TestExecutionContext.CurrentContext
                                    .CurrentTest.Properties
                                    .Add("AfterTestHook_ThreadId", Thread.CurrentThread.ManagedThreadId);
            });
        }
    }

    internal class SynchronousHookInvocationTests
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

            [Test, ActivateSynchronousHook]
            public void TestPasses_WithAssertPass()
            {
                Assert.Pass("Test passed.");
            }

            [Test, ActivateSynchronousHook]
            public void TestFails_WithAssertFail()
            {
                Assert.Fail("Test failed with Assert.Fail");
            }

            [Test, ActivateSynchronousHook]
            public void TestFails_WithException()
            {
                throw new Exception("Test failed with Exception");
            }
        }

        [Test]
        [NonParallelizable]
        public void SynchronousHookInvocation_HookExecutesInSameThreadAsTest()
        {
            var testResult = TestsUnderTest.Execute();

            Assert.That(testResult.TestRunResult.Passed, Is.EqualTo(1));
            Assert.That(testResult.TestRunResult.Failed, Is.EqualTo(2));

            foreach (var testCase in testResult.TestRunResult.TestCases)
            {
                var testThreadId = int.Parse(testCase.Properties["TestThreadId"].First());
                var beforeTestHookThreadId = int.Parse(testCase.Properties["BeforeTestHook_ThreadId"].First());
                var afterTestHookThreadId = int.Parse(testCase.Properties["AfterTestHook_ThreadId"].First());

                Assert.That((new List<int>() { testThreadId, beforeTestHookThreadId, afterTestHookThreadId }).All
                (n => n == testThreadId));
            }
        }
    }
}
