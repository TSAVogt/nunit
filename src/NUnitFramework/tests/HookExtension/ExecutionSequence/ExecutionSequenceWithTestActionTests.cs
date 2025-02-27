// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension.ExecutionSequence
{
    internal class ActivateMultipleTestHooks : NUnitAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context?.HookExtension?.BeforeTest.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod("BeforeTest_Hook");
            });

            context?.HookExtension?.BeforeTest.AddHandler(async (sender, eventArgs) =>
            {
                await Task.Delay(100);
                TestLog.LogCurrentMethod("BeforeTest_Hook");
            });

            context?.HookExtension?.AfterTest.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod("AfterTest_Hook");
            });

            context?.HookExtension?.AfterTest.AddHandler(async (sender, eventArgs) =>
            {
                await Task.Delay(100);
                TestLog.LogCurrentMethod("AfterTest_Hook");
            });
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class SomeTestActionAttribute : Attribute, ITestAction
    {
        public void BeforeTest(ITest test)
        {
            TestLog.LogCurrentMethod("BeforeTest_Action");
        }

        public void AfterTest(ITest test)
        {
            TestLog.LogCurrentMethod("AfterTest_Action");
        }

        public ActionTargets Targets { get; }
    }

    internal class ExecutionSequenceWithTestActionTests
    {
        [TestSetupUnderTest]
        [SomeTestAction]
        public class TestUnderTest
        {
            [Test, ActivateMultipleTestHooks]
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
                "BeforeTest_Action",
                
                "BeforeTest_Hook",
                "BeforeTest_Hook",
                
                "TestPasses",

                "AfterTest_Hook",
                "AfterTest_Hook",

                "TearDown",
                "AfterTest_Action",
                "OneTimeTearDown"
            ]));
        }
    }
}
