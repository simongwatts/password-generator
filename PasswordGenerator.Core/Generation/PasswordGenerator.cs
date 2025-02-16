using System.Security.Cryptography;
using System.Text;
using PasswordGenerator.Core.Interfaces;

namespace PasswordGenerator.Core.Generation;

public sealed class PasswordGenerator : IPasswordGenerator
{
    private const string Lowercase = "abcdefghjkmnpqrstuvwxyz";
    private const string Uppercase = "ABCDEFGHJKMNPQRSTUVWXYZ";
    private const string Numbers = "23456789";
    private static readonly string AsciiSymbols;

    static PasswordGenerator()
    {
        // All printable ASCII symbols (32-126 excluding letters/numbers)
        AsciiSymbols = new string(
            Enumerable.Range(32, 126 - 32 + 1)
                .Select(c => (char)c)
                .Where(c => !char.IsLetterOrDigit(c))
                .ToArray()
        );
    }

    public string Generate(PasswordGenerationOptions options)
    {
        ValidateOptions(options);
        var (fullSet, filteredSets) = BuildCharacterSet(options);
        return GenerateSecurePassword(options, fullSet, filteredSets);
    }

    public IEnumerable<string> GenerateMultiple(PasswordGenerationOptions options, int count)
        => Enumerable.Range(0, count).Select(_ => Generate(options));

    private static void ValidateOptions(PasswordGenerationOptions options)
    {
        if (options.Length < 8 || options.Length > 64)
            throw new ArgumentException("Password length must be between 8-64 characters");

        if (!options.UseUpper && !options.UseLower &&
            !options.UseNumbers && !options.UseSymbols)
            throw new ArgumentException("At least one character set must be enabled");
    }

    private (string fullSet, Dictionary<char, string> filteredSets) BuildCharacterSet(PasswordGenerationOptions options)
    {
        var filteredSets = new Dictionary<char, string>();
        var fullSet = new StringBuilder();

        // Build and filter character sets
        var lower = GetFilteredSet("lower", options.UseLower,
            options.ExcludeAmbiguous ? Lowercase : "abcdefghijklmnopqrstuvwxyz",
            options.ExcludeCharacters);

        var upper = GetFilteredSet("upper", options.UseUpper,
            options.ExcludeAmbiguous ? Uppercase : "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
            options.ExcludeCharacters);

        var numbers = GetFilteredSet("numbers", options.UseNumbers,
            options.ExcludeAmbiguous ? Numbers : "0123456789",
            options.ExcludeCharacters);

        var symbols = GetFilteredSet("symbols", options.UseSymbols,
            AsciiSymbols,
            options.ExcludeCharacters);

        // Build final sets
        if (options.UseLower) fullSet.Append(lower);
        if (options.UseUpper) fullSet.Append(upper);
        if (options.UseNumbers) fullSet.Append(numbers);
        if (options.UseSymbols) fullSet.Append(symbols);

        // Check combined set
        if (fullSet.Length == 0)
            throw new ArgumentException("All enabled character sets are empty after exclusions");

        if (fullSet.Length < 10)
            throw new ArgumentException("Combined character set too small after exclusions (min 10)");

        return (fullSet.ToString(), new Dictionary<char, string>
        {
            ['l'] = lower,
            ['u'] = upper,
            ['n'] = numbers,
            ['s'] = symbols
        });
    }

    private string GetFilteredSet(string setName, bool isEnabled, string baseSet, string excludeChars)
    {
        if (!isEnabled) return string.Empty;

        var filtered = baseSet.Except(excludeChars).ToArray();
        if (filtered.Length == 0)
            throw new ArgumentException($"{setName} set empty after exclusions");

        return new string(filtered);
    }

    private string GenerateSecurePassword(
        PasswordGenerationOptions options,
        string fullSet,
        Dictionary<char, string> filteredSets)
    {
        using var rng = RandomNumberGenerator.Create();
        var password = new StringBuilder(options.Length);

        // Add required characters from each enabled set
        foreach (var set in filteredSets)
        {
            if (set.Value.Length > 0)
            {
                password.Append(GetRandomCharacter(set.Value, rng));
            }
        }

        // Fill remaining length
        while (password.Length < options.Length)
        {
            password.Append(GetRandomCharacter(fullSet, rng));
        }

        return Shuffle(password.ToString());
    }

    private static char GetRandomCharacter(string characterSet, RandomNumberGenerator rng)
    {
        // Handle edge case first
        if (characterSet.Length == 0)
            throw new ArgumentException("Character set must not be empty");

        // Calculate safe range to avoid modulo bias
        uint numPossibleValues = uint.MaxValue - (uint.MaxValue % (uint)characterSet.Length);
        byte[] buffer = new byte[4];
        uint randomValue;

        do
        {
            rng.GetBytes(buffer);
            randomValue = BitConverter.ToUInt32(buffer);
        } while (randomValue >= numPossibleValues); // Re-roll if in biased range

        return characterSet[(int)(randomValue % (uint)characterSet.Length)];
    }

    private static string Shuffle(string input)
    {
        var array = input.ToCharArray();
        RandomNumberGenerator.Shuffle(array.AsSpan());
        return new string(array);
    }
}