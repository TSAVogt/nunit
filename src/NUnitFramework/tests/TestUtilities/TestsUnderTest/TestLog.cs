// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

public static class TestLog
{
    private static readonly AsyncLocal<List<string>> _logs = new AsyncLocal<List<string>>();

    // Each aync context gets its own instance of TestLog
    public static List<string> Logs
    {
        get
        {
            if (_logs.Value == null)
            {
                _logs.Value = new List<string>();
            }
            return _logs.Value;
        }
    }

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
