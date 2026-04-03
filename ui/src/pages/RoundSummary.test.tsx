import { describe, expect, test, mock, beforeEach, afterEach } from "bun:test";
import { render, screen, waitFor } from "@testing-library/react";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import RoundSummary from "./RoundSummary";
import type { Round } from "../types/round";
import type { Course } from "../types/course";
import type { ReactElement } from "react";
import * as db from "../utils/db";

function renderWithRouter(ui: ReactElement, initialEntries: string[] = ["/"]) {
  return render(
    <MemoryRouter initialEntries={initialEntries}>
      <Routes>
        <Route path="/rounds/:id/summary" element={ui} />
      </Routes>
    </MemoryRouter>
  );
}

const mockCourse: Course = {
  id: "course-1",
  name: "Test Disc Golf Course",
  holes: [
    { number: 1, par: 3, length: 100 },
    { number: 2, par: 4, length: 150 },
  ],
  totalPar: 7,
  totalLength: 250,
};

describe("RoundSummary", () => {
  let mockGetRounds: ReturnType<typeof mock>;
  let mockGetCourses: ReturnType<typeof mock>;
  let originalDbModule: typeof db;

  beforeEach(() => {
    // Store original before mocking
    originalDbModule = { ...db };
    
    mockGetRounds = mock(() => Promise.resolve([]));
    mockGetCourses = mock(() => Promise.resolve([]));
    
    mock.module("../utils/db", () => ({
      getRounds: mockGetRounds,
      getCourses: mockGetCourses,
    }));
  });

  afterEach(() => {
    // Restore the original module
    mock.module("../utils/db", () => originalDbModule);
    mock.restore();
  });

  test("displays time of day alongside date for the round", async () => {
    const mockRound: Round = {
      id: "round-1",
      courseId: "course-1",
      date: new Date("2026-03-27T14:32:00"),
      players: [{ id: "player-1", name: "Alice" }],
      scores: [{ playerId: "player-1", holeNumber: 1, score: 3 }],
    };

    mockGetRounds.mockImplementation(() => Promise.resolve([mockRound]));
    mockGetCourses.mockImplementation(() => Promise.resolve([mockCourse]));

    renderWithRouter(<RoundSummary />, ["/rounds/round-1/summary"]);

    await waitFor(() => {
      expect(screen.queryByText("Loading...")).toBeNull();
    });

    // Verify that time is present by checking for time separator (: or . depending on locale)
    const dateElement = screen.getByText(/27/);
    expect(dateElement.textContent).toMatch(/\d{2}[.:]\d{2}/);
  });

  test("renders date with different time correctly", async () => {
    const mockRound: Round = {
      id: "round-2",
      courseId: "course-1",
      date: new Date("2026-03-27T09:15:00"),
      players: [{ id: "player-1", name: "Alice" }],
      scores: [{ playerId: "player-1", holeNumber: 1, score: 3 }],
    };

    mockGetRounds.mockImplementation(() => Promise.resolve([mockRound]));
    mockGetCourses.mockImplementation(() => Promise.resolve([mockCourse]));

    renderWithRouter(<RoundSummary />, ["/rounds/round-2/summary"]);

    await waitFor(() => {
      expect(screen.queryByText("Loading...")).toBeNull();
    });

    // Verify that time component is present (HH:MM or HH.MM pattern)
    const dateElement = screen.getByText(/27/);
    expect(dateElement.textContent).toMatch(/\d{2}[.:]\d{2}/);
  });

  test("displays midnight time (00:00) with time component", async () => {
    const mockRound: Round = {
      id: "round-3",
      courseId: "course-1",
      date: new Date("2026-03-27T00:00:00"),
      players: [{ id: "player-1", name: "Alice" }],
      scores: [{ playerId: "player-1", holeNumber: 1, score: 3 }],
    };

    mockGetRounds.mockImplementation(() => Promise.resolve([mockRound]));
    mockGetCourses.mockImplementation(() => Promise.resolve([mockCourse]));

    renderWithRouter(<RoundSummary />, ["/rounds/round-3/summary"]);

    await waitFor(() => {
      expect(screen.queryByText("Loading...")).toBeNull();
    });

    // Verify that time component is present (HH:MM or HH.MM pattern)
    const dateElement = screen.getByText(/27/);
    expect(dateElement.textContent).toMatch(/\d{2}[.:]\d{2}/);
  });

  test("shows not found message when round does not exist", async () => {
    mockGetRounds.mockImplementation(() => Promise.resolve([]));
    mockGetCourses.mockImplementation(() => Promise.resolve([mockCourse]));

    renderWithRouter(<RoundSummary />, ["/rounds/nonexistent/summary"]);

    await waitFor(() => {
      expect(screen.queryByText("Loading...")).toBeNull();
    });

    expect(screen.getByText("Round not found")).toBeDefined();
  });

  test("displays player scores correctly", async () => {
    const mockRound: Round = {
      id: "round-1",
      courseId: "course-1",
      date: new Date("2026-03-27T14:32:00"),
      players: [
        { id: "player-1", name: "Alice" },
        { id: "player-2", name: "Bob" },
      ],
      scores: [
        { playerId: "player-1", holeNumber: 1, score: 3 },
        { playerId: "player-1", holeNumber: 2, score: 4 },
        { playerId: "player-2", holeNumber: 1, score: 4 },
        { playerId: "player-2", holeNumber: 2, score: 5 },
      ],
    };

    mockGetRounds.mockImplementation(() => Promise.resolve([mockRound]));
    mockGetCourses.mockImplementation(() => Promise.resolve([mockCourse]));

    renderWithRouter(<RoundSummary />, ["/rounds/round-1/summary"]);

    await waitFor(() => {
      expect(screen.queryByText("Loading...")).toBeNull();
    });

    expect(screen.getByText("Alice")).toBeDefined();
    expect(screen.getByText("Bob")).toBeDefined();
    expect(screen.getByText("E")).toBeDefined(); // Alice: 7 total, 7 par = E
    expect(screen.getByText("+2")).toBeDefined(); // Bob: 9 total, 7 par = +2
  });
});
