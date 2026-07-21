---
sidebar_position: 2
---

# A02 — Security Misconfiguration

Security misconfiguration is one of the most commonly seen issues. These rules detect dangerous default settings, missing hardening middleware, and hardcoded credentials in .NET applications.

## OWASPA02001 — Developer exception page in production

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A02 Security Misconfiguration |

### What it detects

`app.UseDeveloperExceptionPage()` called unconditionally (outside an `if (app.Environment.IsDevelopment())` block).

### Why it matters

The developer exception page reveals stack traces, source file paths, environment variables, and other sensitive information. It must never be shown in production.

### ❌ Non-compliant

```csharp
app.UseDeveloperExceptionPage();  // always on
app.UseRouting();
```

### ✅ Compliant

```csharp
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
```

---

## OWASPA02002 — Missing HTTPS redirection

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A02 Security Misconfiguration |

### What it detects

A `Configure`/`ConfigureApp` startup method that never calls `app.UseHttpsRedirection()`.

### Why it matters

Without HTTPS redirection, plain HTTP requests are served as-is instead of being upgraded, leaving traffic open to interception.

### ❌ Non-compliant

```csharp
public void Configure(IApplicationBuilder app)
{
    app.UseRouting();
    app.UseEndpoints(e => e.MapControllers());
    // UseHttpsRedirection() missing
}
```

### ✅ Compliant

```csharp
public void Configure(IApplicationBuilder app)
{
    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseEndpoints(e => e.MapControllers());
}
```

---

## OWASPA02003 — Directory browsing enabled

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A02 Security Misconfiguration |

### What it detects

`app.UseDirectoryBrowser()` called in the middleware pipeline.

### Why it matters

Directory browsing lists all files in a directory over HTTP, exposing the file system structure and potentially sensitive files to unauthenticated users.

### ❌ Non-compliant

```csharp
app.UseDirectoryBrowser();
```

### ✅ Compliant

Remove the `UseDirectoryBrowser()` call entirely, or restrict access with authorization middleware.

---

## OWASPA02004 — Error details exposed

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A02 Security Misconfiguration |

### What it detects

`IncludeErrorDetails` or `IncludeErrorDetailPolicy` assigned `true` (e.g. on `ExceptionHandlerOptions` or hub options).

### Why it matters

Detailed error responses leak implementation details (class names, stack traces, file paths) that help attackers plan targeted attacks.

### ❌ Non-compliant

```csharp
options.IncludeErrorDetails = true;
```

### ✅ Compliant

```csharp
options.IncludeErrorDetails = false;
// Log the exception internally instead of returning it to the client
```

---

## OWASPA02005 — Antiforgery not configured

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A02 Security Misconfiguration |

### What it detects

A `ConfigureServices`/`AddServices`/`AddApplicationServices` method that never calls `services.AddAntiforgery()`.

### Why it matters

Without antiforgery services registered, CSRF-protection attributes such as `[ValidateAntiForgeryToken]` have nothing backing them, and state-changing endpoints may be left vulnerable to Cross-Site Request Forgery.

### ❌ Non-compliant

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();
    // AddAntiforgery() missing
}
```

### ✅ Compliant

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();
    services.AddAntiforgery();
}
```

---

## OWASPA02006 — Hardcoded credential in configuration

| Property | Value |
|----------|-------|
| **Severity** | Error |
| **Category** | A02 Security Misconfiguration |

### What it detects

String literals that appear to be passwords, API keys, or connection strings assigned to variables or properties whose names contain `password`, `secret`, `apikey`, or similar keywords.

### Why it matters

Hardcoded credentials in source code are checked into version control and are trivially discoverable. A single leaked repository exposes all environments using those credentials.

### ❌ Non-compliant

```csharp
var connectionString = "Server=db;Database=app;User=sa;Password=P@ssw0rd123!";
builder.Services.AddDbContext<AppDb>(o => o.UseSqlServer(connectionString));
```

### ✅ Compliant

```csharp
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDb>(o => o.UseSqlServer(connectionString));
```

Store credentials in environment variables, user secrets (`dotnet user-secrets`), or a secrets manager (Azure Key Vault, AWS Secrets Manager).
