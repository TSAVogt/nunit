// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData;

namespace NUnit.Framework.Tests.Assertions
{
    [TestFixture]
    public class AssertThatTests
    {
#pragma warning disable NUnit2010 // Use EqualConstraint for better assertion messages in case of failure
        [Test]
        public void AssertionPasses_Boolean()
        {
            Assert.That(2 + 2 == 4);
        }

        [Test]
        public void AssertionPasses_BooleanWithMessage()
        {
            Assert.That(2 + 2 == 4, "Not Equal");
        }

        [Test]
        public void AssertionPasses_BooleanWithNullMessage()
        {
            Assert.That(2 + 2 == 4, default(string));
        }

        [Test]
        public void AssertionPasses_BooleanWithMessageStringFunc()
        {
            string GetExceptionMessage() => $"Not Equal to {4}";
            Assert.That(2 + 2 == 4, GetExceptionMessage);
        }
#pragma warning restore NUnit2010 // Use EqualConstraint for better assertion messages in case of failure

        [Test]
        public void AssertionPasses_ActualAndConstraint()
        {
            Assert.That(2 + 2, Is.EqualTo(4));
        }

        [Test]
        public void AssertionPasses_ActualAndConstraintWithMessage()
        {
            Assert.That(2 + 2, Is.EqualTo(4), "Should be 4");
        }

        [Test]
        public void AssertionPasses_ActualAndConstraintWithNullMessage()
        {
            Assert.That(2 + 2, Is.EqualTo(4), default(string));
        }

        [Test]
        public void AssertionPasses_ActualAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => "Not Equal to 4";
            Assert.That(2 + 2, Is.EqualTo(4), GetExceptionMessage);
        }

        [Test]
        public void AssertionPasses_ActualLambdaAndConstraint()
        {
            Assert.That(() => 2 + 2, Is.EqualTo(4));
        }

        [Test]
        public void AssertionPasses_ActualLambdaAndConstraintWithMessage()
        {
            Assert.That(() => 2 + 2, Is.EqualTo(4), "Should be 4");
        }

        [Test]
        public void AssertionPasses_ActualLambdaAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => $"Not Equal to {4}";
            Assert.That(() => 2 + 2, Is.EqualTo(4), GetExceptionMessage);
        }

        [Test]
        public void AssertionPasses_DelegateAndConstraint()
        {
            Assert.That(ReturnsFour, Is.EqualTo(4));
        }

        [Test]
        public void AssertionPasses_DelegateAndConstraintWithMessage()
        {
            Assert.That(ReturnsFour, Is.EqualTo(4), "Message");
        }

        [Test]
        public void AssertionPasses_DelegateAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => "Not Equal to 4";
            Assert.That(ReturnsFour, Is.EqualTo(4), GetExceptionMessage);
        }

        private int ReturnsFour() => 4;

        [Test]
        public void TestEquatableWithConvertible()
        {
            var actual = new Number(42);
            var expected = new Number(42.0);

            Assert.That(actual, Is.EqualTo(expected));
        }

        private readonly struct Number : IEquatable<Number>, IConvertible
        {
            private readonly double _value;

            public Number(int value) => _value = value;
            public Number(double value) => _value = value;

            public bool Equals(Number other)
            {
                return _value == other._value;
            }

            TypeCode IConvertible.GetTypeCode() => TypeCode.Object;
            byte IConvertible.ToByte(IFormatProvider? provider) => throw new NotImplementedException();
            sbyte IConvertible.ToSByte(IFormatProvider? provider) => throw new NotImplementedException();
            ushort IConvertible.ToUInt16(IFormatProvider? provider) => throw new NotImplementedException();
            short IConvertible.ToInt16(IFormatProvider? provider) => throw new NotImplementedException();
            uint IConvertible.ToUInt32(IFormatProvider? provider) => throw new NotImplementedException();
            int IConvertible.ToInt32(IFormatProvider? provider) => throw new NotImplementedException();
            ulong IConvertible.ToUInt64(IFormatProvider? provider) => throw new NotImplementedException();
            long IConvertible.ToInt64(IFormatProvider? provider) => throw new NotImplementedException();
            string IConvertible.ToString(IFormatProvider? provider) => throw new NotImplementedException();
            bool IConvertible.ToBoolean(IFormatProvider? provider) => throw new NotImplementedException();
            char IConvertible.ToChar(IFormatProvider? provider) => throw new NotImplementedException();
            DateTime IConvertible.ToDateTime(IFormatProvider? provider) => throw new NotImplementedException();
            decimal IConvertible.ToDecimal(IFormatProvider? provider) => throw new NotImplementedException();
            double IConvertible.ToDouble(IFormatProvider? provider) => throw new NotImplementedException();
            float IConvertible.ToSingle(IFormatProvider? provider) => throw new NotImplementedException();
            object IConvertible.ToType(Type conversionType, IFormatProvider? provider) => throw new NotImplementedException();
        }

