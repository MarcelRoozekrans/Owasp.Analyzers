---
sidebar_position: 3
---

# A03 — Software Supply Chain Failures

OWASP's 2025 definition of this category spans the whole software supply chain — dependencies, build systems, and distribution infrastructure. Today, Owasp.Analyzers covers the dependency slice of that: these rules use MSBuild targets to detect known-vulnerable or deprecated NuGet packages at build time. Build-system and distribution-infrastructure compromise (e.g. CI/CD tampering, unsigned artifacts, dependency confusion) are not yet covered.

## OWASPA03001 — Known-vulnerable NuGet package

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A03 Software Supply Chain Failures |
| **Technique** | MSBuild target |

### What it detects

NuGet package references to packages with known CVEs that are commonly found in .NET projects. The rule checks package IDs and version ranges against a built-in list of vulnerable versions.

Examples of detected packages:
- `Newtonsoft.Json` versions prior to 13.0.1 (CVE-2024-21907)
- `System.Text.RegularExpressions` versions prior to 4.3.1 (CVE-2019-0820, ReDoS)
- `log4net` versions prior to 2.0.15

### ❌ Non-compliant

```xml
<PackageReference Include="Newtonsoft.Json" Version="12.0.3" />
```

### ✅ Compliant

```xml
<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
```

### How to check for vulnerabilities

```bash
dotnet list package --vulnerable
```

This command queries the NuGet advisory database and lists all packages in your solution with known vulnerabilities.

---

## OWASPA03002 — Deprecated or end-of-life NuGet package

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A03 Software Supply Chain Failures |
| **Technique** | MSBuild target |

### What it detects

References to NuGet packages that have been deprecated by their authors or have reached end-of-life. These packages no longer receive security patches.

Examples:
- `Microsoft.AspNetCore.All` (deprecated, use `Microsoft.AspNetCore.App`)
- `Newtonsoft.Json.Bson` (deprecated)
- `System.Net.Http` (deprecated, use the built-in `HttpClient`)

### ❌ Non-compliant

```xml
<PackageReference Include="Microsoft.AspNetCore.All" Version="2.2.8" />
```

### ✅ Compliant

```xml
<PackageReference Include="Microsoft.AspNetCore.App" />
```

### How to check for deprecations

```bash
dotnet list package --deprecated
```

Or in Visual Studio / Rider — deprecated packages are shown with a deprecation warning in the NuGet package manager UI.
