# UI Design Guidelines

FribaScore's visual identity draws from a **retro-futurist** aesthetic: dark backgrounds, warm amber accents, CRT-era textures, and clean modern typography. The result feels like a high-tech instrument built in an analog workshop.

---

## Colour Palette

All colours are defined as CSS custom properties and should be referenced by name, never hardcoded.

```css
:root {
  --bg:           #242424;               /* Primary background — dark charcoal */
  --bg-deep:      #1a1a1a;               /* Deeper background for contrast sections */
  --accent:       #c77c27;               /* Amber orange — the signature accent */
  --accent-dim:   rgba(199,124,39,0.30); /* Borders, dividers, subtle glow */
  --accent-glow:  rgba(199,124,39,0.15); /* Box shadows, card hover glow */
  --accent-tint:  rgba(199,124,39,0.06); /* Badge/tag backgrounds, faint fills */
  --text:         #f0e6d3;               /* Primary text — warm off-white */
  --text-muted:   #9a8f82;               /* Secondary text — warm gray */
  --glass-bg:     rgba(255,255,255,0.04);/* Glass card surfaces */
  --glass-border: rgba(255,255,255,0.08);/* Subtle light borders */
}
```

### Colour Usage Rules

| Use | Colour |
|-----|--------|
| Primary text | `--text` |
| Secondary / supporting text | `--text-muted` |
| Interactive elements, highlights | `--accent` |
| Card and panel surfaces | `--glass-bg` |
| Card borders | `--glass-border` |
| Hover glows, shadows | `--accent-glow` / `--accent-dim` |
| Dark section backgrounds | `--bg-deep` |

> **Do not** use pure white (`#ffffff`) or pure black (`#000000`). All colours should have warmth.

---

## Typography

Two typefaces are used. Both are loaded from Google Fonts.

```html
<link href="https://fonts.googleapis.com/css2?family=Bebas+Neue&family=Space+Grotesk:wght@300;400;500;600;700&display=swap" rel="stylesheet" />
```

### Typefaces

| Role | Font | Notes |
|------|------|-------|
| Display / Headings | `Bebas Neue` | All-caps, condensed. Used for hero text, section titles, logos, stat numbers. |
| Body / UI | `Space Grotesk` | Weights 300–700. Used for all body copy, navigation, buttons, labels. |

### Type Scale

| Element | Size | Weight | Letter spacing | Other |
|---------|------|--------|----------------|-------|
| Hero H1 | `clamp(3.8rem, 7.5vw, 6.5rem)` | — (Bebas Neue) | `3px` | Line-height `0.92` |
| Section title | `clamp(2.6rem, 5vw, 4rem)` | — (Bebas Neue) | `2px` | Line-height `1` |
| Section label | `0.7rem` | `700` | `3.5px` | Uppercase, `--accent` colour |
| Body copy | `1.05rem` | `400` | — | Line-height `1.75`, `--text-muted` |
| Card body | `0.875rem` | `400` | — | Line-height `1.65` |
| Nav links | `0.875rem` | `500` | `0.4px` | |
| Small labels | `0.7–0.78rem` | `700` | `1.5–2px` | Uppercase where used as metadata |

### Section Labels

Sections are introduced with a small all-caps label above the title:

```html
<p class="section-label">Why FribaScore</p>
<h2 class="section-title">Everything You Need<br />On The Fairway</h2>
```

---

## Backgrounds & Textures

The retro-futurist atmosphere comes largely from layered background effects.

### Grid Pattern

A subtle amber grid is applied to the full page via `body::before`:

```css
body::before {
  content: '';
  position: fixed; inset: 0;
  background-image:
    linear-gradient(rgba(199,124,39,0.04) 1px, transparent 1px),
    linear-gradient(90deg, rgba(199,124,39,0.04) 1px, transparent 1px);
  background-size: 56px 56px;
  pointer-events: none;
  z-index: 0;
}
```

### Grain Overlay

A subtle film-grain texture sits above everything via `body::after` using an inline SVG noise filter (opacity `0.5`). This adds the analog/retro feel.

### Scanlines (Hero)

The hero section adds a CRT scanline effect:

```css
background: repeating-linear-gradient(
  0deg, transparent, transparent 3px,
  rgba(0,0,0,0.06) 3px, rgba(0,0,0,0.06) 4px
);
```

### Ambient Orbs

Large blurred radial gradients placed off-screen edges create a soft ambient glow, giving depth without hard visual elements. Use sparingly — one or two per major section at most.

```css
.orb {
  border-radius: 50%;
  filter: blur(100px);
  background: radial-gradient(circle, rgba(199,124,39,0.20) 0%, transparent 70%);
}
```

---

## Glass Morphism Cards

Cards and panels use a "frosted glass" treatment:

```css
.card {
  background: var(--glass-bg);
  border: 1px solid var(--glass-border);
  border-radius: 10px;
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
}
```

### Card Hover State

Cards lift and glow on hover:

```css
.card:hover {
  border-color: var(--accent-dim);
  box-shadow: 0 8px 32px var(--accent-glow), inset 0 1px 0 rgba(255,255,255,0.06);
  transform: translateY(-5px);
  transition: border-color 0.25s, box-shadow 0.25s, transform 0.25s;
}
```

---

