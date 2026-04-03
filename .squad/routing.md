# Work Routing

How to decide who handles what.

## Routing Table

| Work Type | Route To | Examples |
|-----------|----------|---------|
| Architecture, decisions, scope | LD-7 | What to build next, trade-offs, breaking changes |
| React components, UI, CSS, PWA | FE-2 | New components, styling, animations, service worker |
| IndexedDB, data layer, .NET API | BE-8 | db.ts changes, data models, future API endpoints |
| Tests, quality, edge cases | QT-3 | Writing tests, reviewing coverage, finding bugs |
| CI/CD, GitHub Actions, workflow, pipeline, deploy | OPS-9 | Workflow authoring, cron schedules, artifact handling |
| Code review | LD-7 | Review PRs, check quality, architecture feedback |
| Session logging, decisions merge | Scribe | Automatic — never needs routing |
| Work queue, GitHub issues | Ralph | Backlog scanning, issue triage, PR monitoring |

## Issue Routing

| Label | Action | Who |
|-------|--------|-----|
| `squad` | Triage: analyze issue, assign `squad:{member}` label | LD-7 |
| `squad:ld-7` | Pick up issue | LD-7 |
| `squad:fe-2` | Pick up issue | FE-2 |
| `squad:be-8` | Pick up issue | BE-8 |
| `squad:qt-3` | Pick up issue | QT-3 |

### How Issue Assignment Works

1. When a GitHub issue gets the `squad` label, **LD-7** triages it — analyzing content, assigning the right `squad:{member}` label, and commenting with triage notes.
2. When a `squad:{member}` label is applied, that member picks up the issue in their next session.
3. Members can reassign by removing their label and adding another member's label.
4. The `squad` label is the "inbox" — untriaged issues waiting for LD-7 review.

## Rules

1. **Eager by default** — spawn all agents who could usefully start work, including anticipatory downstream work.
2. **Scribe always runs** after substantial work, always as `mode: "background"`. Never blocks.
3. **Quick facts → coordinator answers directly.** Don't spawn an agent for simple lookups.
4. **When two agents could handle it**, pick the one whose domain is the primary concern.
5. **"Team, ..." → fan-out.** Spawn all relevant agents in parallel as `mode: "background"`.
6. **Anticipate downstream work.** If a feature is being built, spawn QT-3 to write test cases from requirements simultaneously.
7. **Issue-labeled work** — when a `squad:{member}` label is applied to an issue, route to that member. LD-7 handles all `squad` (base label) triage.
