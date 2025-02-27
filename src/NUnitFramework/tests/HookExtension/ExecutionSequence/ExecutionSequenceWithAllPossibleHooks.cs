// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension.ExecutionSequence
{
    internal class ActivateAllPossibleTestHooks : NUnitAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context?.HookExtension?.BeforeAnySetUps.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod("BeforeAnySetUps_TestHook");
            });
            
            context?.HookExtension?.AfterAnySetUps.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod("AfterAnySetUps_TestHook");
            });
            
            context?.HookExtension?.BeforeTest.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod("BeforeTest_TestHook");
            });
            
            context?.HookExtension?.AfterTest.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod("AfterTest_TestHook");
            });
            
            context?.HookExtension?.BeforeAnyTearDowns.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod("BeforeAnyTearDowns_TestHook");
            });
            
            context?.HookExtension?.AfterAnyTearDowns.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod("AfterAnyTearDowns_TestHook");
            });
        }
    }

    internal class ExecutionSequenceWithAllPossibleHooks
    {
        [TestSetupUnderTest]
        public class TestUnderTest
        {
            [OneTimeSetUp]
            public void OneTimeSetUp()
            {
                TestLog.LogCurrentMethod();
            }

            [SetUp]
            public void Setup()
            {
                TestLog.LogCurrentMethod();
            }

            [Test, ActivateAllPossibleTestHooks]
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
                "OneTimeSetUp",

                "BeforeAnySetUps_TestHook", 
                "Setup", 
                "AfterAnySetUps_TestHook", 
                
                "BeforeTest_TestHook", 
                "TestPasses", 
                "AfterTest_TestHook", 
                
                "BeforeAnyTearDowns_TestHook", 
                "TearDown", 
                "AfterAnyTearDowns_TestHook",

                "OneTimeTearDown"
            ]));
        }
    }
}
