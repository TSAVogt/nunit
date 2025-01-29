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
            context?.HookExtension?.AfterTest.AddHandler((sender, eventArgs) => 
                throw new Exception("Synchronous BeforeTestHook crashed!!"));
        }
    }

    internal class ActivateAsyncAfterTestHookThrowingException : NUnitAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context?.HookExtension?.AfterTest.AddHandler(async (sender, eventArgs) =>
            {
                await Task.Delay(1);
                throw new Exception("Asynchronous BeforeTestHook crashed!!");
            });
        }
    }

    internal class ExceptionFromAfterTestHookMarksTestResultStateToError
    {
        [TestSetupUnderTest]
        public class TestUnderTest
        {
            [Test, ActivateAfterTestHookThrowingException]
            public void ExceptionFromBeforeTestHook_TestResultIsSetToFailure()
            {
                Assert.That(1, Is.EqualTo(1));
            }

            [Test, ActivateAsyncAfterTestHookThrowingException]
            public void ExceptionFromAsyncBeforeTestHook_TestResultIsSetToFailure()
            {
                Assert.That(1, Is.EqualTo(1));
            }
        }

        [Test]
        public void ExceptionFromAfterTestHook_MarksTestResultState_ToError()
        {
            var testResult = TestsUnderTest.Execute();

            Assert.Multiple(() =>
            {
                // overall test result
                Assert.That(testResult.TestRunResult.Failed, Is.EqualTo(2));

                foreach (var testCase in testResult.TestRunResult.TestCases)
                {
                    Assert.That(testCase.Result, Is.EqualTo("Failed"));
                }
            });
        }
    }
}
