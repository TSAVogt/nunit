// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
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
                    eventArgs.Context.CurrentTest.Name.Contains("4Failed") => OutcomeMatched,
                ResultState { Status: TestStatus.Passed } when
                    eventArgs.Context.CurrentTest.Name.Contains("4Passed") => OutcomeMatched,
                ResultState { Status: TestStatus.Skipped } when
                    eventArgs.Context.CurrentTest.Name.Contains("4Ignored") => OutcomeMatched,
                ResultState { Status: TestStatus.Warning } when
                    eventArgs.Context.CurrentTest.Name.Contains("4Warning") => OutcomeMatched,
                _ => OutcomeMismatch
            };

            TestLog.Log(
                $"{outcomeMatchStatement}: {eventArgs.Context.CurrentTest.Name} -> {eventArgs.Context.CurrentResult.ResultState}");
        });
    }
}

public enum FailingReason
{
    Assertion4Failed,
    Exception4Failed,
    IgnoreAssertion4Ignored,
    IgnoreException4Ignored,
    Warn4Warning,
    None4Passed
}

public class AfterSetUpHooksEvaluateTestOutcomeTests
{
    [TestSetupUnderTest]
    [NonParallelizable]
    [AfterSetUpOutcomeLogger]
    [TestFixtureSource(nameof(GetFailingReasons))]
    public class TestsUnderTestsWithDifferentSetUpOutcome
    {
        private readonly FailingReason _failingReason;
        public static IEnumerable<FailingReason> GetFailingReasons()
        {
            return Enum.GetValues(typeof(FailingReason)).Cast<FailingReason>();
        }

        public TestsUnderTestsWithDifferentSetUpOutcome(FailingReason failingReason) => _failingReason = failingReason;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            switch (_failingReason)
            {
                case FailingReason.Assertion4Failed:
                    Assert.Fail("OneTimeSetUp fails by Assertion_Failed.");
                    break;
                case FailingReason.Exception4Failed:
                    throw new Exception("OneTimeSetUp throwing an exception.");
                case FailingReason.None4Passed:
                    break;
                case FailingReason.IgnoreAssertion4Ignored:
                    Assert.Ignore("OneTimeSetUp ignored by Assert.Ignore.");
                    break;
                case FailingReason.IgnoreException4Ignored:
                    throw new IgnoreException("OneTimeSetUp ignored by IgnoreException.");
                case FailingReason.Warn4Warning:
                    Assert.Warn("OneTimeSetUp with warning.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [Test]
        public void SomeTest()
        {
            var fixtureName = TestContext.CurrentContext.Test.Parent.Name;
            if(!(fixtureName.Contains("4Passed") || fixtureName.Contains("4Warning")))
            {
                TestLog.Log(AfterSetUpOutcomeLogger.OutcomeMismatch + $" -> Test Method of '{fixtureName}' executed unexpected!");
            }
        }
    }


    [Test]
    [NonParallelizable]
    public void CheckSetUpOutcomes()
    {
        var testResult = TestsUnderTest.Execute();

        Assert.Multiple(() =>
        {
            foreach (string log in testResult.Logs)
            {
                Assert.That(log, Does.Not.Contain(AfterSetUpOutcomeLogger.OutcomeMismatch));
            }
        });
    }
}
