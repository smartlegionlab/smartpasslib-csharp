# Smart Password Library C# <sup>v1.0.3</sup>

**C# implementation of deterministic smart password generator. Same secret + same length = same password across all platforms (Python, JS, Kotlin, Go, C#).**

---

[![GitHub top language](https://img.shields.io/github/languages/top/smartlegionlab/smartpasslib-csharp)](https://github.com/smartlegionlab/smartpasslib-csharp)
[![GitHub license](https://img.shields.io/github/license/smartlegionlab/smartpasslib-csharp)](https://github.com/smartlegionlab/smartpasslib-csharp/blob/master/LICENSE)
[![GitHub release](https://img.shields.io/github/v/release/smartlegionlab/smartpasslib-csharp)](https://github.com/smartlegionlab/smartpasslib-csharp/)
[![GitHub stars](https://img.shields.io/github/stars/smartlegionlab/smartpasslib-csharp?style=social)](https://github.com/smartlegionlab/smartpasslib-csharp/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/smartlegionlab/smartpasslib-csharp?style=social)](https://github.com/smartlegionlab/smartpasslib-csharp/network/members)

---

## ⚠️ Disclaimer

**By using this software, you agree to the full disclaimer terms.**

**Summary:** Software provided "AS IS" without warranty. You assume all risks.

**Full legal disclaimer:** See [DISCLAIMER.md](https://github.com/smartlegionlab/smartpasslib-csharp/blob/master/DISCLAIMER.md)

---

## Core Principles

- **Deterministic Generation**: Same secret + same length = same password, every time
- **Zero Storage**: Passwords exist only when generated, never stored
- **Cross-Platform**: Compatible with Python, JS, Kotlin, Go implementations
- **.NET Standard 2.0+**: Works on .NET Framework, .NET Core, .NET 5+, Xamarin, Unity

## Key Features

- **Smart Password Generation**: Deterministic from secret phrase
- **Public/Private Key System**: 30 iterations for private key, 60 for public key
- **Secret Verification**: Verify secret without exposing it
- **Random Password Generation**: Cryptographically secure random passwords
- **Authentication Codes**: Short codes for 2FA/MFA (4-20 chars)
- **No External Dependencies**: Uses only System.Security.Cryptography

## Security Model

- **Proof of Knowledge**: Public keys verify secrets without exposing them
- **Deterministic Certainty**: Mathematical certainty in password regeneration
- **Ephemeral Passwords**: Passwords exist only in memory during generation
- **Local Computation**: No data leaves your device
- **No Recovery Backdoors**: Lost secret = permanently lost passwords (by design)

---

## Research Paradigms & Publications

- **[Pointer-Based Security Paradigm](https://doi.org/10.5281/zenodo.17204738)** - Architectural Shift from Data Protection to Data Non-Existence
- **[Local Data Regeneration Paradigm](https://doi.org/10.5281/zenodo.17264327)** - Ontological Shift from Data Transmission to Synchronous State Discovery

---

## Technical Foundation

**Key derivation (same as Python/JS/Kotlin/Go versions):**

| Key Type    | Iterations | Purpose                                               |
|-------------|------------|-------------------------------------------------------|
| Private Key | 30         | Password generation (never stored, never transmitted) |
| Public Key  | 60         | Verification (stored on server)                       |

**Character Set:** `abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$&*-_`

## Installation

### Manual
Copy `SmartPasswordLib.cs` to your project.

### Or build your own DLL
```bash
dotnet build -c Release
```

## Quick Usage

### Generate Smart Password
```csharp
using SmartLegionLab.SmartPasswordLib;

var secret = "MyCatHippo2026";
var length = 16;

var password = SmartPasswordGenerator.GenerateSmartPassword(secret, length);
Console.WriteLine(password); // e.g., "jrh_E5V!2#neNjnP"
```

### Generate Public/Private Keys
```csharp
var secret = "MyCatHippo2026";

var publicKey = SmartPasswordGenerator.GeneratePublicKey(secret);
var privateKey = SmartPasswordGenerator.GeneratePrivateKey(secret);

Console.WriteLine($"Public Key (store on server): {publicKey}");
Console.WriteLine($"Private Key (never store): {privateKey}");
```

### Verify Secret Against Public Key
```csharp
var secret = "MyCatHippo2026";
var storedPublicKey = "..."; // from server

var isValid = SmartPasswordGenerator.VerifySecret(secret, storedPublicKey);
if (isValid)
{
    var password = SmartPasswordGenerator.GenerateSmartPassword(secret, 16);
}
```

### Generate Random Passwords
```csharp
// Strong random (cryptographically secure)
var strong = SmartPasswordGenerator.GenerateStrongPassword(20);

// Base random
var basePwd = SmartPasswordGenerator.GenerateBasePassword(16);

// Authentication code (4-20 chars)
var code = SmartPasswordGenerator.GenerateCode(8);
```

### Metadata Manager
```csharp
var manager = new SmartPasswordManager();
var smartPassword = new SmartPassword(publicKey, "GitHub Account", 16);
manager.AddSmartPassword(smartPassword);

var retrieved = manager.GetSmartPassword(publicKey);
manager.DeleteSmartPassword(publicKey);
```

## API Reference

### Constants

| Constant  | Type   | Description                       |
|-----------|--------|-----------------------------------|
| `Version` | string | Library version                   |
| `Chars`   | string | Character set used for generation |

### Methods (SmartPasswordGenerator)

| Method                                      | Parameters        | Returns | Description                      |
|---------------------------------------------|-------------------|---------|----------------------------------|
| `GeneratePrivateKey(secret)`                | secret: string    | string  | Private key (30 iterations)      |
| `GeneratePublicKey(secret)`                 | secret: string    | string  | Public key (60 iterations)       |
| `VerifySecret(secret, publicKey)`           | secret, publicKey | bool    | Verify secret matches public key |
| `GenerateSmartPassword(secret, length)`     | secret, length    | string  | Deterministic password           |
| `GenerateStrongPassword(length)`            | length            | string  | Cryptographically random         |
| `GenerateBasePassword(length)`              | length            | string  | Simple random password           |
| `GenerateCode(length)`                      | length            | string  | Short code (4-20 chars)          |

### Classes

| Class                  | Description                                          |
|------------------------|------------------------------------------------------|
| `SmartPassword`        | Metadata container (PublicKey, Description, Length)  |
| `SmartPasswordManager` | Persistent storage for password metadata (JSON file) |

### Input Validation

| Parameter       | Minimum  | Maximum    |
|-----------------|----------|------------|
| Secret phrase   | 12 chars | unlimited  |
| Password length | 12 chars | 1000 chars |
| Code length     | 4 chars  | 20 chars   |

## Security Requirements

### Secret Phrase
- **Minimum 12 characters** (enforced)
- Case-sensitive
- Use mix of: uppercase, lowercase, numbers, symbols, emoji, or Cyrillic
- Never store digitally
- **NEVER use your password description as secret phrase**

### Strong Secret Examples
```
✅ "MyCatHippo2026"          — mixed case + numbers
✅ "P@ssw0rd!LongSecret"     — special chars + numbers + length
✅ "КотБегемот2026НаДиете"   — Cyrillic + numbers
✅ "GitHubPersonal2026!"     — description + extra chars (not description alone)
```

### Weak Secret Examples (avoid)
```
❌ "GitHub Account"          — using description as secret (weak!)
❌ "password"                — dictionary word, too short
❌ "1234567890"              — only digits, too short
❌ "qwerty123"               — keyboard pattern
❌ Same as description       — never use the same value as password description
```

## Cross-Platform Compatibility

SmartPasswordLib C# produces **identical passwords** to:

| Platform   | Repository                                                                   |
|------------|------------------------------------------------------------------------------|
| Python     | [smartpasslib](https://github.com/smartlegionlab/smartpasslib)               |
| JavaScript | [smartpasslib-js](https://github.com/smartlegionlab/smartpasslib-js)         |
| Kotlin     | [smartpasslib-kotlin](https://github.com/smartlegionlab/smartpasslib-kotlin) |
| Go         | [smartpasslib-go](https://github.com/smartlegionlab/smartpasslib-go)         |
| C#         | [smartpasslib-csharp](https://github.com/smartlegionlab/smartpasslib-csharp) |

## Testing

### Linux / macOS / Windows

```bash
# Clone repository
git clone https://github.com/smartlegionlab/smartpasslib-csharp.git
cd smartpasslib-csharp

# Arch Linux
sudo pacman -S dotnet-sdk

# Run tests

# Smart Password Manager
dotnet run

# Tests
dotnet run -- --test
```

### Expected output

# Tests

```text
========================================
SMART PASSWORD LIBRARY C#
========================================

RUNNING TESTS...


[SMART PASSWORD (Deterministic)]
------------------------------------------------------------
  Secret phrase: MyCatHippo2026
  Password length: 16
  Generated password: jrh_E5V!2#neNjnP

  PASS: Smart password length is 16
  Same secret -> same password: jrh_E5V!2#neNjnP4ffR
  PASS: Determinism (same secret -> same password)
  Secret A -> PSJGMgGnOfgRg9Sd
  Secret B -> IBJ0XgDnjRjB!kG3
  PASS: Different secrets -> different passwords
  Length 12: jrh_E5V!2#ne
  Length 24: jrh_E5V!2#neNjnP4ffRkWcv
  PASS: Different lengths -> different passwords

[PUBLIC & PRIVATE KEYS]
------------------------------------------------------------

  Public key (60 iterations) - STORE ON SERVER:
  cca9f8eeb9a245ca805883f4cee7ca059c44bfd17e837948f058a053d9f70ebe

  Private key (30 iterations) - NEVER STORE:
  7035d0e549e3342171052e7b00264b78629dfff256ad3fa118c075edd53d454e

  PASS: Public key != Private key
  Verification with correct secret: True
  Verification with wrong secret: False
  PASS: Correct secret verification
  PASS: Wrong secret verification

[RANDOM PASSWORD GENERATORS]
------------------------------------------------------------
  Strong random (crypto secure): vl3S4KLuL!-FqbQx
  Base random:                   ead7FrYLxcAvSO@N
  Auth code (2FA):               G70ufGk5

  PASS: Strong random length
  PASS: Base random length
  PASS: Auth code length

[INPUT VALIDATION]
------------------------------------------------------------
  Secret too short: rejected
  PASS: Secret shorter than 12 chars rejected
  Code too short: rejected
  PASS: Code shorter than 4 chars rejected
  Password too short: rejected
  PASS: Password shorter than 12 chars rejected
  PASS: Password uses only allowed characters

[MANAGER TESTS (CRUD Operations)]
------------------------------------------------------------
  Test file: /tmp/smartpass_test_9ccdd5d9-ff15-4dbc-8108-cedd4dac3288.json

  PASS: CREATE: Add new password
  Added: Test Account (16 chars)
  PASS: READ: Get password by public key
  Retrieved: Test Account
  PASS: UPDATE: Update description and length
  Updated: Updated Account (24 chars)
  PASS: DELETE: Remove password
  Deleted: removed
  Before clear: 2 passwords
  PASS: CLEAR: Remove all passwords
  After clear: 0 passwords
  Test file cleaned up

[MANAGER PERSISTENCE (Save/Load)]
------------------------------------------------------------
  Saved: 2 password(s)
  File: /tmp/smartpass_persist_22703d0d-d063-427b-b779-5a2b6d1c17ba.json
  File exists: True
  Loaded: 2 password(s)
  PASS: PERSISTENCE: Data survives file reload
  Test file cleaned up

[CROSS-PLATFORM COMPATIBILITY]
------------------------------------------------------------
  Secret: MyCatHippo2026
  Expected password (Python/JS/Kotlin/Go): jrh_E5V!2#neNjnP
  Actual password (C#):                   jrh_E5V!2#neNjnP
  PASS: Cross-platform compatibility (same as Python/JS/Kotlin/Go)

============================================================
RESULTS: 21 passed, 0 failed (total: 21)
============================================================

ALL TESTS PASSED! Library is ready for production.

```

# Manager

```text
========================================
SMART PASSWORD LIBRARY C#
========================================

Config file: /home/user/.config/smart_password_manager/passwords.json
Total passwords: 3

Your existing passwords:
  - Account 1 (100 chars)
  - Account 2 (100 chars)
  - MyCatHippo2026 (16 chars)


MENU:
  1. Add new password
  2. Get password by description
  3. Show all passwords
  0. Exit
Choose:
```

## Ecosystem

**Core Libraries:**
- **[smartpasslib](https://github.com/smartlegionlab/smartpasslib)** - Python
- **[smartpasslib-js](https://github.com/smartlegionlab/smartpasslib-js)** - JavaScript
- **[smartpasslib-kotlin](https://github.com/smartlegionlab/smartpasslib-kotlin)** - Kotlin
- **[smartpasslib-go](https://github.com/smartlegionlab/smartpasslib-go)** - Go
- **[smartpasslib-csharp](https://github.com/smartlegionlab/smartpasslib-csharp)** - C# (this)

**CLI Applications:**
- **[CLI PassMan (Python)](https://github.com/smartlegionlab/clipassman)**
- **[CLI PassGen (Python)](https://github.com/smartlegionlab/clipassgen)**
- **[CLI Manager (C#)](https://github.com/smartlegionlab/SmartPasswordManagerCsharpCli)**
- **[CLI Generator (C#)](https://github.com/smartlegionlab/SmartPasswordGeneratorCsharpCli)** 

**Desktop Applications:**
- **[Desktop Manager (Python)](https://github.com/smartlegionlab/smart-password-manager-desktop)**
- **[Desktop Manager (C#)](https://github.com/smartlegionlab/SmartPasswordManagerCsharpDesktop)**

**Other:**
- **[Web Manager](https://github.com/smartlegionlab/smart-password-manager-web)**
- **[Android Manager](https://github.com/smartlegionlab/smart-password-manager-android)**

## License

**[BSD 3-Clause License](LICENSE)**

Copyright (©) 2026, [Alexander Suvorov](https://github.com/smartlegionlab)

## Author

**Alexander Suvorov** - [GitHub](https://github.com/smartlegionlab)

---

## Support

- **Issues**: [GitHub Issues](https://github.com/smartlegionlab/smartpasslib-csharp/issues)
- **Documentation**: This README

---

