using Bogus;
using FluentAssertions;
using Todos.Core.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Todos.UnitTest.Extensions;

public class EncriptionExtensionTests
{
    private readonly Faker _faker;
    private readonly ITestOutputHelper _testOutputHelper;

    public EncriptionExtensionTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _faker = new Faker();
    }
    
    [Fact]
    public void Hash256_ShouldEncryptString_WhenValueIsNotNull()
    {
        var hashed = _faker.Random.Word().Hash256(_faker.Random.Hash());
        _testOutputHelper.WriteLine(hashed);
        hashed.Should().NotBeNullOrWhiteSpace();
    }
}