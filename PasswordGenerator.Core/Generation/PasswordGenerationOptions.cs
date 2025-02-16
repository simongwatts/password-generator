namespace PasswordGenerator.Core.Generation;

public sealed class PasswordGenerationOptions
{
    public int Length { get; set; } = 16;
    public bool UseUpper { get; set; } = true;
    public bool UseLower { get; set; } = true;
    public bool UseNumbers { get; set; } = true;
    public bool UseSymbols { get; set; } = true;
    public string ExcludeCharacters { get; set; } = string.Empty;
    public bool ExcludeAmbiguous { get; set; } = true;
}