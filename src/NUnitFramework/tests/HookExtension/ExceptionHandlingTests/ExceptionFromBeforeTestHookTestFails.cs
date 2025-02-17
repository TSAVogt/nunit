// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension.ExceptionHandlingTests
{
    internal class ActivateBeforeTestHookThrowingException : NUnitAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context?.HookExtension?.BeforeTest.AddHandler((sender, eventArgs) =>
                throw new Exception("Synchronous BeforeTestHook crashed!!"));
        }
    }

    internal class ActivateAsyncBeforeTestHookThrowingException : NUnitAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context?.HookExtension?.BeforeTest.AddHandler(async (sender, eventArgs) =>
            {
                await Task.Delay(1);
                throw new Exception("Asynchronous BeforeTestHook crashed!!");
            });
        }
    }

    internal class ExceptionFromBeforeTestHookTestFails
    {
        [TestSetupUnderTest]
        public class TestUnderTest
        {
            [Test, ActivateBeforeTestHookThrowingException]
            public void ExceptionFromSynchronousBeforeTestHooksTestResultIsSetToFailed() { }

            [Test, ActivateAsyncBeforeTestHookThrowingException]
            public void ExceptionFromAsynchronousBeforeTestHooksTestResultIsSetToFailed() { }

            [Test,
             ActivateBeforeTestHookThrowingException,
             ActivateAsyncBeforeTestHookThrowingException]
            public void ExceptionFromMultipleBeforeTestHooksTestResultIsSetToFailed() { }
        }

        [Test]
        [NonParallelizable]
        public void ExceptionFromBeforeTestHooks_TestResultIsSetToFailed()
        {
            var result = TestsUnderTest.Execute();

            Assert.That(result.TestRunResult.Passed, Is.EqualTo(0));
            Assert.That(result.TestRunResult.Skipped, Is.EqualTo(0));

            // overall test result
            Assert.That(result.TestRunResult.Failed, Is.EqualTo(3));

            foreach (var testCase in result.TestRunResult.TestCases)
            {
                Assert.That(testCase.Result, Is.EqualTo("Failed"));
            }
        }
    }
}
