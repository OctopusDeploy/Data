using System;
using FluentAssertions;
using Octopus.Data;
using Xunit;

namespace Tests
{
    public class ResultTests
    {
        [Fact]
        public void CheckFailureWithFailureCheck()
        {
            var result = new TestClassWithResultMethod().DoSomething(true);
            var assertion = result.Should().BeAssignableTo<IFailureResult>();
            assertion.Subject.ErrorString.Should().Be("Some failure reason");
        }

        [Fact]
        public void CheckFailureWithSuccessCheck()
        {
            var result = new TestClassWithResultMethod().DoSomething(true);
            result.Should().NotBeAssignableTo<ISuccessResult<TestObjectBeingReturned>>();
        }

        [Fact]
        public void CheckSuccessWithSuccessCheck()
        {
            var result = new TestClassWithResultMethod().DoSomething(false);
            var assertion = result.Should().BeAssignableTo<ISuccessResult<TestObjectBeingReturned>>();
            "Some Name".Should().Be(assertion.Subject.Value.Name);
        }

        [Fact]
        public void CheckSuccessWithFailureCheck()
        {
            var result = new TestClassWithResultMethod().DoSomething(false);
            result.Should().NotBeAssignableTo<IFailureResult>();
        }

        [Fact]
        public void CheckSuccessWithNullableType()
        {
            var result = new TestClassWithResultMethod().DoSomething(false);
            var assertion = result.Should().BeAssignableTo<ISuccessResult<TestObjectBeingReturned>>();
            "Some Name".Should().Be(assertion.Subject.Value?.Name);
        }

        [Fact]
        public void CheckSuccessNoObjectWithFailureCheck()
        {
            var result = new TestClassWithResultMethod().DoSomethingWithNoObjectToReturn(false);
            result.Should().NotBeAssignableTo<IFailureResult>();
        }

        [Fact]
        public void CheckSuccessNoObjectWithSuccessCheck()
        {
            var result = new TestClassWithResultMethod().DoSomethingWithNoObjectToReturn(false);
            result.Should().BeAssignableTo<ISuccessResult>();
        }

        class TestClassWithResultMethod
        {
            public IResult<TestObjectBeingReturned> DoSomething(bool fail)
            {
                if (fail)
                    return Result<TestObjectBeingReturned>.Failed("Some failure reason");
                return Result<TestObjectBeingReturned>.Success(new TestObjectBeingReturned("Some Name"));
            }

            public IResult DoSomethingWithNoObjectToReturn(bool fail)
            {
                if (fail)
                    return Result.Failed("Some failure reason");
                return Result.Success();
            }
        }

        class TestObjectBeingReturned
        {
            public TestObjectBeingReturned(string name)
            {
                Name = name;
            }

            public string Name { get; }
        }
    }
}