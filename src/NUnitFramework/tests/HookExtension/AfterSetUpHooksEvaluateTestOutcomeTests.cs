// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension;

public class AfterSetUpOutcomeLogger : NUnitAttribute, IApplyToContext
{
    internal static readonly string OutcomeMatched = "Outcome Matched";
    internal static readonly string OutcomeMismatch = "Outcome Mismatch!!!";

    public void ApplyToContext(TestExecutionContext context)
    {
        context.HookExtension?.AfterAnySetUps.AddHandler((sender, eventArgs) =>
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

public class FailingReason : IEnumerable

{
    public static IEnumerable<Reason> Reasons
    {
        get
        {
            yield return Reason.Assertion;
            yield return Reason.Exception;
        }
    }

    public IEnumerator GetEnumerator()
    {
        throw new System.NotImplementedException();
    }
}

public enum Reason
{
    Assertion,
    Exception
}

public class AfterSetUpHooksEvaluateTestOutcomeTests
{
    //[TestSetupUnderTest]
    //[AfterSetUpOutcomeLogger]
    [TestFixtureSource(typeof(FailingReason))]
    public class TestsUnderTestsWithDifferentSetUpOutcome
    {
        private readonly FailingReason _failingReason;

        public TestsUnderTestsWithDifferentSetUpOutcome(FailingReason failingReason) => this._failingReason = failingReason;

        [OneTimeSetUp]
        public void OneTimeSetUp() => TestLog.LogCurrentMethod();

        [Test]
        public void SomeTest() => TestLog.LogCurrentMethod();
    }

    [Test]
    [NonParallelizable]
    public void CheckSetUpOutcomes()
    {
        var testResult = TestsUnderTest.Execute();

        Assert.That(TestLog.Logs, Is.EqualTo(new[]
        {
            "SomeTest"
        }));
    }
}
