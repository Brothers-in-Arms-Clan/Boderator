using CSharpFunctionalExtensions;
using FluentAssertions;
using FluentAssertions.Execution;

namespace ArmaForces.Boderator.Core.Tests.TestUtilities
{
    public static class ResultAssertionsExtensions
    {
        public static void ShouldBeFailure(this Result result)
        {
            using var scope = new AssertionScope();

            if (result.IsSuccess)
            {
                result.IsSuccess.Should().BeFalse();
            }
        }
        
        public static void ShouldBeFailure(this Result result, string expectedError)
        {
            using var scope = new AssertionScope();

            if (result.IsSuccess)
            {
                result.IsSuccess.Should().BeFalse();
            }
            else
            {
                result.Error.Should().Be(expectedError);
            }
        }

        public static void ShouldBeFailure<T>(this Result<T> result, string expectedError)
        {
            using var scope = new AssertionScope();

            if (result.IsSuccess)
            {
                result.IsSuccess.Should().BeFalse();
            }
            else
            {
                result.Error.Should().Be(expectedError);
            }
        }
        
        public static void ShouldBeSuccess(this Result result)
        {
            using var scope = new AssertionScope();

            if (result.IsFailure)
            {
                result.IsSuccess.Should().BeTrue();
                result.Error.Should().BeNull();
            }
        }
        
        public static void ShouldBeSuccess<T>(this Result<T> result, T expectedValue)
        {
            using var scope = new AssertionScope();

            if (result.IsSuccess)
            {
                result.Value.Should().BeEquivalentTo(expectedValue);
            }
            else
            {
                result.IsSuccess.Should().BeTrue();
                result.Error.Should().BeNull();
            }
        }
    }
}
