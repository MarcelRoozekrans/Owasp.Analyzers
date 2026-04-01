# Release & Packaging Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Add a NuGet package icon and a fully automated Release-Please + GitHub Actions CI/CD pipeline to publish `Owasp.Analyzers` to NuGet.org.

**Architecture:** Shield+checkmark SVG/PNG icon lives in `assets/`; shared NuGet metadata moves to `Directory.Build.props`; Release-Please manages versioning via conventional commits; two GitHub Actions workflows handle CI (build+test) and release (version bump + NuGet publish).

**Tech Stack:** .NET 10, MSBuild, GitHub Actions, googleapis/release-please-action@v4, NuGet.org

---

### Task 1: Create the package icon

**Files:**
- Create: `assets/logo.svg`
- Create: `assets/logo.png` (generated from SVG via PowerShell)

**Step 1: Create `assets/logo.svg`**

```xml
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 128 128" width="128" height="128">
  <!-- Shield shape -->
  <path d="M64 8 L112 28 L112 68 C112 96 88 116 64 124 C40 116 16 96 16 68 L16 28 Z"
        fill="#E8162B"/>
  <!-- Checkmark -->
  <polyline points="36,66 56,86 92,46"
            fill="none" stroke="white" stroke-width="10"
            stroke-linecap="round" stroke-linejoin="round"/>
</svg>
```

Create the directory and file:
```bash
mkdir -p assets
# Write the SVG content above to assets/logo.svg
```

**Step 2: Generate `assets/logo.png` from the SVG using PowerShell**

Run this PowerShell script to produce a 128×128 PNG using Windows built-in rendering:

```powershell
Add-Type -AssemblyName System.Drawing
Add-Type -AssemblyName System.Windows.Forms

$width = 128
$height = 128
$bmp = New-Object System.Drawing.Bitmap($width, $height)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$g.Clear([System.Drawing.Color]::Transparent)

# Shield fill (OWASP red #E8162B)
$brush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 232, 22, 43))
$shieldPoints = @(
    [System.Drawing.PointF]::new(64, 8),
    [System.Drawing.PointF]::new(112, 28),
    [System.Drawing.PointF]::new(112, 68),
    [System.Drawing.PointF]::new(64, 124),
    [System.Drawing.PointF]::new(16, 68),
    [System.Drawing.PointF]::new(16, 28)
)
$g.FillPolygon($brush, $shieldPoints)

# Checkmark (white, thick)
$pen = New-Object System.Drawing.Pen([System.Drawing.Color]::White, 10)
$pen.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
$pen.EndCap = [System.Drawing.Drawing2D.LineCap]::Round
$pen.LineJoin = [System.Drawing.Drawing2D.LineJoin]::Round
$g.DrawLines($pen, @(
    [System.Drawing.PointF]::new(36, 66),
    [System.Drawing.PointF]::new(56, 86),
    [System.Drawing.PointF]::new(92, 46)
))

$g.Dispose()
$bmp.Save("assets\logo.png", [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
Write-Host "Done: assets\logo.png"
```

Save as `assets/generate-logo.ps1` and run:
```bash
powershell -ExecutionPolicy Bypass -File assets/generate-logo.ps1
```

Expected: `assets/logo.png` exists and is ~3-5 KB.

**Step 3: Verify the PNG visually**

Open `assets/logo.png` in an image viewer and confirm: red shield, white checkmark.

**Step 4: Commit**

```bash
git add assets/logo.svg assets/logo.png assets/generate-logo.ps1
git commit -m "feat: add OWASP red shield+checkmark package icon"
```

---

### Task 2: Create `Directory.Build.props` and update the csproj

**Files:**
- Create: `Directory.Build.props` (repo root)
- Modify: `src/Owasp.Analyzers/Owasp.Analyzers.csproj`

**Step 1: Create `Directory.Build.props` at repo root**

```xml
<Project>
  <PropertyGroup>
    <Authors>Marcel Roozekrans</Authors>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
    <PackageProjectUrl>https://github.com/MarcelRoozekrans/Owasp.Analyzers</PackageProjectUrl>
    <RepositoryUrl>https://github.com/MarcelRoozekrans/Owasp.Analyzers</RepositoryUrl>
    <RepositoryType>git</RepositoryType>
    <PackageIcon>logo.png</PackageIcon>
    <PackageReadmeFile>README.md</PackageReadmeFile>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
</Project>
```

**Step 2: Update `src/Owasp.Analyzers/Owasp.Analyzers.csproj`**

Remove from the csproj:
- `<Version>1.0.0</Version>` — now managed by Release-Please in `Directory.Build.props`
- `<Authors>Marcel Roozekrans</Authors>` — moved to `Directory.Build.props`
- `<Nullable>enable</Nullable>` — moved to `Directory.Build.props`
- `<ImplicitUsings>enable</ImplicitUsings>` — moved to `Directory.Build.props`

