---
sidebar_position: 1
---

# Installation

## NuGet package

Add the package to your project:

```bash
dotnet add package Owasp.Analyzers
```

Or via the NuGet Package Manager in Visual Studio / Rider — search for `Owasp.Analyzers`.

The package is marked as `DevelopmentDependency` so it will not appear as a transitive runtime dependency in consuming projects.

## Requirements

- .NET SDK 6.0 or later
- Any IDE with Roslyn analyzer support (Visual Studio 2022, JetBrains Rider, VS Code with C# Dev Kit)

## IDE integration

Analyzers are automatically picked up by Visual Studio and Rider. Diagnostics appear as squiggles in the editor and in the Error List / Problems panel.

For VS Code, install the **C# Dev Kit** extension — analyzers run via the language server.

## Configuring severity with `.editorconfig`

You can override the default severity for any rule in your `.editorconfig`:

```ini
[*.cs]
# Downgrade SSRF to a warning instead of an error
dotnet_diagnostic.OWASPA10001.severity = warning

# Suppress CORS wildcard rule entirely
dotnet_diagnostic.OWASPA01004.severity = none
```

Valid values: `error`, `warning`, `suggestion`, `silent`, `none`.

See [Configuration](../configuration) for the full reference.
