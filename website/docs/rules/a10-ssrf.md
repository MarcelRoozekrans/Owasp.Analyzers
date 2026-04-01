---
sidebar_position: 10
---

# A10 — Server-Side Request Forgery (SSRF)

SSRF vulnerabilities allow attackers to make the server send HTTP requests to internal resources, cloud metadata endpoints, or other services that should not be publicly accessible. These rules use **taint analysis** to track user-controlled URLs to HTTP client sinks.

See [Taint Engine](../taint-engine) for details on how the analysis works.

## OWASPA10001 — SSRF via HttpClient

| Property | Value |
|----------|-------|
| **Severity** | Error |
| **Category** | A10 SSRF |
| **Technique** | Taint analysis |

### What it detects

User-controlled data (from query strings, route values, request body, or headers) flowing into `HttpClient.GetAsync()`, `HttpClient.PostAsync()`, `HttpClient.SendAsync()`, or similar `HttpClient` methods as the URL argument.

### Why it matters

An attacker can set the URL to `http://169.254.169.254/latest/meta-data/` (AWS instance metadata) or internal services like `http://internal-db:5432/`, gaining access to cloud credentials and internal infrastructure.

### ❌ Non-compliant

```csharp
[HttpGet]
public async Task<IActionResult> Proxy(string url)
{
    // ❌ OWASPA10001: user-controlled URL passed to HttpClient
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

## OWASPA10002 — SSRF via WebClient

| Property | Value |
|----------|-------|
| **Severity** | Error |
| **Category** | A10 SSRF |
| **Technique** | Taint analysis |

### What it detects

User-controlled data flowing into `WebClient.DownloadString()`, `WebClient.DownloadData()`, `WebClient.UploadString()`, or the `WebClient.BaseAddress` property.

### ❌ Non-compliant

```csharp
[HttpPost]
public IActionResult Fetch(string url)
{
    // ❌ OWASPA10002: SSRF via WebClient
    using var client = new WebClient();
    var data = client.DownloadString(url);
    return Content(data);
}
```

### ✅ Compliant

Use `HttpClient` with a validated allowlist URL (see OWASPA10001 example). Avoid `WebClient` — it is a legacy API superseded by `HttpClient`.

---

## OWASPA10003 — AllowAutoRedirect without validation

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A10 SSRF |

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
