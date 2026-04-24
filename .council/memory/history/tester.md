<!-- Tester appends project-specific learnings here. Never cleared. -->

[2026-04-03] [LEARNING] With Bun, `mock.module()` should be restored by re-registering the original module in `afterEach`; `mock.restore()` alone is not enough.
Source: legacy decisions record deleted during migration; legacy tester history record deleted during migration

[2026-04-03] [LEARNING] For routing-dependent UI tests, use `MemoryRouter` with route entries instead of relying on production navigation state.
Source: `docs/development/testing.md`; legacy tester charter deleted during migration

[2026-04-05] [LEARNING] Frontend tests rely on `ui/bunfig.toml` preloads for `happy-dom` and global cleanup; do not duplicate that setup per test file.
Source: `ui/bunfig.toml`; `docs/development/testing.md`

[2026-04-05] [LEARNING] Backend unit tests should reference `Application` and `Contracts`, while integration tests boot the full API and use `api/fribascore.slnx`.
Source: legacy tester history record deleted during migration; `api/test/FribaScore.Api.Tests.Unit/FribaScore.Api.Tests.Unit.csproj`; `api/test/FribaScore.Api.Tests.Integration/FribaScore.Api.Tests.Integration.csproj`; `api/fribascore.slnx`
