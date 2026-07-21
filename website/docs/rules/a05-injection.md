---
sidebar_position: 5
---

# A05 — Injection

Injection vulnerabilities occur when user-controlled data is included in a command or query without proper sanitization. These rules use **taint analysis** to track data from HTTP request sources to dangerous sinks.

See [Taint Engine](../taint-engine) for details on how the analysis works.

## OWASPA05001 — SQL injection

| Property | Value |
|----------|-------|
| **Severity** | Error |
| **Category** | A05 Injection |
| **Technique** | Taint analysis |

### What it detects

User-controlled data (from `HttpContext`, route values, query strings, form fields, or request body) flowing into a SQL command string via string concatenation or interpolation.

### ❌ Non-compliant

```csharp
[HttpGet]
public IActionResult Search(string term)
{
    var sql = "SELECT * FROM Products WHERE Name = '" + term + "'";
    // ❌ OWASPA05001: tainted value reaches SQL command
    return Ok(_db.Execute(sql));
}
```

### ✅ Compliant

```csharp
[HttpGet]
public IActionResult Search(string term)
{
    // Use parameterized queries
    var results = _db.Products
        .Where(p => p.Name == term)
        .ToList();
    return Ok(results);
}
```

Or with raw SQL using parameters:

```csharp
var results = _db.Database
    .SqlQuery<Product>($"SELECT * FROM Products WHERE Name = {term}")
    .ToList();
```

---

## OWASPA05002 — OS command injection

| Property | Value |
|----------|-------|
| **Severity** | Error |
| **Category** | A05 Injection |
| **Technique** | Taint analysis |

### What it detects

User-controlled data flowing into `Process.Start()`, `ProcessStartInfo.FileName`, or `ProcessStartInfo.Arguments`.

### ❌ Non-compliant

```csharp
[HttpPost]
public IActionResult Convert(string filename)
{
    // ❌ OWASPA05002: user input in process arguments
    Process.Start("ffmpeg", $"-i {filename} output.mp4");
    return Ok();
}
```

### ✅ Compliant

```csharp
[HttpPost]
public IActionResult Convert(string filename)
{
    // Validate against an allowlist before using in a command
    if (!IsAllowedFilename(filename))
        return BadRequest("Invalid filename");

    var psi = new ProcessStartInfo("ffmpeg")
    {
        ArgumentList = { "-i", filename, "output.mp4" }  // argument list, not shell string
    };
    Process.Start(psi);
    return Ok();
}
```

---

## OWASPA05003 — Path traversal

| Property | Value |
|----------|-------|
| **Severity** | Error |
| **Category** | A05 Injection |
| **Technique** | Taint analysis |

### What it detects

User-controlled data flowing into `File.ReadAllText`, `File.WriteAllText`, `File.Open`, `Directory.GetFiles`, or similar `System.IO` file operations.

### ❌ Non-compliant

```csharp
[HttpGet]
public IActionResult Download(string path)
{
    // ❌ OWASPA05003: user-controlled path — could be "../../etc/passwd"
    var content = File.ReadAllText(path);
    return Content(content);
}
```

### ✅ Compliant

```csharp
[HttpGet]
public IActionResult Download(string filename)
{
    // Resolve to a safe base directory and verify the result
    var safeBase = Path.GetFullPath("/var/app/uploads");
    var fullPath = Path.GetFullPath(Path.Combine(safeBase, filename));
    if (!fullPath.StartsWith(safeBase, StringComparison.OrdinalIgnoreCase))
        return BadRequest("Invalid path");

    return PhysicalFile(fullPath, "application/octet-stream");
}
```

---

## OWASPA05004 — LDAP injection

| Property | Value |
|----------|-------|
| **Severity** | Error |
| **Category** | A05 Injection |
| **Technique** | Taint analysis |

### What it detects

User-controlled data flowing into LDAP search filter strings (e.g., `DirectorySearcher.Filter`).

### ❌ Non-compliant

```csharp
[HttpGet]
public IActionResult FindUser(string username)
{
    var searcher = new DirectorySearcher();
    // ❌ OWASPA05004: LDAP injection via filter
    searcher.Filter = $"(&(objectClass=user)(sAMAccountName={username}))";
    return Ok(searcher.FindOne());
}
```

### ✅ Compliant

```csharp
[HttpGet]
public IActionResult FindUser(string username)
{
    // Escape special LDAP characters
    var escaped = username.Replace("\\", "\\5c")
                          .Replace("*",  "\\2a")
                          .Replace("(",  "\\28")
                          .Replace(")",  "\\29")
                          .Replace("\0", "\\00");
    var searcher = new DirectorySearcher();
    searcher.Filter = $"(&(objectClass=user)(sAMAccountName={escaped}))";
    return Ok(searcher.FindOne());
}
```

---

## OWASPA05005 — XPath injection

| Property | Value |
|----------|-------|
| **Severity** | Error |
| **Category** | A05 Injection |
| **Technique** | Taint analysis |

### What it detects

User-controlled data flowing into `XPathNavigator.Select()`, `XmlNode.SelectNodes()`, `XmlNode.SelectSingleNode()`, or similar XPath query methods.

### ❌ Non-compliant

```csharp
[HttpGet]
public IActionResult FindProduct(string name)
{
    // ❌ OWASPA05005: XPath injection
    var nodes = doc.SelectNodes($"//Product[Name='{name}']");
    return Ok(nodes?.Count);
}
```

### ✅ Compliant

Use a parameterized XPath API or an `XmlNamespaceManager` with variables, or sanitize the input to only allow safe characters before embedding it in a query.

---

## OWASPA05006 — Cross-site scripting (XSS)

| Property | Value |
|----------|-------|
| **Severity** | Error |
| **Category** | A05 Injection |
| **Technique** | Taint analysis |

### What it detects

User-controlled data flowing into `Response.Write()`, `HtmlHelper.Raw()`, or `Html.Raw()` — sinks that emit unencoded HTML to the response.

### ❌ Non-compliant

```csharp
[HttpGet]
public IActionResult Greet(string name)
{
    // ❌ OWASPA05006: XSS — name is written raw into HTML
    return Content($"<h1>Hello, {name}!</h1>", "text/html");
}
```

### ✅ Compliant

```csharp
[HttpGet]
public IActionResult Greet(string name)
{
    // Encode user input before embedding in HTML
    var encoded = HtmlEncoder.Default.Encode(name);
    return Content($"<h1>Hello, {encoded}!</h1>", "text/html");
}
```

Razor views encode output automatically — only use `@Html.Raw()` with fully trusted content.
