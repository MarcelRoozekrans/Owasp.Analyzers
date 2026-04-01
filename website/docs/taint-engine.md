---
sidebar_position: 12
---

# Taint Engine

Several rules in Owasp.Analyzers use **taint analysis** — an intra-method data-flow analysis that tracks values from untrusted _sources_ to dangerous _sinks_.

## How it works

The taint engine is a Roslyn-based intra-procedural analyzer built on the `SemanticModel` and control-flow graph. It operates on a single method at a time and tracks:

1. **Source** — a value that originates from user-controlled input (HTTP request data)
2. **Propagation** — the value is assigned, passed as an argument, returned from a method, or concatenated into a string
3. **Sink** — the value is used in a dangerous operation (SQL query, file path, HTTP client URL, etc.)

If a tainted value reaches a sink without passing through a sanitizer, the rule raises a diagnostic.

## Sources

The following are recognized as taint sources (user-controlled input):

| Source | Examples |
|--------|---------|
| ASP.NET Core action parameters | `string id`, `MyDto dto` on controller actions |
| `HttpContext.Request.Query` | `.Query["param"]`, `.QueryString` |
| `HttpContext.Request.Form` | `.Form["field"]`, `.Form.Files` |
| `HttpContext.Request.RouteValues` | `.RouteValues["id"]` |
| `HttpContext.Request.Headers` | `.Headers["X-Custom"]` |
| `HttpContext.Request.Body` / `BodyReader` | Stream reads from the request body |
| `HttpContext.Request.Cookies` | `.Cookies["name"]` |

## Sinks

| Sink | Rule |
|------|------|
| SQL command string (concatenation/interpolation) | OWASPA03001 |
| `Process.Start()` arguments | OWASPA03002 |
| `File.*` / `Directory.*` / `Path.Combine` paths | OWASPA03003 |
| `DirectorySearcher.Filter` | OWASPA03004 |
| `XPathNavigator.Select()` / `XmlNode.SelectNodes()` | OWASPA03005 |
| `Response.Write()` / `Html.Raw()` | OWASPA03006 |
| `ILogger.*` message arguments | OWASPA09003 |
| `HttpClient.GetAsync/PostAsync/SendAsync` URL | OWASPA10001 |
| `WebClient.DownloadString/DownloadData` URL | OWASPA10002 |

## Propagation

The engine tracks taint through:

- **Variable assignment** — `var x = taintedValue; Sink(x);`
- **String concatenation and interpolation** — `"SELECT * WHERE id = " + tainted`
- **Method return values** — if a tainted value is passed into a method that returns it
- **Object construction** — tainted values passed to constructors (e.g., `new Uri(tainted)`)

## Sanitizers

The engine stops tracking a tainted value when it passes through a recognized sanitizer:

| Sanitizer | Effect |
|-----------|--------|
| `int.Parse()`, `Guid.Parse()`, `Enum.Parse()` | Parsing to a non-string type removes taint |
| `HtmlEncoder.Default.Encode()` | Removes taint for XSS sinks |
| `Uri` construction with `UriKind.Absolute` validation | Removes taint for SSRF sinks |
| Regex match with `^[a-zA-Z0-9]+$` pattern | Removes taint (allowlist pattern) |

## Limitations

The taint engine is **intra-procedural** — it only analyzes within a single method body. It does not track taint across method boundaries, class fields, or asynchronous continuations.

This means:
- If user input is passed to a helper method and that helper passes it to a sink, the rule will **not** fire.
- If user input is stored in a field and read later, the rule will **not** fire.

This is a conscious trade-off: intra-procedural analysis has zero false negatives within its scope and no false positives from path confusion across methods.

For deeper inter-procedural analysis, consider tools like [Semgrep](https://semgrep.dev/) or [CodeQL](https://codeql.github.com/) as a complement to Owasp.Analyzers.
