// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// OneTimeTearDownCommand performs any teardown actions
    /// specified for a suite and calls Dispose on the user
    /// test object, if any.
    /// </summary>
    public class OneTimeTearDownCommand : AfterTestCommand
    {
        /// <summary>
        /// Construct a OneTimeTearDownCommand
        /// </summary>
        /// <param name="innerCommand">The command wrapped by this command</param>
        /// <param name="setUpTearDownItem">A SetUpTearDownList for use by the command</param>
        public OneTimeTearDownCommand(TestCommand innerCommand, SetUpTearDownItem setUpTearDownItem)
            : base(innerCommand)
        {
            Guard.ArgumentValid(innerCommand.Test is TestSuite, "OneTimeTearDownCommand may only apply to a TestSuite", nameof(innerCommand));
            Guard.ArgumentNotNull(setUpTearDownItem, nameof(setUpTearDownItem));

            AfterTest = context =>
            {
                TestResult suiteResult = context.CurrentResult;

                // TearDown is only executed if there was a corresponding setUp on the same level.
                // Also the event should only be fired if there are actual tearDown methods to be executed.
                bool eventShouldBeFired = (setUpTearDownItem.SetUpWasRun && setUpTearDownItem.HasTearDownMethods);

                try
                {
                    if (eventShouldBeFired)
                        context.Listener.OneTimeTearDownStarted(Test);

                    setUpTearDownItem.RunTearDown(context);

                    if (eventShouldBeFired)
                        context.Listener.OneTimeTearDownFinished(Test);
                }
                catch (Exception ex)
                {
                    suiteResult.RecordTearDownException(ex);
                }
            };
        }
    }
}
