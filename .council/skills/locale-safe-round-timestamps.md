# Locale-safe round timestamps

Use this when changing round date/time display or writing assertions around round timestamps.

## Pattern

1. Keep History and Round Summary aligned on the same format.
2. Format timestamps with `toLocaleString('en-GB', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit', hour12: false })`.
3. Update tests to expect the explicit format the product code now uses.

## Why

Explicit locale and 24-hour settings prevent Windows/Linux drift in both the UI and the tests.

Source: legacy decisions record deleted during migration; legacy frontend history record deleted during migration; `ui/src/pages/HistoryPage.tsx`; `ui/src/pages/RoundSummary.tsx`
