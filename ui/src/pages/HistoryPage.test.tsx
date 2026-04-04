import { describe, expect, test, mock, beforeEach, afterEach } from "bun:test";
import { render, screen, waitFor } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import HistoryPage from "./HistoryPage";
import type { Round } from "../types/round";
import type { Course } from "../types/course";
import type { ReactElement } from "react";
import * as db from "../utils/db";

function renderWithRouter(ui: ReactElement) {
  return render(<MemoryRouter>{ui}</MemoryRouter>);
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

describe("HistoryPage", () => {
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

  test("displays time of day alongside date for rounds", async () => {
    const mockRound: Round = {
      id: "round-1",
      courseId: "course-1",
      date: new Date("2026-03-27T14:32:00"),
      players: [{ id: "player-1", name: "Alice" }],
      scores: [],
    };

    mockGetRounds.mockImplementation(() => Promise.resolve([mockRound]));
    mockGetCourses.mockImplementation(() => Promise.resolve([mockCourse]));

    renderWithRouter(<HistoryPage />);

    await waitFor(() => {
      expect(screen.queryByText("Loading...")).toBeNull();
    });

    // Verify specific en-GB format: 27/03/2026, 14:32
    const metaText = screen.getByText(/27/);
    expect(metaText.textContent).toMatch(/27\/03\/2026, 14:32/);
  });

  test("distinguishes rounds with different times on same date", async () => {
    const mockRound1: Round = {
      id: "round-1",
      courseId: "course-1",
      date: new Date("2026-03-27T09:15:00"),
      players: [{ id: "player-1", name: "Alice" }],
      scores: [],
    };

    const mockRound2: Round = {
      id: "round-2",
      courseId: "course-1",
      date: new Date("2026-03-27T16:45:00"),
      players: [{ id: "player-2", name: "Bob" }],
      scores: [],
    };

    mockGetRounds.mockImplementation(() => Promise.resolve([mockRound1, mockRound2]));
    mockGetCourses.mockImplementation(() => Promise.resolve([mockCourse]));

    renderWithRouter(<HistoryPage />);

    await waitFor(() => {
      expect(screen.queryByText("Loading...")).toBeNull();
    });

    const listItems = screen.getAllByRole("listitem");
    expect(listItems.length).toBe(2);

    // Rounds are sorted by date descending, so round-2 (16:45) comes first
    const firstMeta = listItems[0].querySelector(".history-page__meta");
    const secondMeta = listItems[1].querySelector(".history-page__meta");

    // Verify both have time components in 24-hour format and are different
    expect(firstMeta?.textContent).toMatch(/16:45/);
    expect(secondMeta?.textContent).toMatch(/09:15/);
    expect(firstMeta?.textContent).not.toBe(secondMeta?.textContent);
  });

  test("displays midnight time (00:00) with time component", async () => {
    const mockRound: Round = {
      id: "round-1",
      courseId: "course-1",
      date: new Date("2026-03-27T00:00:00"),
      players: [{ id: "player-1", name: "Alice" }],
      scores: [],
    };

    mockGetRounds.mockImplementation(() => Promise.resolve([mockRound]));
    mockGetCourses.mockImplementation(() => Promise.resolve([mockCourse]));

    renderWithRouter(<HistoryPage />);

    await waitFor(() => {
      expect(screen.queryByText("Loading...")).toBeNull();
    });

    // Verify that time component is present in 24-hour format
    const metaText = screen.getByText(/27/);
    expect(metaText.textContent).toMatch(/00:00/);
  });

  test("shows empty state when no rounds exist", async () => {
    mockGetRounds.mockImplementation(() => Promise.resolve([]));
    mockGetCourses.mockImplementation(() => Promise.resolve([mockCourse]));

    renderWithRouter(<HistoryPage />);

    await waitFor(() => {
      expect(screen.queryByText("Loading...")).toBeNull();
    });

    expect(screen.getByText(/No rounds yet/)).toBeDefined();
  });

  test("displays player count correctly", async () => {
    const mockRound: Round = {
      id: "round-1",
      courseId: "course-1",
      date: new Date("2026-03-27T14:32:00"),
      players: [
        { id: "player-1", name: "Alice" },
        { id: "player-2", name: "Bob" },
        { id: "player-3", name: "Charlie" },
      ],
      scores: [],
    };

    mockGetRounds.mockImplementation(() => Promise.resolve([mockRound]));
    mockGetCourses.mockImplementation(() => Promise.resolve([mockCourse]));

    renderWithRouter(<HistoryPage />);

    await waitFor(() => {
      expect(screen.queryByText("Loading...")).toBeNull();
    });

    expect(screen.getByText(/3 players/)).toBeDefined();
  });
});
