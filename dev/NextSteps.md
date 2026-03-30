# Next Steps for FribaScore UI

Based on the requirements and current project state, here's a phased plan to build out the Frisbee Golf Scorecard app. Focus on offline-first functionality first, then add sync and PWA features.

## Phase 1: Core Data Types and Start Round (1-2 hours)

- Add missing TypeScript types for Player, Round, and ScoreEntry to `types/`.
- Implement "Start Round" page: Add button to CourseDetails, create StartRound component/page for player selection.
- Update routing in AppRoutes.tsx.

## Phase 2: Local Storage Setup (1-2 hours)

- Install IndexedDB library (e.g., `idb` or `dexie`).
- Set up IndexedDB stores for courses, players, rounds, and sync queue in a new `utils/db.ts`.
- Integrate persistence into existing components (load/save data locally).

## Phase 3: Round Scoring (2-4 hours)

- Create RoundScoring page and ScoreCard component for hole-by-hole scoring.
- Add HoleScore sub-component for individual hole inputs.
- Save scores to IndexedDB as users progress; navigate to summary on completion.

## Phase 4: Remaining Pages and Polish (2-4 hours each)

- Round Summary: Display results, save round, trigger sync if online.
- History: List past rounds from IndexedDB.
- Settings: Player management, sign-in, sync status.
- PWA Setup: Add manifest.json, service worker, install prompts.

## Phase 5: Backend Integration (Separate)

- Set up .NET Web API in `backend/` for courses, auth, players, rounds, sync.
- Add sync logic to frontend (upload local data when online/signed in).

## Tips

- Test each phase with `npm run dev` and mobile browser.
- Run `npm run build` and `npm run lint` after changes.
- Prioritize offline functionality; sync is optional.
