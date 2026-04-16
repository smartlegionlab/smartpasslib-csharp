using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SmartLegionLab.SmartPasswordLib;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine();
        Console.WriteLine("========================================");
        Console.WriteLine("SMART PASSWORD LIBRARY C#");
        Console.WriteLine("========================================");
        Console.WriteLine();

        if (args.Length > 0 && args[0] == "--test")
        {
            RunTests();
        }
        else
        {
            RunManager();
        }
    }

    static void RunTests()
    {
        Console.WriteLine("RUNNING TESTS...");
        Console.WriteLine();

        var passed = 0;
        var failed = 0;
        var totalTests = 0;

        void Test(string name, Action test)
        {
            totalTests++;
            try
            {
                test();
                Console.WriteLine($"  PASS: {name}");
                passed++;
            }
            catch (Exception e)
            {
                Console.WriteLine($"  FAIL: {name} - {e.Message}");
                failed++;
            }
        }

        void Section(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"[{title}]");
            Console.WriteLine("------------------------------------------------------------");
        }

        var secret = "MyCatHippo2026";
        var wrongSecret = "WrongSecret123456";

        Section("SMART PASSWORD (Deterministic)");
        var password = SmartPasswordGenerator.GenerateSmartPassword(secret, 16);
        Console.WriteLine($"  Secret phrase: {secret}");
        Console.WriteLine($"  Password length: 16");
        Console.WriteLine($"  Generated password: {password}");
        Console.WriteLine();
        Test("Smart password length is 16", () =>
        {
            if (password.Length != 16) throw new Exception($"Expected 16, got {password.Length}");
        });

        var pwd1 = SmartPasswordGenerator.GenerateSmartPassword(secret, 20);
        var pwd2 = SmartPasswordGenerator.GenerateSmartPassword(secret, 20);
        Console.WriteLine($"  Same secret -> same password: {pwd1}");
        Test("Determinism (same secret -> same password)", () =>
        {
            if (pwd1 != pwd2) throw new Exception("Passwords don't match");
        });

        var diff1 = SmartPasswordGenerator.GenerateSmartPassword("SecretOne123456", 16);
        var diff2 = SmartPasswordGenerator.GenerateSmartPassword("SecretTwo123456", 16);
        Console.WriteLine($"  Secret A -> {diff1}");
        Console.WriteLine($"  Secret B -> {diff2}");
        Test("Different secrets -> different passwords", () =>
        {
            if (diff1 == diff2) throw new Exception("Passwords are the same");
        });

        var shortPwd = SmartPasswordGenerator.GenerateSmartPassword(secret, 12);
        var longPwd = SmartPasswordGenerator.GenerateSmartPassword(secret, 24);
        Console.WriteLine($"  Length 12: {shortPwd}");
        Console.WriteLine($"  Length 24: {longPwd}");
        Test("Different lengths -> different passwords", () =>
        {
            if (shortPwd.Length != 12 || longPwd.Length != 24) throw new Exception("Length mismatch");
        });

        Section("PUBLIC & PRIVATE KEYS");
        var pubKey = SmartPasswordGenerator.GeneratePublicKey(secret);
        var privKey = SmartPasswordGenerator.GeneratePrivateKey(secret);
        Console.WriteLine();
        Console.WriteLine($"  Public key (60 iterations) - STORE ON SERVER:");
        Console.WriteLine($"  {pubKey}");
        Console.WriteLine();
        Console.WriteLine($"  Private key (30 iterations) - NEVER STORE:");
        Console.WriteLine($"  {privKey}");
        Console.WriteLine();
        Test("Public key != Private key", () =>
        {
            if (pubKey == privKey) throw new Exception("Keys are the same");
        });

        var isValid = SmartPasswordGenerator.VerifySecret(secret, pubKey);
        var isInvalid = SmartPasswordGenerator.VerifySecret(wrongSecret, pubKey);
        Console.WriteLine($"  Verification with correct secret: {isValid}");
        Console.WriteLine($"  Verification with wrong secret: {isInvalid}");
        Test("Correct secret verification", () =>
        {
            if (!isValid) throw new Exception("Correct secret failed");
        });
        Test("Wrong secret verification", () =>
        {
            if (isInvalid) throw new Exception("Wrong secret passed");
        });

        Section("RANDOM PASSWORD GENERATORS");
        var strong = SmartPasswordGenerator.GenerateStrongPassword(16);
        var basePwd = SmartPasswordGenerator.GenerateBasePassword(16);
        var code = SmartPasswordGenerator.GenerateCode(8);
        Console.WriteLine($"  Strong random (crypto secure): {strong}");
        Console.WriteLine($"  Base random:                   {basePwd}");
        Console.WriteLine($"  Auth code (2FA):               {code}");
        Console.WriteLine();
        Test("Strong random length", () =>
        {
            if (strong.Length != 16) throw new Exception($"Expected 16, got {strong.Length}");
        });
        Test("Base random length", () =>
        {
            if (basePwd.Length != 16) throw new Exception($"Expected 16, got {basePwd.Length}");
        });
        Test("Auth code length", () =>
        {
            if (code.Length != 8) throw new Exception($"Expected 8, got {code.Length}");
        });

        Section("INPUT VALIDATION");
        var exceptionCaught = false;
        try { SmartPasswordGenerator.GenerateSmartPassword("short", 16); }
        catch (ArgumentException) { exceptionCaught = true; }
        Console.WriteLine($"  Secret too short: {(exceptionCaught ? "rejected" : "not rejected")}");
        Test("Secret shorter than 12 chars rejected", () =>
        {
            if (!exceptionCaught) throw new Exception("Should reject short secret");
        });

        exceptionCaught = false;
        try { SmartPasswordGenerator.GenerateCode(2); }
        catch (ArgumentException) { exceptionCaught = true; }
        Console.WriteLine($"  Code too short: {(exceptionCaught ? "rejected" : "not rejected")}");
        Test("Code shorter than 4 chars rejected", () =>
        {
            if (!exceptionCaught) throw new Exception("Should reject short code");
        });

        exceptionCaught = false;
        try { SmartPasswordGenerator.GenerateStrongPassword(5); }
        catch (ArgumentException) { exceptionCaught = true; }
        Console.WriteLine($"  Password too short: {(exceptionCaught ? "rejected" : "not rejected")}");
        Test("Password shorter than 12 chars rejected", () =>
        {
            if (!exceptionCaught) throw new Exception("Should reject short password");
        });

        var allCharsOk = password.All(c => SmartPasswordGenerator.Chars.Contains(c));
        Test("Password uses only allowed characters", () =>
        {
            if (!allCharsOk) throw new Exception("Invalid characters in password");
        });

        Section("MANAGER TESTS (CRUD Operations)");
        var testFile = Path.Combine(Path.GetTempPath(), $"smartpass_test_{Guid.NewGuid()}.json");
        var manager = new SmartPasswordManager(testFile);
        Console.WriteLine($"  Test file: {testFile}");
        Console.WriteLine();

        var testSecret = "TestSecret123456";
        var testPubKey = SmartPasswordGenerator.GeneratePublicKey(testSecret);
        var testPwd = new SmartPassword(testPubKey, "Test Account", 16);
        manager.AddSmartPassword(testPwd);
        var retrieved = manager.GetSmartPassword(testPubKey);
        Test("CREATE: Add new password", () =>
        {
            if (retrieved == null) throw new Exception("Password not found after add");
            if (retrieved.Description != "Test Account") throw new Exception("Description mismatch");
            if (retrieved.Length != 16) throw new Exception("Length mismatch");
        });
        Console.WriteLine($"  Added: {retrieved?.Description} ({retrieved?.Length} chars)");

        var samePwd = manager.GetSmartPassword(testPubKey);
        Test("READ: Get password by public key", () =>
        {
            if (samePwd == null) throw new Exception("Password not found");
            if (samePwd.PublicKey != testPubKey) throw new Exception("Public key mismatch");
        });
        Console.WriteLine($"  Retrieved: {samePwd?.Description}");

        var updated = manager.UpdateSmartPassword(testPubKey, "Updated Account", 24);
        var updatedPwd = manager.GetSmartPassword(testPubKey);
        Test("UPDATE: Update description and length", () =>
        {
            if (!updated) throw new Exception("Update returned false");
            if (updatedPwd.Description != "Updated Account") throw new Exception("Description not updated");
            if (updatedPwd.Length != 24) throw new Exception("Length not updated");
        });
        Console.WriteLine($"  Updated: {updatedPwd?.Description} ({updatedPwd?.Length} chars)");

        var deleted = manager.DeleteSmartPassword(testPubKey);
        var afterDelete = manager.GetSmartPassword(testPubKey);
        Test("DELETE: Remove password", () =>
        {
            if (!deleted) throw new Exception("Delete returned false");
            if (afterDelete != null) throw new Exception("Password still exists after delete");
        });
        Console.WriteLine($"  Deleted: {(afterDelete == null ? "removed" : "failed")}");

        manager.AddSmartPassword(new SmartPassword("key1", "Test 1", 16));
        manager.AddSmartPassword(new SmartPassword("key2", "Test 2", 16));
        Console.WriteLine($"  Before clear: {manager.PasswordCount} passwords");
        manager.Clear();
        Test("CLEAR: Remove all passwords", () =>
        {
            if (manager.PasswordCount != 0) throw new Exception($"Expected 0, got {manager.PasswordCount}");
        });
        Console.WriteLine($"  After clear: {manager.PasswordCount} passwords");
        if (File.Exists(testFile)) File.Delete(testFile);
        Console.WriteLine($"  Test file cleaned up");

        Section("MANAGER PERSISTENCE (Save/Load)");
        var persistFile = Path.Combine(Path.GetTempPath(), $"smartpass_persist_{Guid.NewGuid()}.json");
        var manager1 = new SmartPasswordManager(persistFile);
        var persistSecret1 = "PersistTest123456";
        var persistPubKey1 = SmartPasswordGenerator.GeneratePublicKey(persistSecret1);
        manager1.AddSmartPassword(new SmartPassword(persistPubKey1, "Persist Account 1", 20));
        var persistSecret2 = "PersistTest789012";
        var persistPubKey2 = SmartPasswordGenerator.GeneratePublicKey(persistSecret2);
        manager1.AddSmartPassword(new SmartPassword(persistPubKey2, "Persist Account 2", 24));
        Console.WriteLine($"  Saved: {manager1.PasswordCount} password(s)");
        Console.WriteLine($"  File: {persistFile}");
        Console.WriteLine($"  File exists: {File.Exists(persistFile)}");
        System.Threading.Thread.Sleep(100);
        var manager2 = new SmartPasswordManager(persistFile);
        Console.WriteLine($"  Loaded: {manager2.PasswordCount} password(s)");
        Test("PERSISTENCE: Data survives file reload", () =>
        {
            if (manager2.PasswordCount != 2) throw new Exception($"Expected 2, got {manager2.PasswordCount}");
            var loaded1 = manager2.GetSmartPassword(persistPubKey1);
            if (loaded1 == null) throw new Exception("Password 1 not found after reload");
            if (loaded1.Description != "Persist Account 1") throw new Exception("Description 1 mismatch");
            if (loaded1.Length != 20) throw new Exception("Length 1 mismatch");
            var loaded2 = manager2.GetSmartPassword(persistPubKey2);
            if (loaded2 == null) throw new Exception("Password 2 not found after reload");
            if (loaded2.Description != "Persist Account 2") throw new Exception("Description 2 mismatch");
            if (loaded2.Length != 24) throw new Exception("Length 2 mismatch");
        });
        if (File.Exists(persistFile)) File.Delete(persistFile);
        Console.WriteLine($"  Test file cleaned up");

        Section("CROSS-PLATFORM COMPATIBILITY");
        var crossSecret = "MyCatHippo2026";
        var crossPassword = SmartPasswordGenerator.GenerateSmartPassword(crossSecret, 16);
        var expectedPassword = "jrh_E5V!2#neNjnP";
        Console.WriteLine($"  Secret: {crossSecret}");
        Console.WriteLine($"  Expected password (Python/JS/Kotlin/Go): {expectedPassword}");
        Console.WriteLine($"  Actual password (C#):                   {crossPassword}");
        Test("Cross-platform compatibility (same as Python/JS/Kotlin/Go)", () =>
        {
            if (crossPassword != expectedPassword)
                throw new Exception($"Expected '{expectedPassword}', got '{crossPassword}'");
        });

        Console.WriteLine();
        Console.WriteLine("============================================================");
        Console.WriteLine($"RESULTS: {passed} passed, {failed} failed (total: {totalTests})");
        Console.WriteLine("============================================================");
        if (failed == 0)
        {
            Console.WriteLine();
            Console.WriteLine("ALL TESTS PASSED! Library is ready for production.");
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine("SOME TESTS FAILED! Please fix.");
            Console.WriteLine();
        }
    }

    static void RunManager()
    {
        var manager = new SmartPasswordManager();

        Console.WriteLine($"Config file: {manager.FilePath}");
        Console.WriteLine($"Total passwords: {manager.PasswordCount}");
        Console.WriteLine();

        if (manager.PasswordCount > 0)
        {
            Console.WriteLine("Your existing passwords:");
            foreach (var pwd in manager.Passwords.Values)
            {
                Console.WriteLine($"  - {pwd.Description} ({pwd.Length} chars)");
            }
            Console.WriteLine();
        }

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("MENU:");
            Console.WriteLine("  1. Add new password");
            Console.WriteLine("  2. Get password by description");
            Console.WriteLine("  3. Update password description or length");
            Console.WriteLine("  4. Delete password");
            Console.WriteLine("  5. Show all passwords");
            Console.WriteLine("  0. Exit");
            Console.Write("Choose: ");

            var choice = Console.ReadLine();
            if (choice == "0") break;
            if (choice == "1") AddPassword(manager);
            else if (choice == "2") GetPassword(manager);
            else if (choice == "3") UpdatePassword(manager);
            else if (choice == "4") DeletePassword(manager);
            else if (choice == "5") ShowAllPasswords(manager);
            else Console.WriteLine("Invalid choice");
        }
    }

    static void AddPassword(SmartPasswordManager manager)
    {
        Console.WriteLine();
        Console.WriteLine("--- ADD NEW PASSWORD ---");
        Console.Write("Description: ");
        var desc = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(desc))
        {
            Console.WriteLine("Description cannot be empty");
            return;
        }

        Console.Write("Secret phrase: ");
        var secret = ReadSecretHidden();

        if (string.IsNullOrEmpty(secret))
        {
            Console.WriteLine("Secret phrase cannot be empty");
            return;
        }

        if (secret.Length < 12)
        {
            Console.WriteLine($"Secret phrase too short! ({secret.Length} chars, need 12+)");
            return;
        }

        Console.Write("Length (default 16, min 12, max 100): ");
        var lengthInput = Console.ReadLine();
        var length = string.IsNullOrEmpty(lengthInput) ? 16 : int.TryParse(lengthInput, out var l) ? l : 16;
        if (length < 12) length = 12;
        if (length > 100) length = 100;

        var pubKey = SmartPasswordGenerator.GeneratePublicKey(secret);
        if (manager.GetSmartPassword(pubKey) != null)
        {
            Console.WriteLine("Password with this secret phrase already exists!");
            return;
        }

        var newPwd = new SmartPassword(pubKey, desc, length);
        manager.AddSmartPassword(newPwd);
        var password = SmartPasswordGenerator.GenerateSmartPassword(secret, length);

        Console.WriteLine();
        Console.WriteLine("Password added successfully!");
        Console.WriteLine($"   Description: {desc}");
        Console.WriteLine($"   Length: {length}");
        Console.WriteLine($"   Generated password: {password}");
        Console.WriteLine();
        Console.WriteLine("IMPORTANT: Save this password now! It won't be shown again.");
    }

    static void GetPassword(SmartPasswordManager manager)
    {
        if (manager.PasswordCount == 0)
        {
            Console.WriteLine("No passwords found. Add one first.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("--- GET PASSWORD ---");
        Console.WriteLine("Available passwords:");
        var index = 1;
        var list = new List<SmartPassword>();
        foreach (var pwd in manager.Passwords.Values)
        {
            Console.WriteLine($"  {index}. {pwd.Description} ({pwd.Length} chars)");
            list.Add(pwd);
            index++;
        }

        Console.Write("Select number: ");
        if (!int.TryParse(Console.ReadLine(), out var selected) || selected < 1 || selected > list.Count)
        {
            Console.WriteLine("Invalid selection");
            return;
        }

        var selectedPwd = list[selected - 1];
        Console.WriteLine();
        Console.WriteLine($"Description: {selectedPwd.Description}");
        Console.WriteLine($"Length: {selectedPwd.Length} chars");
        Console.Write("Enter your secret phrase: ");
        var secret = ReadSecretHidden();

        if (string.IsNullOrEmpty(secret))
        {
            Console.WriteLine();
            Console.WriteLine("Secret phrase cannot be empty");
            return;
        }

        try
        {
            var isValid = SmartPasswordGenerator.VerifySecret(secret, selectedPwd.PublicKey);
            if (!isValid)
            {
                Console.WriteLine();
                Console.WriteLine("Invalid secret phrase! Password cannot be retrieved.");
                return;
            }

            var password = SmartPasswordGenerator.GenerateSmartPassword(secret, selectedPwd.Length);
            Console.WriteLine();
            Console.WriteLine("Secret verified!");
            Console.WriteLine($"   Generated password: {password}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine();
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static void UpdatePassword(SmartPasswordManager manager)
    {
        if (manager.PasswordCount == 0)
        {
            Console.WriteLine("No passwords found. Add one first.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("--- UPDATE PASSWORD ---");
        Console.WriteLine("Available passwords:");
        var index = 1;
        var list = new List<SmartPassword>();
        foreach (var pwd in manager.Passwords.Values)
        {
            Console.WriteLine($"  {index}. {pwd.Description} ({pwd.Length} chars)");
            list.Add(pwd);
            index++;
        }

        Console.Write("Select number to update: ");
        if (!int.TryParse(Console.ReadLine(), out var selected) || selected < 1 || selected > list.Count)
        {
            Console.WriteLine("Invalid selection");
            return;
        }

        var selectedPwd = list[selected - 1];
        Console.WriteLine();
        Console.WriteLine($"Current description: {selectedPwd.Description}");
        Console.WriteLine($"Current length: {selectedPwd.Length}");
        Console.WriteLine();

        Console.Write("New description (leave empty to keep): ");
        var newDesc = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(newDesc))
            newDesc = selectedPwd.Description;

        Console.Write("New length (leave empty to keep, min 12, max 100): ");
        var lengthInput = Console.ReadLine();
        int? newLength = null;
        if (!string.IsNullOrEmpty(lengthInput))
        {
            if (int.TryParse(lengthInput, out var l))
            {
                if (l < 12) l = 12;
                if (l > 100) l = 100;
                newLength = l;
            }
        }

        var success = manager.UpdateSmartPassword(selectedPwd.PublicKey, newDesc, newLength);
        if (success)
        {
            Console.WriteLine();
            Console.WriteLine("Password updated successfully!");
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine("Failed to update password.");
        }
    }

    static void DeletePassword(SmartPasswordManager manager)
    {
        if (manager.PasswordCount == 0)
        {
            Console.WriteLine("No passwords found. Add one first.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("--- DELETE PASSWORD ---");
        Console.WriteLine("Available passwords:");
        var index = 1;
        var list = new List<SmartPassword>();
        foreach (var pwd in manager.Passwords.Values)
        {
            Console.WriteLine($"  {index}. {pwd.Description} ({pwd.Length} chars)");
            list.Add(pwd);
            index++;
        }

        Console.Write("Select number to delete: ");
        if (!int.TryParse(Console.ReadLine(), out var selected) || selected < 1 || selected > list.Count)
        {
            Console.WriteLine("Invalid selection");
            return;
        }

        var selectedPwd = list[selected - 1];
        Console.Write($"Are you sure you want to delete '{selectedPwd.Description}'? (y/n): ");
        var confirm = Console.ReadLine();

        if (confirm?.ToLower() == "y")
        {
            var success = manager.DeleteSmartPassword(selectedPwd.PublicKey);
            if (success)
            {
                Console.WriteLine("Password deleted successfully!");
            }
            else
            {
                Console.WriteLine("Failed to delete password.");
            }
        }
        else
        {
            Console.WriteLine("Deletion cancelled.");
        }
    }

    static void ShowAllPasswords(SmartPasswordManager manager)
    {
        if (manager.PasswordCount == 0)
        {
            Console.WriteLine("No passwords found.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("--- ALL PASSWORDS (metadata only) ---");
        foreach (var pwd in manager.Passwords.Values)
        {
            Console.WriteLine($"  - {pwd.Description} ({pwd.Length} chars)");
        }
        Console.WriteLine($"Total: {manager.PasswordCount} passwords");
    }

    static string ReadSecretHidden()
    {
        var secret = "";
        ConsoleKeyInfo key;
        while (true)
        {
            key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
                break;
            if (key.Key == ConsoleKey.Backspace && secret.Length > 0)
            {
                secret = secret[0..^1];
                Console.Write("\b \b");
            }
            else if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
            {
                secret += key.KeyChar;
            }
        }
        Console.WriteLine();
        return secret;
    }
}