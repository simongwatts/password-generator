// PasswordGenerator.Tests/PasswordGeneratorTests.cs
using PasswordGenerator.Core.Generation;
using PasswordGenerator.Core.Interfaces;
using Xunit;
using System.Linq;
using System.Security.Cryptography;

namespace PasswordGenerator.Tests;

public class PasswordGeneratorTests
{
    private readonly IPasswordGenerator _generator = new Core.Generation.PasswordGenerator();

    [Fact]
    public void Generate_ValidOptions_ReturnsCorrectLength()
    {
        var options = new PasswordGenerationOptions { Length = 12 };
        var result = _generator.Generate(options);
        Assert.Equal(12, result.Length);
    }

    [Fact]
    public void Generate_ExcludesSpecifiedCharacters()
    {
        var options = new PasswordGenerationOptions
        {
            Length = 50,
            ExcludeCharacters = "aA1!",
            UseUpper = true,
            UseLower = true,
            UseNumbers = true,
            UseSymbols = true
        };

        var passwords = _generator.GenerateMultiple(options, 100);

        foreach (var password in passwords)
        {
            Assert.DoesNotContain('a', password);
            Assert.DoesNotContain('A', password);
            Assert.DoesNotContain('1', password);
            Assert.DoesNotContain('!', password);
        }
    }

    [Fact]
    public void Generate_WithExcludeAmbiguous_OmitsAmbiguousCharacters()
    {
        var options = new PasswordGenerationOptions
        {
            ExcludeAmbiguous = true,
            UseLower = true,
            UseUpper = true,
            UseNumbers = true,
            Length = 20
        };

        var passwords = _generator.GenerateMultiple(options, 100);
        var ambiguousChars = new[] { 'i', 'I', '1', 'l', 'L', 'o', 'O', '0' };

        foreach (var password in passwords)
        {
            Assert.DoesNotContain(password, c => ambiguousChars.Contains(c));
        }
    }

    [Fact]
    public void Generate_WithAllOptionsDisabled_ThrowsException()
    {
        var options = new PasswordGenerationOptions
        {
            UseUpper = false,
            UseLower = false,
            UseNumbers = false,
            UseSymbols = false
        };

        Assert.Throws<ArgumentException>(() => _generator.Generate(options));
    }

    [Fact]
    public void Generate_WithEmptyCharacterSet_ThrowsException()
    {
        var options = new PasswordGenerationOptions
        {
            UseUpper = true,
            ExcludeCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ" // Will empty uppercase set
        };

        var ex = Assert.Throws<ArgumentException>(() => _generator.Generate(options));
        Assert.Contains("upper set empty after exclusions", ex.Message);
    }

    //[Fact]
    //public void Generate_WithAllEmptySets_ThrowsCombinedException()
    //{
    //    var options = new PasswordGenerationOptions
    //    {
    //        UseUpper = true,
    //        UseLower = true,
    //        ExcludeCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"
    //    };

    //    var ex = Assert.Throws<ArgumentException>(() => _generator.Generate(options));
    //    Assert.Contains("All enabled character sets are empty", ex.Message);
    //}

    [Fact]
    public void Generate_WithPartialEmptySet_ThrowsSpecificException()
    {
        var options = new PasswordGenerationOptions
        {
            UseLower = true,
            ExcludeCharacters = "abcdefghijklmnopqrstuvwxyz"
        };

        var ex = Assert.Throws<ArgumentException>(() => _generator.Generate(options));
        Assert.Contains("lower set empty", ex.Message);
    }
}