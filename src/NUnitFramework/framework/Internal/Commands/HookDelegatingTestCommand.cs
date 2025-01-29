// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Internal.Commands
{
    public class HookDelegatingTestCommand : DelegatingTestCommand
    {
        public HookDelegatingTestCommand(TestCommand innerCommand) : base(innerCommand) { }

        public override TestResult Execute(TestExecutionContext context)
        {
            // exceptions from Hooks are handled in HookExtension
            context.HookExtension?.OnBeforeTest(context);

            // exceptions from tests are handled here
            try
            {
                innerCommand.Execute(context);
            }
            catch (Exception ex)
            {
                context.CurrentResult.RecordException(ex);
            }

            // exceptions from Hooks are handled in HookExtension
            context.HookExtension?.OnAfterTest(context);
            
            return context.CurrentResult;
        }
    }
}
