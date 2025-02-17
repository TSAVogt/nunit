// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension.ExceptionHandlingTests
{
    internal class ActivateAfterTestHook : NUnitAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context?.HookExtension?.AfterTest.AddHandler((sender, eventArgs) =>
            {
                TestExecutionContext.CurrentContext.CurrentTest.Properties.Add("NotImplementedException_SyncHook", "HandledSync");
            });

            context?.HookExtension?.AfterTest.AddHandler(async (sender, eventArgs) =>
            {
                TestExecutionContext.CurrentContext.CurrentTest.Properties.Add("NotImplementedException_AsyncHook", "HandledAsync"); 
                await Task.Delay(1);
            });
        }
    }

    internal class AfterTestHookExecutesAfterExceptionFromTest
    {
        [TestSetupUnderTest]
        public class TestUnderTest
        {
            [Test, ActivateAfterTestHook]
            public void TestFails_WithException()
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        [NonParallelizable]
        public void AfterTestHookExecutes_HookHandlesExceptionFromTest()
        {
            var testResult = TestsUnderTest.Execute();

            Assert.That(testResult.TestRunResult.TestCases.Count, Is.GreaterThan(0));
            Assert.That(testResult.TestRunResult.Failed, Is.EqualTo(1));

            foreach (var testCase in testResult.TestRunResult.TestCases)
            {
                Assert.That(testCase.Properties["NotImplementedException_SyncHook"].First(), Is.EqualTo("HandledSync"));
                Assert.That(testCase.Properties["NotImplementedException_AsyncHook"].First(), Is.EqualTo("HandledAsync"));
            }
        }
    }

}
