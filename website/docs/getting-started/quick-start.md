---
sidebar_position: 2
---

# Quick Start

This guide walks you through triggering your first diagnostic and fixing it.

## 1. Install the package

```bash
dotnet add package Owasp.Analyzers
```

## 2. Write code that triggers a rule

Add this to any ASP.NET Core controller:

```csharp
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    [HttpGet("{id}")]
    // ❌ OWASPA01001: missing [Authorize] or [AllowAnonymous]
    public IActionResult GetProfile(int id)
    {
        return Ok(id);
    }
}
```

## 3. Build and see the diagnostic

```bash
dotnet build
```

Output:
```
warning OWASPA01001: Action 'GetProfile' on controller 'UserController' is not decorated with [Authorize] or [AllowAnonymous]. Unauthenticated access may be unintentional.
```

## 4. Fix it

Add the appropriate attribute:

```csharp
[HttpGet("{id}")]
[Authorize]  // ✅ authenticated access required
public IActionResult GetProfile(int id)
{
    return Ok(id);
}
```

Or, if public access is intentional:

```csharp
[HttpGet("{id}")]
[AllowAnonymous]  // ✅ explicitly opts out
public IActionResult GetProfile(int id)
{
    return Ok(id);
}
```

## 5. Suppress a false positive

If you have a legitimate reason to skip a rule on a specific line, use `#pragma`:

```csharp
#pragma warning disable OWASPA01001
[HttpGet("{id}")]
public IActionResult GetProfile(int id) { ... }
#pragma warning restore OWASPA01001
```

Or suppress globally in `.editorconfig`:

```ini
[*.cs]
dotnet_diagnostic.OWASPA01001.severity = none
```

## Next steps

- [Browse all rules](../rules/a01-broken-access-control)
- [Configure severity](../configuration)
- [Learn about taint analysis](../taint-engine)
