---
sidebar_position: 9
---

# A09 — Security Logging and Monitoring Failures

Insufficient logging and monitoring allows attackers to operate undetected. These rules detect swallowed exceptions, log injection vulnerabilities, and accidental logging of sensitive data.

## OWASPA09001 — Empty catch block (swallowed exception)

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A09 Logging Failures |

### What it detects

`catch` blocks that contain no statements — exceptions are silently discarded without any logging or re-throwing.

### Why it matters

Swallowed exceptions hide failures, making it impossible to detect attacks, diagnose problems, or trigger alerts. An empty catch is one of the most common causes of invisible security incidents.

### ❌ Non-compliant

```csharp
try
{
    ProcessPayment(order);
}
catch (Exception)
{
    // silent failure — attacker can probe with no trace
}
```

### ✅ Compliant

```csharp
try
{
    ProcessPayment(order);
}
catch (Exception ex)
{
    _logger.LogError(ex, "Payment processing failed for order {OrderId}", order.Id);
    throw;  // or return appropriate error response
}
```

---

## OWASPA09002 — Catch block without logging

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A09 Logging Failures |

### What it detects

`catch` blocks that have statements but none of them involve a logging call (`ILogger`, `Log.`, `_logger.`, `logger.`).

### ❌ Non-compliant

```csharp
catch (Exception ex)
{
    return StatusCode(500, "Internal error");
    // exception not logged anywhere
}
```

### ✅ Compliant

```csharp
catch (Exception ex)
{
    _logger.LogError(ex, "Unexpected error handling request");
    return StatusCode(500, "Internal error");
}
```

---

## OWASPA09003 — Log injection via taint

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A09 Logging Failures |
| **Technique** | Taint analysis |

### What it detects

User-controlled data flowing directly into a logging call without sanitization. An attacker who can inject newlines into log messages can forge log entries and obscure evidence of an attack.

### ❌ Non-compliant

```csharp
[HttpGet]
public IActionResult GetItem(string id)
{
    // ❌ OWASPA09003: user input injected into log
    _logger.LogInformation("Fetching item: " + id);
    return Ok(_items[id]);
}
```

### ✅ Compliant

```csharp
[HttpGet]
public IActionResult GetItem(string id)
{
    // Use structured logging — the id is a parameter, not concatenated
    _logger.LogInformation("Fetching item: {ItemId}", id);
    return Ok(_items[id]);
}
```

Using structured logging (message templates with `{Parameter}`) prevents log injection because the value is stored separately from the message template in most log sinks.

---

## OWASPA09004 — Sensitive data in log message

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A09 Logging Failures |

### What it detects

Log messages (string literals or interpolated strings passed to logging methods) that contain keywords suggesting sensitive data: `password`, `secret`, `token`, `apikey`, `credential`, `ssn`, `creditcard`.

### Why it matters

Logging sensitive data (passwords, tokens, PII) writes them to log files, log aggregation systems, and monitoring dashboards — violating privacy regulations and creating new attack surfaces.

### ❌ Non-compliant

```csharp
_logger.LogDebug($"User login attempt: username={username}, password={password}");
```

### ✅ Compliant

```csharp
_logger.LogDebug("User login attempt: username={Username}", username);
// Never log the password
```
