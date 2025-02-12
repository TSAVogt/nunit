// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension;

public class AfterTestOutcomeLogger : NUnitAttribute, IApplyToContext
{
    internal static readonly string OutcomeMatched = "Outcome Matched";
    internal static readonly string OutcomeMismatch = "Outcome Mismatch!!!";

    public void ApplyToContext(TestExecutionContext context)
    {
        context.HookExtension?.AfterTest.AddHandler((sender, eventArgs) =>
        {
            string outcomeMatchStatement = eventArgs.Context.CurrentResult.ResultState switch
            {
                ResultState { Status: TestStatus.Failed } when
                    eventArgs.Context.CurrentTest.MethodName.StartsWith("FailedTest") => OutcomeMatched,
                ResultState { Status: TestStatus.Passed } when
                    eventArgs.Context.CurrentTest.MethodName.StartsWith("PassedTest") => OutcomeMatched,
                ResultState { Status: TestStatus.Skipped } when
                    eventArgs.Context.CurrentTest.MethodName.StartsWith("TestIgnored") => OutcomeMatched,
                ResultState { Status: TestStatus.Warning } when
                    eventArgs.Context.CurrentTest.MethodName.StartsWith("WarningTest") => OutcomeMatched,
                _ => OutcomeMismatch
            };

            TestLog.Log(
                $"{outcomeMatchStatement}: {eventArgs.Context.CurrentTest.MethodName} -> {eventArgs.Context.CurrentResult.ResultState}");
        });
    }
}

public class AfterTestHooksEvaluateTestOutcomeTests
{
    [TestSetupUnderTest]
    [AfterTestOutcomeLogger]
    public class TestsUnderTestsWithMixedOutcome
    {
        [Test]
        public void PassedTest() { }

        [Test]
        public void FailedTestByAssertion()
        {
            Assert.Fail();
        }

        [Test]
        public void FailedTestByException()
        {
            throw new System.Exception();
        }

        [TestCase(ExpectedResult = 1)]
        public int FailedTestByWrongExpectedResult() => 2;

        [Test]
        public void TestIgnoredByAssertIgnore()
        {
            Assert.Ignore();
        }

        [Test]
        public void TestIgnoredByException()
        {
            throw new IgnoreException("Ignore this test");
        }

        [Test]
        public void WarningTestWithWarnings()
        {
            Assert.Warn("Some warning.");
        }
    }

    [Test]
    [NonParallelizable]
    public void CheckThatAfterTestHooksEvaluateTestOutcome()
    {
        var testResult = TestsUnderTest.Execute();

        Assert.That(testResult.Logs.Length, Is.Not.EqualTo(0));
        Assert.Multiple(() =>
            {
                foreach (string logLine in testResult.Logs)
                {
                    Assert.That(logLine, Does.StartWith(AfterTestOutcomeLogger.OutcomeMatched));
                }

                // H-TODO: clean up warning outcome. Just added for understanding how Assert.Warn is handled. See also AfterSetUpHooksEvaluateTestOutcomeTests.cs
                Assert.That(testResult.TestRunResult.TestCases.Where(t=>t.Name.StartsWith("Warning")).Single().Result, Is.EqualTo("Warning"));
            });
    }
}
