# Docusaurus Website & Documentation Design

**Date:** 2026-04-01

## Goal

Add a Docusaurus 3.7 website (`website/`) with extensive per-rule documentation for all 27 OWASP analyzer rules, deployed to GitHub Pages at `marcelroozekrans.github.io/Owasp.Analyzers`. Also rewrite `README.md` with badges, full rule table, and a link to the docs site.

---

## Section 1 — Website Structure & Config

**Stack:** Docusaurus 3.7, React 19, Node 24 — identical to AdoNet.Async.

**Location:** `website/` at repo root.

**Key config (`website/docusaurus.config.js`):**
- `url`: `https://marcelroozekrans.github.io`
- `baseUrl`: `/Owasp.Analyzers/`
- `organizationName`: `MarcelRoozekrans`
- `projectName`: `Owasp.Analyzers`
- Tagline: `"Roslyn analyzers for the OWASP Top 10 — catch security issues at compile time"`
- Primary color: OWASP red `#E8162B`
- Syntax highlighting: C#, bash, JSON (Prism)
- Navbar: Docs | GitHub | NuGet

**`website/package.json`:**
- Name: `owasp-analyzers-docs`
- Docusaurus 3.7, React 19, MDX 3, Node ≥18

---

## Section 2 — Documentation Content

All markdown lives in `website/docs/`. One page per OWASP category with rule ID, description, bad/good code examples, and fix guidance.

```
website/docs/
  intro.md                           ← overview, how Roslyn analyzers work, OWASP Top 10 2021
  getting-started/
    installation.md                  ← dotnet add package, IDE setup, .editorconfig
    quick-start.md                   ← first diagnostic, suppress, configure severity
  rules/
    a01-broken-access-control.md     ← OWASPA01001–005
    a02-cryptographic-failures.md    ← OWASPA02001–008
    a03-injection.md                 ← OWASPA03001–003 (taint)
    a04-insecure-design.md           ← OWASPA04001–002
    a05-security-misconfiguration.md ← OWASPA05001
    a06-vulnerable-components.md     ← OWASPA06001–002 (MSBuild targets)
    a07-authentication-failures.md   ← OWASPA07001–005
    a08-data-integrity.md            ← OWASPA08001–004
    a09-logging-failures.md          ← OWASPA09001–004 (taint)
    a10-ssrf.md                      ← OWASPA10001–003 (taint)
  configuration.md                   ← severity overrides, suppression, .editorconfig
  taint-engine.md                    ← how taint analysis works, sources/sinks
```

**Each rule page includes:**
- Rule ID, title, severity, category
- What it detects and why it matters (OWASP context)
- ❌ Non-compliant code example (C#)
- ✅ Compliant code example (C#)
- How to fix / suppress

---

## Section 3 — GitHub Actions & README

**`.github/workflows/docs.yml`** (mirrors AdoNet.Async):
```yaml
on:
  push:
    branches: [main]
    paths: ['website/**']
  workflow_dispatch:

permissions:
  contents: read
  pages: write
  id-token: write

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
      - run: npm install
      - run: npm run build
      - uses: actions/upload-pages-artifact@v3
        with:
          path: website/build

  deploy:
    environment:
      name: github-pages
      url: ${{ steps.deployment.outputs.page_url }}
    needs: build
    runs-on: ubuntu-latest
    steps:
      - id: deployment
        uses: actions/deploy-pages@v4
```

**`README.md` rewrite:**
- NuGet version badge, CI badge, license badge
- Short description + link to `marcelroozekrans.github.io/Owasp.Analyzers`
- Installation snippet
- Full rules table — one row per rule (not collapsed ranges)
- Link to docs site for detailed per-rule guidance

---

## Sidebar Structure (`website/sidebars.js`)

```
Introduction
Getting Started
  Installation
  Quick Start
Rules
  A01 Broken Access Control
  A02 Cryptographic Failures
  A03 Injection
  A04 Insecure Design
  A05 Security Misconfiguration
  A06 Vulnerable Components
  A07 Authentication Failures
  A08 Data Integrity Failures
  A09 Logging Failures
  A10 SSRF
Configuration
Taint Engine
```

---

## Manual Step After Implementation

Enable GitHub Pages in the repo:
- Settings → Pages → Source: **GitHub Actions**
- First push to `website/**` on `main` triggers the deploy workflow automatically.
