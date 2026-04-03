import { describe, expect, test, mock, beforeEach } from "bun:test";
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
  beforeEach(() => {
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

    mock.module("../utils/db", () => ({
      getRounds: mock(() => Promise.resolve([mockRound])),
      getCourses: mock(() => Promise.resolve([mockCourse])),
    }));

    renderWithRouter(<HistoryPage />);

    await waitFor(() => {
      expect(screen.queryByText("Loading...")).toBeNull();
    });

    const metaText = screen.getByText(/27/);
    expect(metaText.textContent).toMatch(/14/);
    expect(metaText.textContent).toMatch(/32/);
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

    mock.module("../utils/db", () => ({
      getRounds: mock(() => Promise.resolve([mockRound1, mockRound2])),
      getCourses: mock(() => Promise.resolve([mockCourse])),
    }));

    renderWithRouter(<HistoryPage />);

    await waitFor(() => {
      expect(screen.queryByText("Loading...")).toBeNull();
    });

    const listItems = screen.getAllByRole("listitem");
    expect(listItems.length).toBe(2);

    // Rounds are sorted by date descending, so round-2 (16:45) comes first
    const firstMeta = listItems[0].querySelector(".history-page__meta");
    const secondMeta = listItems[1].querySelector(".history-page__meta");

    expect(firstMeta?.textContent).toMatch(/16/);
    expect(firstMeta?.textContent).toMatch(/45/);
    expect(secondMeta?.textContent).toMatch(/09/);
    expect(secondMeta?.textContent).toMatch(/15/);
  });

  test("displays midnight time (00:00) with time component", async () => {
    const mockRound: Round = {
      id: "round-1",
      courseId: "course-1",
      date: new Date("2026-03-27T00:00:00"),
      players: [{ id: "player-1", name: "Alice" }],
      scores: [],
    };

    mock.module("../utils/db", () => ({
      getRounds: mock(() => Promise.resolve([mockRound])),
      getCourses: mock(() => Promise.resolve([mockCourse])),
    }));

    renderWithRouter(<HistoryPage />);

    await waitFor(() => {
      expect(screen.queryByText("Loading...")).toBeNull();
    });

    const metaText = screen.getByText(/27/);
    expect(metaText.textContent).toMatch(/00/);
  });

  test("shows empty state when no rounds exist", async () => {
    mock.module("../utils/db", () => ({
      getRounds: mock(() => Promise.resolve([])),
      getCourses: mock(() => Promise.resolve([mockCourse])),
    }));

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

    mock.module("../utils/db", () => ({
      getRounds: mock(() => Promise.resolve([mockRound])),
      getCourses: mock(() => Promise.resolve([mockCourse])),
    }));

    renderWithRouter(<HistoryPage />);

    await waitFor(() => {
      expect(screen.queryByText("Loading...")).toBeNull();
    });

    expect(screen.getByText(/3 players/)).toBeDefined();
  });
});
