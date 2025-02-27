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

    internal class ExecutionProceedsOnlyAfterAllAfterAfterTestHooksExecute
    {
        [TestSetupUnderTest]
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
                "TestPasses",
                "AfterTestHook",
                "AfterTestHook",
                "AfterTestHook",
                "AfterTestHook",
                "TearDown",
                "OneTimeTearDown"
            ]));
        }
    }
}
