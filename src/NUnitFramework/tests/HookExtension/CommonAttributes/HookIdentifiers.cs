// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Tests.HookExtension.CommonAttributes
{
    internal static class HookIdentifiers
    {
        internal static readonly string Hook = "_Hook";

        internal static readonly string AfterTestHook = $"AfterTest{Hook}";
        internal static readonly string BeforeAnySetUpsHook = $"BeforeAnySetUps{Hook}";
        internal static readonly string AfterAnySetUpsHook = $"AfterAnySetUps{Hook}";
        internal static readonly string BeforeTestHook = $"BeforeTest{Hook}";
        internal static readonly string BeforeAnyTearDownsHook = $"BeforeAnyTearDowns{Hook}";
        internal static readonly string AfterAnyTearDownsHook = $"AfterAnyTearDowns{Hook}";
    }
}
