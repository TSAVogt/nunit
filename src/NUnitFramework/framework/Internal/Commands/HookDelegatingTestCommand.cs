// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary/>
    /// H-ToDo: Documentation needed for class
    public class HookDelegatingTestCommand : DelegatingTestCommand
    {
        /// <summary/>
        public HookDelegatingTestCommand(TestCommand innerCommand) : base(innerCommand)
        {
        }

        /// <summary/>
        public override TestResult Execute(TestExecutionContext context)
        {
            try
            {
                context.HookExtension?.OnBeforeTest(context);
                innerCommand.Execute(context);
            }
            catch (Exception ex)
            {
                context.CurrentResult.RecordException(ex);
            }

            // exceptions from after test hooks are handled by framework
            context.HookExtension?.OnAfterTest(context);

            return context.CurrentResult;
        }
    }
}
