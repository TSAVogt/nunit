// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.HookExtensions;

/// <summary>
/// Hook Extension interface to run custom code synchronously before or after any test activity.
/// </summary>
public class HookExtension
{
    /// <summary>
    /// Default ctor of <see cref="HookExtension"/> class.
    /// </summary>
    public HookExtension()
    {
        BeforeAnySetUps = new AsyncEvent<TestHookIMethodEventArgs>(out _invokeBeforeAnySetUps);
        AfterAnySetUps = new AsyncEvent<TestHookIMethodEventArgs>(out _invokeAfterAnySetUps);
        BeforeTest = new AsyncEvent<TestHookTestMethodEventArgs>(out _invokeBeforeTest);
        AfterTest = new AsyncEvent<TestHookTestMethodEventArgs>(out _invokeAfterTest);
        BeforeAnyTearDowns = new AsyncEvent<TestHookIMethodEventArgs>(out _invokeBeforeAnyTearDowns);
        AfterAnyTearDowns = new AsyncEvent<TestHookIMethodEventArgs>(out _invokeAfterAnyTearDowns);
    }

    private AsyncEventHandler<TestHookIMethodEventArgs> _invokeBeforeAnySetUps;
    private AsyncEventHandler<TestHookIMethodEventArgs> _invokeAfterAnySetUps;
    private AsyncEventHandler<TestHookTestMethodEventArgs> _invokeBeforeTest;
    private AsyncEventHandler<TestHookTestMethodEventArgs> _invokeAfterTest;
    private AsyncEventHandler<TestHookIMethodEventArgs> _invokeBeforeAnyTearDowns;
    private AsyncEventHandler<TestHookIMethodEventArgs> _invokeAfterAnyTearDowns;

    /// <summary>
    /// Gets or sets the hook event that is triggered before any setup methods are executed.
    /// </summary>
    public AsyncEvent<TestHookIMethodEventArgs> BeforeAnySetUps { get; set; }
    /// <summary>
    /// Gets or sets the hook event that is triggered after any setup methods are executed.
    /// </summary>
    public AsyncEvent<TestHookIMethodEventArgs> AfterAnySetUps { get; set; }
    /// <summary>
    /// Gets or sets the hook event that is triggered before a test method is executed.
    /// </summary>
    public AsyncEvent<TestHookTestMethodEventArgs> BeforeTest { get; set; }
    /// <summary>
    /// Gets or sets the hook event that is triggered after a test method is executed.
    /// </summary>
    public AsyncEvent<TestHookTestMethodEventArgs> AfterTest { get; set; }
    /// <summary>
    /// Gets or sets the hook event that is triggered before any teardown methods are executed.
    /// </summary>
    public AsyncEvent<TestHookIMethodEventArgs> BeforeAnyTearDowns { get; set; }
    /// <summary>
    /// Gets or sets the hook event that is triggered after any teardown methods are executed.
    /// </summary>
    public AsyncEvent<TestHookIMethodEventArgs> AfterAnyTearDowns { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HookExtension"/> class by copying hooks from another instance.
    /// </summary>
    /// <param name="other">The instance of <see cref="HookExtension"/> to copy hooks from.</param>
    public HookExtension(HookExtension other) : this()
    {
        other._invokeBeforeAnySetUps?.GetInvocationList()?.ToList().ForEach(d => _invokeBeforeAnySetUps += d as AsyncEventHandler<TestHookIMethodEventArgs>);
        other._invokeAfterAnySetUps?.GetInvocationList()?.ToList().ForEach(d => _invokeAfterAnySetUps += d as AsyncEventHandler<TestHookIMethodEventArgs>);
        other._invokeBeforeTest?.GetInvocationList()?.ToList().ForEach(d => _invokeBeforeTest += d as AsyncEventHandler<TestHookTestMethodEventArgs>);
        other._invokeAfterTest?.GetInvocationList()?.ToList().ForEach(d => _invokeAfterTest += d as AsyncEventHandler<TestHookTestMethodEventArgs>);
        other._invokeBeforeAnyTearDowns?.GetInvocationList()?.ToList().ForEach(d => _invokeBeforeAnyTearDowns += d as AsyncEventHandler<TestHookIMethodEventArgs>);
        other._invokeAfterAnyTearDowns?.GetInvocationList()?.ToList().ForEach(d => _invokeAfterAnyTearDowns += d as AsyncEventHandler<TestHookIMethodEventArgs>);
    }

    internal async Task OnBeforeAnySetUps(TestExecutionContext context, IMethodInfo method)
    {
        try
        {
            await _invokeBeforeAnySetUps(this, new TestHookIMethodEventArgs(context, method));
        }
        catch
        {
            // hook extension must not throw exceptions so they are caught and ignored!
        }
    }

    internal async Task OnAfterAnySetUps(TestExecutionContext context, IMethodInfo method)
    {
        try
        {
            await _invokeAfterAnySetUps(this, new TestHookIMethodEventArgs(context, method));
        }
        catch
        {
            // hook extension must not throw exceptions so they are caught and ignored!
        }
    }

    internal async Task OnBeforeTest(TestExecutionContext context)
    {
        try
        {
            await _invokeBeforeTest(this, new TestHookTestMethodEventArgs(context));
        }
        catch
        {
            // hook extension must not throw exceptions so they are caught and ignored!
        }
    }

    internal async Task OnAfterTest(TestExecutionContext context)
    {
        try
        {
            await _invokeAfterTest(this, new TestHookTestMethodEventArgs(context));
        }
        catch
        {
            // hook extension must not throw exceptions so they are caught and ignored!
        }
    }

    internal async Task OnBeforeAnyTearDowns(TestExecutionContext context, IMethodInfo method)
    {
        try
        {
            await _invokeBeforeAnyTearDowns(this, new TestHookIMethodEventArgs(context, method));
        }
        catch
        {
            // hook extension must not throw exceptions so they are caught and ignored!
        }
    }

    internal async Task OnAfterAnyTearDowns(TestExecutionContext context, IMethodInfo method)
    {
        try
        {
            await _invokeAfterAnyTearDowns(this, new TestHookIMethodEventArgs(context, method));
        }
        catch
        {
            // hook extension must not throw exceptions so they are caught and ignored!
        }
    }
}

/// <summary/>
public delegate Task AsyncEventHandler<TEventArgs>(object? sender, TEventArgs e);
