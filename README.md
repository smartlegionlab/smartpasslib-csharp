# SmartPassLib C# <sup>v1.0.5</sup>

---

**Smart Passwords Library**: Cryptographic password generation and management without storage. 
Generate passwords from secrets, verify knowledge without exposure, manage metadata securely.

**Now with Cross-Platform Determinism**: Same secret + same parameters = identical password on 
**C#, Python, Go, Kotlin, JavaScript** and any language with SHA-256.

**Decentralized by Design**: Unlike traditional password managers that store encrypted vaults on central servers, 
smartpasslib stores nothing. Your secrets never leave your device. Passwords are regenerated on-demand — 
**no cloud, no database, no trust required**.

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

- **Zero-Storage Security**: No passwords or secret phrases are ever stored or transmitted
- **Decentralized Architecture**: No central servers, no cloud dependency, no third-party trust required
- **Cross-Platform Deterministic Generation**: Identical secret + parameters = identical password **on any language** (SHA-256 based)
- **Metadata Only**: Store only verification metadata (public keys, descriptions, lengths)
- **On-Demand Regeneration**: Passwords are recalculated when needed, never retrieved from storage
- **Cryptographically Secure**: Uses SHA-256

## Key Features

- **Decentralized & Serverless**: No central database, no cloud lock-in, complete user sovereignty
- **Smart Password Generation**: Deterministic from secret phrase
- **Public/Private Key System**: 30 iterations for private key, 60 for public key
- **Secret Verification**: Verify secret without exposing it
- **Random Password Generation**: Cryptographically secure random passwords
- **Authentication Codes**: Short codes for 2FA/MFA (4-20 chars)
- **No External Dependencies**: Uses only System.Security.Cryptography
- **Full Test Coverage**: 100% tested for reliability and security

## Security Model

- **Proof of Knowledge**: Public keys verify secrets without exposing them
- **Decentralized Trust**: No third party needed — you control your secrets completely
- **Deterministic Security**: Same input = same output, always reproducible across platforms
- **No Vulnerable Metadata Storage**: Only public keys and descriptions can be stored (optional)
- **Zero Storage of Secrets**: Secret phrases exist only in your memory, private keys are derived on-demand and never persisted
- **No Recovery Backdoors**: Lost secret = permanently lost passwords (by design)

---

## Research Paradigms & Publications

- **[Pointer-Based Security Paradigm](https://doi.org/10.5281/zenodo.17204738)** - Architectural Shift from Data Protection to Data Non-Existence
- **[Local Data Regeneration Paradigm](https://doi.org/10.5281/zenodo.17264327)** - Ontological Shift from Data Transmission to Synchronous State Discovery

---

## Technical Foundation

**Key derivation (same as Python/JS/Kotlin/Go versions):**

| Key Type    | Iterations | Purpose                                                 |
|-------------|------------|---------------------------------------------------------|
| Private Key | 30         | Password generation (never stored, never transmitted)   |
| Public Key  | 60         | Verification (stored on local)                          |

**Character Set:** `abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$&*-_`

**Decentralized Architecture**:
- No central authority required
- Metadata can be synced via any channel (USB, cloud, even paper)
- Your security depends only on your secret phrase, not on any service provider
- Works offline — no internet connection required

## Installation

### Manual
Copy `SmartPassLib.cs` to your project.

## Quick Usage

### Generate Smart Password
```csharp
using SmartLegionLab.SmartPassLib;

var secret = "MyStrongSecretPhrase2026!";
var length = 16;

var password = SmartPasswordGenerator.GenerateSmartPassword(secret, length);
Console.WriteLine(password); // e.g., "jrh_E5V!2#neNjnP"
```

### Generate Public/Private Keys
```csharp
var secret = "MyStrongSecretPhrase2026!";

var publicKey = SmartPasswordGenerator.GeneratePublicKey(secret);
var privateKey = SmartPasswordGenerator.GeneratePrivateKey(secret);

Console.WriteLine($"Public Key (store on local): {publicKey}");
Console.WriteLine($"Private Key (never store): {privateKey}");
```

### Verify Secret Against Public Key
```csharp
var secret = "MyStrongSecretPhrase2026!";
var storedPublicKey = "..."; // from local

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
✅ "MyStrongSecretPhrase2026!"   — mixed case + numbers + symbols
✅ "P@ssw0rd!LongSecret"         — special chars + numbers + length
✅ "КотБегемот2026НаДиете"       — Cyrillic + numbers
```

### Weak Secret Examples (avoid)
```
❌ "GitHub Account"              — using description as secret (weak!)
❌ "password"                    — dictionary word, too short
❌ "1234567890"                  — only digits, too short
❌ "qwerty123"                   — keyboard pattern
❌ Same as description           — never use the same value as password description
```

### Decentralized Nature

**There is no "forgot password" button.** This is by design:

- No central server can reset your passwords
- No support team can recover your access
- Your secret phrase is the ONLY key

**This is the price of true decentralization** — you are completely in control.

## Cross-Platform Implementations

The same deterministic algorithm is available in multiple languages.
SmartPassLib C# produces **identical passwords** to:

| Language   | Repository                                                                   |
|------------|:-----------------------------------------------------------------------------|
| Python     | [smartpasslib](https://github.com/smartlegionlab/smartpasslib)               |
| JavaScript | [smartpasslib-js](https://github.com/smartlegionlab/smartpasslib-js)         |
| Kotlin     | [smartpasslib-kotlin](https://github.com/smartlegionlab/smartpasslib-kotlin) |
| Go         | [smartpasslib-go](https://github.com/smartlegionlab/smartpasslib-go)         |

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
- **Documentation**: This [README](README.md)

