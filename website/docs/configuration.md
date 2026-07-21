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
| OWASPA01006 | A01 Broken Access Control | Error | SSRF via HttpClient (taint analysis) |
| OWASPA01007 | A01 Broken Access Control | Error | SSRF via WebClient (taint analysis) |
| OWASPA01008 | A01 Broken Access Control | Warning | AllowAutoRedirect without URL validation |
| OWASPA02001 | A02 Security Misconfiguration | Warning | Developer exception page enabled unconditionally |
| OWASPA02002 | A02 Security Misconfiguration | Warning | Missing HTTPS redirection |
| OWASPA02003 | A02 Security Misconfiguration | Warning | Directory browsing enabled |
| OWASPA02004 | A02 Security Misconfiguration | Warning | Error details exposed to client |
| OWASPA02005 | A02 Security Misconfiguration | Warning | Antiforgery services not configured |
| OWASPA02006 | A02 Security Misconfiguration | Error | Hardcoded credential in source code |
| OWASPA03001 | A03 Software Supply Chain Failures | Warning | Known-vulnerable NuGet package reference |
| OWASPA03002 | A03 Software Supply Chain Failures | Warning | Deprecated or end-of-life NuGet package |
| OWASPA04001 | A04 Cryptographic Failures | Warning | Weak hashing algorithm (MD5 / SHA1) |
| OWASPA04002 | A04 Cryptographic Failures | Warning | ECB cipher mode |
| OWASPA04003 | A04 Cryptographic Failures | Info | System.Random used (not cryptographically secure) |
| OWASPA04004 | A04 Cryptographic Failures | Error | Hardcoded cryptographic key or IV |
| OWASPA04005 | A04 Cryptographic Failures | Warning | Legacy TLS protocol (SSL2/3, TLS 1.0/1.1) |
| OWASPA04006 | A04 Cryptographic Failures | Error | Certificate validation disabled |
| OWASPA04007 | A04 Cryptographic Failures | Warning | HTTP URL used (not HTTPS) |
| OWASPA04008 | A04 Cryptographic Failures | Warning | HSTS not configured alongside HTTPS redirection |
| OWASPA05001 | A05 Injection | Error | SQL injection (taint analysis) |
| OWASPA05002 | A05 Injection | Error | OS command injection (taint analysis) |
| OWASPA05003 | A05 Injection | Error | Path traversal (taint analysis) |
| OWASPA05004 | A05 Injection | Error | LDAP injection (taint analysis) |
| OWASPA05005 | A05 Injection | Error | XPath injection (taint analysis) |
| OWASPA05006 | A05 Injection | Error | XSS via unencoded output (taint analysis) |
| OWASPA06001 | A06 Insecure Design | Warning | Missing rate limiting on authentication endpoints |
| OWASPA07001 | A07 Authentication Failures | Error | JWT signed with SecurityAlgorithms.None |
| OWASPA07002 | A07 Authentication Failures | Warning | JWT lifetime validation disabled |
| OWASPA07003 | A07 Authentication Failures | Error | JWT signing key validation disabled |
| OWASPA07004 | A07 Authentication Failures | Warning | Cookie missing HttpOnly or Secure flag |
| OWASPA07005 | A07 Authentication Failures | Warning | Cookie SameSite=None without Secure |
| OWASPA08001 | A08 Software or Data Integrity Failures | Error | BinaryFormatter usage |
| OWASPA08002 | A08 Software or Data Integrity Failures | Error | NetDataContractSerializer / SoapFormatter usage |
| OWASPA08003 | A08 Software or Data Integrity Failures | Error | TypeNameHandling not None in Newtonsoft.Json |
| OWASPA08004 | A08 Software or Data Integrity Failures | Error | JavaScriptSerializer with SimpleTypeResolver |
| OWASPA09001 | A09 Security Logging and Alerting Failures | Warning | Log injection via user-controlled input (taint analysis) |
| OWASPA09002 | A09 Security Logging and Alerting Failures | Warning | Sensitive data keyword in log message |
| OWASPA10001 | A10 Mishandling of Exceptional Conditions | Warning | Empty catch block (swallowed exception) |
| OWASPA10002 | A10 Mishandling of Exceptional Conditions | Warning | Catch block without logging |

## Example `.editorconfig`

```ini
# .editorconfig at solution root
root = true

[*.cs]
# Treat all OWASP rules as errors
dotnet_diagnostic.OWASPA01001.severity = error
dotnet_diagnostic.OWASPA01004.severity = error

# Downgrade informational rules to silent
dotnet_diagnostic.OWASPA04003.severity = silent

# Suppress deprecated package rule (we track upgrades in a separate process)
dotnet_diagnostic.OWASPA03002.severity = none
```
