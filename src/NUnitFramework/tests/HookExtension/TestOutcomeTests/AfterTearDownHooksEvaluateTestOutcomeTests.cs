// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension.TestOutcomeTests;

public class AfterTearDownHooksEvaluateTestOutcomeTests
{
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
                        eventArgs.Context.CurrentTest.FullName.Contains("4Failed") => OutcomeMatched,
                    ResultState { Status: TestStatus.Passed } when
                        eventArgs.Context.CurrentTest.FullName.Contains("4Passed") => OutcomeMatched,
                    ResultState { Status: TestStatus.Skipped } when
                        eventArgs.Context.CurrentTest.FullName.Contains("4Ignored") => OutcomeMatched,
                    ResultState { Status: TestStatus.Inconclusive } when
                       eventArgs.Context.CurrentTest.FullName.Contains("4Inconclusive") => OutcomeMatched,
                    ResultState { Status: TestStatus.Warning } when
                        eventArgs.Context.CurrentTest.FullName.Contains("4Warning") => OutcomeMatched,
                    _ => OutcomeMismatch
                };

                TestLog.Log($"{outcomeMatchStatement}: {eventArgs.Context.CurrentTest.FullName} -> {eventArgs.Context.CurrentResult.ResultState}");
            });
        }
    }

    public enum FailingReason
    {
        Assertion4Failed,
        MultiAssertion4Failed,
        Exception4Failed,
        IgnoreAssertion4Ignored,
        IgnoreException4Ignored,
        Inconclusive4Inconclusive,
        Warning4Warning, // Warn counts on OneTimeSetUp level as passed and on SetUp level as warning!
        None4Passed
    }

    private static IEnumerable<FailingReason> GetRelevantFailingReasons()
    {
        var failingReasons = Enum.GetValues(typeof(FailingReason)).Cast<FailingReason>();

        // H-ToDo: remove before final checkin
        // Apply filtering
        //failingReasons = failingReasons.Where(reason => reason.ToString().EndsWith("4Passed"));
        return failingReasons;
    }

    // H-Todo: enrich by failing setups, tests, ...
    [TestSetupUnderTest]
    [NonParallelizable]
    [AfterSetUpOutcomeLogger]
    [TestFixtureSource(nameof(GetFixtureConfig))]
    public class TestsUnderTestsWithDifferentSetUpOutcome
    {
        private readonly FailingReason _failingReason;

        private static IEnumerable<TestFixtureData> GetFixtureConfig()
        {
            foreach (var failingReason in GetRelevantFailingReasons())
            {
                yield return new TestFixtureData(failingReason);
            }
        }

        public TestsUnderTestsWithDifferentSetUpOutcome(FailingReason failingReason)
        {
            _failingReason = failingReason;
        }


        [SetUp]
        public void SetUp()
        {
            ExecuteFailingReason();
        }

        private void ExecuteFailingReason()
        {
            switch (_failingReason)
            {
                case FailingReason.Assertion4Failed:
                    Assert.Fail("SetUp fails by Assertion_Failed.");
                    break;
                case FailingReason.MultiAssertion4Failed:
                    Assert.Multiple(() =>
                    {
                        Assert.Fail("1st failure");
                        Assert.Fail("2nd failure");
                    });
                    break;
                case FailingReason.Exception4Failed:
                    throw new Exception("SetUp throwing an exception.");
                case FailingReason.None4Passed:
                    break;
                case FailingReason.IgnoreAssertion4Ignored:
                    Assert.Ignore("SetUp ignored by Assert.Ignore.");
                    break;
                case FailingReason.IgnoreException4Ignored:
                    throw new IgnoreException("SetUp ignored by IgnoreException.");
                case FailingReason.Inconclusive4Inconclusive:
                    Assert.Inconclusive("SetUp ignored by Assert.Inconclusive.");
                    break;
                case FailingReason.Warning4Warning:
                    Assert.Warn("SetUp with warning.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [Test]
        public void SomeTest()
        {
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

            foreach (TestCase testCase in testResult.TestRunResult.TestCases)
            {
                Assert.That(testCase.FullName,
                    Does.Contain(testCase.Result == "Skipped" ? "Ignored" : testCase.Result));
            }
            // H-TODO: This asserts checks the assumption that an Assert.Warn will have a passed outcome.
            Assert.That(testResult.TestRunResult.Passed, Is.EqualTo(GetRelevantFailingReasons().Count(reason => reason.ToString().EndsWith("4Passed"))));
            Assert.That(testResult.TestRunResult.Failed, Is.EqualTo(GetRelevantFailingReasons().Count(reason => reason.ToString().EndsWith("4Failed"))));
            Assert.That(testResult.TestRunResult.Skipped, Is.EqualTo(GetRelevantFailingReasons().Count(reason => reason.ToString().EndsWith("4Ignored"))));
            Assert.That(testResult.TestRunResult.Total, Is.EqualTo(GetRelevantFailingReasons().Count()));
        });
    }
}
