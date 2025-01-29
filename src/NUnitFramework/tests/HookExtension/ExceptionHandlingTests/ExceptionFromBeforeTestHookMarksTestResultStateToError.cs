// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

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

    internal class ExceptionFromBeforeTestHookMarksTestResultStateToError
    {
        [Test, ActivateBeforeTestHookThrowingException]
        public void ExceptionFromBeforeTestHook_TestResultIsSetToFailure()
        {
            Assert.That(TestExecutionContext.CurrentContext.CurrentResult.ResultState, Is.EqualTo(ResultState.Error));
            Assert.That(TestExecutionContext.CurrentContext.CurrentResult.StackTrace, Is.Not.Null);
        }

        [Test, ActivateAsyncBeforeTestHookThrowingException]
        public void ExceptionFromAsyncBeforeTestHook_TestResultIsSetToFailure()
        {
            Assert.That(TestExecutionContext.CurrentContext.CurrentResult.ResultState, Is.EqualTo(ResultState.Error));
            Assert.That(TestExecutionContext.CurrentContext.CurrentResult.StackTrace, Is.Not.Null);
        }
    }
}
