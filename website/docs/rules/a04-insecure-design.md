---
sidebar_position: 4
---

# A04 — Insecure Design

Insecure design refers to missing or ineffective security controls in the application architecture. This category focuses on design-level weaknesses rather than implementation bugs.

## OWASPA04002 — Missing rate limiting on authentication endpoints

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A04 Insecure Design |

### What it detects

ASP.NET Core applications that expose login, registration, or password-reset endpoints without configuring rate limiting middleware (`app.UseRateLimiter()` or `[EnableRateLimiting]`).

### Why it matters

Without rate limiting, authentication endpoints are vulnerable to brute-force and credential-stuffing attacks. An attacker can make thousands of login attempts per second with no artificial delay.

### ❌ Non-compliant

```csharp
// Program.cs — no rate limiter configured
app.UseRouting();
app.MapControllers();
app.Run();
```

```csharp
[HttpPost("login")]
public IActionResult Login(LoginDto dto)  // no rate limit attribute
{
    var user = _auth.Validate(dto.Username, dto.Password);
    if (user == null) return Unauthorized();
    return Ok(GenerateToken(user));
}
```

### ✅ Compliant

```csharp
// Program.cs
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("auth", cfg =>
    {
        cfg.PermitLimit = 5;
        cfg.Window = TimeSpan.FromMinutes(1);
    });
});

app.UseRateLimiter();
```

```csharp
[HttpPost("login")]
[EnableRateLimiting("auth")]
public IActionResult Login(LoginDto dto)
{
    var user = _auth.Validate(dto.Username, dto.Password);
    if (user == null) return Unauthorized();
    return Ok(GenerateToken(user));
}
```

### How to fix

Register rate limiting middleware in `Program.cs` and apply `[EnableRateLimiting]` to authentication-related endpoints.
