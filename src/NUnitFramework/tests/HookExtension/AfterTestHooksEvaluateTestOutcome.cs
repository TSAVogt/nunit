// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
            string outcomeMatchStatement;
            var currentResultState = eventArgs.Context.CurrentResult.ResultState;
            if ((currentResultState == ResultState.Error || currentResultState == ResultState.Failure) && eventArgs.Context.CurrentTest.MethodName.StartsWith("FailedTest"))
            {
                outcomeMatchStatement = OutcomeMatched;
            } else if (currentResultState == ResultState.Success && eventArgs.Context.CurrentTest.MethodName.StartsWith("PassedTest"))
            {
                outcomeMatchStatement = OutcomeMatched;
            }
            else
            {
                outcomeMatchStatement = OutcomeMismatch;
            }
            TestLog.Log($"{outcomeMatchStatement}: {eventArgs.Context.CurrentTest.MethodName} -> {currentResultState}");
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
        public int FailedTestByWrongExpextedResult() => 2;
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
            });
    }
}
