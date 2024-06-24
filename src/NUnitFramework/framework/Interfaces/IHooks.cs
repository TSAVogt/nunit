namespace NUnit.Framework.Interfaces;

/// <summary>
/// H-TODO
/// </summary>
public interface IHooks 
{
    void BeforeOneTimeSetUp(string methodName);
    void AfterOneTimeSetUp(string methodName);
    void BeforeSetUp(string methodName);
    void AfterSetUp(string methodName);
    void BeforeTest(string methodName);
    void AfterTest(string methodName);
    void BeforeTearDown(string methodName);
    void AfterTearDown(string methodName);
    void BeforeOneTimeTearDown(string methodName);
    void AfterOneTimeTearDown(string methodName);
}
