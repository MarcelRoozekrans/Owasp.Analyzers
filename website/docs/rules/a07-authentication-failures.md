---
sidebar_position: 7
---

# A07 — Identification and Authentication Failures

Authentication failures cover weaknesses in JWT validation, cookie security, and session management. These rules detect dangerous defaults that silently disable security checks.

## OWASPA07001 — JWT signed with SecurityAlgorithms.None

| Property | Value |
|----------|-------|
| **Severity** | Error |
| **Category** | A07 Authentication Failures |

### What it detects

JWT tokens created with `SecurityAlgorithms.None` as the signing algorithm, which produces unsigned tokens that any client can forge.

### ❌ Non-compliant

```csharp
var token = new JwtSecurityToken(
    claims: claims,
    signingCredentials: new SigningCredentials(
        new SymmetricSecurityKey(key),
        SecurityAlgorithms.None  // ❌ unsigned token
    )
);
```

### ✅ Compliant

```csharp
var token = new JwtSecurityToken(
    claims: claims,
    expires: DateTime.UtcNow.AddHours(1),
    signingCredentials: new SigningCredentials(
        new SymmetricSecurityKey(key),
        SecurityAlgorithms.HmacSha256
    )
);
```

---

## OWASPA07002 — JWT lifetime validation disabled

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A07 Authentication Failures |

### What it detects

`TokenValidationParameters` with `ValidateLifetime = false`.

### Why it matters

Disabling lifetime validation means expired tokens are accepted forever. Revoked or stolen tokens remain valid indefinitely.

### ❌ Non-compliant

```csharp
var parameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = false,  // ❌ expired tokens accepted
    IssuerSigningKey = new SymmetricSecurityKey(key)
};
```

### ✅ Compliant

```csharp
var parameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ClockSkew = TimeSpan.FromMinutes(5),
    IssuerSigningKey = new SymmetricSecurityKey(key)
};
```

---

## OWASPA07003 — JWT signing key validation disabled

| Property | Value |
|----------|-------|
| **Severity** | Error |
| **Category** | A07 Authentication Failures |

### What it detects

`TokenValidationParameters` with `ValidateIssuerSigningKey = false`.

### Why it matters

Disabling signing key validation means the JWT signature is not verified. Any token can be accepted regardless of whether it was signed with the correct key, allowing attackers to forge arbitrary tokens.

### ❌ Non-compliant

```csharp
var parameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = false,  // ❌ signature not verified
    IssuerSigningKey = new SymmetricSecurityKey(key)
};
```

### ✅ Compliant

```csharp
var parameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key)
};
```

---

## OWASPA07004 — Cookie missing HttpOnly or Secure flag

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A07 Authentication Failures |

### What it detects

`CookieOptions` or `ResponseCookies.Append()` calls where `HttpOnly` or `Secure` is not set to `true`.

### Why it matters

- Without `HttpOnly`, JavaScript can read the cookie, making it vulnerable to XSS-based session theft.
- Without `Secure`, the cookie is transmitted over plain HTTP, where it can be intercepted.

### ❌ Non-compliant

```csharp
Response.Cookies.Append("session", token, new CookieOptions
{
    // HttpOnly and Secure not set — defaults to false
    Expires = DateTimeOffset.UtcNow.AddHours(1)
});
```

### ✅ Compliant

```csharp
Response.Cookies.Append("session", token, new CookieOptions
{
    HttpOnly = true,
    Secure = true,
    SameSite = SameSiteMode.Strict,
    Expires = DateTimeOffset.UtcNow.AddHours(1)
});
```

---

## OWASPA07005 — Cookie SameSite=None without Secure

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A07 Authentication Failures |

### What it detects

`CookieOptions` with `SameSite = SameSiteMode.None` but `Secure` not set to `true`.

### Why it matters

Browsers reject `SameSite=None` cookies that are not also marked `Secure`. If this combination is used, modern browsers will drop the cookie silently, breaking functionality — and older browsers will send it over HTTP, enabling interception.

### ❌ Non-compliant

```csharp
new CookieOptions
{
    SameSite = SameSiteMode.None,
    // Secure not set
}
```

### ✅ Compliant

```csharp
new CookieOptions
{
    SameSite = SameSiteMode.None,
    Secure = true
}
```
