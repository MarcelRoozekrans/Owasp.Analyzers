# Docusaurus Website & Documentation Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Scaffold a Docusaurus 3.7 website in `website/`, write extensive per-rule documentation for all 37 OWASP analyzer rules, deploy to GitHub Pages, and rewrite `README.md`.

**Architecture:** `website/` mirrors AdoNet.Async's layout — Docusaurus 3.7 + React 19, `website/docs/` holds all markdown, GitHub Actions deploys on push to `website/**`. The existing `docs/plans/` folder is untouched (internal plans only).

**Tech Stack:** Docusaurus 3.7, React 19, MDX 3, Node 24, GitHub Pages, GitHub Actions

---

### Task 1: Scaffold Docusaurus project

**Files:**
- Create: `website/package.json`
- Create: `website/.gitignore`
- Create: `website/docusaurus.config.js`
- Create: `website/sidebars.js`
- Create: `website/static/.gitkeep`
- Create: `website/src/css/custom.css`

**Step 1: Create `website/package.json`**

```json
{
  "name": "owasp-analyzers-docs",
  "version": "0.0.0",
  "private": true,
  "scripts": {
    "docusaurus": "docusaurus",
    "start": "docusaurus start",
    "build": "docusaurus build",
    "swizzle": "docusaurus swizzle",
    "deploy": "docusaurus deploy",
    "clear": "docusaurus clear",
    "serve": "docusaurus serve"
  },
  "dependencies": {
    "@docusaurus/core": "^3.7.0",
    "@docusaurus/preset-classic": "^3.7.0",
    "@mdx-js/react": "^3.0.0",
    "clsx": "^2.0.0",
    "prism-react-renderer": "^2.3.0",
    "react": "^19.0.0",
    "react-dom": "^19.0.0"
  },
  "devDependencies": {
    "@docusaurus/module-type-aliases": "^3.7.0",
    "@docusaurus/types": "^3.7.0"
  },
  "browserslist": {
    "production": [">0.5%", "not dead", "not op_mini all"],
    "development": ["last 3 chrome version", "last 3 firefox version", "last 5 safari version"]
  },
  "engines": {
    "node": ">=18.0"
  }
}
```

**Step 2: Create `website/.gitignore`**

```
node_modules/
build/
.docusaurus/
```

**Step 3: Create `website/src/css/custom.css`**

```css
:root {
  --ifm-color-primary: #e8162b;
  --ifm-color-primary-dark: #d11427;
  --ifm-color-primary-darker: #c61325;
  --ifm-color-primary-darkest: #a3101e;
  --ifm-color-primary-light: #ea2c3f;
  --ifm-color-primary-lighter: #ec3849;
  --ifm-color-primary-lightest: #f05d6d;
  --ifm-code-font-size: 95%;
  --docusaurus-highlighted-code-line-bg: rgba(0, 0, 0, 0.1);
}

[data-theme='dark'] {
  --ifm-color-primary: #f05d6d;
  --ifm-color-primary-dark: #ed4457;
  --ifm-color-primary-darker: #eb3a4e;
  --ifm-color-primary-darkest: #e61329;
  --ifm-color-primary-light: #f3768396;
  --ifm-color-primary-lighter: #f47f8b;
  --ifm-color-primary-lightest: #f8a3ab;
  --docusaurus-highlighted-code-line-bg: rgba(0, 0, 0, 0.3);
}
```

**Step 4: Create `website/docusaurus.config.js`**

```js
// @ts-check
import { themes as prismThemes } from 'prism-react-renderer';

/** @type {import('@docusaurus/types').Config} */
const config = {
  title: 'Owasp.Analyzers',
  tagline: 'Roslyn analyzers for the OWASP Top 10 — catch security issues at compile time',
  favicon: 'img/favicon.ico',

  url: 'https://marcelroozekrans.github.io',
  baseUrl: '/Owasp.Analyzers/',

  organizationName: 'MarcelRoozekrans',
  projectName: 'Owasp.Analyzers',

  onBrokenLinks: 'throw',
  onBrokenMarkdownLinks: 'warn',

  i18n: {
    defaultLocale: 'en',
    locales: ['en'],
  },

  presets: [
    [
      'classic',
      /** @type {import('@docusaurus/preset-classic').Options} */
      ({
        docs: {
          sidebarPath: './sidebars.js',
          editUrl: 'https://github.com/MarcelRoozekrans/Owasp.Analyzers/tree/main/website/',
        },
        blog: false,
        theme: {
          customCss: './src/css/custom.css',
        },
      }),
    ],
  ],

  themeConfig:
    /** @type {import('@docusaurus/preset-classic').ThemeConfig} */
    ({
      image: 'img/social-card.png',
      navbar: {
        title: 'Owasp.Analyzers',
        logo: {
          alt: 'Owasp.Analyzers Logo',
          src: 'img/logo.png',
        },
        items: [
          {
            type: 'docSidebar',
            sidebarId: 'docsSidebar',
            position: 'left',
            label: 'Docs',
          },
          {
            href: 'https://github.com/MarcelRoozekrans/Owasp.Analyzers',
            label: 'GitHub',
            position: 'right',
          },
          {
            href: 'https://www.nuget.org/packages/Owasp.Analyzers',
            label: 'NuGet',
            position: 'right',
          },
        ],
      },
      footer: {
        style: 'dark',
        links: [
          {
            title: 'Docs',
            items: [
              { label: 'Introduction', to: '/docs/intro' },
              { label: 'Getting Started', to: '/docs/getting-started/installation' },
              { label: 'Rules', to: '/docs/rules/a01-broken-access-control' },
            ],
          },
          {
            title: 'Community',
            items: [
              { label: 'GitHub', href: 'https://github.com/MarcelRoozekrans/Owasp.Analyzers' },
              { label: 'Issues', href: 'https://github.com/MarcelRoozekrans/Owasp.Analyzers/issues' },
            ],
          },
          {
            title: 'More',
            items: [
              { label: 'NuGet', href: 'https://www.nuget.org/packages/Owasp.Analyzers' },
              { label: 'OWASP Top 10', href: 'https://owasp.org/Top10/' },
              { label: 'License (MIT)', href: 'https://github.com/MarcelRoozekrans/Owasp.Analyzers/blob/main/LICENSE' },
            ],
          },
        ],
        copyright: `Copyright © ${new Date().getFullYear()} Marcel Roozekrans. Built with Docusaurus.`,
      },
      prism: {
        theme: prismThemes.github,
        darkTheme: prismThemes.dracula,
        additionalLanguages: ['csharp', 'bash', 'json', 'xml'],
      },
    }),
};

export default config;
```

**Step 5: Create `website/sidebars.js`**

```js
/** @type {import('@docusaurus/plugin-content-docs').SidebarsConfig} */
const sidebars = {
  docsSidebar: [
    'intro',
    {
      type: 'category',
      label: 'Getting Started',
      items: ['getting-started/installation', 'getting-started/quick-start'],
    },
    {
      type: 'category',
      label: 'Rules',
      items: [
        'rules/a01-broken-access-control',
        'rules/a02-cryptographic-failures',
        'rules/a03-injection',
        'rules/a04-insecure-design',
        'rules/a05-security-misconfiguration',
        'rules/a06-vulnerable-components',
        'rules/a07-authentication-failures',
        'rules/a08-data-integrity',
        'rules/a09-logging-failures',
        'rules/a10-ssrf',
      ],
    },
    'configuration',
    'taint-engine',
  ],
};

export default sidebars;
```

