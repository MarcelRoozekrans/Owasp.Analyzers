---
sidebar_position: 11
---

# Configuration

## Overriding severity

Every rule's severity can be overridden in an `.editorconfig` file in your project or solution root:

```ini
[*.cs]
dotnet_diagnostic.<RuleId>.severity = <level>
```

Valid severity levels:

| Level | Effect |
|-------|--------|
| `error` | Breaks the build |
| `warning` | Shown as a warning, does not break the build |
| `suggestion` | Shown as a suggestion/hint in the IDE |
| `silent` | Runs the analyzer but hides results |
| `none` | Disables the rule entirely |

## Suppressing a single occurrence

Use `#pragma` to suppress a specific diagnostic on a line:

```csharp
#pragma warning disable OWASPA01001
[HttpGet]
public IActionResult PublicEndpoint() => Ok();
#pragma warning restore OWASPA01001
```

Or use the `[SuppressMessage]` attribute:

```csharp
[System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "OWASPA01001")]
public IActionResult PublicEndpoint() => Ok();
```

## Full rule reference

| Rule ID | Category | Default Severity | Description |
|---------|----------|-----------------|-------------|
| OWASPA01001 | A01 Broken Access Control | Warning | Controller action missing authorization attribute |
| OWASPA01002 | A01 Broken Access Control | Warning | Hardcoded role string in [Authorize] |
| OWASPA01003 | A01 Broken Access Control | Warning | IsInRole called with hardcoded string |
| OWASPA01004 | A01 Broken Access Control | Warning | CORS AllowAnyOrigin (wildcard) |
| OWASPA01005 | A01 Broken Access Control | Warning | POST/PUT/DELETE action missing antiforgery token |
| OWASPA02001 | A02 Cryptographic Failures | Warning | Weak hashing algorithm (MD5 / SHA1) |
| OWASPA02002 | A02 Cryptographic Failures | Warning | ECB cipher mode |
| OWASPA02003 | A02 Cryptographic Failures | Info | System.Random used (not cryptographically secure) |
| OWASPA02004 | A02 Cryptographic Failures | Error | Hardcoded cryptographic key or IV |
| OWASPA02005 | A02 Cryptographic Failures | Warning | Legacy TLS protocol (SSL2/3, TLS 1.0/1.1) |
| OWASPA02006 | A02 Cryptographic Failures | Error | Certificate validation disabled |
| OWASPA02007 | A02 Cryptographic Failures | Warning | HTTP URL used (not HTTPS) |
| OWASPA02008 | A02 Cryptographic Failures | Warning | HSTS not configured alongside HTTPS redirection |
| OWASPA03001 | A03 Injection | Error | SQL injection (taint) |
| OWASPA03002 | A03 Injection | Error | OS command injection (taint) |
| OWASPA03003 | A03 Injection | Error | Path traversal (taint) |
| OWASPA03004 | A03 Injection | Error | LDAP injection (taint) |
| OWASPA03005 | A03 Injection | Error | XPath injection (taint) |
| OWASPA03006 | A03 Injection | Error | XSS via unencoded output (taint) |
| OWASPA04002 | A04 Insecure Design | Warning | Missing rate limiting on authentication endpoints |
| OWASPA05001 | A05 Security Misconfiguration | Warning | Developer exception page enabled unconditionally |
| OWASPA05002 | A05 Security Misconfiguration | Warning | Directory browsing enabled |
| OWASPA05003 | A05 Security Misconfiguration | Warning | Detailed errors exposed to client |
| OWASPA05004 | A05 Security Misconfiguration | Warning | Swagger enabled in production |
| OWASPA05005 | A05 Security Misconfiguration | Warning | HTTP logging with sensitive headers |
| OWASPA05006 | A05 Security Misconfiguration | Error | Hardcoded credential in source code |
| OWASPA06001 | A06 Vulnerable Components | Warning | Known-vulnerable NuGet package reference |
| OWASPA06002 | A06 Vulnerable Components | Warning | Deprecated or end-of-life NuGet package |
| OWASPA07001 | A07 Authentication Failures | Error | JWT signed with SecurityAlgorithms.None |
| OWASPA07002 | A07 Authentication Failures | Warning | JWT lifetime validation disabled |
| OWASPA07003 | A07 Authentication Failures | Error | JWT signing key validation disabled |
| OWASPA07004 | A07 Authentication Failures | Warning | Cookie missing HttpOnly or Secure flag |
| OWASPA07005 | A07 Authentication Failures | Warning | Cookie SameSite=None without Secure |
| OWASPA08001 | A08 Data Integrity Failures | Error | BinaryFormatter usage |
| OWASPA08002 | A08 Data Integrity Failures | Error | NetDataContractSerializer / SoapFormatter usage |
| OWASPA08003 | A08 Data Integrity Failures | Error | TypeNameHandling not None in Newtonsoft.Json |
| OWASPA08004 | A08 Data Integrity Failures | Error | JavaScriptSerializer with SimpleTypeResolver |
| OWASPA09001 | A09 Logging Failures | Warning | Empty catch block (swallowed exception) |
| OWASPA09002 | A09 Logging Failures | Warning | Catch block without logging |
| OWASPA09003 | A09 Logging Failures | Warning | Log injection via user-controlled input (taint) |
| OWASPA09004 | A09 Logging Failures | Warning | Sensitive data keyword in log message |
| OWASPA10001 | A10 SSRF | Error | SSRF via HttpClient (taint) |
| OWASPA10002 | A10 SSRF | Error | SSRF via WebClient (taint) |
| OWASPA10003 | A10 SSRF | Warning | AllowAutoRedirect without URL validation |

## Example `.editorconfig`

```ini
# .editorconfig at solution root
root = true

[*.cs]
# Treat all OWASP rules as errors
dotnet_diagnostic.OWASPA01001.severity = error
dotnet_diagnostic.OWASPA01004.severity = error

# Downgrade informational rules to silent
dotnet_diagnostic.OWASPA02003.severity = silent

# Suppress deprecated package rule (we track upgrades in a separate process)
dotnet_diagnostic.OWASPA06002.severity = none
```
