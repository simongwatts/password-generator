# Password Generator 🔐

A password generator built with .NET 8 that creates passwords with customisable options.

## Features ✨

- **Custom Length (8-64 chars)** - Generate passwords from 8 to 64 characters
- **Character Set Control** - Include/exclude:
  - Uppercase letters (A-Z)
  - Lowercase letters (a-z)
  - Numbers (0-9)
  - Symbols (!@#$%^&* etc.)
- **Security First**:
  - Uses .NET's `RandomNumberGenerator` (CSPRNG)
  - Automatic ambiguous character exclusion (e.g., 1/l/I/0/O)
  - Configurable character exclusions
- **Cross-Platform** - Runs on Windows, Linux, and macOS
- **Multi-Output** - Generate multiple passwords at once

## Usage 🚀

Run the application and follow the prompts to generate secure passwords. Options allow customisation of length and character sets.

### From Source 🛠️

To build and run from source:

git clone https://github.com/simongwatts/password-generator.git

cd password-generator

dotnet run --project PasswordGenerator.Cli -- --length 24 --count 3

## Tech Stack 🏗️

.NET 8 - Core framework

C# - Primary language

System.Security.Cryptography - Random generation

## License 📜

This project is licensed under the MIT License - see the LICENSE file for details.

## Disclaimer ⚠️

This tool should not be used for generating passwords for sensitive systems without proper security review. Always follow your organisation's password policies.