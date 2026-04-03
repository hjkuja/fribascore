# Locale-Agnostic Test Assertions

## Purpose
Write date and time test assertions that pass on any locale, matching both colon (`:`) and period (`.`) time separators used across different platforms and locales.

## Pattern
```typescript
// Match time in HH:MM or HH.MM format
/\d{2}[.:]\d{2}/
```

## Usage

### Testing Formatted Dates with Times
When asserting that a date/time string is rendered (e.g., from `toLocaleString()`), use the regex pattern instead of checking specific digits:

```typescript
const dateElement = screen.getByText(/round/i);

// ✅ Locale-agnostic — works Windows (14.32), Linux (14:32), macOS, etc.
expect(dateElement.textContent).toMatch(/\d{2}[.:]\d{2}/);

// ❌ Locale-specific — fails when locale changes time separator
expect(dateElement.textContent).toMatch(/14/); // might be "14" or "2:00 PM"
expect(dateElement.textContent).toContain("14:32"); // only works in en-US
```

### Testing Distinct Times
When verifying that different times render differently:

```typescript
const time1Text = screen.getByTestId("time-1").textContent;
const time2Text = screen.getByTestId("time-2").textContent;

// Extract time component using the same regex
const time1Match = time1Text?.match(/\d{2}[.:]\d{2}/);
const time2Match = time2Text?.match(/\d{2}[.:]\d{2}/);

expect(time1Match?.[0]).not.toBe(time2Match?.[0]);
```

## When NOT to Use
- **Exact format verification:** If you absolutely need to verify the exact format, set an explicit locale in production code and match it in tests.
- **Date-only comparisons:** For date-only assertions (no time), use `.toMatch(/\d{1,2}/)` or similar, without time separators.
- **Non-time numeric checks:** Don't use this pattern for unrelated numbers; be specific about context.

## Implementation Notes

### Why Regex Instead of Hardcoded Strings?
- Tests are more robust across locales
- Avoids CI/local environment mismatches
- Remains representative of actual user behavior
- No need to mock or force specific locales in production

### Related Patterns
- **DateTime formatting in code:** Use `toLocaleString(undefined, { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' })`
- **For database assertions:** Compare `Date` objects directly, not formatted strings

## Examples

### HistoryPage.test.tsx
```typescript
test("displays round with time", () => {
  render(<HistoryPage />);
  const roundElement = screen.getByText(/March 27/);
  expect(roundElement.textContent).toMatch(/\d{2}[.:]\d{2}/);
});
```

### RoundSummary.test.tsx
```typescript
test("shows correct round date and time", () => {
  render(<RoundSummary />);
  expect(screen.getByTestId("round-date").textContent).toMatch(/\d{2}[.:]\d{2}/);
});
```

## References
- **Issue:** #10 (History page — show time of day)
- **Branch:** squad/10-history-time-of-day
- **Related Decision:** Locale-Agnostic Date/Time Test Assertions (2026-04-03)
