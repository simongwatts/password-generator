using System.CommandLine;
using PasswordGenerator.Core.Generation;
using PasswordGenerator.Core.Interfaces;

namespace PasswordGenerator.Cli;

internal class Program
{
    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("Secure Password Generator - CLI Tool");

        var lengthOption = new Option<int>("--length", () => 16, "Password length (8-64)");
        var upperOption = new Option<bool>("--upper", () => true, "Include uppercase letters");
        var lowerOption = new Option<bool>("--lower", () => true, "Include lowercase letters");
        var numbersOption = new Option<bool>("--numbers", () => true, "Include numbers");
        var symbolsOption = new Option<bool>("--symbols", () => true, "Include symbols");
        var excludeOption = new Option<string>("--exclude", "Characters to exclude");
        var countOption = new Option<int>("--count", () => 1, "Number of passwords to generate");
        var ambiguousOption = new Option<bool>("--no-ambiguous", "Exclude ambiguous characters");

        rootCommand.AddOption(lengthOption);
        rootCommand.AddOption(upperOption);
        rootCommand.AddOption(lowerOption);
        rootCommand.AddOption(numbersOption);
        rootCommand.AddOption(symbolsOption);
        rootCommand.AddOption(excludeOption);
        rootCommand.AddOption(countOption);
        rootCommand.AddOption(ambiguousOption);

        rootCommand.SetHandler((int length, bool upper, bool lower, bool numbers,
            bool symbols, string exclude, int count, bool noAmbiguous) =>
        {
            var options = new PasswordGenerationOptions
            {
                Length = length,
                UseUpper = upper,
                UseLower = lower,
                UseNumbers = numbers,
                UseSymbols = symbols,
                ExcludeCharacters = exclude ?? string.Empty,
                ExcludeAmbiguous = noAmbiguous
            };

            try
            {
                IPasswordGenerator generator = new Core.Generation.PasswordGenerator();
                var passwords = generator.GenerateMultiple(options, count);

                Console.WriteLine($"=== Generated Password{(count > 1 ? "s" : "")} ===");
                foreach (var pwd in passwords)
                {
                    Console.WriteLine(pwd);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Environment.Exit(1);
            }
        }, lengthOption, upperOption, lowerOption, numbersOption,
            symbolsOption, excludeOption, countOption, ambiguousOption);

        return await rootCommand.InvokeAsync(args);
    }
}