Add to the existing `<ItemGroup>` that has the `None` includes:
```xml
<None Include="..\..\assets\logo.png" Pack="true" PackagePath="\" />
<None Include="..\..\README.md" Pack="true" PackagePath="\" />
```

The final csproj `<PropertyGroup>` should look like:
```xml
<PropertyGroup>
  <TargetFramework>net10.0</TargetFramework>
  <EnforceExtendedAnalyzerRules>true</EnforceExtendedAnalyzerRules>
  <NoWarn>RS2008</NoWarn>
  <RootNamespace>Owasp.Analyzers</RootNamespace>
  <PackageId>Owasp.Analyzers</PackageId>
  <Description>Roslyn analyzers covering the OWASP Top 10 2021 for C#/.NET projects.</Description>
  <PackageTags>roslyn;analyzer;owasp;security</PackageTags>
  <IncludeBuildOutput>false</IncludeBuildOutput>
  <DevelopmentDependency>true</DevelopmentDependency>
</PropertyGroup>
```

**Step 3: Check if `README.md` exists at repo root**

```bash
ls README.md 2>/dev/null || echo "MISSING"
```

If missing, create a minimal `README.md`:

```markdown
# Owasp.Analyzers

Roslyn analyzers covering the [OWASP Top 10 2021](https://owasp.org/Top10/) for C#/.NET projects.

## Installation

```
dotnet add package Owasp.Analyzers
```

## Rules

| ID | Category | Severity | Description |
|----|----------|----------|-------------|
| OWASPA01001 | A01 Broken Access Control | Warning | Controller action missing authorization |
| OWASPA01002 | A01 Broken Access Control | Warning | CORS wildcard origin |
| OWASPA01003 | A01 Broken Access Control | Warning | Hardcoded role string |
| OWASPA01004 | A01 Broken Access Control | Warning | Missing antiforgery token |
| OWASPA02001–004 | A02 Cryptographic Failures | Error/Warning | Weak crypto, hardcoded secrets |
| OWASPA03001–003 | A03 Injection | Error | SQL/command/LDAP injection via taint |
| OWASPA04001–002 | A04 Insecure Design | Warning | Insecure design patterns |
| OWASPA05001 | A05 Security Misconfiguration | Warning | Security misconfiguration |
| OWASPA06001–002 | A06 Vulnerable Components | Warning | Vulnerable/deprecated NuGet packages |
| OWASPA07001–005 | A07 Auth Failures | Warning/Error | Authentication failures |
| OWASPA08001–004 | A08 Data Integrity | Warning/Error | Data integrity failures |
| OWASPA09001–004 | A09 Logging Failures | Warning | Logging and monitoring failures |
| OWASPA10001–003 | A10 SSRF | Error/Warning | Server-side request forgery |

## License

MIT
```

**Step 4: Verify the build still passes**

```bash
dotnet build src/Owasp.Analyzers/Owasp.Analyzers.csproj -c Release
```

Expected: Build succeeded, 0 errors.

**Step 5: Verify `dotnet pack` produces a valid package with icon**

```bash
dotnet pack src/Owasp.Analyzers/Owasp.Analyzers.csproj -c Release -o artifacts/
```

Then inspect the package contents:
```bash
# Rename .nupkg to .zip and inspect, or use:
dotnet tool install --global NuGetPackageExplorer 2>/dev/null || true
# Or simply verify the nupkg contains logo.png:
powershell -Command "Add-Type -AssemblyName System.IO.Compression.FileSystem; \$zip = [System.IO.Compression.ZipFile]::OpenRead('artifacts/Owasp.Analyzers.1.0.0.nupkg'); \$zip.Entries | Select-Object FullName"
```

Expected: Entry `logo.png` and `README.md` appear in the package.

**Step 6: Commit**

```bash
git add Directory.Build.props src/Owasp.Analyzers/Owasp.Analyzers.csproj README.md
git commit -m "feat: add Directory.Build.props, icon and readme to NuGet package"
```

---

### Task 3: Add Release-Please configuration

**Files:**
- Create: `release-please-config.json`
- Create: `.release-please-manifest.json`

**Step 1: Create `release-please-config.json`**

```json
{
  "release-type": "simple",
  "bump-minor-pre-major": true,
  "bump-patch-for-minor-pre-major": true,
  "extra-files": [
    {
      "type": "xml",
      "path": "Directory.Build.props",
      "xpath": "//Project/PropertyGroup/Version"
    }
  ]
}
```

> The `extra-files` entry tells Release-Please to update the `<Version>` tag in `Directory.Build.props` when it bumps the version.

