using CSharpFunctionalExtensions;
using FluentAssertions;
using FluentAssertions.Execution;

namespace ArmaForces.Boderator.Core.Tests.TestUtilities
{
    public static class ResultAssertionsExtensions
    {
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