**Step 6: Install dependencies and verify build**

```bash
cd c:/Projects/Prive/Owasp.Analyzers/website
npm install
npm run build
```

Expected: `website/build/` directory created, no errors.

**Step 7: Copy logo to static assets**

```bash
mkdir -p website/static/img
cp assets/logo.png website/static/img/logo.png
cp assets/logo.png website/static/img/social-card.png
```

**Step 8: Commit**

```bash
git add website/
git commit -m "feat: scaffold Docusaurus website"
```

---

### Task 2: Write `intro.md` and getting-started docs

**Files:**
- Create: `website/docs/intro.md`
- Create: `website/docs/getting-started/installation.md`
- Create: `website/docs/getting-started/quick-start.md`

**Step 1: Create `website/docs/intro.md`**

```markdown
---
sidebar_position: 1
slug: /intro
---

# Introduction

**Owasp.Analyzers** is a collection of Roslyn diagnostic analyzers that surface [OWASP Top 10 2021](https://owasp.org/Top10/) security vulnerabilities as compiler warnings and errors in your C#/.NET projects.

## How it works

Roslyn analyzers run inside the compiler pipeline — no external tools, no CI-only scans. Every build checks your code against the rules. Violations appear inline in your IDE (Visual Studio, Rider, VS Code) and as `dotnet build` output, exactly like ordinary compiler warnings.

```
warning OWASPA01001: Action 'GetProfile' is not decorated with [Authorize] or [AllowAnonymous]
error   OWASPA03001: User-controlled data flows into SQL command without parameterization
```

## Coverage

| Category | Rules | Technique |
|----------|-------|-----------|
| [A01 Broken Access Control](./rules/a01-broken-access-control) | 5 | Syntax / Semantic |
| [A02 Cryptographic Failures](./rules/a02-cryptographic-failures) | 8 | Syntax / Semantic |
| [A03 Injection](./rules/a03-injection) | 6 | Taint analysis |
| [A04 Insecure Design](./rules/a04-insecure-design) | 1 | Syntax |
| [A05 Security Misconfiguration](./rules/a05-security-misconfiguration) | 6 | Syntax |
| [A06 Vulnerable Components](./rules/a06-vulnerable-components) | 2 | MSBuild target |
| [A07 Authentication Failures](./rules/a07-authentication-failures) | 5 | Semantic |
| [A08 Data Integrity Failures](./rules/a08-data-integrity) | 4 | Semantic |
| [A09 Logging Failures](./rules/a09-logging-failures) | 4 | Syntax / Taint |
| [A10 SSRF](./rules/a10-ssrf) | 3 | Taint analysis |

## What's next?

- [Install the package](./getting-started/installation)
- [See your first diagnostic](./getting-started/quick-start)
- [Browse the rules](./rules/a01-broken-access-control)
```

**Step 2: Create `website/docs/getting-started/installation.md`**

```markdown
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
```

**Step 3: Create `website/docs/getting-started/quick-start.md`**

```markdown
---
sidebar_position: 2
---

# Quick Start

This guide walks you through triggering your first diagnostic and fixing it.

## 1. Install the package

```bash
dotnet add package Owasp.Analyzers
```

## 2. Write code that triggers a rule

Add this to any ASP.NET Core controller:

```csharp
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    [HttpGet("{id}")]
    // ❌ OWASPA01001: missing [Authorize] or [AllowAnonymous]
    public IActionResult GetUser(int id) { ... }
}
```

## 3. Build

```bash
dotnet build
```

You will see:

```
warning OWASPA01001: Action 'GetUser' is not decorated with [Authorize] or [AllowAnonymous]
```

## 4. Fix it

```csharp
[HttpGet("{id}")]
[Authorize]          // ✅ explicit authorization declared
public IActionResult GetUser(int id) { ... }
```

## 5. Suppress a false positive

If a rule fires where you have intentionally accepted the risk, suppress it inline:

```csharp
#pragma warning disable OWASPA01004 // CORS wildcard — intentional for public API
app.UseCors(b => b.AllowAnyOrigin());
#pragma warning restore OWASPA01004
```

Or permanently in `.editorconfig`:

```ini
dotnet_diagnostic.OWASPA01004.severity = none
```

## Next steps

- [Browse all rules](../rules/a01-broken-access-control)
- [Configure severities](../configuration)
- [Learn about taint analysis](../taint-engine)
```

**Step 4: Commit**

```bash
git add website/docs/
git commit -m "docs: add intro and getting-started pages"
```

---

### Task 3: Write A01 and A02 rule pages

**Files:**
- Create: `website/docs/rules/a01-broken-access-control.md`
- Create: `website/docs/rules/a02-cryptographic-failures.md`

**Step 1: Create `website/docs/rules/a01-broken-access-control.md`**

```markdown
---
sidebar_position: 1
---

# A01 — Broken Access Control

OWASP A01 covers failures that allow users to act outside their intended permissions. This category is the #1 most prevalent web application security risk.

## OWASPA01001 — Controller action missing authorization

**Severity:** Warning

Every public controller action must explicitly declare its authorization intent. An action with neither `[Authorize]` nor `[AllowAnonymous]` is ambiguous and may be accidentally exposed.

```csharp
// ❌ Triggers OWASPA01001
[HttpGet("{id}")]
public IActionResult GetProfile(int id) => Ok();

// ✅ Fix: declare intent explicitly
[HttpGet("{id}")]
[Authorize]
public IActionResult GetProfile(int id) => Ok();

// ✅ Also valid: opt out deliberately
[HttpGet("public")]
[AllowAnonymous]
public IActionResult GetPublicInfo() => Ok();
```

**Fix:** Add `[Authorize]` (or `[AllowAnonymous]` for intentionally public actions). You can also place `[Authorize]` on the controller class to apply it to all actions.

---

## OWASPA01002 — Hardcoded role in `[Authorize]`

**Severity:** Warning

Hardcoded role string literals in `[Authorize(Roles = "...")]` are brittle — a typo silently grants access to no one (or everyone). Use constants or policy names instead.

```csharp
// ❌ Triggers OWASPA01002
[Authorize(Roles = "Admin")]
public IActionResult DeleteUser(int id) => Ok();

// ✅ Fix: use a constant
public static class Roles
{
    public const string Admin = "Admin";
}

[Authorize(Roles = Roles.Admin)]
public IActionResult DeleteUser(int id) => Ok();
```

---

## OWASPA01003 — Hardcoded string in `User.IsInRole()`

**Severity:** Warning

Same problem as OWASPA01002 but in imperative code.

```csharp
// ❌ Triggers OWASPA01003
if (User.IsInRole("Moderator")) { ... }

