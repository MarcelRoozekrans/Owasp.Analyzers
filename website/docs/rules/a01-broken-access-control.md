---
sidebar_position: 1
---

# A01 — Broken Access Control

OWASP ranks Broken Access Control as the #1 web application security risk. These rules detect missing authorization checks and misconfigurations that allow unauthenticated or unauthorized users to access protected resources.

## OWASPA01001 — Missing authorization on controller action

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A01 Broken Access Control |

### What it detects

Controller actions in ASP.NET Core that lack both `[Authorize]` and `[AllowAnonymous]` attributes, where the controller itself also has no `[Authorize]` attribute.

### Why it matters

Without explicit authorization, an action is implicitly public. Forgetting `[Authorize]` on a single action can expose sensitive data or operations to unauthenticated users.

### ❌ Non-compliant

```csharp
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetProfile(int id)  // no authorization!
    {
        return Ok(GetUser(id));
    }
}
```

### ✅ Compliant

```csharp
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    [HttpGet("{id}")]
    [Authorize]
    public IActionResult GetProfile(int id)
    {
        return Ok(GetUser(id));
    }

    [HttpGet("public")]
    [AllowAnonymous]
    public IActionResult PublicInfo() => Ok("public");
}
```

### How to fix

Add `[Authorize]` to the action (or to the controller class to cover all actions), or add `[AllowAnonymous]` if public access is intentional.

---

## OWASPA01002 — Hardcoded role string

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A01 Broken Access Control |

### What it detects

Hardcoded string literals passed to `[Authorize(Roles = "...")]` or `User.IsInRole("...")`.

### Why it matters

Hardcoded role strings create maintenance burden and are prone to typos that silently break authorization checks.

### ❌ Non-compliant

```csharp
[Authorize(Roles = "Admin")]
public IActionResult DeleteUser(int id) { ... }
```

### ✅ Compliant

```csharp
public static class Roles
{
    public const string Admin = "Admin";
}

[Authorize(Roles = Roles.Admin)]
public IActionResult DeleteUser(int id) { ... }
```

---

## OWASPA01003 — IsInRole with hardcoded string

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A01 Broken Access Control |

### What it detects

Calls to `User.IsInRole("literal")` with a hardcoded string argument.

### ❌ Non-compliant

```csharp
if (User.IsInRole("Admin"))
{
    // ...
}
```

### ✅ Compliant

```csharp
if (User.IsInRole(Roles.Admin))
{
    // ...
}
```

---

## OWASPA01004 — CORS wildcard origin

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A01 Broken Access Control |

### What it detects

CORS policy configured with `AllowAnyOrigin()`, which sets the `Access-Control-Allow-Origin: *` header and allows any website to make cross-origin requests to your API.

### Why it matters

A wildcard CORS policy allows malicious websites to make authenticated cross-origin requests on behalf of users, enabling CSRF-style attacks against APIs that rely on cookies or other ambient credentials.

### ❌ Non-compliant

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});
```

### ✅ Compliant

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", policy =>
        policy.WithOrigins("https://myapp.example.com")
              .AllowAnyMethod()
              .AllowAnyHeader());
});
```

---

## OWASPA01005 — Missing antiforgery token

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A01 Broken Access Control |

### What it detects

HTTP POST/PUT/PATCH/DELETE controller actions in ASP.NET Core MVC that are not protected with `[ValidateAntiForgeryToken]` or `[AutoValidateAntiforgeryToken]` at the controller or action level.

### Why it matters

Without antiforgery token validation, your endpoints are vulnerable to Cross-Site Request Forgery (CSRF) attacks where a malicious site can trigger state-changing requests on behalf of authenticated users.

### ❌ Non-compliant

```csharp
[HttpPost]
public IActionResult CreateOrder(OrderDto dto)  // CSRF-vulnerable
{
    _orders.Add(dto);
    return Ok();
}
```

### ✅ Compliant

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult CreateOrder(OrderDto dto)
{
    _orders.Add(dto);
    return Ok();
}
```

Or apply globally at the controller level:

```csharp
[AutoValidateAntiforgeryToken]
public class OrderController : Controller { ... }
```
