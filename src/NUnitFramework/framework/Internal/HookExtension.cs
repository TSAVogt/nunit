// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Hook Extension interface to run custom code synchronously before or after any test activity.
    /// </summary>
    public class HookExtension
    {
        /// <summary/>
        public event SetUpTearDownHookHandler BeforeAnySetUps;
        /// <summary/>
        public event SetUpTearDownHookHandler AfterAnySetUps;


        /// <summary/>
        public event TestHookHandler BeforeTest;
        /// <summary/>
        public event TestHookHandler AfterTest;

        /// <summary/>
        public event SetUpTearDownHookHandler BeforeAnyTearDowns;
        /// <summary/>
        public event SetUpTearDownHookHandler AfterAnyTearDowns;

        /// <summary/>
        public void OnBeforeAnySetUps(TestExecutionContext context, IMethodInfo method)
        {
            BeforeAnySetUps?.Invoke(context, method);
        }

        /// <summary/>
        public void OnAfterAnySetUps(TestExecutionContext context, IMethodInfo method)
        {
            AfterAnySetUps?.Invoke(context, method);
        }

        /// <summary/>
        public void OnBeforeTest(TestExecutionContext context, TestMethod testMethod)
        {
            BeforeTest?.Invoke(context, testMethod);
        }

        /// <summary/>
        public void OnAfterTest(TestExecutionContext context, TestMethod testMethod)
        {
            AfterTest?.Invoke(context, testMethod);
        }

        /// <summary/>
        public void OnBeforeAnyTearDowns(TestExecutionContext context, IMethodInfo method)
        {
            BeforeAnyTearDowns?.Invoke(context, method);
        }

        /// <summary/>
        public void OnAfterAnyTearDowns(TestExecutionContext context, IMethodInfo method)
        {
            AfterAnyTearDowns?.Invoke(context, method);
        }
    }

    /// <summary/>
    public delegate void SetUpTearDownHookHandler(TestExecutionContext context, IMethodInfo method);

    /// <summary/>
    public delegate void TestHookHandler(TestExecutionContext context, TestMethod testMethod);
}
