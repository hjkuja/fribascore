# FribaScore Roadmap

![Completion](https://progress-bar.xyz/34/?title=Completion&color=c77c27&width=250)

**15 of 44 items complete.** Items are grouped by area. Each pending row has an **Issue** column — link or create a GitHub issue there when you pick the item up so progress is traceable.

> **Updating the progress bar:** Change the number in the URL above (`/33/`) when items are closed. Formula: `round(done / total * 100)`.

---

## ✅ Core Features — Done

| # | Item | Notes |
|---|------|-------|
| 1 | Course list (browse) | `CourseList` component + `/courses` route |
| 2 | Course details (hole-by-hole table) | `CourseDetails` component + `/courses/:id` route |
| 3 | Start a round | `/courses/:id/start-round`, `StartRound` page |
| 4 | Player selection (modal, up to 6) | `PlayerSelectModal` component |
| 5 | Player management — create, edit, delete | `PlayersManagement` on Settings page |
| 6 | Hole-by-hole scoring | `ScoreCard` + `HoleScore` components |
| 7 | Auto-save scores (no explicit save action) | Scores persisted in IndexedDB on each entry |
| 8 | Round summary — totals + relative-to-par | `RoundSummary` page |
| 9 | Round history list | `HistoryPage` + links to summaries |
| 10 | IndexedDB data layer | `utils/db.ts` — courses, rounds, players stores |
| 11 | Development seed data | `data/dummyCourses.ts` loaded in local development when no courses exist |
| 12 | App routing + layout shell | `AppRoutes.tsx`, `AppLayout.tsx`, hamburger nav |

---

## 🎨 UI / Design System

The design spec (`docs/specs/ui-design.md`) defines a retro-futurist aesthetic. Shared tokens, typography, and some glass/background effects are in place, but the full visual rollout is still incomplete.

| Status | Item | Issue |
|--------|------|-------|
| ✅ | CSS design system — full custom-property palette (`--bg`, `--text`, `--accent`, `--glass-bg`, etc.) | [#16](https://github.com/hjkuja/fribascore/issues/16) |
| ✅ | Google Fonts — Bebas Neue + Space Grotesk loaded from Google Fonts | [#17](https://github.com/hjkuja/fribascore/issues/17) |
| ⬜ | Background effects — complete the current amber grid / film-grain foundation with scanlines and ambient orbs | [#18](https://github.com/hjkuja/fribascore/issues/18) |
| ⬜ | Glassmorphism card style — roll the existing card treatment out consistently across the app | [#19](https://github.com/hjkuja/fribascore/issues/19) |
| ⬜ | Button design system — adopt the existing primary + outline variants across the remaining screens | [#20](https://github.com/hjkuja/fribascore/issues/20) |
| ⬜ | Navigation bar — sticky glass, Bebas Neue logo, branded, mobile collapse | [#21](https://github.com/hjkuja/fribascore/issues/21) |
| ⬜ | Home page — hero / marketing layout (currently a placeholder) | [#22](https://github.com/hjkuja/fribascore/issues/22) |
| ⬜ | Start Round page — styled layout | [#23](https://github.com/hjkuja/fribascore/issues/23) |
| ⬜ | Round Scoring page — styled layout | [#24](https://github.com/hjkuja/fribascore/issues/24) |

---

## 📱 PWA

See `docs/specs/pwa.md` for full spec.

| Status | Item | Issue |
|--------|------|-------|
| ⬜ | Web App Manifest — `manifest.json` in `ui/public/`, linked in `index.html` | — |
| ⬜ | App icon — copy `icon-512.png` to `ui/public/`, replace `vite.svg` favicon | [#30](https://github.com/hjkuja/fribascore/issues/30) |
| ⬜ | Service worker — add `vite-plugin-pwa`, configure auto-registration | — |
| ⬜ | Offline caching strategy — precache app shell; network-first for API calls | — |
| ⬜ | iOS meta tags — `apple-mobile-web-app-capable`, `apple-touch-icon`, status bar style | — |
| ⬜ | Install prompt — intercept `beforeinstallprompt`; manual instructions for iOS | — |

---

## 🛠️ Developer Experience

Early DX work can happen in parallel, but it is **not** a product feature and **does not block** auth or the core API slices.

| Status | Item | Issue |
|--------|------|-------|
| ⬜ | Aspire AppHost for local dev orchestration — optional local DX for UI + API + local infra; not a prerequisite for auth or core API work | [#33](https://github.com/hjkuja/fribascore/issues/33) |

---

## 🖥️ Backend API

See `docs/api/overview.md`. The backend is scaffolded under `api/`, and auth endpoints are implemented. Core resource APIs are still in progress.

| Status | Item | Issue |
|--------|------|-------|
| ✅ | .NET Web API project scaffold (`api/` directory, solution file, CI hook) | [#25](https://github.com/hjkuja/fribascore/issues/25) |
| ✅ | Auth endpoints — `POST /auth/login`, `POST /auth/logout`, `GET /auth/me` | [#26](https://github.com/hjkuja/fribascore/issues/26) |
| ⬜ | Rename `AppDbContext` → `FribaDbContext`, regenerate initial migration | [#37](https://github.com/hjkuja/fribascore/issues/37) |
| ⬜ | Configuration options binding — implement Options pattern for typed config | [#38](https://github.com/hjkuja/fribascore/issues/38) |
| ⬜ | Courses API — stabilize the current `/api/courses` route group and remaining backend behavior | [#27](https://github.com/hjkuja/fribascore/issues/27) |
| ⬜ | Players API — finish the `/api/players` surface, ownership rules, and missing update behavior | [#28](https://github.com/hjkuja/fribascore/issues/28) |
| ⬜ | Rounds API — finish the `/api/rounds` surface and ownership rules | [#29](https://github.com/hjkuja/fribascore/issues/29) |

---

## ☁️ Cloud Sync

See `docs/specs/backend-sync.md`. Sync is additive — it must never block offline use.

| Status | Item | Issue |
|--------|------|-------|
| ⬜ | Frontend auth flow — sign-in / sign-out UI and cookie-session state handling | — |
| ⬜ | Sync queue — local queue of unsynced mutations, flushed when online + authenticated | — |
| ⬜ | Course sync — remote → local; replace seed data after first sync | — |
| ⬜ | Player sync — bidirectional, last-write-wins on modification time | — |
| ⬜ | Round sync — local → remote after round completion | — |
| ⬜ | Online/offline detection — trigger sync on `online` event + app load | — |

---

## 🧪 Testing

| Status | Item | Issue |
|--------|------|-------|
| ⬜ | Component test coverage — most pages and several components lack tests | — |
| ⬜ | E2E tests — configure Playwright in `test/e2e/playwright/`, write smoke tests | — |
| ⬜ | Upgrade backend test projects from xUnit v2 to xUnit v3 | [#36](https://github.com/hjkuja/fribascore/issues/36) |

---

## 📝 Notes

- **Tracking issues:** When you open a GitHub issue for an item, paste the link (e.g. `[#12](https://github.com/hjkuja/fribascore/issues/12)`) in the **Issue** column and bump the progress bar %.
- **GitHub Project:** If the backlog grows large, consider creating a [GitHub Project](https://github.com/hjkuja/fribascore/projects) and linking it here for Kanban/table views with automatic status tracking.
- **Milestones:** Grouping issues into milestones (e.g. *v0.2 — PWA*, *v0.3 — Backend*) gives free per-milestone progress bars in GitHub's UI.