// ✅ Fix: use a constant
if (User.IsInRole(Roles.Moderator)) { ... }
```

---

## OWASPA01004 — CORS wildcard allows any origin

**Severity:** Warning

`AllowAnyOrigin()` allows cross-origin requests from any domain. This bypasses the Same-Origin Policy and can expose APIs to CSRF-like attacks in browsers.

```csharp
// ❌ Triggers OWASPA01004
app.UseCors(b => b.AllowAnyOrigin().AllowAnyMethod());

// ✅ Fix: restrict to known origins
app.UseCors(b => b
    .WithOrigins("https://myapp.example.com")
    .AllowAnyMethod()
    .AllowAnyHeader());
```

---

## OWASPA01005 — Missing `[ValidateAntiForgeryToken]`

**Severity:** Warning

State-changing actions (`[HttpPost]`, `[HttpPut]`, `[HttpDelete]`, `[HttpPatch]`) without `[ValidateAntiForgeryToken]` are vulnerable to Cross-Site Request Forgery (CSRF).

```csharp
// ❌ Triggers OWASPA01005
[HttpPost]
public IActionResult UpdateProfile(ProfileModel model) => Ok();

// ✅ Fix: validate the anti-forgery token
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult UpdateProfile(ProfileModel model) => Ok();
```

**Note:** If you use cookie-based auth with SPA clients, configure the anti-forgery middleware globally instead of per-action.
```

**Step 2: Create `website/docs/rules/a02-cryptographic-failures.md`**

```markdown
---
sidebar_position: 2
---

# A02 — Cryptographic Failures

OWASP A02 covers failures related to cryptography — using weak algorithms, exposing data in transit, and hardcoding secrets.

## OWASPA02001 — Weak cryptographic algorithm

**Severity:** Warning

MD5, SHA1, DES, RC2, and TripleDES are broken or deprecated. Use AES-GCM for encryption and SHA-256 or SHA-3 for hashing.

```csharp
// ❌ Triggers OWASPA02001
using var md5 = MD5.Create();
var hash = md5.ComputeHash(data);

// ✅ Fix: use SHA-256
using var sha256 = SHA256.Create();
var hash = sha256.ComputeHash(data);
```

**Weak algorithms detected:** `MD5`, `SHA1`, `SHA1Managed`, `MD5CryptoServiceProvider`, `DES`, `DESCryptoServiceProvider`, `RC2`, `RC2CryptoServiceProvider`, `TripleDES`, `TripleDESCryptoServiceProvider`.

---

## OWASPA02002 — ECB cipher mode is insecure

**Severity:** Warning

AES in ECB mode is deterministic — identical plaintext blocks produce identical ciphertext blocks, leaking patterns. Use AES-GCM or AES-CBC with HMAC authentication.

```csharp
// ❌ Triggers OWASPA02002
aes.Mode = CipherMode.ECB;

// ✅ Fix: use GCM (authenticated encryption)
using var aesGcm = new AesGcm(key, AesGcm.TagByteSizes.MaxSize);
```

---

## OWASPA02003 — `System.Random` is not cryptographically secure

**Severity:** Info

`System.Random` uses a deterministic PRNG — do not use it for tokens, session IDs, passwords, or any security-sensitive value.

```csharp
// ❌ Triggers OWASPA02003
var token = new Random().Next().ToString();

// ✅ Fix: use a cryptographic RNG
var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
```

---

## OWASPA02004 — Hardcoded cryptographic key

**Severity:** Error

Hardcoded keys committed to source control are a permanent secret compromise.

```csharp
// ❌ Triggers OWASPA02004
byte[] key = { 0x01, 0x02, 0x03, 0x04, ... };

// ✅ Fix: load from configuration / secret store
var key = Convert.FromBase64String(configuration["Crypto:Key"]!);
```

**Variable names detected:** names containing `key`, `secret`, `password`, `salt`, `nonce`, `hmackey` (case-insensitive).

---

## OWASPA02005 — Legacy TLS protocol

**Severity:** Warning

`Ssl3`, `Tls` (1.0), and `Tls11` are deprecated and vulnerable to POODLE, BEAST, and other attacks.

```csharp
// ❌ Triggers OWASPA02005
ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

// ✅ Fix: allow only TLS 1.2 and 1.3
ServicePointManager.SecurityProtocol =
    SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
```

---

## OWASPA02006 — Certificate validation disabled

**Severity:** Error

A callback that always returns `true` disables TLS certificate verification entirely — man-in-the-middle attacks become trivial.

```csharp
// ❌ Triggers OWASPA02006
handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

// ✅ Fix: remove the callback (use default OS validation)
// Or for custom CAs:
handler.ServerCertificateCustomValidationCallback =
    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator; // only in trusted dev environments
```

---

## OWASPA02007 — Hardcoded HTTP URL

**Severity:** Warning

HTTP URLs transmit data in plaintext. Use HTTPS, especially for API endpoints and authentication flows.

```csharp
// ❌ Triggers OWASPA02007
var url = "http://api.example.com/users";

// ✅ Fix: use HTTPS
var url = "https://api.example.com/users";
```

---

## OWASPA02008 — Missing HSTS middleware

**Severity:** Warning

`UseHsts()` sends the `Strict-Transport-Security` header, instructing browsers to always use HTTPS. Without it, users are vulnerable on their first HTTP request.

```csharp
// ❌ Triggers OWASPA02008 — UseHsts() missing from Configure/ConfigureApp
public void Configure(IApplicationBuilder app)
{
    app.UseRouting();
    app.UseAuthorization();
}

// ✅ Fix
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (!env.IsDevelopment())
        app.UseHsts();

    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseAuthorization();
}
```
```

**Step 3: Commit**

```bash
git add website/docs/rules/
git commit -m "docs: add A01 and A02 rule pages"
```

---

### Task 4: Write A03 through A05 rule pages

**Files:**
- Create: `website/docs/rules/a03-injection.md`
- Create: `website/docs/rules/a04-insecure-design.md`
- Create: `website/docs/rules/a05-security-misconfiguration.md`

**Step 1: Create `website/docs/rules/a03-injection.md`**

```markdown
---
sidebar_position: 3
---

# A03 — Injection

OWASP A03 covers injection vulnerabilities — SQL, OS command, path traversal, LDAP, XPath, and XSS — where user-controlled data reaches a dangerous sink without sanitization.

All A03 rules use the built-in **taint engine**. See [Taint Engine](../taint-engine) for how it works.

## OWASPA03001 — SQL Injection

**Severity:** Error

User input that flows into a SQL `CommandText` property without parameterization allows attackers to manipulate queries, bypass auth, and exfiltrate data.

```csharp
// ❌ Triggers OWASPA03001
public IActionResult GetUser(string username)
{
    var cmd = new SqlCommand();
    cmd.CommandText = "SELECT * FROM Users WHERE Name = '" + username + "'";
    // ...
}

// ✅ Fix: use parameterized queries
public IActionResult GetUser(string username)
{
    var cmd = new SqlCommand("SELECT * FROM Users WHERE Name = @name");
    cmd.Parameters.AddWithValue("@name", username);
    // ...
}
```

**Tracked sinks:** `SqlCommand.CommandText`, `NpgsqlCommand.CommandText`, `MySqlCommand.CommandText`.

---

## OWASPA03002 — OS Command Injection

**Severity:** Error

User input flowing into `Process.Start()` or `ProcessStartInfo.FileName`/`.Arguments` allows arbitrary command execution on the host.

```csharp
// ❌ Triggers OWASPA03002
public IActionResult RunReport(string reportName)
{
    Process.Start("generate-report.sh", reportName);
    // ...
}

