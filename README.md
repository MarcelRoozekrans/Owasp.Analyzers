# Owasp.Analyzers

[![NuGet](https://img.shields.io/nuget/v/Owasp.Analyzers.svg)](https://www.nuget.org/packages/Owasp.Analyzers)
[![CI](https://github.com/MarcelRoozekrans/Owasp.Analyzers/actions/workflows/ci.yml/badge.svg)](https://github.com/MarcelRoozekrans/Owasp.Analyzers/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![GitHub Sponsors](https://img.shields.io/github/sponsors/MarcelRoozekrans?style=flat&logo=githubsponsors&color=ea4aaa&label=Sponsor)](https://github.com/sponsors/MarcelRoozekrans)

Roslyn analyzers covering the [OWASP Top 10 2025](https://owasp.org/Top10/2025/) for C#/.NET — catch security vulnerabilities at compile time.

📖 **[Full documentation](https://marcelroozekrans.github.io/Owasp.Analyzers/docs/intro)**

## Installation

```bash
dotnet add package Owasp.Analyzers
```

The package is marked as `DevelopmentDependency` — it does not appear as a transitive runtime dependency.

## Rules

| Rule ID | Category | Severity | Description |
|---------|----------|----------|-------------|
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

For detailed documentation with code examples and fix guidance, see the **[docs site](https://marcelroozekrans.github.io/Owasp.Analyzers/docs/intro)**.

### Migrating from 1.x (OWASP 2021) to 2.x (OWASP 2025)

OWASP retired the 2021 Top 10 in favor of [Top 10:2025](https://owasp.org/Top10/2025/), which reorders several categories, renames others, and introduces two new ones (`A03 Software Supply Chain Failures`, `A10 Mishandling of Exceptional Conditions`). Version 2.0.0 of this package renumbers every diagnostic ID to match the new category it lives under — this is a breaking change for any `.editorconfig` severity overrides keyed by rule ID. The old → new mapping:

| 2021 ID | 2025 ID |
|---------|---------|
| OWASPA02001–008 | OWASPA04001–008 |
| OWASPA03001–006 | OWASPA05001–006 |
| OWASPA04002 | OWASPA06001 |
| OWASPA05001–006 | OWASPA02001–006 |
| OWASPA06001–002 | OWASPA03001–002 |
| OWASPA07001–005 | OWASPA07001–005 *(unchanged)* |
| OWASPA08001–004 | OWASPA08001–004 *(unchanged)* |
| OWASPA09001–002 | OWASPA10001–002 |
| OWASPA09003–004 | OWASPA09001–002 |
| OWASPA10001–003 | OWASPA01006–008 |

Update any `dotnet_diagnostic.OWASPAxxxxx.severity` overrides in your `.editorconfig` to the new IDs after upgrading.

The MSBuild opt-out for the package-vulnerability check was also renamed, from `OwaspA06Enabled` to `OwaspA03Enabled` (matching the new `A03 Software Supply Chain Failures` category), and its warning codes changed from `OWASP-A06-001`/`OWASP-A06-002` to `OWASP-A03-001`/`OWASP-A03-002`. If you set `<OwaspA06Enabled>false</OwaspA06Enabled>` in a `.csproj` today, it's still honored in 2.x, but you should switch to `<OwaspA03Enabled>false</OwaspA03Enabled>` — the old property name will be removed in a future major version.

## Configuration

Override severity in `.editorconfig`:

```ini
[*.cs]
dotnet_diagnostic.OWASPA01006.severity = warning
dotnet_diagnostic.OWASPA01004.severity = none
```

See [Configuration](https://marcelroozekrans.github.io/Owasp.Analyzers/docs/configuration) for the full reference.

## License

[MIT](LICENSE)
