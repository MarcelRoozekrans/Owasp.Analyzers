# Owasp.Analyzers

[![NuGet](https://img.shields.io/nuget/v/Owasp.Analyzers.svg)](https://www.nuget.org/packages/Owasp.Analyzers)
[![CI](https://github.com/MarcelRoozekrans/Owasp.Analyzers/actions/workflows/ci.yml/badge.svg)](https://github.com/MarcelRoozekrans/Owasp.Analyzers/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

Roslyn analyzers covering the [OWASP Top 10 2021](https://owasp.org/Top10/) for C#/.NET — catch security vulnerabilities at compile time.

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
| OWASPA02001 | A02 Cryptographic Failures | Warning | Weak hashing algorithm (MD5 / SHA1) |
| OWASPA02002 | A02 Cryptographic Failures | Warning | ECB cipher mode |
| OWASPA02003 | A02 Cryptographic Failures | Info | System.Random used (not cryptographically secure) |
| OWASPA02004 | A02 Cryptographic Failures | Error | Hardcoded cryptographic key or IV |
| OWASPA02005 | A02 Cryptographic Failures | Warning | Legacy TLS protocol (SSL2/3, TLS 1.0/1.1) |
| OWASPA02006 | A02 Cryptographic Failures | Error | Certificate validation disabled |
| OWASPA02007 | A02 Cryptographic Failures | Warning | HTTP URL used (not HTTPS) |
| OWASPA02008 | A02 Cryptographic Failures | Warning | HSTS not configured alongside HTTPS redirection |
| OWASPA03001 | A03 Injection | Error | SQL injection (taint analysis) |
| OWASPA03002 | A03 Injection | Error | OS command injection (taint analysis) |
| OWASPA03003 | A03 Injection | Error | Path traversal (taint analysis) |
| OWASPA03004 | A03 Injection | Error | LDAP injection (taint analysis) |
| OWASPA03005 | A03 Injection | Error | XPath injection (taint analysis) |
| OWASPA03006 | A03 Injection | Error | XSS via unencoded output (taint analysis) |
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
| OWASPA10001 | A10 SSRF | Error | SSRF via HttpClient (taint analysis) |
| OWASPA10002 | A10 SSRF | Error | SSRF via WebClient (taint analysis) |
| OWASPA10003 | A10 SSRF | Warning | AllowAutoRedirect without URL validation |

For detailed documentation with code examples and fix guidance, see the **[docs site](https://marcelroozekrans.github.io/Owasp.Analyzers/docs/intro)**.

## Configuration

Override severity in `.editorconfig`:

```ini
[*.cs]
dotnet_diagnostic.OWASPA10001.severity = warning
dotnet_diagnostic.OWASPA01004.severity = none
```

See [Configuration](https://marcelroozekrans.github.io/Owasp.Analyzers/docs/configuration) for the full reference.

## License

[MIT](LICENSE)
