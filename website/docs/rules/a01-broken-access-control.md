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

---

## OWASPA01006 — SSRF via HttpClient

| Property | Value |
|----------|-------|
| **Severity** | Error |
| **Category** | A01 Broken Access Control |
| **Technique** | Taint analysis |

### What it detects

User-controlled data (from query strings, route values, request body, or headers) flowing into `HttpClient.GetAsync()`, `HttpClient.PostAsync()`, `HttpClient.SendAsync()`, or similar `HttpClient` methods as the URL argument.

### Why it matters

An attacker can set the URL to `http://169.254.169.254/latest/meta-data/` (AWS instance metadata) or internal services like `http://internal-db:5432/`, gaining access to cloud credentials and internal infrastructure that should be gated behind access control.

### ❌ Non-compliant

```csharp
[HttpGet]
public async Task<IActionResult> Proxy(string url)
{
    // ❌ OWASPA01006: user-controlled URL passed to HttpClient
    var response = await _httpClient.GetAsync(url);
    var content = await response.Content.ReadAsStringAsync();
    return Content(content);
}
```

### ✅ Compliant

```csharp
[HttpGet]
public async Task<IActionResult> Proxy(string resourceId)
{
    // Validate against an allowlist — never use user input as a full URL
    if (!_allowedResources.TryGetValue(resourceId, out var safeUrl))
        return BadRequest("Unknown resource");

    var response = await _httpClient.GetAsync(safeUrl);
    var content = await response.Content.ReadAsStringAsync();
    return Content(content);
}
```

---

## OWASPA01007 — SSRF via WebClient

| Property | Value |
|----------|-------|
| **Severity** | Error |
| **Category** | A01 Broken Access Control |
| **Technique** | Taint analysis |

### What it detects

User-controlled data flowing into `WebClient.DownloadString()`, `WebClient.DownloadData()`, `WebClient.UploadString()`, or the `WebClient.BaseAddress` property.

### ❌ Non-compliant

```csharp
[HttpPost]
public IActionResult Fetch(string url)
{
    // ❌ OWASPA01007: SSRF via WebClient
    using var client = new WebClient();
    var data = client.DownloadString(url);
    return Content(data);
}
```

### ✅ Compliant

Use `HttpClient` with a validated allowlist URL (see OWASPA01006 example). Avoid `WebClient` — it is a legacy API superseded by `HttpClient`.

---

## OWASPA01008 — AllowAutoRedirect without validation

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A01 Broken Access Control |

### What it detects

`HttpClientHandler` configured with `AllowAutoRedirect = true` (the default) in a context where the initial URL is user-controlled — allowing an attacker to redirect the server to an internal endpoint via an open redirect.

### Why it matters

Even if the initial URL is validated, a redirect response can point the client to an internal resource. With `AllowAutoRedirect = true`, the `HttpClient` will follow that redirect without re-validation.

### ❌ Non-compliant

```csharp
var handler = new HttpClientHandler
{
    AllowAutoRedirect = true  // follows redirects, including to internal hosts
};
var client = new HttpClient(handler);
// then used with user-controlled URL
```

### ✅ Compliant

```csharp
var handler = new HttpClientHandler
{
    AllowAutoRedirect = false  // validate redirect targets manually
};
var client = new HttpClient(handler);
```

If you need to follow redirects, validate each redirect target against your allowlist before following.
