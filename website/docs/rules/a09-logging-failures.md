---
sidebar_position: 9
---

# A09 — Security Logging and Alerting Failures

Great logging with no alerting is of minimal value in identifying security incidents. These rules detect log injection vulnerabilities and accidental logging of sensitive data.

## OWASPA09001 — Log injection via taint

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A09 Security Logging and Alerting Failures |
| **Technique** | Taint analysis |

### What it detects

User-controlled data flowing directly into a logging call without sanitization. An attacker who can inject newlines into log messages can forge log entries and obscure evidence of an attack.

### ❌ Non-compliant

```csharp
[HttpGet]
public IActionResult GetItem(string id)
{
    // ❌ OWASPA09001: user input injected into log
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

## OWASPA09002 — Sensitive data in log message

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A09 Security Logging and Alerting Failures |

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
