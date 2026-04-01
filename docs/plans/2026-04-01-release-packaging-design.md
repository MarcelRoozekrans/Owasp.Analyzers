# Release & Packaging Design

**Date:** 2026-04-01

## Goal

Prepare the `Owasp.Analyzers` NuGet package for public release with a package icon and a fully automated CI/CD pipeline matching the pattern used in [AdoNet.Async](https://github.com/MarcelRoozekrans/AdoNet.Async).

---

## Section 1 — Icon

- Create `assets/logo.svg`: OWASP-red (`#E8162B`) shield with a white checkmark
- Provide `assets/logo.png` (128×128) as the NuGet-compatible rendition
- Reference in `Owasp.Analyzers.csproj`:
  ```xml
  <PackageIcon>logo.png</PackageIcon>
  <None Include="..\..\assets\logo.png" Pack="true" PackagePath="\" />
  ```

---

## Section 2 — NuGet Package Metadata

- Create `Directory.Build.props` at the repo root with shared metadata:
  - `Authors`, `PackageLicenseExpression` (MIT), `PackageProjectUrl`, `RepositoryUrl`, `RepositoryType`
  - `PackageIcon`, `PackageReadmeFile`
  - `Nullable`, `ImplicitUsings`
- Remove `Version` from `Owasp.Analyzers.csproj` — managed by Release-Please via `Directory.Build.props`
- Ensure a `README.md` exists at repo root (create minimal one if absent)

---

## Section 3 — Release-Please & GitHub Actions

### Files

| File | Purpose |
|------|---------|
| `.github/workflows/ci.yml` | Build + test on every push/PR to `main` |
| `.github/workflows/release-please.yml` | Automate versioning and NuGet publish |
| `release-please-config.json` | Release-Please configuration (type: simple) |
| `.release-please-manifest.json` | Current version tracking, starts at `1.0.0` |

### CI Workflow (`ci.yml`)

Triggers: push and pull_request to `main`

Jobs:
1. **build-test** — `dotnet build` + `dotnet test` in Release mode on Ubuntu

### Release-Please Workflow (`release-please.yml`)

Job 1 — **release-please**: runs `googleapis/release-please-action@v4`, creates/updates a release PR from conventional commits, outputs `release_created` and `tag_name`.

Job 2 — **publish** (runs only when `release_created == true`):
1. Checkout
2. Setup .NET
3. `dotnet build -c Release`
4. `dotnet pack -c Release --no-build`
5. `dotnet nuget push` with `NUGET_API_KEY` GitHub secret, skip duplicate versions

### Release-Please Config

```json
// release-please-config.json
{
  "release-type": "simple",
  "bump-minor-pre-major": true,
  "bump-patch-for-minor-pre-major": true
}
```

```json
// .release-please-manifest.json
{ ".": "1.0.0" }
```

Release-Please updates `Version` in `Directory.Build.props` automatically on each release.

### Required GitHub Secret

| Secret | Value |
|--------|-------|
| `NUGET_API_KEY` | NuGet.org API key for `Owasp.Analyzers` package |

---

## Decisions

- Release-Please chosen over manual tagging for consistency with AdoNet.Async and zero manual version management
- Icon: shield + checkmark in OWASP red, SVG source kept in `assets/` alongside the PNG
- Single NuGet package (not multiple), `DevelopmentDependency: true` preserved
