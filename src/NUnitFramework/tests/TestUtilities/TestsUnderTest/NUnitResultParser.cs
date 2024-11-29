// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using System.Xml.Linq;

namespace NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

public class TestCase
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string FullName { get; set; }
    public string MethodName { get; set; }
    public string ClassName { get; set; }
    public string Result { get; set; }
    public Dictionary<string, List<string>> Properties { get; set; }
}

public class TestRunResult
{
    public int Total { get; set; }
    public int Passed { get; set; }
    public int Failed { get; set; }
    public int Skipped { get; set; }
    public List<TestCase> TestCases { get; set; }
}

public class NUnitResultParser
{
    public static TestRunResult Parse(string xmlFilePath)
    {
        var doc = XDocument.Load(xmlFilePath);
        var testRunElement = doc.Element("test-run");

        var testRunResult = new TestRunResult
        {
            Total = int.Parse(testRunElement.Attribute("total").Value),
            Passed = int.Parse(testRunElement.Attribute("passed").Value),
            Failed = int.Parse(testRunElement.Attribute("failed").Value),
            Skipped = int.Parse(testRunElement.Attribute("skipped").Value),
            TestCases = new List<TestCase>()
        };

        var testCaseElements = testRunElement.Descendants("test-case");
        foreach (var testCaseElement in testCaseElements)
        {
            var properties = new Dictionary<string, List<string>>();
            foreach (var propertyElement in testCaseElement.Descendants("property"))
            {
                var name = propertyElement.Attribute("name").Value;
                var value = propertyElement.Attribute("value").Value;

                if (!properties.ContainsKey(name))
                {
                    properties[name] = new List<string>();
                }
                properties[name].Add(value);
            }

            var testCase = new TestCase
            {
                Id = testCaseElement.Attribute("id").Value,
                Name = testCaseElement.Attribute("name").Value,
                FullName = testCaseElement.Attribute("fullname").Value,
                MethodName = testCaseElement.Attribute("methodname").Value,
                ClassName = testCaseElement.Attribute("classname").Value,
                Result = testCaseElement.Attribute("result").Value,
                Properties = properties
            };

            testRunResult.TestCases.Add(testCase);
        }

        return testRunResult;
    }
}
