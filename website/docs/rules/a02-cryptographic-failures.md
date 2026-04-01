---
sidebar_position: 2
---

# A02 — Cryptographic Failures

Cryptographic failures cover weak algorithms, insecure configurations, hardcoded secrets, and disabled security checks. These rules detect the most common cryptographic mistakes in .NET code.

## OWASPA02001 — Weak hashing algorithm

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A02 Cryptographic Failures |

### What it detects

Use of MD5 or SHA1 via `MD5.Create()`, `SHA1.Create()`, `MD5.HashData()`, or `SHA1.HashData()`.

### Why it matters

MD5 and SHA1 are cryptographically broken and must not be used for password hashing, digital signatures, or integrity verification. They are trivially reversible via rainbow tables and vulnerable to collision attacks.

### ❌ Non-compliant

```csharp
using var md5 = MD5.Create();
byte[] hash = md5.ComputeHash(data);
```

### ✅ Compliant

```csharp
byte[] hash = SHA256.HashData(data);
// or for passwords:
string hashed = BCrypt.HashPassword(password);
```

---

## OWASPA02002 — ECB block cipher mode

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A02 Cryptographic Failures |

### What it detects

Symmetric ciphers (AES, DES, 3DES) configured with `CipherMode.ECB`.

### Why it matters

ECB mode encrypts each block independently with the same key. Identical plaintext blocks produce identical ciphertext blocks, leaking structural information about the plaintext. The "ECB penguin" attack demonstrates that ECB-encrypted images reveal their content.

### ❌ Non-compliant

```csharp
using var aes = Aes.Create();
aes.Mode = CipherMode.ECB;
```

### ✅ Compliant

```csharp
using var aes = Aes.Create();
aes.Mode = CipherMode.CBC;  // or GCM via AesGcm
aes.GenerateIV();
```

---

## OWASPA02003 — System.Random for security purposes

| Property | Value |
|----------|-------|
| **Severity** | Info |
| **Category** | A02 Cryptographic Failures |

### What it detects

Instantiation of `System.Random` or `new Random()` in code paths that may be used for security-sensitive values (tokens, salts, keys).

### Why it matters

`System.Random` is a pseudo-random number generator seeded with the system clock — it is not cryptographically secure. An attacker who can observe or guess the seed can predict all generated values.

### ❌ Non-compliant

```csharp
var rng = new Random();
string token = rng.Next().ToString();
```

### ✅ Compliant

```csharp
byte[] token = RandomNumberGenerator.GetBytes(32);
string tokenHex = Convert.ToHexString(token);
```

---

## OWASPA02004 — Hardcoded cryptographic key or IV

| Property | Value |
|----------|-------|
| **Severity** | Error |
| **Category** | A02 Cryptographic Failures |

### What it detects

Hardcoded byte arrays or strings assigned to properties named `Key` or `IV` on symmetric cipher objects.

### Why it matters

Hardcoded keys are discoverable by anyone with access to the binary or source code. A compromised key invalidates all encrypted data.

### ❌ Non-compliant

```csharp
using var aes = Aes.Create();
aes.Key = new byte[] { 0x00, 0x01, 0x02, /* ... */ 0x0f };
aes.IV  = new byte[] { 0x10, 0x11, /* ... */ };
```

### ✅ Compliant

```csharp
using var aes = Aes.Create();
aes.GenerateKey();
aes.GenerateIV();
```

Or load from a secure configuration source (environment variable, Azure Key Vault, etc.).

---

## OWASPA02005 — Legacy TLS protocol version

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A02 Cryptographic Failures |

### What it detects

Use of `SslProtocols.Ssl2`, `SslProtocols.Ssl3`, `SslProtocols.Tls` (TLS 1.0), or `SslProtocols.Tls11` (TLS 1.1).

### Why it matters

SSL 2/3, TLS 1.0, and TLS 1.1 have known vulnerabilities (POODLE, BEAST, etc.) and have been deprecated by major browsers and RFCs. They must not be used in production.

### ❌ Non-compliant

```csharp
var handler = new HttpClientHandler
{
    SslProtocols = SslProtocols.Tls | SslProtocols.Tls11
};
```

### ✅ Compliant

```csharp
var handler = new HttpClientHandler
{
    SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13
};
```

---

## OWASPA02006 — Certificate validation disabled

| Property | Value |
|----------|-------|
| **Severity** | Error |
| **Category** | A02 Cryptographic Failures |

### What it detects

`ServerCertificateCustomValidationCallback` set to a lambda that always returns `true`, or `ServicePointManager.ServerCertificateValidationCallback` assigned a delegate that ignores errors.

### Why it matters

Disabling certificate validation makes TLS meaningless — any certificate is accepted, enabling trivial man-in-the-middle attacks.

### ❌ Non-compliant

```csharp
var handler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (_, _, _, _) => true
};
```

### ✅ Compliant

```csharp
// Use the default handler — it validates certificates properly
var client = new HttpClient();
```

If you need to trust a custom CA, add it to the system certificate store rather than bypassing validation entirely.

---

## OWASPA02007 — HTTP URL (not HTTPS)

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A02 Cryptographic Failures |

### What it detects

String literals starting with `http://` used as URL arguments to `HttpClient`, `WebClient`, or `HttpWebRequest`.

### Why it matters

Plain HTTP transmits data in cleartext. Even if the server redirects to HTTPS, the initial request is unencrypted and the redirect itself can be intercepted (SSL stripping).

### ❌ Non-compliant

```csharp
var response = await client.GetAsync("http://api.example.com/data");
```

### ✅ Compliant

```csharp
var response = await client.GetAsync("https://api.example.com/data");
```

---

## OWASPA02008 — Missing HSTS configuration

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A02 Cryptographic Failures |

### What it detects

ASP.NET Core applications that call `app.UseHttpsRedirection()` without also calling `app.UseHsts()` in the middleware pipeline.

### Why it matters

HTTPS redirection alone doesn't prevent the first request from being sent over HTTP. HSTS instructs browsers to always use HTTPS for a domain, preventing protocol downgrade attacks.

### ❌ Non-compliant

```csharp
app.UseHttpsRedirection();
// UseHsts() missing
app.UseRouting();
```

### ✅ Compliant

```csharp
app.UseHsts();
app.UseHttpsRedirection();
app.UseRouting();
```
