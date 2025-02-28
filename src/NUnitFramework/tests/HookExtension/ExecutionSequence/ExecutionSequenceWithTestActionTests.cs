// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Tests.HookExtension.CommonAttributes;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension.ExecutionSequence
{
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
            [Test]
            [ActivateBeforeTestHooks]
            [ActivateAfterTestHooks]
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
                
                HookIdentifiers.BeforeTestHook,
                HookIdentifiers.BeforeTestHook,
                
                nameof(TestUnderTest.TestPasses),

                HookIdentifiers.AfterTestHook,
                HookIdentifiers.AfterTestHook,

                nameof(TestUnderTest.TearDown),
                "AfterTest_Action",
                nameof(TestUnderTest.OneTimeTearDown)
            ]));
        }
    }
}
