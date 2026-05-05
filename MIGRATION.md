# Migration Guide: v1.0.5 to v4.0.0

> **📌 Version Note:** smartpasslib-csharp jumps from v1.0.5 directly to v4.0.0 to align with Python version v4.0.0. All smartpasslib implementations (Python, C#, JS, Go, Kotlin) now share the same version number and algorithm.

## ⚠️ Breaking Change Notice

**smartpasslib-csharp v4.0.0 is NOT backward compatible with v1.0.5**

| Version    | Status      | Why                                                              |
|------------|-------------|------------------------------------------------------------------|
| v1.0.5     | Deprecated  | Fixed iterations (30/60), limited character set                  |
| **v4.0.0** | **Current** | Dynamic iterations (15-30/45-60), expanded charset, max security |

Passwords generated with v1.0.5 cannot be regenerated using v4.0.0 
due to fundamental changes in the deterministic generation algorithm.

---

## Why the change?

**smartpasslib-csharp v4.0.0 introduces fundamental improvements:**

- **Dynamic iteration counts** — deterministic steps vary per secret (15-30 for private, 45-60 for public)
- **Expanded character set** — Google-compatible symbols: `!@#$%^&*()_+-=[]{};:,.<>?/`
- **Enhanced key derivation** — salt separation for public/private keys
- **Unified length validation** — password length must be 12-100 characters
- **Input validation** — secret phrases must be at least 12 characters
- **Maximum security** — no secret exposure in logs or iterations
- **Cross-platform consistency** — identical algorithm with Python/JS/Kotlin/Go v4.0.0

---

## What changed:

| Aspect                 | v1.0.5             | v4.0.0                                |
|------------------------|--------------------|---------------------------------------|
| Private key iterations | Fixed 30           | Dynamic 15-30                         |
| Public key iterations  | Fixed 60           | Dynamic 45-60                         |
| Character set          | `abc...!@#$&*-_`   | `!@#$%^&*()_+-=[]{};:,.<>?/A-Za-z0-9` |
| Password max length    | 1000               | 100                                   |
| Secret validation      | None (min 4 chars) | Min 12 characters (enforced)          |
| Code max length        | 20                 | 100                                   |
| Key derivation salt    | None               | "private"/"public"                    |
| Secret in iterations   | Yes (exposed)      | No (secure)                           |

---

## Migration Steps from v1.0.5 to v4.0.0

### Step 1: Install old version (if still on v1.0.5)

Keep your current v1.0.5 implementation.

### Step 2: Retrieve existing passwords

For each service, generate the actual password using your secret phrase and the old version:

```csharp
using SmartLegionLab.SmartPassLib;

var oldPassword = SmartPasswordGenerator.GenerateSmartPassword("your_secret_phrase", length);
```

Keep these passwords accessible during migration.

### Step 3: Update to v4.0.0

Replace `SmartPassLib.cs` with the new v4.0.0 version.

### Step 4: Generate new passwords

Using the **same secret phrases and lengths**, generate new passwords:

```csharp
using SmartLegionLab.SmartPassLib;

var newPassword = SmartPasswordGenerator.GenerateSmartPassword("your_secret_phrase", length);
```

### Step 5: Update your services

Replace old passwords with newly generated ones on each website/service.

### Step 6: Verify

- Log in using new passwords
- Confirm regeneration works (same secret → same password)

---

## What about stored metadata?

If you have stored `passwords.json` files with public keys from v1.0.5:

**These public keys are NOT compatible** with v4.0.0 because:

- Iteration counts changed (60 → 45-60 dynamic)
- Salt "public" added to key derivation

**Solution:** You need to regenerate public keys and update your metadata storage:

```csharp
// Old v1.0.5 public key (no longer works)
var oldPublicKey = "...";

// New v4.0.0 public key
var newPublicKey = SmartPasswordGenerator.GeneratePublicKey("your_secret_phrase");
```

---

## Important Notes

- **No automatic migration** — manual regeneration required for each password
- **Your secret phrases remain the same** — only generated passwords change
- **Secret phrases shorter than 12 characters will now throw ArgumentException**
- **Password lengths between 101 and 1000 will now throw ArgumentException**
- **Old v1.0.5 code still available** in previous releases
- Test with non-essential accounts first

---

## Verification Test

After migration, verify that the same secret produces the same password on all platforms:

```csharp
// C# v4.0.0
var csharpPassword = SmartPasswordGenerator.GenerateSmartPassword("TestSecret2026!", 12);

// Python v4.0.0 (should be identical)
// python_password = SmartPasswordMaster.generate_smart_password("TestSecret2026!", 12)

// Both passwords must match exactly
```

---

## Rollback

If you need to rollback to v1.0.5:

```bash
# Revert to old version
git checkout v1.0.5
```

---

## Need Help?

- **Issues**: [GitHub Issues](https://github.com/smartlegionlab/smartpasslib-csharp/issues)
- **Core Library Issues**: [smartpasslib Issues](https://github.com/smartlegionlab/smartpasslib/issues)
- **Migration Questions**: Open an issue with tag `migration`

---

