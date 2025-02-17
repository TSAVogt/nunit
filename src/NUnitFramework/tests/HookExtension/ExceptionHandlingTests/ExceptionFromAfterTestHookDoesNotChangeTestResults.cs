// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension.ExceptionHandlingTests
{
    internal class ActivateAfterTestHookThrowingException : NUnitAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context?.HookExtension?.AfterTest.AddHandler((sender, eventArgs) =>
            {
                TestExecutionContext.CurrentContext
                                    .CurrentTest.Properties
                                    .Add("AfterTestHookThrowingException", "Executed");

                throw new Exception("AfterTestHook crashed!!");
            });
        }
    }

    internal class ExceptionFromAfterTestHookDoesNotChangeTestResults
    {
        [TestSetupUnderTest]
        public class TestUnderTest
        {
            [Test, ActivateAfterTestHookThrowingException]
            public void TestPasses() { }
        }

        [Test]
        [NonParallelizable]
        [Explicit("Complete the requirement")]
        // H-ToDo: Complete the requirement
        public void ExceptionFromAfterTestHook_TestResultIsSetToFailed()
        {
            var testResult = TestsUnderTest.Execute();

            // no test passes as there is exception in after test hook
            Assert.That(testResult.TestRunResult.Passed, Is.EqualTo(1));

            foreach (var testCase in testResult.TestRunResult.TestCases)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(testCase.Properties["AfterTestHookThrowingException"].First(), Is.EqualTo("Executed"));
                    Assert.That(testCase.Result, Is.EqualTo("Passed"));
                });
            }
        }

        [Test]
        [NonParallelizable]
        [Explicit("Complete the requirement")]
        // H-ToDo: Complete the requirement
        public void ExceptionFromAfterTestHook_ExceptionIsAvailableInTestContext()
        {

        }
    }
}
