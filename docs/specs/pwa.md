# PWA Spec

This document covers the requirements and implementation plan for making FribaScore a Progressive Web App (PWA).

## Goals

- The app is installable on Android and iOS via "Add to Home Screen"
- The app works fully offline after first load
- The app loads quickly from cache on repeat visits
- The installed experience feels native (no browser chrome, themed UI)

## Status

| Feature | Status |
|---------|--------|
| `manifest.json` | ⬜ Not started |
| Service worker registration | ⬜ Not started |
| Offline caching strategy | ⬜ Not started |
| Install prompt | ⬜ Not started |
| iOS meta tags | ⬜ Not started |

## Web App Manifest (`manifest.json`)

A `manifest.json` must be added to `ui/public/` and linked in `index.html`.

Required fields:

```json
{
  "name": "FribaScore",
  "short_name": "FribaScore",
  "start_url": "/",
  "display": "standalone",
  "background_color": "#ffffff",
  "theme_color": "#2d7a4f",
  "icons": [
    { "src": "/icon-512.png", "sizes": "512x512", "type": "image/png" }
  ]
}
```

> The `icon-512.png` already exists in `dev/` and should be moved or copied to `ui/public/`.

## Service Worker

A service worker is needed to cache app assets for offline use. The recommended approach is to use [Vite PWA plugin](https://vite-pwa-org.netlify.app/) (`vite-plugin-pwa`), which handles service worker generation and manifest injection automatically.

### Caching strategy

| Asset type | Strategy |
|------------|----------|
| App shell (HTML, JS, CSS) | Cache first (precached at install time) |
| Course images / static assets | Cache first |
| API requests (backend) | Network first, fallback to cache |

### Registration

The service worker should be registered in `ui/src/main.tsx` (or via the Vite PWA plugin's auto-registration).

## Install Prompt

An in-app "Install" button or banner should appear when the browser fires the `beforeinstallprompt` event. This is handled in JavaScript and should be shown contextually (e.g., on the home screen or settings page), not as an intrusive popup.

## iOS Considerations

iOS Safari does not support the `beforeinstallprompt` event. To support iOS:
- Add `<meta name="apple-mobile-web-app-capable" content="yes">` to `index.html`
- Add `<meta name="apple-mobile-web-app-status-bar-style" content="default">`
- Add `<link rel="apple-touch-icon" href="/icon-512.png">` to `index.html`
- Consider a manual "how to install" prompt for iOS users

## Acceptance Criteria

- [ ] Lighthouse PWA audit passes
- [ ] App installs successfully on Android Chrome
- [ ] App installs successfully on iOS Safari (manual prompt)
- [ ] Installed app loads without network connection
- [ ] All core features (browsing, scoring) work offline after install
