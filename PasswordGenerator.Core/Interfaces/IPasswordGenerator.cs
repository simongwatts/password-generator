using PasswordGenerator.Core.Generation;

namespace PasswordGenerator.Core.Interfaces;

public interface IPasswordGenerator
{
    string Generate(PasswordGenerationOptions options);
    IEnumerable<string> GenerateMultiple(PasswordGenerationOptions options, int count);
}