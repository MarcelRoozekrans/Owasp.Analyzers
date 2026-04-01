---
sidebar_position: 8
---

# A08 — Software and Data Integrity Failures

Data integrity failures occur when code deserializes untrusted data without type restrictions, allowing attackers to instantiate arbitrary types and execute code. These rules detect the most dangerous .NET deserialization patterns.

## OWASPA08001 — BinaryFormatter usage

| Property | Value |
|----------|-------|
| **Severity** | Error |
| **Category** | A08 Data Integrity Failures |

### What it detects

Any use of `BinaryFormatter.Serialize()` or `BinaryFormatter.Deserialize()`.

### Why it matters

`BinaryFormatter` is an inherently unsafe serializer. Deserializing attacker-controlled data with `BinaryFormatter` allows arbitrary code execution (RCE) via gadget chains. Microsoft has [disabled it by default in .NET 5+](https://learn.microsoft.com/en-us/dotnet/standard/serialization/binaryformatter-security-guide) and it will throw by default in modern .NET.

### ❌ Non-compliant

```csharp
var formatter = new BinaryFormatter();
var obj = formatter.Deserialize(stream);
```

### ✅ Compliant

Use a safe alternative:

```csharp
// For general data: System.Text.Json or Newtonsoft.Json
var obj = JsonSerializer.Deserialize<MyType>(stream);

// For binary: MessagePack, Protobuf-net, or similar
var obj = MessagePackSerializer.Deserialize<MyType>(stream);
```

---

## OWASPA08002 — Unsafe deserializer (NetDataContractSerializer / SoapFormatter)

| Property | Value |
|----------|-------|
| **Severity** | Error |
| **Category** | A08 Data Integrity Failures |

### What it detects

Use of `NetDataContractSerializer` or `SoapFormatter`, both of which deserialize arbitrary types and are vulnerable to the same gadget-chain attacks as `BinaryFormatter`.

### ❌ Non-compliant

```csharp
var serializer = new NetDataContractSerializer();
var obj = serializer.ReadObject(stream);
```

### ✅ Compliant

```csharp
var obj = JsonSerializer.Deserialize<MyType>(stream);
```

---

## OWASPA08003 — TypeNameHandling in Newtonsoft.Json

| Property | Value |
|----------|-------|
| **Severity** | Error |
| **Category** | A08 Data Integrity Failures |

### What it detects

`JsonSerializerSettings` with `TypeNameHandling` set to any value other than `None`.

### Why it matters

When `TypeNameHandling` is not `None`, Newtonsoft.Json embeds .NET type names in the JSON and instantiates them during deserialization. This allows attackers who control the JSON to instantiate any type in the process, leading to RCE via known .NET gadget chains.

### ❌ Non-compliant

```csharp
var settings = new JsonSerializerSettings
{
    TypeNameHandling = TypeNameHandling.All  // ❌ RCE risk
};
var obj = JsonConvert.DeserializeObject(json, settings);
```

### ✅ Compliant

```csharp
var settings = new JsonSerializerSettings
{
    TypeNameHandling = TypeNameHandling.None  // default, safe
};
// Or use System.Text.Json which does not support TypeNameHandling
var obj = JsonSerializer.Deserialize<MyType>(json);
```

---

## OWASPA08004 — JavaScriptSerializer with SimpleTypeResolver

| Property | Value |
|----------|-------|
| **Severity** | Error |
| **Category** | A08 Data Integrity Failures |

### What it detects

`JavaScriptSerializer` instantiated with a `SimpleTypeResolver`, which enables polymorphic type resolution and RCE via gadget chains.

### ❌ Non-compliant

```csharp
var serializer = new JavaScriptSerializer(new SimpleTypeResolver());
var obj = serializer.Deserialize<object>(json);
```

### ✅ Compliant

```csharp
// JavaScriptSerializer without a type resolver is safer, but prefer:
var obj = JsonSerializer.Deserialize<MyType>(json);
```

`JavaScriptSerializer` is a legacy API. Prefer `System.Text.Json` for all new code.
