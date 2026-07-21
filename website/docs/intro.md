---
sidebar_position: 1
slug: /intro
---

# Introduction

**Owasp.Analyzers** is a collection of Roslyn diagnostic analyzers that surface [OWASP Top 10 2025](https://owasp.org/Top10/2025/) security vulnerabilities as compiler warnings and errors in your C#/.NET projects.

## How it works

Roslyn analyzers run inside the compiler pipeline — no external tools, no CI-only scans. Every build checks your code against the rules. Violations appear inline in your IDE (Visual Studio, Rider, VS Code) and as `dotnet build` output, exactly like ordinary compiler warnings.

```
warning OWASPA01001: Action 'GetProfile' is not decorated with [Authorize] or [AllowAnonymous]
error   OWASPA05001: User-controlled data flows into SQL command without parameterization
```

## Coverage

| Category | Rules | Technique |
|----------|-------|-----------|
| [A01 Broken Access Control](./rules/a01-broken-access-control) | 8 | Syntax / Semantic / Taint analysis |
| [A02 Security Misconfiguration](./rules/a02-security-misconfiguration) | 6 | Syntax |
| [A03 Software Supply Chain Failures](./rules/a03-software-supply-chain-failures) | 2 | MSBuild target |
| [A04 Cryptographic Failures](./rules/a04-cryptographic-failures) | 8 | Syntax / Semantic |
| [A05 Injection](./rules/a05-injection) | 6 | Taint analysis |
| [A06 Insecure Design](./rules/a06-insecure-design) | 1 | Syntax |
| [A07 Authentication Failures](./rules/a07-authentication-failures) | 5 | Semantic |
| [A08 Software or Data Integrity Failures](./rules/a08-data-integrity) | 4 | Semantic |
| [A09 Security Logging and Alerting Failures](./rules/a09-logging-failures) | 2 | Syntax / Taint analysis |
| [A10 Mishandling of Exceptional Conditions](./rules/a10-mishandling-exceptional-conditions) | 2 | Syntax |

## What's next?

- [Install the package](./getting-started/installation)
- [See your first diagnostic](./getting-started/quick-start)
- [Browse the rules](./rules/a01-broken-access-control)
