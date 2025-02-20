// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension.ExecutionSequence
{
    internal class ActivateAfterTestHooks : NUnitAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context?.HookExtension?.AfterTest.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod("AfterTestHook");
            });

            context?.HookExtension?.AfterTest.AddHandler(async (sender, eventArgs) =>
            {
                await Task.Delay(100);
                TestLog.LogCurrentMethod("AfterTestHook");
            });

            context?.HookExtension?.AfterTest.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod("AfterTestHook");
                throw new Exception("After test hook crashed");
            });

            context?.HookExtension?.AfterTest.AddHandler(async (sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod("AfterTestHook");
                await Task.Delay(100);
                throw new Exception("After test hook crashed");
            });
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class SomeTestAttrAttribute : Attribute, ITestAction
    {
        public void BeforeTest(ITest test)
        {
            TestLog.LogCurrentMethod();
        }

        public void AfterTest(ITest test)
        {
            TestLog.LogCurrentMethod();
        }

        public ActionTargets Targets { get; }
    }

    internal class ExecutionProceedsOnlyAfterAllAfterAfterTestHooksExecute
    {
        [TestSetupUnderTest, SomeTestAttr]
        public class TestUnderTest
        {
            [Test, ActivateAfterTestHooks]
            public void TestPasses()
            {
                TestLog.LogCurrentMethod();
            }

            [TearDown]
            public void TearDown()
            {
                TestLog.LogCurrentMethod();
            }

            [OneTimeTearDown]
            public void OneTimeTearDown()
            {
                TestLog.LogCurrentMethod();
            }
        }

        [Test]
        [NonParallelizable]
        public void TestProceedsAfterAllAfterTestHooksExecute()
        {
            var testResult = TestsUnderTest.Execute();

            Assert.That(testResult.Logs, Is.EqualTo([
                "BeforeTest",
                "TestPasses",
                "AfterTestHook",
                "AfterTestHook",
                "AfterTestHook",
                "AfterTestHook",
                "TearDown",
                "AfterTest",
                "OneTimeTearDown"
            ]));
        }
    }
}