**Step 2: Create `.release-please-manifest.json`**

```json
{
  ".": "1.0.0"
}
```

**Step 3: Add `<Version>` to `Directory.Build.props`**

Release-Please needs a `<Version>` tag in `Directory.Build.props` to update. Add it to the `<PropertyGroup>`:

```xml
<Version>1.0.0</Version>
```

**Step 4: Verify the JSON files are valid**

```bash
powershell -Command "Get-Content release-please-config.json | ConvertFrom-Json"
powershell -Command "Get-Content .release-please-manifest.json | ConvertFrom-Json"
```

Expected: No errors, objects printed.

**Step 5: Commit**

```bash
git add release-please-config.json .release-please-manifest.json Directory.Build.props
git commit -m "chore: add release-please configuration"
```

---

### Task 4: Add GitHub Actions — CI workflow

**Files:**
- Create: `.github/workflows/ci.yml`

**Step 1: Create `.github/workflows/ci.yml`**

```yaml
name: CI

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]

jobs:
  build-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Restore
        run: dotnet restore

      - name: Build
        run: dotnet build -c Release --no-restore

      - name: Test
        run: dotnet test -c Release --no-build --verbosity normal
```

**Step 2: Validate the YAML syntax**

```bash
powershell -Command "
  \$content = Get-Content '.github/workflows/ci.yml' -Raw
  Write-Host 'File exists and has' \$content.Length 'characters'
"
```

Expected: File exists with content.

**Step 3: Commit**

```bash
git add .github/workflows/ci.yml
git commit -m "ci: add CI workflow for build and test"
```

---

### Task 5: Add GitHub Actions — Release-Please workflow

**Files:**
- Create: `.github/workflows/release-please.yml`

**Step 1: Create `.github/workflows/release-please.yml`**

```yaml
name: Release Please

on:
  push:
    branches: [main]

permissions:
  contents: write
  pull-requests: write

jobs:
  release-please:
    runs-on: ubuntu-latest
    outputs:
      release_created: ${{ steps.release.outputs.release_created }}
      tag_name: ${{ steps.release.outputs.tag_name }}
    steps:
      - uses: googleapis/release-please-action@v4
        id: release
        with:
          token: ${{ secrets.GITHUB_TOKEN }}
          config-file: release-please-config.json
          manifest-file: .release-please-manifest.json

  publish:
    needs: release-please
    if: ${{ needs.release-please.outputs.release_created }}
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Restore
        run: dotnet restore

      - name: Build
        run: dotnet build -c Release --no-restore

      - name: Test
        run: dotnet test -c Release --no-build --verbosity normal

      - name: Pack
        run: dotnet pack src/Owasp.Analyzers/Owasp.Analyzers.csproj -c Release --no-build -o artifacts/

      - name: Push to NuGet
        run: |
          dotnet nuget push artifacts/*.nupkg \
            --api-key ${{ secrets.NUGET_API_KEY }} \
            --source https://api.nuget.org/v3/index.json \
            --skip-duplicate
```

**Step 2: Validate the YAML syntax**

```bash
powershell -Command "
  \$content = Get-Content '.github/workflows/release-please.yml' -Raw
  Write-Host 'File exists and has' \$content.Length 'characters'
"
```

**Step 3: Commit**

```bash
git add .github/workflows/release-please.yml
git commit -m "ci: add release-please workflow with NuGet publish"
```

---

### Task 6: Final verification

**Step 1: Run the full build and test suite**

```bash
dotnet build -c Release
dotnet test -c Release --no-build
```

Expected: All tests pass, 0 errors.

**Step 2: Verify pack produces correct package**

```bash
dotnet pack src/Owasp.Analyzers/Owasp.Analyzers.csproj -c Release -o artifacts/
powershell -Command "
  Add-Type -AssemblyName System.IO.Compression.FileSystem
  \$pkg = Get-ChildItem artifacts/*.nupkg | Select-Object -First 1
  \$zip = [System.IO.Compression.ZipFile]::OpenRead(\$pkg.FullName)
  \$zip.Entries | Select-Object FullName
  \$zip.Dispose()
"
```

Expected output includes:
- `logo.png`
- `README.md`
- `analyzers/dotnet/cs/Owasp.Analyzers.dll`
- `build/Owasp.Analyzers.targets`

**Step 3: Confirm GitHub secret is set**

In the GitHub repo → Settings → Secrets and variables → Actions, ensure `NUGET_API_KEY` is set. This is a manual step — create an API key at https://www.nuget.org/account/apikeys scoped to push the `Owasp.Analyzers` package.

**Step 4: Push to `main` to trigger CI**

```bash
git push origin master:main
```

Expected: CI workflow runs and passes on GitHub Actions.