#pragma warning disable NUnit2010 // Use EqualConstraint for better assertion messages in case of failure

        [Test]
        public void FailureThrowsAssertionException_Boolean()
        {
            Assert.Throws<AssertionException>(() => Assert.That(2 + 2 == 5));
        }

        [Test]
        public void FailureThrowsAssertionException_BooleanWithMessage()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(2 + 2 == 5, "message"));
            Assert.That(ex?.Message, Does.Contain("message"));
            Assert.That(ex?.Message, Does.Contain("Assert.That(2 + 2 == 5, Is.True)"));
        }

        [Test]
        public void FailureThrowsAssertionException_BooleanWithMessageStringFunc()
        {
            string GetExceptionMessage() => "Not Equal to 4";
            var ex = Assert.Throws<AssertionException>(() => Assert.That(2 + 2 == 5, GetExceptionMessage));
            Assert.That(ex?.Message, Does.Contain("Not Equal to 4"));
            Assert.That(ex?.Message, Does.Contain("Assert.That(2 + 2 == 5, Is.True)"));
        }
#pragma warning restore NUnit2010 // Use EqualConstraint for better assertion messages in case of failure

        [Test]
        public void FailureThrowsAssertionException_ActualAndConstraint()
        {
            Assert.Throws<AssertionException>(() => Assert.That(2 + 2, Is.EqualTo(5)));
        }

        [Test]
        public void FailureThrowsAssertionException_ActualAndConstraintWithMessage()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(2 + 2, Is.EqualTo(5), "Error"));
            Assert.That(ex?.Message, Does.Contain("Error"));
            Assert.That(ex?.Message, Does.Contain("Assert.That(2 + 2, Is.EqualTo(5))"));
        }

        [Test]
        public void FailureThrowsAssertionException_ActualAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => "error";
            var ex = Assert.Throws<AssertionException>(() => Assert.That(2 + 2, Is.EqualTo(5), GetExceptionMessage));
            Assert.That(ex?.Message, Does.Contain("error"));
            Assert.That(ex?.Message, Does.Contain("Assert.That(2 + 2, Is.EqualTo(5))"));
        }

        [Test]
        public void FailureThrowsAssertionException_ActualLambdaAndConstraint()
        {
            Assert.Throws<AssertionException>(() => Assert.That(() => 2 + 2, Is.EqualTo(5)));
        }

        [Test]
        public void FailureThrowsAssertionException_ActualLambdaAndConstraintWithMessage()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(() => 2 + 2, Is.EqualTo(5), "Error"));
            Assert.That(ex?.Message, Does.Contain("Error"));
            Assert.That(ex?.Message, Does.Contain("Assert.That(() => 2 + 2, Is.EqualTo(5))"));
        }

        [Test]
        public void FailureThrowsAssertionException_ActualLambdaAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => "error";
            var ex = Assert.Throws<AssertionException>(() => Assert.That(() => 2 + 2, Is.EqualTo(5), GetExceptionMessage));
            Assert.That(ex?.Message, Does.Contain("error"));
            Assert.That(ex?.Message, Does.Contain("Assert.That(() => 2 + 2, Is.EqualTo(5))"));
        }

        [Test]
        public void FailureThrowsAssertionException_DelegateAndConstraint()
        {
            Assert.Throws<AssertionException>(() => Assert.That(ReturnsFive, Is.EqualTo(4)));
        }

        [Test]
        public void FailureThrowsAssertionException_DelegateAndConstraintWithMessage()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(ReturnsFive, Is.EqualTo(4), "Error"));
            Assert.That(ex?.Message, Does.Contain("Error"));
            Assert.That(ex?.Message, Does.Contain("Assert.That(ReturnsFive, Is.EqualTo(4))"));
        }

        [Test]
        public void FailureThrowsAssertionException_DelegateAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => "error";
            var ex = Assert.Throws<AssertionException>(() => Assert.That(ReturnsFive, Is.EqualTo(4), GetExceptionMessage));
            Assert.That(ex?.Message, Does.Contain("error"));
            Assert.That(ex?.Message, Does.Contain("Assert.That(ReturnsFive, Is.EqualTo(4))"));
        }

        [Test]
        public void AssertionsAreCountedCorrectly()
        {
            ITestResult result = TestBuilder.RunTestFixture(typeof(AssertCountFixture));

            int totalCount = 0;
            foreach (var childResult in result.Children)
            {
                int expectedCount = childResult.Name == "ThreeAsserts" ? 3 : 1;
                Assert.That(childResult.AssertCount, Is.EqualTo(expectedCount), $"Bad count for {childResult.Name}");
                totalCount += expectedCount;
            }

            Assert.That(result.AssertCount, Is.EqualTo(totalCount), "Fixture count is not correct");
        }

        [Test]
        public void PassingAssertion_DoesNotCallExceptionStringFunc()
        {
            // Arrange
            var funcWasCalled = false;

            string GetExceptionMessage()
            {
                funcWasCalled = true;
                return "Func was called";
            }

            // Act
#pragma warning disable NUnit2045 // Use Assert.Multiple
#pragma warning disable NUnit2010 // Use EqualConstraint for better assertion messages in case of failure
            Assert.That(0 + 1 == 1, GetExceptionMessage);
#pragma warning restore NUnit2010 // Use EqualConstraint for better assertion messages in case of failure
#pragma warning restore NUnit2045 // Use Assert.Multiple

            // Assert
            Assert.That(!funcWasCalled, "The getExceptionMessage function was called when it should not have been.");
        }

        [Test]
        public void FailingAssertion_CallsExceptionStringFunc()
        {
            // Arrange
            var funcWasCalled = false;

            string GetExceptionMessage()
            {
                funcWasCalled = true;
                return "Func was called";
            }

            // Act
#pragma warning disable NUnit2010 // Use EqualConstraint for better assertion messages in case of failure
            var ex = Assert.Throws<AssertionException>(() => Assert.That(1 + 1 == 1, GetExceptionMessage));
#pragma warning restore NUnit2010 // Use EqualConstraint for better assertion messages in case of failure

            // Assert
            Assert.That(ex?.Message, Does.Contain("Func was called"));
            Assert.That(ex?.Message, Does.Contain("Assert.That(1 + 1 == 1, Is.True)"));
            Assert.That(funcWasCalled, "The getExceptionMessage function was not called when it should have been.");
        }

        [Test]
        public void OnlyFailingAssertion_FormatsString()
        {
            const string text = "String was formatted";
            var formatCounter = new FormatCounter();

            Assert.That(1 + 1, Is.EqualTo(2), $"{text} {formatCounter}");
            Assert.That(formatCounter.NumberOfToStringCalls, Is.EqualTo(0), "The interpolated string should not have been evaluated");

            Assert.That(() => Assert.That(1 + 1, Is.Not.EqualTo(2), $"{text} {formatCounter}"),
                Throws.InstanceOf<AssertionException>()
                    .With.Message.Contains(text).
                    And
                    .With.Message.Contains("Assert.That(1 + 1, Is.Not.EqualTo(2)"));

            Assert.That(formatCounter.NumberOfToStringCalls, Is.EqualTo(1), "The interpolated string should have been evaluated once");
        }

        private sealed class FormatCounter
        {
            public int NumberOfToStringCalls { get; private set; }

            public override string ToString()
            {
                NumberOfToStringCalls++;
                return string.Empty;
            }
        }

        private int ReturnsFive()
        {
            return 5;
        }

        [Test]
        public void AssertThatSuccess()
        {
            Assert.That(async () => await AsyncReturnOne(), Is.EqualTo(1));
        }

        [Test]
        public void AssertThatFailure()
        {
            Assert.Throws<AssertionException>(() =>
                Assert.That(async () => await AsyncReturnOne(), Is.EqualTo(2)));
        }

        [Test, Platform(Exclude = "Linux", Reason = "Intermittent failures on Linux")]
        public void AssertThatErrorTask()
        {
#pragma warning disable NUnit2021 // Incompatible types for EqualTo constraint
            var exception =
            Assert.Throws<InvalidOperationException>(() =>
                Assert.That(async () => await ThrowInvalidOperationExceptionTask(), Is.EqualTo(1)));
#pragma warning restore NUnit2021 // Incompatible types for EqualTo constraint

            Assert.That(exception?.StackTrace, Does.Contain("ThrowInvalidOperationExceptionTask"));
        }

        [Test]
        public void AssertThatErrorGenericTask()
        {
            var exception =
            Assert.Throws<InvalidOperationException>(() =>
                Assert.That(async () => await ThrowInvalidOperationExceptionGenericTask(), Is.EqualTo(1)));

            Assert.That(exception?.StackTrace, Does.Contain("ThrowInvalidOperationExceptionGenericTask"));
        }

        private static Task<int> AsyncReturnOne()
        {
            return Task.Run(() => 1);
        }

        private static async Task<int> ThrowInvalidOperationExceptionGenericTask()
        {
            await AsyncReturnOne();
            throw new InvalidOperationException();
        }

        private static async Task ThrowInvalidOperationExceptionTask()
        {
            await AsyncReturnOne();
            throw new InvalidOperationException();
        }

        [Test]
        public void AssertThatWithLambda()
        {
            Assert.That(() => true);
        }

        [Test]
        public void AssertThatWithFalseLambda()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(() => false, "Error"));
            Assert.That(ex?.Message, Does.Contain("Error"));
            Assert.That(ex?.Message, Does.Contain("Assert.That(() => false, Is.True)"));
        }

        [TestCase(default(string), default(string))]
        [TestCase("", "")]
        public void AssertWithStrings(string? actual, string? expected)
        {
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void AssertWithExpectedClassImplicitConvertibleToString()
        {
            const string value = "Implicit Cast";
            var instance = new ClassWithImplicitCastToString(value);

            Assert.That(value, Is.EqualTo(instance), "EqualStringConstaint");
        }

        [Test]
        public void AssertWithExpectedStructImplicitConvertibleToString()
        {
            const string value = "Implicit Cast";
            var instance = new StructWithImplicitCastToString(value);

            Assert.That(value, Is.EqualTo(instance), "EqualStringConstaint");
        }

        [Test]
        public void AssertWithTypeImplicitConvertibleToString()
        {
            const string value = "Implicit Cast";
            var instance = new ClassWithImplicitCastToString(value);

            Assert.Multiple(() =>
            {
                Assert.That(instance.Value, Is.EqualTo(value), "Value");
                Assert.That(instance, Is.EqualTo(value), "EqualStringConstaint");
                Assert.That(instance, Is.EqualTo(value.ToLowerInvariant()).IgnoreCase, "EqualStringConstaint.IgnoreCase");
                Assert.That(instance, Is.EqualTo(value.Replace(" ", string.Empty)).IgnoreWhiteSpace, "EqualStringConstaint.IgnoreWhiteSpace");
            });
        }

        private sealed class StructWithImplicitCastToString
        {
            public string Value { get; }

            public StructWithImplicitCastToString(string value)
            {
                Value = value;
            }

            public static implicit operator string(StructWithImplicitCastToString instance) => instance.Value;
        }

        private sealed class ClassWithImplicitCastToString
        {
            public string Value { get; }

            public ClassWithImplicitCastToString(string value)
            {
                Value = value;
            }

            public static implicit operator string(ClassWithImplicitCastToString instance) => instance.Value;
        }

        [Test]
        public void AssertWithTypeWhichImplementsIEquatableString()
        {
            const string value = "Equatable<string>";
            var intance = new TypeWhichImplementsIEquatableString(value);

            Assert.Multiple(() =>
            {
                Assert.That(intance.Value, Is.EqualTo(value), "Value");
                Assert.That(intance, Is.EqualTo(value), "EqualStringConstaint");
                Assert.That(() => Assert.That(intance, Is.EqualTo(value.ToLowerInvariant()).IgnoreCase),
                            Throws.InvalidOperationException);
            });
        }

        private sealed class TypeWhichImplementsIEquatableString : IEquatable<string>
        {
            public string Value { get; }

            public TypeWhichImplementsIEquatableString(string value)
            {
                Value = value;
            }

            public bool Equals(string? other)
            {
                return Value.Equals(other);
            }
        }

        [TestCase("Hello", "World")]
        [TestCase('A', 'B')]
        [TestCase(false, true)]
        [TestCase(SomeEnum.One, SomeEnum.Two)]
        public void AssertThatWithTypesNotSupportingTolerance(object? x, object? y)
        {
            Assert.That(() => Assert.That(x, Is.EqualTo(y).Within(0.1)),
                        Throws.InstanceOf<NotSupportedException>().With.Message.Contains("Tolerance"));
        }

        [Test]
        public void AssertThatEqualsWithClassWithSomeToleranceAwareMembers()
        {
            var zero = new ClassWithSomeToleranceAwareMembers(0, 0.0, string.Empty, null);
            var instance = new ClassWithSomeToleranceAwareMembers(1, 1.1, "1.1", zero);

            Assert.Multiple(() =>
            {
                Assert.That(new ClassWithSomeToleranceAwareMembers(1, 1.1, "1.1", zero), Is.EqualTo(instance).UsingPropertiesComparer());
                Assert.That(new ClassWithSomeToleranceAwareMembers(1, 1.2, "1.1", zero), Is.Not.EqualTo(instance).UsingPropertiesComparer());
                Assert.That(new ClassWithSomeToleranceAwareMembers(1, 1.2, "1.1", zero), Is.EqualTo(instance).Within(0.1).UsingPropertiesComparer());
                Assert.That(new ClassWithSomeToleranceAwareMembers(1, 1.1, "1.1", null), Is.Not.EqualTo(instance).UsingPropertiesComparer());
                Assert.That(new ClassWithSomeToleranceAwareMembers(1, 1.1, "2.2", zero), Is.Not.EqualTo(instance).UsingPropertiesComparer());
                Assert.That(new ClassWithSomeToleranceAwareMembers(1, 2.2, "1.1", zero), Is.Not.EqualTo(instance).UsingPropertiesComparer());
                Assert.That(new ClassWithSomeToleranceAwareMembers(2, 1.1, "1.1", zero), Is.Not.EqualTo(instance).UsingPropertiesComparer());
            });
        }

        [Test]
        [DefaultFloatingPointTolerance(0.1)]
        public void AssertThatEqualsWithClassWithSomeToleranceAwareMembersUsesDefaultFloatingPointTolerance()
        {
            var zero = new ClassWithSomeToleranceAwareMembers(0, 0.0, string.Empty, null);
            var instance = new ClassWithSomeToleranceAwareMembers(1, 1.1, "1.1", zero);

            Assert.That(new ClassWithSomeToleranceAwareMembers(1, 1.2, "1.1", zero), Is.EqualTo(instance).UsingPropertiesComparer());
        }

        private sealed class ClassWithSomeToleranceAwareMembers
        {
            public ClassWithSomeToleranceAwareMembers(int valueA, double valueB, string valueC, ClassWithSomeToleranceAwareMembers? chained)
            {
                ValueA = valueA;
                ValueB = valueB;
                ValueC = valueC;
                Chained = chained;
            }

            public int ValueA { get; }
            public double ValueB { get; }
            public string ValueC { get; }
            public ClassWithSomeToleranceAwareMembers? Chained { get; }

            public override string ToString()
            {
                return $"{ValueA} {ValueB} '{ValueC}' [{Chained}]";
            }
        }

        [Test]
        public void AssertThatEqualsWithStructWithSomeToleranceAwareMembers()
        {
            var instance = new StructWithSomeToleranceAwareMembers(1, 1.1, "1.1", SomeEnum.One);

            Assert.Multiple(() =>
            {
                Assert.That(new StructWithSomeToleranceAwareMembers(1, 1.1, "1.1", SomeEnum.One), Is.EqualTo(instance).UsingPropertiesComparer());
                Assert.That(new StructWithSomeToleranceAwareMembers(1, 1.2, "1.1", SomeEnum.One), Is.Not.EqualTo(instance).UsingPropertiesComparer());
                Assert.That(new StructWithSomeToleranceAwareMembers(1, 1.2, "1.1", SomeEnum.One), Is.EqualTo(instance).Within(0.1).UsingPropertiesComparer());
                Assert.That(new StructWithSomeToleranceAwareMembers(1, 1.1, "1.1", SomeEnum.Two), Is.Not.EqualTo(instance).Within(0.1).UsingPropertiesComparer());
                Assert.That(new StructWithSomeToleranceAwareMembers(1, 2.2, "1.1", SomeEnum.One), Is.Not.EqualTo(instance).UsingPropertiesComparer());
                Assert.That(new StructWithSomeToleranceAwareMembers(2, 1.1, "1.1", SomeEnum.One), Is.Not.EqualTo(instance).UsingPropertiesComparer());
            });
        }

        [Test]
        public void AssertThatEqualsWithStructMemberDifferences()
        {
            var instance = new StructWithSomeToleranceAwareMembers(1, 1.1, "1.1", SomeEnum.One);

            Assert.That(() =>
                Assert.That(new StructWithSomeToleranceAwareMembers(2, 1.1, "1.1", SomeEnum.One), Is.EqualTo(instance).UsingPropertiesComparer()),
                Throws.InstanceOf<AssertionException>().With.Message.Contains("at property StructWithSomeToleranceAwareMembers.ValueA")
                                                       .And.Message.Contains("Expected: 1"));
            Assert.That(() =>
                Assert.That(new StructWithSomeToleranceAwareMembers(1, 1.2, "1.1", SomeEnum.One), Is.EqualTo(instance).UsingPropertiesComparer()),
                Throws.InstanceOf<AssertionException>().With.Message.Contains("at property StructWithSomeToleranceAwareMembers.ValueB")
                                                       .And.Message.Contains("Expected: 1.1"));
            Assert.That(() =>
                Assert.That(new StructWithSomeToleranceAwareMembers(1, 1.1, "1.2", SomeEnum.One), Is.EqualTo(instance).UsingPropertiesComparer()),
                Throws.InstanceOf<AssertionException>().With.Message.Contains("at property StructWithSomeToleranceAwareMembers.ValueC")
                                                       .And.Message.Contains("Expected: \"1.1\""));
            Assert.That(() =>
                Assert.That(new StructWithSomeToleranceAwareMembers(1, 1.1, "1.1", SomeEnum.Two), Is.EqualTo(instance).UsingPropertiesComparer()),
                Throws.InstanceOf<AssertionException>().With.Message.Contains("at property StructWithSomeToleranceAwareMembers.ValueD")
                                                       .And.Message.Contains("Expected: One"));

            /*
             * Uncomment this block to see the actual exception messages. Test will fail.
             *
            Assert.Multiple(() =>
            {
                Assert.That(new StructWithSomeToleranceAwareMembers(2, 1.1, "1.1", SomeEnum.One), Is.EqualTo(instance).UsingPropertiesComparer());
                Assert.That(new StructWithSomeToleranceAwareMembers(1, 1.1, "1.1", SomeEnum.One), Is.EqualTo(instance).UsingPropertiesComparer());
                Assert.That(new StructWithSomeToleranceAwareMembers(1, 1.2, "1.1", SomeEnum.One), Is.EqualTo(instance).UsingPropertiesComparer());
                Assert.That(new StructWithSomeToleranceAwareMembers(1, 1.1, "1.2", SomeEnum.One), Is.EqualTo(instance).UsingPropertiesComparer());
                Assert.That(new StructWithSomeToleranceAwareMembers(1, 1.1, "1.1", SomeEnum.Two), Is.EqualTo(instance).UsingPropertiesComparer());
            });
            */
        }

        private enum SomeEnum
        {
            One = 1,
            Two = 2,
        }

        private readonly struct StructWithSomeToleranceAwareMembers
        {
            public StructWithSomeToleranceAwareMembers(int valueA, double valueB, string valueC, SomeEnum valueD)
            {
                ValueA = valueA;
                ValueB = valueB;
                ValueC = valueC;
                ValueD = valueD;
            }

            public int ValueA { get; }
            public double ValueB { get; }
            public string ValueC { get; }
            public SomeEnum ValueD { get; }

            public override string ToString()
            {
                return $"{ValueA} {ValueB} '{ValueC}' {ValueD}";
            }
        }

        [Test]
        public void AssertThatEqualsWithStructWithNoToleranceAwareMembers()
        {
            var instance = new StructWithNoToleranceAwareMembers("1.1", SomeEnum.One);

            Assert.Multiple(() =>
            {
                Assert.That(new StructWithNoToleranceAwareMembers("1.1", SomeEnum.One), Is.EqualTo(instance));
                Assert.That(new StructWithNoToleranceAwareMembers("1.2", SomeEnum.One), Is.Not.EqualTo(instance));
                Assert.That(new StructWithNoToleranceAwareMembers("1.1", SomeEnum.Two), Is.Not.EqualTo(instance));
                Assert.That(() =>
                    Assert.That(new StructWithNoToleranceAwareMembers("1.2", SomeEnum.One),
                                Is.EqualTo(instance).Within(0.1)),
                    Throws.InstanceOf<NotSupportedException>().With.Message.Contains("Tolerance"));
            });
        }

        private readonly struct StructWithNoToleranceAwareMembers
        {
            public StructWithNoToleranceAwareMembers(string valueA, SomeEnum valueB)
            {
                ValueA = valueA;
                ValueB = valueB;
            }

            public string ValueA { get; }
            public SomeEnum ValueB { get; }

            public override string ToString()
            {
                return $"'{ValueA}' {ValueB}";
            }
        }

        [Test]
        public void AssertThatEqualsWithRecord()
        {
            var zero = new SomeRecord(0, 0.0, string.Empty, null);
            var instance = new SomeRecord(1, 1.1, "1.1", zero);

            Assert.Multiple(() =>
            {
                Assert.That(new SomeRecord(1, 1.1, "1.1", zero), Is.EqualTo(instance));
                Assert.That(new SomeRecord(1, 1.2, "1.1", zero), Is.Not.EqualTo(instance));
                Assert.That(new SomeRecord(1, 1.1, "1.1", null), Is.Not.EqualTo(instance));
                Assert.That(new SomeRecord(1, 1.1, "2.2", zero), Is.Not.EqualTo(instance));
                Assert.That(new SomeRecord(1, 2.2, "1.1", zero), Is.Not.EqualTo(instance));
                Assert.That(new SomeRecord(2, 1.1, "1.1", zero), Is.Not.EqualTo(instance));
#pragma warning disable NUnit2047 // Incompatible types for Within constraint
                Assert.That(() =>
                    Assert.That(new SomeRecord(1, 1.2, "1.1", zero),
                                Is.EqualTo(instance).Within(0.1)),
                    Throws.InstanceOf<NotSupportedException>().With.Message.Contains("Tolerance"));
#pragma warning restore NUnit2047 // Incompatible types for Within constraint
            });
        }

        private sealed record SomeRecord
        {
            public SomeRecord(int valueA, double valueB, string valueC, SomeRecord? chained)
            {
                ValueA = valueA;
                ValueB = valueB;
                ValueC = valueC;
                Chained = chained;
            }

            public int ValueA { get; }
            public double ValueB { get; }
            public string ValueC { get; }
            public SomeRecord? Chained { get; }

            public override string ToString()
            {
                return $"{ValueA} {ValueB} '{ValueC}' [{Chained}]";
            }
        }

        [Test]
        public void AssertWithRecursiveClass()
        {
            LinkedList list1 = new(1, new(2, new(3)));
            LinkedList list2 = new(1, new(2, new(3)));

            Assert.That(list1, Is.Not.EqualTo(list2));
            Assert.That(list1, Is.EqualTo(list2).UsingPropertiesComparer());
        }

        [Test]
        public void AssertWithCyclicRecursiveClass()
        {
            LinkedList list1 = new(1);
            LinkedList list2 = new(1);

            list1.Next = list1;
            list2.Next = list2;

            Assert.That(list1, Is.Not.EqualTo(list2)); // Reference comparison
            Assert.That(list1, Is.EqualTo(list2).UsingPropertiesComparer());
        }

        [Test]
        public void AssertRecordsComparingProperties()
        {
            var record1 = new Record("Name", [1, 2, 3]);
            var record2 = new Record("Name", [1, 2, 3]);

            Assert.That(record1, Is.Not.EqualTo(record2)); // Record's generated method does not handle collections
            Assert.That(record1, Is.EqualTo(record2).UsingPropertiesComparer());
        }

        [Test]
        public void AssertRecordsComparingProperties_WhenRecordHasUserDefinedEqualsMethod()
        {
            var record1 = new ParentRecord(new RecordWithOverriddenEquals("Name"), [1, 2, 3]);
            var record2 = new ParentRecord(new RecordWithOverriddenEquals("NAME"), [1, 2, 3]);

            Assert.That(record1, Is.Not.EqualTo(record2)); // ParentRecord's generated method does not handle collections
            Assert.That(record1, Is.EqualTo(record2).UsingPropertiesComparer());
        }

        private sealed class LinkedList
        {
            public LinkedList(int value, LinkedList? next = null)
            {
                Value = value;
                Next = next;
            }

            public int Value { get; }

            public LinkedList? Next { get; set; }
        }

        [Test]
        public void EqualMemberWithIndexer()
        {
            var members = new Members("Hello", "World", "NUnit");
            var copy = new Members("Hello", "World", "NUnit");

            Assert.That(members[1], Is.EqualTo("World"));
            Assert.That(copy, Is.Not.EqualTo(members));
            Assert.That(() => Assert.That(copy, Is.EqualTo(members).UsingPropertiesComparer()), Throws.InstanceOf<NotSupportedException>());
        }

        private sealed class Members
        {
            private readonly string[] _members;

            public Members(params string[] members)
            {
                _members = members;
            }

            public string this[int index] => _members[index];
        }

        [Test]
        public void TestPropertyFailureSecondLevel()
        {
            var one = new ParentClass(new ChildClass(new GrandChildClass(1)), new ChildClass(new GrandChildClass(2), new GrandChildClass(3)));
            var two = new ParentClass(new ChildClass(new GrandChildClass(1)), new ChildClass(new GrandChildClass(2), new GrandChildClass(4)));

            Assert.That(() => Assert.That(two, Is.EqualTo(one).UsingPropertiesComparer()),
                        Throws.InstanceOf<AssertionException>().With.Message.Contains("at property ParentClass.Two")
                                                               .And.Message.Contains("at property ChildClass.Values")
                                                               .And.Message.Contains("at index [1]")
                                                               .And.Message.Contains("at property GrandChildClass.Value")
                                                               .And.Message.Contains("Expected: 3"));

            /*
             * Uncomment this block to see the actual exception messages. Test will fail.
             *
            Assert.That(two, Is.EqualTo(one).UsingPropertiesComparer());
             */
        }

        [Test]
        public void UseAssertThatWithCollectionExpression_EqualTo()
        {
            var actual = new[] { 1, 2, 3 };
            Assert.That(actual, Is.EqualTo([1, 2, 3]));
        }

        [Test]
        public void UseAssertThatWithCollectionExpression_EquivalentTo()
        {
            var actual = new[] { 3, 2, 1 };
            Assert.That(actual, Is.EquivalentTo([1, 2, 3]));
        }

        [Test]
        public void UseAssertThatWithCollectionExpression_SubsetOf()
        {
            var actual = new[] { 1, 2, 3 };
            Assert.That(actual, Is.SubsetOf([1, 2, 3, 4, 5]));
        }

        [Test]
        public void UseAssertThatWithCollectionExpression_SupersetOf()
        {
            var actual = new[] { 1, 2, 3 };
            Assert.That(actual, Is.SupersetOf([1, 2]));
        }

        private record Record(string Name, int[] Collection);

        private record ParentRecord(RecordWithOverriddenEquals Child, int[] Collection);

        private record RecordWithOverriddenEquals(string Name)
        {
            public virtual bool Equals(RecordWithOverriddenEquals? other)
            {
                return string.Equals(Name, other?.Name, StringComparison.OrdinalIgnoreCase);
            }

            public override int GetHashCode()
            {
                return Name.ToUpperInvariant().GetHashCode();
            }
        }

        private sealed class ParentClass
        {
            public ParentClass(ChildClass one, ChildClass two)
            {
                One = one;
                Two = two;
            }

            public ChildClass One { get; }

            public ChildClass Two { get; }
        }

        private sealed class ChildClass
        {
            public ChildClass(params GrandChildClass[] values) => Values = values;

            public GrandChildClass[] Values { get; }
        }

        private sealed class GrandChildClass
        {
            public GrandChildClass(int value) => Value = value;

            public int Value { get; }
        }
    }
}
