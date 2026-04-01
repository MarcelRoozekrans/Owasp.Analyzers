---
sidebar_position: 1
slug: /intro
---

# Introduction

**Owasp.Analyzers** is a collection of Roslyn diagnostic analyzers that surface [OWASP Top 10 2021](https://owasp.org/Top10/) security vulnerabilities as compiler warnings and errors in your C#/.NET projects.

## How it works

Roslyn analyzers run inside the compiler pipeline — no external tools, no CI-only scans. Every build checks your code against the rules. Violations appear inline in your IDE (Visual Studio, Rider, VS Code) and as `dotnet build` output, exactly like ordinary compiler warnings.

```
warning OWASPA01001: Action 'GetProfile' is not decorated with [Authorize] or [AllowAnonymous]
error   OWASPA03001: User-controlled data flows into SQL command without parameterization
```

## Coverage

| Category | Rules | Technique |
|----------|-------|-----------|
| [A01 Broken Access Control](./rules/a01-broken-access-control) | 5 | Syntax / Semantic |
| [A02 Cryptographic Failures](./rules/a02-cryptographic-failures) | 8 | Syntax / Semantic |
| [A03 Injection](./rules/a03-injection) | 6 | Taint analysis |
| [A04 Insecure Design](./rules/a04-insecure-design) | 1 | Syntax |
| [A05 Security Misconfiguration](./rules/a05-security-misconfiguration) | 6 | Syntax |
| [A06 Vulnerable Components](./rules/a06-vulnerable-components) | 2 | MSBuild target |
| [A07 Authentication Failures](./rules/a07-authentication-failures) | 5 | Semantic |
| [A08 Data Integrity Failures](./rules/a08-data-integrity) | 4 | Semantic |
| [A09 Logging Failures](./rules/a09-logging-failures) | 4 | Syntax / Taint |
| [A10 SSRF](./rules/a10-ssrf) | 3 | Taint analysis |

## What's next?

- [Install the package](./getting-started/installation)
- [See your first diagnostic](./getting-started/quick-start)
- [Browse the rules](./rules/a01-broken-access-control)
