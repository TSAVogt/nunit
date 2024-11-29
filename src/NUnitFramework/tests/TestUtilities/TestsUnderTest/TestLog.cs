// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

public static class TestLog
{
    public static List<string> Logs { get; } = new List<string>();

    public static void Log(string infoToLog)
    {
        Logs.Add(infoToLog);
    }

    public static void LogCurrentMethod([CallerMemberName] string callerMethodName = "")
    {
        Log(callerMethodName);
    }

    public static void LogCurrentMethodWithContextInfo(string contextInfo, [CallerMemberName] string callerMethodName = "")
    {
        Log($"{callerMethodName}({contextInfo})");
    }
}
