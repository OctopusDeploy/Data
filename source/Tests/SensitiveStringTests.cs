using System;
using FluentAssertions;
using Octopus.Data.Model;
using Xunit;

namespace Tests
{
    public class SensitiveStringTests
    {
        [Fact]
        public void ComparingSensitiveStringPointersAreEqualWorks()
        {
            var a = "Test Value".ToSensitiveString();
            var b = "Test Value".ToSensitiveString();

            (a == b).Should().BeTrue("equality operator should return true for identical contents");
        }

        [Fact]
        public void ComparingSensitiveStringPointersAreNotEqualWorks()
        {
            var a = "Test Value".ToSensitiveString();
            var b = "Test Value2".ToSensitiveString();

            (a != b).Should().BeTrue("inequality operator should return true for different contents");
        }

        [Fact]
        public void ComparingSensitiveStringsToEqualOperatorStringWorks()
        {
            var a = "Test Value".ToSensitiveString();

            (a == "Test Value").Should().BeTrue("equality operator should return true for identical contents");
        }

        [Fact]
        public void ComparingSensitiveStringsToEqualMethodStringWorks()
        {
            var a = "Test Value".ToSensitiveString();

            a.Equals("Test Value").Should().BeTrue("equals method should return true for identical contents");
        }

        [Fact]
        public void ComparingSensitiveStringsToNotEqualStringWorks()
        {
            var a = "Test Value".ToSensitiveString();

            (a != "Test Value2").Should().BeTrue("inequality operator should return true for different contents");
        }

        [Fact]
        public void ComparingSensitiveStringsToNullWorks()
        {
            var a = "Test Value".ToSensitiveString();

            (a == null).Should().BeFalse("equality operator should return false for a non-null value compared with null");
        }

        [Fact]
        public void ComparingNullSensitiveStringsToNullWorks()
        {
            SensitiveString? a = null;

            (a == null).Should().BeTrue("equality operator should return true for a null value compared with null");
        }
    }
}