// ✅ Fix: validate against an allowlist
private static readonly string[] AllowedReports = ["sales", "inventory"];

public IActionResult RunReport(string reportName)
{
    if (!AllowedReports.Contains(reportName))
        return BadRequest();

    Process.Start("generate-report.sh", reportName);
    // ...
}
```

---

## OWASPA03003 — Path Traversal

**Severity:** Error

User input in file system paths allows attackers to read or write arbitrary files (e.g. `../../etc/passwd`).

```csharp
// ❌ Triggers OWASPA03003
public IActionResult Download(string filename)
{
    var content = File.ReadAllBytes("uploads/" + filename);
    return File(content, "application/octet-stream");
}

// ✅ Fix: sanitize and validate
public IActionResult Download(string filename)
{
    var safeName = Path.GetFileName(filename); // strip directory components
    var fullPath = Path.GetFullPath(Path.Combine("uploads", safeName));

    if (!fullPath.StartsWith(Path.GetFullPath("uploads")))
        return BadRequest();

    return File(System.IO.File.ReadAllBytes(fullPath), "application/octet-stream");
}
```

---

## OWASPA03004 — LDAP Injection

**Severity:** Error

Unsanitized input in LDAP filter strings allows directory traversal and authentication bypass.

```csharp
// ❌ Triggers OWASPA03004
var filter = "(uid=" + username + ")";
searcher.Filter = filter;

// ✅ Fix: escape LDAP special characters
var filter = $"(uid={EscapeLdap(username)})";
```

---

## OWASPA03005 — XPath Injection

**Severity:** Error

Unsanitized user input in XPath queries can expose or modify XML data.

```csharp
// ❌ Triggers OWASPA03005
var nodes = doc.SelectNodes($"//user[name='{username}']");

// ✅ Fix: use parameterized XPath (XPathExpression + XsltArgumentList)
var expr = XPathExpression.Compile("//user[name=$name]");
var args = new XsltArgumentList();
args.AddParam("name", "", username);
```

---

## OWASPA03006 — Cross-Site Scripting (XSS)

**Severity:** Error

User input written directly to the HTTP response without HTML-encoding allows script injection.

```csharp
// ❌ Triggers OWASPA03006
Response.Write("<p>Hello " + username + "</p>");

// ✅ Fix: encode before rendering
Response.Write("<p>Hello " + HtmlEncoder.Default.Encode(username) + "</p>");
```

**Sanitizer methods recognized by the taint engine:** `HtmlEncode`, `UrlEncode`, `EscapeDataString`, `Encode`, `HtmlAttributeEncode`, `JavaScriptStringEncode`.
```

**Step 2: Create `website/docs/rules/a04-insecure-design.md`**

```markdown
---
sidebar_position: 4
---

# A04 — Insecure Design

OWASP A04 covers missing or ineffective security controls at the design level.

## OWASPA04002 — Missing rate limiting on auth endpoint

**Severity:** Warning

Authentication endpoints (login, sign-in, token, authenticate) without rate limiting are vulnerable to brute-force credential attacks.

```csharp
// ❌ Triggers OWASPA04002
[HttpPost]
public IActionResult Login(LoginModel model)
{
    // No rate limiting — attacker can try millions of passwords
    var user = _userService.Authenticate(model.Username, model.Password);
    // ...
}

// ✅ Fix: apply rate limiting
[HttpPost]
[EnableRateLimiting("auth")]   // ASP.NET Core rate limiting middleware
public IActionResult Login(LoginModel model) { ... }
```

**Detection:** Any `[HttpPost]` or `[HttpGet]` method whose name contains `login`, `signin`, `authenticate`, `token`, or `auth` (case-insensitive) without a `RateLimit` attribute.

**Setup for ASP.NET Core rate limiting:**

```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("auth", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromMinutes(1);
    });
});
```
```

**Step 3: Create `website/docs/rules/a05-security-misconfiguration.md`**

```markdown
---
sidebar_position: 5
---

# A05 — Security Misconfiguration

OWASP A05 covers insecure default configurations, overly permissive settings, and missing hardening.

## OWASPA05001 — Developer exception page in production

**Severity:** Warning

`UseDeveloperExceptionPage()` returns full stack traces to clients. This must only run in the `Development` environment.

```csharp
// ❌ Triggers OWASPA05001 — no environment check
app.UseDeveloperExceptionPage();

// ✅ Fix: guard with IsDevelopment()
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseExceptionHandler("/Error");
```

---

## OWASPA05002 — Missing HTTPS redirection

**Severity:** Warning

Without `UseHttpsRedirection()`, HTTP requests are served as-is rather than redirected to HTTPS.

```csharp
// ❌ Missing from Configure/ConfigureApp
public void Configure(IApplicationBuilder app)
{
    app.UseRouting();
}

// ✅ Fix
public void Configure(IApplicationBuilder app)
{
    app.UseHttpsRedirection();
    app.UseRouting();
}
```

---

## OWASPA05003 — Directory browsing enabled

**Severity:** Warning

`UseDirectoryBrowser()` exposes the full file listing of a directory to any visitor.

```csharp
// ❌ Triggers OWASPA05003
app.UseDirectoryBrowser();

// ✅ Fix: remove it, or restrict access
app.UseDirectoryBrowser(new DirectoryBrowserOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(env.WebRootPath, "downloads")),
    RequestPath = "/downloads"
});
// And protect the route with authorization middleware
```

---

## OWASPA05004 — Error details exposed

**Severity:** Warning

Setting `IncludeErrorDetails = true` exposes internal exception details to API clients.

```csharp
// ❌ Triggers OWASPA05004
options.IncludeErrorDetails = true;

// ✅ Fix: only enable in development
options.IncludeErrorDetails = env.IsDevelopment();
```

---

## OWASPA05005 — Antiforgery not configured

**Severity:** Warning

Without `AddAntiforgery()`, CSRF protection is not available even if `[ValidateAntiForgeryToken]` is used.

```csharp
// ❌ Missing from ConfigureServices
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
}

// ✅ Fix
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllersWithViews();
    services.AddAntiforgery();
}
```

---

## OWASPA05006 — Hardcoded credential

**Severity:** Error

Credentials committed to source control are permanently compromised.

```csharp
// ❌ Triggers OWASPA05006
string password = "Sup3rS3cr3t!";
string apiKey = "sk-1234abcd";

// ✅ Fix: load from environment / secret store
string password = configuration["Auth:Password"]!;
string apiKey = Environment.GetEnvironmentVariable("API_KEY")!;
```

**Variable names detected:** names containing `password`, `passwd`, `pwd`, `credential`, `secret`, `apikey`, `api_key` (case-insensitive) assigned a non-empty string literal.
```

**Step 4: Commit**

```bash
git add website/docs/rules/
git commit -m "docs: add A03, A04, A05 rule pages"
```

