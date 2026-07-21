---
sidebar_position: 10
---

# A10 — Mishandling of Exceptional Conditions

Applications fail in three ways: they don't prevent unusual situations, don't detect them, or respond poorly (or not at all) once they occur. This new OWASP Top 10:2025 category covers improper error handling — including uncaught exceptions and exceptions that are silently discarded instead of being handled or logged.

## OWASPA10001 — Empty catch block (swallowed exception)

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A10 Mishandling of Exceptional Conditions |

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

## OWASPA10002 — Catch block without logging

| Property | Value |
|----------|-------|
| **Severity** | Warning |
| **Category** | A10 Mishandling of Exceptional Conditions |

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
