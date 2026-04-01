# Owasp.Analyzers

Roslyn analyzers covering the [OWASP Top 10 2021](https://owasp.org/Top10/) for C#/.NET projects.

## Installation

```
dotnet add package Owasp.Analyzers
```

## Rules

| ID | Category | Severity | Description |
|----|----------|----------|-------------|
| OWASPA01001 | A01 Broken Access Control | Warning | Controller action missing authorization |
| OWASPA01002 | A01 Broken Access Control | Warning | CORS wildcard origin |
| OWASPA01003 | A01 Broken Access Control | Warning | Hardcoded role string |
| OWASPA01004 | A01 Broken Access Control | Warning | Missing antiforgery token |
| OWASPA02001–004 | A02 Cryptographic Failures | Error/Warning | Weak crypto, hardcoded secrets |
| OWASPA03001–003 | A03 Injection | Error | SQL/command/LDAP injection via taint |
| OWASPA04001–002 | A04 Insecure Design | Warning | Insecure design patterns |
| OWASPA05001 | A05 Security Misconfiguration | Warning | Security misconfiguration |
| OWASPA06001–002 | A06 Vulnerable Components | Warning | Vulnerable/deprecated NuGet packages |
| OWASPA07001–005 | A07 Auth Failures | Warning/Error | Authentication failures |
| OWASPA08001–004 | A08 Data Integrity | Warning/Error | Data integrity failures |
| OWASPA09001–004 | A09 Logging Failures | Warning | Logging and monitoring failures |
| OWASPA10001–003 | A10 SSRF | Error/Warning | Server-side request forgery |

## License

MIT
