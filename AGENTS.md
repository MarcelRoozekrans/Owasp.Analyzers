# AGENTS.md: Owasp.Analyzers

Roslyn analyzers for the OWASP Top 10 2025 in C# and .NET. The repo ships one NuGet package,
`Owasp.Analyzers`: a development dependency that holds the analyzer assembly plus an MSBuild targets
file. Consumers use it to find security defects at compile time.

## Layout

- `src/Owasp.Analyzers` is the only packable project. It targets `net10.0` and does not ship a
  library: `IncludeBuildOutput` is false and the analyzer DLL is packed into `analyzers/dotnet/cs`.
  `Analyzers/A01` to `A10` hold one analyzer class per OWASP category, `Taint/` the shared taint engine.
- `src/Owasp.Analyzers/build/Owasp.Analyzers.targets` ships in the package and runs in the consumer's
  build, not here. It implements A03 with `dotnet list package` and emits MSBuild warnings with the
  codes `OWASP-A03-001` and `OWASP-A03-002`, not Roslyn IDs: the README table lists them as
  OWASPA03001 and OWASPA03002, but `.editorconfig` severity settings do not reach them. A03 has no
  analyzer class, no unit tests and no release tracking entry. Opt-out is `OwaspA03Enabled`, and the old `OwaspA06Enabled` is
  still honored.
- `tests/Owasp.Analyzers.Tests` targets `net10.0` and `net11.0`, so it needs the .NET 10 and the
  .NET 11 runtime. Each test compiles a code string in memory through `AnalyzerTestHelper`.
- `website/` is the Docusaurus docs site, deployed by `docs.yml` on changes under `website/**`. It is
  not part of the solution.
- `artifacts/` is ignored. The publish job packs into it and pushes `artifacts/*.nupkg`, so pack
  locally to another folder, for example `-o /tmp/pack`.

## Build and test

There is no `global.json`. Build and test need both the .NET 10 SDK and the .NET 11 SDK, because the
test project targets `net11.0`. CI installs `10.0.x` and `11.0.x`.

CI job `build-test` in `ci.yml`:

```bash
dotnet restore
dotnet build -c Release --no-restore
dotnet test -c Release --no-build --verbosity normal --collect "XPlat Code Coverage" --settings coverage.runsettings --results-directory TestResults
```

The next step in that job is the `coverage-gate` action from `ci-baseline`, which fails below a
coverage floor and when `codecov.yml` would let a drop pass. These commands do not reproduce it.

CI job `publish` in `release-please.yml` (runs on release): restore, build and test as above without
the coverage options, then:

```bash
dotnet pack src/Owasp.Analyzers/Owasp.Analyzers.csproj -c Release --no-build -o artifacts/
```

Inner loop, not run by CI as such. Run one framework, or a subset of tests:

```bash
dotnet test tests/Owasp.Analyzers.Tests -f net10.0 --filter "FullyQualifiedName~InjectionAnalyzerTests"
```

## Conventions and gotchas

- A rule ID lives in several files, so a new, renamed or removed rule changes all of them:
  the `DiagnosticDescriptor` in `src/Owasp.Analyzers/Analyzers/`, `AnalyzerReleases.Unshipped.md`
  or `AnalyzerReleases.Shipped.md`, the rules table in `README.md`, and the matching page under
  `website/docs/rules/`. A missing release tracking entry only raises warning RS2000 from the
  Roslyn analyzers package, so the build stays green: check the files by hand. Renumbering is a
  breaking change for consumers' `.editorconfig` overrides, as the 2.0.0 migration table in
  `README.md` shows. A03 is the exception: it lives only in the targets file, the README table and
  its website page.
- Diagnostic IDs are `OWASPA<category><number>` and the category string is `OWASP.A<category>`.
- Analyzers call `ConfigureGeneratedCodeAnalysis(None)` and `EnableConcurrentExecution()`. The
  project sets `EnforceExtendedAnalyzerRules`, which makes the Roslyn analyzer rules stricter for
  analyzer code.
- `Version` in `Directory.Build.props` is bumped by release-please through `extra-files`. Do not edit it
  by hand, and do not edit `.release-please-manifest.json` or `CHANGELOG.md`.
- The test project references `Microsoft.CodeAnalysis.CSharp.Workspaces`. Tests for ASP.NET Core
  or `System.Data` rules pass extra metadata references to `GetDiagnosticsAsync`, because the
  in-memory compilation otherwise knows only `System.Private.CoreLib`.

<!-- org-core:start sha256=08a88996bc9165f888fc0260607cfd0670cae128c323e16707c6b452ee66222d (generated from docker-nuc scripts/agents-md/org-core-personal.md; do not edit here) -->
## Org rules

These rules hold in every public MarcelRoozekrans repo.

### Commits and releases

- Conventional Commits. release-please reads them on the default branch, and the changelog sections
  in `release-please-config.json` decide which types count. The standard: `feat`, `fix`, `perf`,
  `deps` and `revert` open or update the release PR; `docs`, `test`, `refactor`, `build`, `ci`,
  `chore` and `style` do not, unless the commit is breaking or has a `Release-As:` footer. The
  release happens when that PR merges. A breaking change needs `!` or a `BREAKING CHANGE:` footer.
- A commit body never contains nested parentheses. release-please cannot parse them and silently
  drops the whole commit from the changelog, while the workflow still reports success.
- After merging a commit that should release, check that it appears in the release-please PR.

### Branches and pull requests

- Every change goes through a branch and a pull request. Never push to the default branch directly.
- A PR body says what the diff does, and nothing the diff does not do.
- Fix review findings in the PR under review, including small ones. File an issue only for a finding
  that belongs to other code, needs a design decision, is its own piece of work, or waits on
  something external.

### CI

- The .NET repos call the shared workflows of `MarcelRoozekrans/ci-baseline`, pinned to `@v1`, never
  `@main`.
- Its coverage gate fails the build when line coverage drops below the floor of 80%, and Codecov
  fails a PR that lowers project coverage by more than 1 percentage point. New code comes with
  tests.
- A repo with a lower floor documents the reason and links the issue that raises it.
<!-- org-core:end -->