## Buttons

Two button variants are used.

### Primary Button

Solid amber fill with dark text. Used for the main call-to-action.

```css
.btn-primary {
  background: var(--accent);
  color: #1a1a1a;
  border: none;
  padding: 0.85rem 2.1rem;
  border-radius: 3px;
  font-family: 'Space Grotesk', sans-serif;
  font-size: 0.95rem;
  font-weight: 700;
  letter-spacing: 0.5px;
}
.btn-primary:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 28px rgba(199,124,39,0.45);
  filter: brightness(1.08);
}
```

### Outline Button

Transparent background with amber border. Used for secondary actions.

```css
.btn-outline {
  border: 1px solid var(--accent);
  color: var(--accent);
  background: transparent;
  padding: 0.5rem 1.3rem;
  border-radius: 3px;
  font-size: 0.85rem;
  font-weight: 600;
  letter-spacing: 0.5px;
}
.btn-outline:hover {
  background: var(--accent);
  color: #1a1a1a;
  box-shadow: 0 0 16px var(--accent-dim);
}
```

> **Border radius on buttons is `3px`** — deliberately sharp to reinforce the retro-technical aesthetic. Do not use large rounded corners on buttons.

---

## Navigation

The nav bar is sticky with a frosted glass background:

```css
nav {
  position: sticky; top: 0;
  height: 64px;
  background: rgba(36,36,36,0.75);
  backdrop-filter: blur(20px);
  border-bottom: 1px solid var(--glass-border);
}
```

- Logo uses `Bebas Neue`, amber colour, letter-spacing `3px`, with a glow text-shadow
- Nav links are `--text-muted` by default, transition to `--accent` on hover
- On mobile (`< 600px`), nav links are hidden; only logo and CTA remain

---

## Dividers

A styled "retro divider" is used between content sections:

```css
.retro-divider {
  display: flex; align-items: center; gap: 1rem;
}
.retro-divider::before, .retro-divider::after {
  content: ''; flex: 1;
  height: 1px; background: var(--glass-border);
}
.retro-divider span {
  font-family: 'Bebas Neue', sans-serif;
  font-size: 0.75rem; letter-spacing: 4px;
  color: var(--text-muted);
}
```

---

## Status Badges

Small pill-shaped badges (e.g., "Now in Beta", status indicators):

```css
.badge {
  display: inline-flex; align-items: center; gap: 0.5rem;
  background: var(--accent-tint);
  border: 1px solid var(--accent-dim);
  border-radius: 100px;
  padding: 0.38rem 1rem;
  font-size: 0.7rem; font-weight: 700;
  letter-spacing: 2px; text-transform: uppercase;
  color: var(--accent);
}
```

A blinking dot `●` (via `::before` with a keyframe animation) can be used for live/active state indicators.

---

## Animation

Animations are subtle and purposeful — they add atmosphere without distracting.

| Animation | Use | Timing |
|-----------|-----|--------|
| `fadeIn` — `opacity` + `translateY(20px → 0)` | Scroll-triggered element reveal | `0.6s ease` |
| Card hover lift — `translateY(-5px)` | Interactive card feedback | `0.25s` |
| Button hover lift — `translateY(-2px)` | Primary button interaction | `0.2s` |
| Pulsing rings — `scale(1 → 1.04)` | Atmospheric disc/logo animation | `3.5s ease-in-out infinite` |
| Float — `translateY(0 → -22px)` | Hero visual floating element | `5.5s ease-in-out infinite` |
| Blink — `opacity(1 → 0.2)` | Status dot | `2s ease-in-out infinite` |

> Prefer `transform` and `opacity` for animations — they are GPU-accelerated and don't cause layout reflow.

---

## Code / Terminal Blocks

When showing code or terminal output, use a styled "code window" with macOS-style traffic light dots:

```
┌──────────────────────────────────┐
│  ● ● ●   filename.ts             │  ← title bar (glass bg)
├──────────────────────────────────┤
│  // code content                 │  ← monospace font, muted amber syntax
└──────────────────────────────────┘
```

Traffic light dot colours: `#ff5f57` (red), `#febc2e` (yellow), `#28c840` (green).

---

## Layout & Spacing

- **Max content width:** `1120px`, centered with auto margins
- **Section padding:** `6.5rem` top/bottom, `2.5rem` horizontal
- **Hero padding:** `5rem` vertical, `2.5rem` horizontal
- **Grid gaps:** `1.4rem` for card grids, `5rem` for two-column feature layouts

### Responsive Breakpoints

| Breakpoint | Change |
|------------|--------|
| `≤ 900px` | Two-column layouts collapse to single column; hero visual scales down |
| `≤ 600px` | Nav links hidden; hero visual hidden; feature grid becomes single column |

---

## Responsive & Mobile

FribaScore is primarily a **mobile field app**. Responsive behaviour should prioritise the scoring flow on small screens. The marketing aesthetic above applies to the landing/home experience; within the scoring UI, the same colour and type system applies but layout simplifies significantly.

Key mobile principles:
- Touch targets minimum `44px` tall
- Avoid hover-dependent interactions as the only way to reveal content
- The frosted-glass card style works well on mobile as a content container
- Keep `Bebas Neue` for score numbers — large numeric values in the display font feel native to the aesthetic
