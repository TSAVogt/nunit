// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Internal.Commands
{
    public class HookDelegatingTestCommand : DelegatingTestCommand
    {
        public HookDelegatingTestCommand(TestCommand innerCommand) : base(innerCommand) { }

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

            context.HookExtension?.OnAfterTest(context);
            return context.CurrentResult;

        }
    }
}
