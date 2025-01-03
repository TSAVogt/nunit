// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Common;
using NUnitLite;

namespace NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

public class TestsUnderTest
{
    public static TestResult Execute(params string[] additionalParameters)
    {
        var type = new StackFrame(1, false).GetMethod()?.ReflectedType;
        var testUnderTestClass = type?.GetNestedTypes().Single(x => x.GetCustomAttribute<TestSetupUnderTestAttribute>() != null);

        TestLog.Logs.Clear();
        StringWriter consoleOutput = new StringWriter();
        string tempFileName = Path.GetTempFileName() + ".xml";
        var parameters = new List<string>
        {
            "--where", $"class == '{testUnderTestClass}' && cat == {TestSetupUnderTestAttribute.Category}",
            $"--result:{tempFileName}"
        };
        if (additionalParameters is { Length: > 0 })
        {
            parameters.AddRange(additionalParameters);
        }
        int errorCode = new AutoRun(Assembly.GetCallingAssembly()).Execute(parameters.ToArray(),
            new ExtendedTextWrapper(consoleOutput), null);

        TestRunResult testRunResult = NUnitResultParser.Parse(tempFileName);
        File.Delete(tempFileName);

        return new TestResult(errorCode, consoleOutput.ToString(), TestLog.Logs.ToArray(), testRunResult);
    }
}