---

### Task 5: Write A06 through A08 rule pages

**Files:**
- Create: `website/docs/rules/a06-vulnerable-components.md`
- Create: `website/docs/rules/a07-authentication-failures.md`
- Create: `website/docs/rules/a08-data-integrity.md`

**Step 1: Create `website/docs/rules/a06-vulnerable-components.md`**

```markdown
---
sidebar_position: 6
---

# A06 — Vulnerable and Outdated Components

OWASP A06 covers the use of components (libraries, frameworks) with known vulnerabilities or that are no longer maintained.

Unlike other categories, A06 is implemented as an **MSBuild target** (not a Roslyn analyzer) because it must query the NuGet advisory database at build time.

## OWASPA06001 — Vulnerable NuGet packages

**Severity:** Warning (MSBuild)

The `OwaspA06VulnerabilityCheck` target runs `dotnet list package --vulnerable --include-transitive` after every build and surfaces vulnerable packages as MSBuild warnings.

**Example output:**

```
warning : [OWASPA06] Vulnerable package detected: Newtonsoft.Json 12.0.1
          Severity: High | CVE-2024-21907
          Fix: upgrade to >= 13.0.1
```

**Fix:** Update the affected package:

```bash
dotnet add package Newtonsoft.Json --version 13.0.3
```

---

## OWASPA06002 — Deprecated NuGet packages

**Severity:** Warning (MSBuild)

Runs `dotnet list package --deprecated --include-transitive` and warns on deprecated packages that should be replaced.

**Fix:** Follow the deprecation notice to find the recommended replacement package.

---

## Disabling A06 checks

If the vulnerability check causes build slowness (it makes a network call), you can disable it per project:

```xml
<!-- In your .csproj -->
<PropertyGroup>
  <OwaspA06Enabled>false</OwaspA06Enabled>
</PropertyGroup>
```
```

**Step 2: Create `website/docs/rules/a07-authentication-failures.md`**

```markdown
---
sidebar_position: 7
---

# A07 — Identification and Authentication Failures

OWASP A07 covers weaknesses in authentication mechanisms — JWT algorithm confusion, disabled token validation, and insecure cookie configuration.

## OWASPA07001 — JWT algorithm bypass via `SecurityAlgorithms.None`

**Severity:** Error

`SecurityAlgorithms.None` disables JWT signature verification entirely, allowing any unsigned token to be accepted.

```csharp
// ❌ Triggers OWASPA07001
var token = new JwtSecurityToken(
    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.None)
);

// ✅ Fix: use a secure algorithm
var token = new JwtSecurityToken(
    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
);
```

---

## OWASPA07002 — Token lifetime validation disabled

**Severity:** Warning

Setting `ValidateLifetime = false` means expired JWT tokens are accepted indefinitely — a compromised token never expires.

```csharp
// ❌ Triggers OWASPA07002
var parameters = new TokenValidationParameters
{
    ValidateLifetime = false,
    // ...
};

// ✅ Fix: keep lifetime validation enabled (it's the default)
var parameters = new TokenValidationParameters
{
    ValidateLifetime = true,
    ClockSkew = TimeSpan.FromMinutes(1),
    // ...
};
```

---

## OWASPA07003 — Token signing key validation disabled

**Severity:** Error

`ValidateIssuerSigningKey = false` means the token signature is not verified — any token is accepted regardless of its signature.

```csharp
// ❌ Triggers OWASPA07003
var parameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = false,
};

// ✅ Fix
var parameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
};
```

---

## OWASPA07004 — `CookieOptions` missing `HttpOnly` or `Secure` flag

**Severity:** Warning

Without `HttpOnly = true`, JavaScript can read the cookie (XSS token theft). Without `Secure = true`, the cookie is sent over HTTP.

```csharp
// ❌ Triggers OWASPA07004
var options = new CookieOptions
{
    Expires = DateTimeOffset.UtcNow.AddHours(1)
    // HttpOnly and Secure not set
};

// ✅ Fix
var options = new CookieOptions
{
    HttpOnly = true,
    Secure = true,
    SameSite = SameSiteMode.Strict,
    Expires = DateTimeOffset.UtcNow.AddHours(1)
};
```

---

## OWASPA07005 — `SameSite=None` cookie without `Secure` flag

**Severity:** Warning

`SameSite = SameSiteMode.None` is required for cross-site cookies (e.g. embedded iframes) but browsers reject such cookies unless `Secure = true`.

```csharp
// ❌ Triggers OWASPA07005
var options = new CookieOptions
{
    SameSite = SameSiteMode.None,
    // Secure not set
};

// ✅ Fix
var options = new CookieOptions
{
    SameSite = SameSiteMode.None,
    Secure = true,
};
```
```

**Step 3: Create `website/docs/rules/a08-data-integrity.md`**

```markdown
---
sidebar_position: 8
---

# A08 — Software and Data Integrity Failures

OWASP A08 covers deserialization of untrusted data using unsafe deserializers that can lead to remote code execution.

## OWASPA08001 — `BinaryFormatter` is unsafe

**Severity:** Error

`BinaryFormatter` is officially [removed in .NET 9](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9/runtime#binaryformatter-removed) and has been flagged as unsafe for years. Deserializing untrusted data with it enables remote code execution.

```csharp
// ❌ Triggers OWASPA08001
var formatter = new BinaryFormatter();
var obj = formatter.Deserialize(stream);

// ✅ Fix: use System.Text.Json or XmlSerializer
var obj = JsonSerializer.Deserialize<MyType>(stream);
```

---

## OWASPA08002 — Unsafe deserializer usage

**Severity:** Error

Several legacy .NET serializers are unsafe for untrusted input.

**Flagged types:** `NetDataContractSerializer`, `SoapFormatter`, `LosFormatter`, `ObjectStateFormatter`.

```csharp
// ❌ Triggers OWASPA08002
var serializer = new NetDataContractSerializer();
var obj = serializer.ReadObject(stream);

// ✅ Fix: migrate to System.Text.Json or DataContractSerializer
// (DataContractSerializer does NOT allow type polymorphism by default)
var serializer = new DataContractSerializer(typeof(MyType));
```

---

## OWASPA08003 — Unsafe `TypeNameHandling` in Json.NET

**Severity:** Error

Json.NET's `TypeNameHandling.All`, `Auto`, `Objects`, or `Arrays` allows the JSON payload to specify which .NET type to deserialize into. An attacker who controls the JSON can instantiate arbitrary types.

```csharp
// ❌ Triggers OWASPA08003
var settings = new JsonSerializerSettings
{
    TypeNameHandling = TypeNameHandling.All
};

// Also triggers on standalone assignment:
settings.TypeNameHandling = TypeNameHandling.Auto;

// ✅ Fix: use TypeNameHandling.None (the default)
var settings = new JsonSerializerSettings
{
    TypeNameHandling = TypeNameHandling.None
};
```

---

## OWASPA08004 — `JavaScriptSerializer` with `SimpleTypeResolver`

**Severity:** Error

`JavaScriptSerializer` with `SimpleTypeResolver` deserializes type information from the JSON payload, enabling type confusion attacks and RCE.

```csharp
// ❌ Triggers OWASPA08004
var serializer = new JavaScriptSerializer(new SimpleTypeResolver());

