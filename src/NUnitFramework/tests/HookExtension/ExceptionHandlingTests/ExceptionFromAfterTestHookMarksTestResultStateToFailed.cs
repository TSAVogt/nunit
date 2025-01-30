// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
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
                throw new Exception("Synchronous AfterTestHook crashed!!"));
        }
    }

    internal class ActivateAsyncAfterTestHookThrowingException : NUnitAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context?.HookExtension?.AfterTest.AddAsyncHandler(async (sender, eventArgs) => 
                throw new Exception("Asynchronous AfterTestHook crashed!!"));
        }
    }

    internal class ExceptionFromAfterTestHookMarksTestResultStateToFailed
    {
        [TestSetupUnderTest]
        public class TestUnderTest
        {
            [Test, ActivateAfterTestHookThrowingException]
            public void ExceptionFromAfterTestHook_TestResultIsSetToFailed() { }

            [Test, ActivateAsyncAfterTestHookThrowingException]
            public void ExceptionFromAsyncAfterTestHook_TestResultIsSetToFailed() { }
        }

        [Test]
        public void ExceptionFromAfterTestHook_MarksTestResultState_ToFailed()
        {
            var testResult = TestsUnderTest.Execute();

            Assert.Multiple(() =>
            {
                // overall test result is failed
                Assert.That(testResult.TestRunResult.Failed, Is.EqualTo(1));

                // all tests are also failed 
                foreach (var testCase in testResult.TestRunResult.TestCases)
                {
                    Assert.That(testCase.Result, Is.EqualTo("Failed"));
                }
            });
        }
    }
}
