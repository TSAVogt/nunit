// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;
using TestResult = NUnit.Framework.Internal.TestResult;

namespace NUnit.Framework.Tests.HookExtension.TestOutcomeTests;

public class AfterTearDownHooksEvaluateTestOutcomeTests
{
    public class AfterTearDownOutcomeLogger : NUnitAttribute, IApplyToContext
    {
        internal static readonly string OutcomeMatched = "Outcome Matched";
        internal static readonly string OutcomeMismatch = "Outcome Mismatch!!!";

        public void ApplyToContext(TestExecutionContext context)
        {
            TestResult beforeHookTestResult = null;
            context.HookExtension?.BeforeAnyTearDowns.AddHandler((sender, eventArgs) =>
            {
                beforeHookTestResult = eventArgs.Context.CurrentResult;
            });

            context.HookExtension?.AfterAnyTearDowns.AddHandler((sender, eventArgs) =>
            {
                var tearDownTestResult = eventArgs.Context.CurrentResult;
                if (eventArgs.ExceptionContext != null)
                {
                    tearDownTestResult = tearDownTestResult.Clone();
                    tearDownTestResult.RecordException(eventArgs.ExceptionContext);
                } else if (tearDownTestResult.AssertionResults.Count > 0)
                {
                    tearDownTestResult = tearDownTestResult.Clone();
                    tearDownTestResult.RecordTestCompletion();
                }
                string outcomeMatchStatement = tearDownTestResult.ResultState switch
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
        Warning4Warning, // Warn counts on OneTimeTearDown level as passed and on TearDown level as warning!
        None4Passed
    }

    private static IEnumerable<FailingReason> GetRelevantFailingReasons()
    {
        var failingReasons = Enum.GetValues(typeof(FailingReason)).Cast<FailingReason>();

        // H-ToDo: remove before final checkin
        // Apply filtering
        //failingReasons = failingReasons.Where(reason => reason.ToString().EndsWith("MultiAssertion4Failed"));
        return failingReasons;
    }

    [TestSetupUnderTest]
    [NonParallelizable]
    [AfterTearDownOutcomeLogger]
    [TestFixtureSource(nameof(GetFixtureConfig))]
    public class TestsUnderTestsWithDifferentTearDownOutcome
    {
        private readonly FailingReason _failingReason;

        private static IEnumerable<TestFixtureData> GetFixtureConfig()
        {
            foreach (var failingReason in GetRelevantFailingReasons())
            {
                yield return new TestFixtureData(failingReason);
            }
        }

        public TestsUnderTestsWithDifferentTearDownOutcome(FailingReason failingReason)
        {
            _failingReason = failingReason;
        }

        [TearDown]
        public void TearDown()
        {
            ExecuteFailingReason();
        }

        private void ExecuteFailingReason()
        {
            switch (_failingReason)
            {
                case FailingReason.Assertion4Failed:
                    Assert.Fail("TearDown fails by Assertion_Failed.");
                    break;
                case FailingReason.MultiAssertion4Failed:
                    Assert.Multiple(() =>
                    {
                        Assert.Fail("1st failure");
                        Assert.Fail("2nd failure");
                        Assert.Multiple(() =>
                        {
                            Assert.That(true, Is.True, "Assertion that is not failing.");
                            Assert.Fail("inner failing failure");
                        });
                    });
                    break;
                case FailingReason.Exception4Failed:
                    throw new Exception("TearDown throwing an exception.");
                case FailingReason.None4Passed:
                    break;
                case FailingReason.IgnoreAssertion4Ignored:
                    Assert.Ignore("TearDown ignored by Assert.Ignore.");
                    break;
                case FailingReason.IgnoreException4Ignored:
                    throw new IgnoreException("TearDown ignored by IgnoreException.");
                case FailingReason.Inconclusive4Inconclusive:
                    Assert.Inconclusive("TearDown ignored by Assert.Inconclusive.");
                    break;
                case FailingReason.Warning4Warning:
                    Assert.Warn("TearDown with warning.");
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
    public void CheckTearDownOutcomes()
    {
        var testResult = TestsUnderTest.Execute();

        Assert.Multiple(() =>
        {
            foreach (string log in testResult.Logs)
            {
                Assert.That(log, Does.Not.Contain(AfterTearDownOutcomeLogger.OutcomeMismatch));
            }

            int numberOfIgnoredOrInconclusive = GetRelevantFailingReasons().Count(reason => reason.ToString().EndsWith("4Ignored") || reason.ToString().EndsWith("4Inconclusive"));
            Assert.That(testResult.TestRunResult.Passed, Is.EqualTo(GetRelevantFailingReasons().Count(reason => reason.ToString().EndsWith("4Passed"))));
            Assert.That(testResult.TestRunResult.Failed, Is.EqualTo(GetRelevantFailingReasons().Count(reason => reason.ToString().EndsWith("4Failed")) + numberOfIgnoredOrInconclusive));
            Assert.That(testResult.TestRunResult.Total, Is.EqualTo(GetRelevantFailingReasons().Count()));
        });
    }
}