// ✅ Fix: use JavaScriptSerializer without a resolver, or switch to System.Text.Json
var serializer = new JavaScriptSerializer();
```
```

**Step 4: Commit**

```bash
git add website/docs/rules/
git commit -m "docs: add A06, A07, A08 rule pages"
```

---

### Task 6: Write A09 and A10 rule pages

**Files:**
- Create: `website/docs/rules/a09-logging-failures.md`
- Create: `website/docs/rules/a10-ssrf.md`

**Step 1: Create `website/docs/rules/a09-logging-failures.md`**

```markdown
---
sidebar_position: 9
---

# A09 — Security Logging and Monitoring Failures

OWASP A09 covers insufficient logging and monitoring — silent exception swallowing, missing audit trails, and log injection vulnerabilities.

## OWASPA09001 — Empty catch block suppresses exceptions silently

**Severity:** Warning

An empty `catch` block discards all information about a failure. Security events (auth failures, unexpected errors) must be logged.

```csharp
// ❌ Triggers OWASPA09001
try { DoSensitiveOperation(); }
catch { }

// ✅ Fix: log and/or rethrow
try { DoSensitiveOperation(); }
catch (Exception ex)
{
    _logger.LogError(ex, "Sensitive operation failed");
    throw;
}
```

---

## OWASPA09002 — Catch block missing logging

**Severity:** Warning

A non-empty catch block that performs no logging (no call to `Log`, `LogError`, `LogWarning`, `Write`, `WriteLine`, etc.) silently swallows security-relevant context.

```csharp
// ❌ Triggers OWASPA09002
catch (Exception ex)
{
    return StatusCode(500); // no logging
}

// ✅ Fix
catch (Exception ex)
{
    _logger.LogError(ex, "Request failed for user {UserId}", userId);
    return StatusCode(500);
}
```

**Recognized logging method names:** `Log`, `LogInformation`, `LogWarning`, `LogError`, `LogDebug`, `LogTrace`, `LogCritical`, `WriteLine`, `Write`, `Info`, `Error`, `Warn`, `Debug`, `Fatal`.

---

## OWASPA09003 — Log injection via tainted user input

**Severity:** Warning

Logging user input without sanitization allows attackers to inject fake log entries, making log analysis unreliable.

Uses the [taint engine](../taint-engine).

```csharp
// ❌ Triggers OWASPA09003
public IActionResult Search(string query)
{
    _logger.LogInformation("Search: " + query);
    // ...
}

// ✅ Fix: use structured logging (never string concatenation)
public IActionResult Search(string query)
{
    _logger.LogInformation("Search: {Query}", query);
    // ...
}
```

---

## OWASPA09004 — Sensitive data in log message

**Severity:** Warning

String literals passed to logging methods that contain keywords like `password`, `token`, or `secret` suggest that sensitive values may be logged.

```csharp
// ❌ Triggers OWASPA09004
_logger.LogInformation("User password reset: " + password);

// ✅ Fix: never log sensitive values
_logger.LogInformation("User {UserId} requested a password reset", userId);
```

**Sensitive keywords detected:** `password`, `passwd`, `secret`, `token`, `apikey`, `api_key`, `credential`, `private_key`, `privatekey`, `ssn`, `creditcard`, `cvv`.
```

**Step 2: Create `website/docs/rules/a10-ssrf.md`**

```markdown
---
sidebar_position: 10
---

# A10 — Server-Side Request Forgery (SSRF)

OWASP A10 covers Server-Side Request Forgery — where an attacker controls the URL of a server-initiated HTTP request, potentially reaching internal services.

All taint-based rules use the built-in [taint engine](../taint-engine).

## OWASPA10001 — SSRF via `HttpClient`

**Severity:** Error

User-controlled data flowing into `HttpClient.GetAsync()`, `PostAsync()`, or `SendAsync()` allows attackers to direct the server to make requests to internal services, cloud metadata endpoints, or localhost.

```csharp
// ❌ Triggers OWASPA10001
public async Task<IActionResult> Proxy(string url)
{
    var response = await _httpClient.GetAsync(url);
    // ...
}

// ✅ Fix: validate against an allowlist
private static readonly Uri[] AllowedHosts =
[
    new Uri("https://api.trusted.com"),
    new Uri("https://cdn.trusted.com"),
];

public async Task<IActionResult> Proxy(string url)
{
    var uri = new Uri(url);
    if (!AllowedHosts.Any(h => h.Host == uri.Host && uri.Scheme == "https"))
        return BadRequest("URL not allowed");

    var response = await _httpClient.GetAsync(uri);
    // ...
}
```

---

## OWASPA10002 — SSRF via `WebClient`/`WebRequest`

**Severity:** Error

Same vulnerability as OWASPA10001 but using the older `WebClient` or `WebRequest` APIs.

```csharp
// ❌ Triggers OWASPA10002
public IActionResult Fetch(string url)
{
    var client = new WebClient();
    var data = client.DownloadString(url);
    return Ok(data);
}

// ✅ Fix: migrate to HttpClient with URL validation (see OWASPA10001)
```

**Tracked sinks:** `WebClient.DownloadString`, `WebClient.UploadString`, `WebRequest.Create`.

---

## OWASPA10003 — `AllowAutoRedirect` enabled

**Severity:** Warning

`AllowAutoRedirect = true` on `HttpClientHandler` or `WebClient` means the HTTP client will follow redirects automatically — including redirects to internal services crafted by an attacker.

```csharp
// ❌ Triggers OWASPA10003
var handler = new HttpClientHandler { AllowAutoRedirect = true };

// ✅ Fix: disable auto-redirect and handle manually
var handler = new HttpClientHandler { AllowAutoRedirect = false };
```
```

**Step 3: Commit**

```bash
git add website/docs/rules/
git commit -m "docs: add A09 and A10 rule pages"
```

---

### Task 7: Write `configuration.md` and `taint-engine.md`

**Files:**
- Create: `website/docs/configuration.md`
- Create: `website/docs/taint-engine.md`

**Step 1: Create `website/docs/configuration.md`**

```markdown
---
sidebar_position: 11
---

# Configuration

## Severity levels

Every rule ships with a default severity. You can override any rule's severity in your `.editorconfig` file:

```ini
[*.cs]
# Escalate a warning to an error (blocks build)
dotnet_diagnostic.OWASPA01001.severity = error

# Downgrade an error to a warning
dotnet_diagnostic.OWASPA10001.severity = warning

# Suppress a rule entirely
dotnet_diagnostic.OWASPA01004.severity = none

