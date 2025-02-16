using PasswordGenerator.Core.Generation;
using PasswordGenerator.Core.Interfaces;

namespace PasswordGenerator.Tests;

public class AmbiguousCharacterTests
{
    private readonly IPasswordGenerator _generator = new Core.Generation.PasswordGenerator();
    private readonly char[] _ambiguousCharacters = { 'i', 'I', '1', 'l', 'L', 'o', 'O', '0' };

    [Fact]
    public void Generate_WithExcludeAmbiguous_OmitsAmbiguousCharacters()
    {
        var options = new PasswordGenerationOptions
        {
            ExcludeAmbiguous = true,
            UseLower = true,
            UseUpper = true,
            UseNumbers = true,
            UseSymbols = false,
            Length = 20
        };

        var passwords = _generator.GenerateMultiple(options, 100);

        foreach (var password in passwords)
        {
            // Check for ambiguous characters
            Assert.DoesNotContain(password, c => _ambiguousCharacters.Contains(c));

            // Verify required character sets
            Assert.Contains(password, c => char.IsLower(c));
            Assert.Contains(password, c => char.IsUpper(c));
            Assert.Contains(password, c => char.IsDigit(c));
        }
    }

    [Fact]
    public void Generate_WithoutExcludeAmbiguous_IncludesAmbiguousCharacters()
    {
        var options = new PasswordGenerationOptions
        {
            ExcludeAmbiguous = false,
            UseLower = true,
            UseUpper = true,
            UseNumbers = true,
            Length = 50  // Increased length for better chance of ambiguous chars
        };

        var passwords = _generator.GenerateMultiple(options, 100);
        var ambiguousFound = passwords.Any(p => p.Intersect(_ambiguousCharacters).Any());

        Assert.True(ambiguousFound,
            "Expected ambiguous characters when exclusion is disabled");
    }
}