# Bun module mock isolation

Use this when a Bun test mocks a module that other tests in the same process also import.

## Pattern

1. Import the real module before mocking it.
2. In `beforeEach`, capture the original exports and call `mock.module(...)` once.
3. Change per-test behavior with `mockImplementation()` on the mock functions.
4. In `afterEach`, restore the original module with `mock.module(..., () => originalModule)` before calling `mock.restore()`.

## Why

Bun module mocks can leak across test files if only `mock.restore()` is called.

Source: legacy decisions record deleted during migration; legacy tester history record deleted during migration