# Demote to suggestion (IDE only, not shown in build output)
dotnet_diagnostic.OWASPA02003.severity = suggestion
```

## Rule reference

| Rule ID | Default Severity | Title |
|---------|-----------------|-------|
| OWASPA01001 | Warning | Controller action missing authorization |
| OWASPA01002 | Warning | Hardcoded role in [Authorize] |
| OWASPA01003 | Warning | Hardcoded string in IsInRole |
| OWASPA01004 | Warning | CORS wildcard allows any origin |
| OWASPA01005 | Warning | Missing [ValidateAntiForgeryToken] |
| OWASPA02001 | Warning | Weak cryptographic algorithm |
| OWASPA02002 | Warning | ECB cipher mode is insecure |
| OWASPA02003 | Info | System.Random is not cryptographically secure |
| OWASPA02004 | Error | Hardcoded cryptographic key |
| OWASPA02005 | Warning | Legacy TLS protocol |
| OWASPA02006 | Error | Certificate validation disabled |
| OWASPA02007 | Warning | Hardcoded HTTP URL |
| OWASPA02008 | Warning | Missing HSTS middleware |
| OWASPA03001 | Error | SQL Injection |
| OWASPA03002 | Error | OS Command Injection |
| OWASPA03003 | Error | Path Traversal |
| OWASPA03004 | Error | LDAP Injection |
| OWASPA03005 | Error | XPath Injection |
| OWASPA03006 | Error | Cross-Site Scripting (XSS) |
| OWASPA04002 | Warning | Missing rate limiting on auth endpoint |
| OWASPA05001 | Warning | Developer exception page in production |
| OWASPA05002 | Warning | Missing HTTPS redirection |
| OWASPA05003 | Warning | Directory browsing enabled |
| OWASPA05004 | Warning | Error details exposed |
| OWASPA05005 | Warning | Antiforgery not configured |
| OWASPA05006 | Error | Hardcoded credential |
| OWASPA07001 | Error | JWT algorithm bypass via SecurityAlgorithms.None |
| OWASPA07002 | Warning | Token lifetime validation disabled |
| OWASPA07003 | Error | Token signing key validation disabled |
| OWASPA07004 | Warning | CookieOptions missing HttpOnly or Secure flag |
| OWASPA07005 | Warning | SameSite=None cookie without Secure flag |
| OWASPA08001 | Error | BinaryFormatter is unsafe |
| OWASPA08002 | Error | Unsafe deserializer usage |
| OWASPA08003 | Error | Unsafe TypeNameHandling in Json.NET |
| OWASPA08004 | Error | JavaScriptSerializer with SimpleTypeResolver |
| OWASPA09001 | Warning | Empty catch block suppresses exceptions silently |
| OWASPA09002 | Warning | Catch block missing logging |
| OWASPA09003 | Warning | Log injection via tainted user input |
| OWASPA09004 | Warning | Sensitive data in log message |
| OWASPA10001 | Error | SSRF via HttpClient |
| OWASPA10002 | Error | SSRF via WebClient/WebRequest |
| OWASPA10003 | Warning | AllowAutoRedirect enabled |

## Inline suppression

Suppress a single occurrence with a pragma:

```csharp
#pragma warning disable OWASPA01004 // reason: public API, CORS is intentional
app.UseCors(b => b.AllowAnyOrigin());
#pragma warning restore OWASPA01004
```

Or attribute-based:

```csharp
[System.Diagnostics.CodeAnalysis.SuppressMessage("OWASP.A01", "OWASPA01004")]
public void ConfigureCors(IApplicationBuilder app) { ... }
```

## Disabling A06 MSBuild target

The vulnerability scan runs `dotnet list package` on every build (a network call). Disable it per project if needed:

```xml
<PropertyGroup>
  <OwaspA06Enabled>false</OwaspA06Enabled>
</PropertyGroup>
```
```

**Step 2: Create `website/docs/taint-engine.md`**

```markdown
---
sidebar_position: 12
---

# Taint Engine

Rules for A03 (Injection), A09 (Log Injection), and A10 (SSRF) use a built-in intra-method taint analysis engine to track user-controlled data from **sources** through **propagation** to **sinks**.

## How it works

The taint engine processes each method body independently:

1. **Sources** — variables assigned from ASP.NET controller inputs (`HttpContext.Request`, action parameters, `RouteData`, etc.) are marked as *tainted*.
2. **Propagation** — any variable assigned from a tainted expression also becomes tainted (string concatenation, interpolation, assignments).
3. **Sanitizers** — certain methods clean tainted data: `HtmlEncode`, `UrlEncode`, `EscapeDataString`, `Encode`, `HtmlAttributeEncode`, `JavaScriptStringEncode`.
4. **Sinks** — when a tainted value reaches a dangerous sink, a diagnostic is reported.

## Taint sources

| Pattern | Example |
|---------|---------|
| Action method parameters | `public IActionResult Get(string id)` |
| `HttpContext.Request.*` | `Request.Query["key"]`, `Request.Form["name"]` |
| `RouteData.Values[...]` | `RouteData.Values["id"]` |
| `Request.QueryString` | `Request.QueryString["q"]` |

## Taint sinks

| Sink | Rule |
|------|------|
| `SqlCommand.CommandText` | OWASPA03001 |
| `NpgsqlCommand.CommandText` | OWASPA03001 |
| `MySqlCommand.CommandText` | OWASPA03001 |
| `Process.Start(fileName, args)` | OWASPA03002 |
| `ProcessStartInfo.FileName` | OWASPA03002 |
| `ProcessStartInfo.Arguments` | OWASPA03002 |
| `File.Open/ReadAllText/WriteAllText/ReadAllBytes` | OWASPA03003 |
| `Directory.GetFiles/GetDirectories` | OWASPA03003 |
| LDAP filter strings | OWASPA03004 |
| XPath query strings | OWASPA03005 |
| `Response.Write(...)` | OWASPA03006 |
| Logging method arguments | OWASPA09003 |
| `HttpClient.GetAsync/PostAsync/SendAsync` | OWASPA10001 |
| `WebClient.DownloadString/UploadString` | OWASPA10002 |
| `WebRequest.Create(...)` | OWASPA10002 |

## Limitations (v1)

- **Intra-method only** — taint does not flow across method call boundaries. A helper method that takes user input and passes it to a sink will not be detected.
- **Simple name matching** — type resolution uses simple type names, not fully qualified names. A user-defined class named `File` could produce false positives.
- **No field/property tracking** — taint through instance fields is not tracked.

These are known v1 constraints. Interprocedural taint and semantic type resolution are planned for a future release.
```

**Step 3: Commit**

```bash
git add website/docs/configuration.md website/docs/taint-engine.md
git commit -m "docs: add configuration and taint-engine reference pages"
```

---

### Task 8: Add GitHub Actions docs workflow

**Files:**
- Create: `.github/workflows/docs.yml`

**Step 1: Create `.github/workflows/docs.yml`**

```yaml
name: Deploy Documentation

on:
  push:
    branches: [main]
    paths:
      - 'website/**'
  workflow_dispatch:

permissions:
  contents: read
  pages: write
  id-token: write

concurrency:
  group: "pages"
  cancel-in-progress: false

jobs:
  build:
    runs-on: ubuntu-latest
    defaults:
      run:
        working-directory: website
    steps:
      - uses: actions/checkout@v4

      - uses: actions/setup-node@v4
        with:
          node-version: 24
          cache: npm
          cache-dependency-path: website/package-lock.json

      - name: Install dependencies
        run: npm ci

      - name: Build
        run: npm run build

      - uses: actions/upload-pages-artifact@v3
        with:
          path: website/build

  deploy:
    environment:
      name: github-pages
      url: ${{ steps.deployment.outputs.page_url }}
    runs-on: ubuntu-latest
    needs: build
    steps:
      - id: deployment
        uses: actions/deploy-pages@v4
```

