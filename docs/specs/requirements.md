# Requirements

This document lists the functional and non-functional requirements for FribaScore.

## Functional Requirements

### Courses
- The app must display a list of available disc golf courses
- Each course must show hole-by-hole details (hole number, par, length)
- Courses are seeded locally on first run; later fetched from the backend when available

### Round Management
- A user must be able to start a round on any course
- Up to 6 players can participate in a single round
- Players are selected or created at the start of a round

### Scoring
- Scores are entered per player per hole during an active round
- Scores are saved automatically as they are entered (no explicit save action)
- A round summary is shown after all holes are scored, displaying:
  - Total score per player
  - Score relative to par per player
  - Course information

### History
- Past rounds must be listed and accessible from the History page
- Round history is stored locally in IndexedDB

### Player Management
- Players can be created, edited, and deleted from the Settings page
- Player data persists across app sessions via IndexedDB

### Offline Support
- All features above must work fully without an internet connection
- No functionality should be blocked by lack of network access

### Optional: Cloud Sync
- When authenticated and online, rounds and player data should sync to a backend
- Sync must not block or degrade the offline experience

## Non-Functional Requirements

### Performance
- The app must load and be interactive quickly on a mid-range smartphone
- Score entry must feel instant with no perceptible delay

### Responsiveness
- The UI must be usable on small screens (mobile-first)
- Touch targets must be appropriately sized for field use

### Reliability
- No data loss on browser refresh, navigation, or accidental tab close
- Auto-save during scoring prevents lost data

### PWA
- The app must be installable as a PWA ("Add to Home Screen")
- The app must function when launched from the home screen without network
- See [PWA Spec](./pwa.md) for full requirements

### Accessibility
- Interactive elements must have appropriate ARIA labels
- Tables and structured content must use semantic HTML

## Out of Scope (Current Phase)
- Live multiplayer / real-time score sharing
- Course creation or editing by end users
- Advanced statistics beyond round totals and relative-to-par
