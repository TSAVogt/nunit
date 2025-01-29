// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension.ExceptionHandlingTests
{
    internal class ActivateHookThrowingException : NUnitAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context?.HookExtension?.AfterTest.AddHandler((sender, eventArgs) =>
            {
                TestExecutionContext.CurrentContext
                                    .CurrentTest.Properties
                                    .Add("AfterTestHookThrowingException", "Executed");

                throw new Exception("BeforeTestHook crashed!!");
            });

            context?.HookExtension?.AfterTest.AddHandler((sender, eventArgs) =>
            {
                TestExecutionContext.CurrentContext
                                    .CurrentTest.Properties
                                    .Add("AfterTestHook2", "Executed");
            });

            context?.HookExtension?.AfterTest.AddHandler((sender, eventArgs) =>
            {
                TestExecutionContext.CurrentContext
                                    .CurrentTest.Properties
                                    .Add("AfterTestHook3", "Executed");
            });
        }
    }

    internal class WithMultipleHooksExceptionFromOneHookDoesNotImpactOtherHooks
    {
        [TestSetupUnderTest]
        public class TestUnderTest
        {
            [Test, ActivateHookThrowingException]
            public void TestPasses_WithSimpleAssert()
            {
                Assert.That(1, Is.EqualTo(1));
            }
        }

        [Test]
        [NonParallelizable]
        public void ExceptionFromOneHook_DoesNotImpactExecutionOfOtherHooks()
        {
            var testResult = TestsUnderTest.Execute();

            // no test passes as there is exception in after test hook
            Assert.That(testResult.TestRunResult.Passed, Is.EqualTo(0));
            
            foreach (var testCase in testResult.TestRunResult.TestCases)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(testCase.Properties["AfterTestHookThrowingException"].First(),
                        Is.EqualTo("Executed"));
                    Assert.That(testCase.Properties["AfterTestHook2"].First(), 
                        Is.EqualTo("Executed"));
                    Assert.That(testCase.Properties["AfterTestHook2"].First(), 
                        Is.EqualTo("Executed"));
                });
            }
        }
    }
}