**Step 2: Verify workflow YAML**

```bash
cat .github/workflows/docs.yml
```

Expected: file prints without error, indentation is correct.

**Step 3: Commit and push**

```bash
git add .github/workflows/docs.yml
git commit -m "ci: add GitHub Pages docs deployment workflow"
git push origin main
```

**Step 4: Enable GitHub Pages**

Run this via gh CLI to enable GitHub Pages with Actions as the source:

```bash
gh api repos/MarcelRoozekrans/Owasp.Analyzers/pages \
  --method POST \
  --field build_type=workflow \
  --field source='{"branch":"main","path":"/"}' 2>/dev/null || \
gh api repos/MarcelRoozekrans/Owasp.Analyzers/pages \
  --method PUT \
  --field build_type=workflow
```

Expected: Pages enabled, deployment triggers automatically after the push.

---

### Task 9: Rewrite `README.md`

**Files:**
- Modify: `README.md`

**Step 1: Rewrite `README.md` with this content**

```markdown
# Owasp.Analyzers

[![NuGet](https://img.shields.io/nuget/v/Owasp.Analyzers.svg)](https://www.nuget.org/packages/Owasp.Analyzers)
[![CI](https://github.com/MarcelRoozekrans/Owasp.Analyzers/actions/workflows/ci.yml/badge.svg)](https://github.com/MarcelRoozekrans/Owasp.Analyzers/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

Roslyn analyzers that surface [OWASP Top 10 2021](https://owasp.org/Top10/) security vulnerabilities as compiler warnings and errors in C#/.NET projects — no external tools, no CI-only scans, just your normal build.

📖 **[Full documentation →](https://marcelroozekrans.github.io/Owasp.Analyzers/)**

## Installation

```bash
dotnet add package Owasp.Analyzers
```

## Rules

| Rule ID | Severity | Description |
|---------|----------|-------------|
| **A01 — Broken Access Control** | | |
| OWASPA01001 | ⚠️ Warning | Controller action missing `[Authorize]` or `[AllowAnonymous]` |
| OWASPA01002 | ⚠️ Warning | Hardcoded role string in `[Authorize]` |
| OWASPA01003 | ⚠️ Warning | Hardcoded string in `User.IsInRole()` |
| OWASPA01004 | ⚠️ Warning | CORS wildcard allows any origin |
| OWASPA01005 | ⚠️ Warning | Missing `[ValidateAntiForgeryToken]` on state-changing action |
| **A02 — Cryptographic Failures** | | |
| OWASPA02001 | ⚠️ Warning | Weak cryptographic algorithm (MD5, SHA1, DES, RC2, TripleDES) |
| OWASPA02002 | ⚠️ Warning | ECB cipher mode is insecure |
| OWASPA02003 | ℹ️ Info | `System.Random` is not cryptographically secure |
| OWASPA02004 | 🔴 Error | Hardcoded cryptographic key or IV |
| OWASPA02005 | ⚠️ Warning | Legacy TLS protocol (SSL3, TLS 1.0/1.1) |
| OWASPA02006 | 🔴 Error | Certificate validation disabled |
| OWASPA02007 | ⚠️ Warning | Hardcoded HTTP URL |
| OWASPA02008 | ⚠️ Warning | Missing HSTS middleware |
| **A03 — Injection** | | |
| OWASPA03001 | 🔴 Error | SQL injection (taint) |
| OWASPA03002 | 🔴 Error | OS command injection (taint) |
| OWASPA03003 | 🔴 Error | Path traversal (taint) |
| OWASPA03004 | 🔴 Error | LDAP injection (taint) |
| OWASPA03005 | 🔴 Error | XPath injection (taint) |
| OWASPA03006 | 🔴 Error | Cross-Site Scripting / XSS (taint) |
| **A04 — Insecure Design** | | |
| OWASPA04002 | ⚠️ Warning | Missing rate limiting on authentication endpoint |
| **A05 — Security Misconfiguration** | | |
| OWASPA05001 | ⚠️ Warning | Developer exception page in production |
| OWASPA05002 | ⚠️ Warning | Missing HTTPS redirection |
| OWASPA05003 | ⚠️ Warning | Directory browsing enabled |
| OWASPA05004 | ⚠️ Warning | Error details exposed to clients |
| OWASPA05005 | ⚠️ Warning | Antiforgery not configured |
| OWASPA05006 | 🔴 Error | Hardcoded credential |
| **A06 — Vulnerable Components** | | |
| OWASPA06001 | ⚠️ Warning | Vulnerable NuGet package detected (MSBuild) |
| OWASPA06002 | ⚠️ Warning | Deprecated NuGet package detected (MSBuild) |
| **A07 — Authentication Failures** | | |
| OWASPA07001 | 🔴 Error | JWT algorithm bypass via `SecurityAlgorithms.None` |
| OWASPA07002 | ⚠️ Warning | Token lifetime validation disabled |
| OWASPA07003 | 🔴 Error | Token signing key validation disabled |
| OWASPA07004 | ⚠️ Warning | `CookieOptions` missing `HttpOnly` or `Secure` flag |
| OWASPA07005 | ⚠️ Warning | `SameSite=None` cookie without `Secure` flag |
| **A08 — Data Integrity Failures** | | |
| OWASPA08001 | 🔴 Error | `BinaryFormatter` is unsafe for deserialization |
| OWASPA08002 | 🔴 Error | Unsafe deserializer (`NetDataContractSerializer`, `SoapFormatter`, etc.) |
| OWASPA08003 | 🔴 Error | Unsafe `TypeNameHandling` in Json.NET |
| OWASPA08004 | 🔴 Error | `JavaScriptSerializer` with `SimpleTypeResolver` |
| **A09 — Logging Failures** | | |
| OWASPA09001 | ⚠️ Warning | Empty catch block suppresses exceptions silently |
| OWASPA09002 | ⚠️ Warning | Catch block missing logging |
| OWASPA09003 | ⚠️ Warning | Log injection via tainted user input |
| OWASPA09004 | ⚠️ Warning | Sensitive data in log message |
| **A10 — SSRF** | | |
| OWASPA10001 | 🔴 Error | SSRF via `HttpClient` (taint) |
| OWASPA10002 | 🔴 Error | SSRF via `WebClient`/`WebRequest` (taint) |
| OWASPA10003 | ⚠️ Warning | `AllowAutoRedirect` enabled |

For detailed documentation, fix guidance, and code examples see the **[docs site](https://marcelroozekrans.github.io/Owasp.Analyzers/)**.

## License

MIT
```

**Step 2: Verify the file**

```bash
head -5 README.md
```

Expected: starts with `# Owasp.Analyzers` and badge lines.

**Step 3: Commit and push**

```bash
git add README.md
git commit -m "docs: rewrite README with badges, full rule table, and docs link"
git push origin main
```

Expected: Push succeeds and CI workflow triggers on GitHub.
