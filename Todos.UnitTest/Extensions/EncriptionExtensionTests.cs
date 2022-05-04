using System;
using Bogus;
using FluentAssertions;
using Todos.Core.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Todos.UnitTest.Extensions;

public class EncriptionExtensionTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public EncriptionExtensionTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    
    [Theory]
    [InlineData("123", "lkjslkjslk9873l;;398!!2@#", null, null)]
    [InlineData("PassW0rd123!@#$%^&*()", "LKjrglkvfr21#@$$#lkjfv./,<>?oksdlkj", null, null)]
    [InlineData("lkjldkfj", null, null, null)]
    [InlineData(null, null, typeof(ArgumentException), "Can't Hash null value")]
    public void Hash256_ShouldEncryptString_WhenValueIsNotNull(string word, string secret, Type ex, string errorMessage)
    {
        try
        {
            var hashed = word.Hash256(secret);
            _testOutputHelper.WriteLine(hashed);
            hashed.Should().NotBeNullOrWhiteSpace();
        }
        catch (Exception e)
        {
            e.Should().BeOfType(ex);
            e.Message.Should().Be(errorMessage);
            _testOutputHelper.WriteLine("Exception valid");
        }
    }
}