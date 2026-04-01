---
sidebar_position: 5
---

# A05 — Security Misconfiguration

Security misconfiguration is the most commonly seen issue. These rules detect dangerous default settings, overly permissive configurations, and hardcoded credentials in .NET applications.

## OWASPA05001 — Developer exception page in production

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A05 Security Misconfiguration |

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

## OWASPA05002 — Directory browsing enabled

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A05 Security Misconfiguration |

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

## OWASPA05003 — Detailed errors enabled in production

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A05 Security Misconfiguration |

### What it detects

`app.UseStatusCodePages()` or custom error handlers that expose internal exception details to the client unconditionally.

### Why it matters

Detailed error messages leak implementation details (class names, database schemas, file paths) that help attackers plan targeted attacks.

### ❌ Non-compliant

```csharp
app.UseStatusCodePages(async ctx =>
{
    var ex = ctx.HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
    await ctx.HttpContext.Response.WriteAsync(ex?.ToString() ?? "Error");
});
```

### ✅ Compliant

```csharp
app.UseExceptionHandler(errApp =>
{
    errApp.Run(async ctx =>
    {
        ctx.Response.StatusCode = 500;
        await ctx.Response.WriteAsync("An error occurred.");
        // Log internally, do not expose ex.ToString() to the client
    });
});
```

---

## OWASPA05004 — Swagger enabled in production

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A05 Security Misconfiguration |

### What it detects

`app.UseSwagger()` or `app.UseSwaggerUI()` called outside a development-environment guard.

### Why it matters

Swagger UI exposes your entire API surface, request/response schemas, and authentication mechanisms. Leaving it enabled in production assists attackers in reconnaissance.

### ❌ Non-compliant

```csharp
app.UseSwagger();
app.UseSwaggerUI();
```

### ✅ Compliant

```csharp
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```

---

## OWASPA05005 — HTTP logging with sensitive headers

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A05 Security Misconfiguration |

### What it detects

`app.UseHttpLogging()` configured to log the `Authorization` header or request body without redaction.

### Why it matters

Logging authorization headers or request bodies can persist credentials, tokens, and sensitive user data in log files.

### ❌ Non-compliant

```csharp
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
});
app.UseHttpLogging();
```

### ✅ Compliant

```csharp
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.RequestPath
                          | HttpLoggingFields.ResponseStatusCode;
    // Do not include RequestBody, ResponseBody, or Authorization headers
});
app.UseHttpLogging();
```

---

## OWASPA05006 — Hardcoded credential in configuration

| Property | Value |
|----------|-------|
| **Severity** | Error |
| **Category** | A05 Security Misconfiguration |

### What it detects

String literals that appear to be passwords, API keys, or connection strings assigned to variables or properties whose names contain `password`, `secret`, `apikey`, `connectionstring`, or similar keywords.

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
