import { describe, expect, test, mock } from "bun:test";
import { render } from "@testing-library/react";
import { HoleScore } from "./HoleScore";
import type { Hole } from "../../types/course";
import type { Player } from "../../types/player";

const mockHole: Hole = { number: 3, par: 4, length: 150 };

const mockPlayers: Player[] = [
  { id: "player-1", name: "Alice" },
  { id: "player-2", name: "Bob" },
];

describe("HoleScore", () => {
  test("renders hole number and par", () => {
    const { getByText } = render(
      <HoleScore
        hole={mockHole}
        players={mockPlayers}
        getScore={() => ""}
        onScoreChange={mock()}
      />
    );
    expect(getByText(/Hole 3/)).toBeDefined();
    expect(getByText(/Par: 4/)).toBeDefined();
  });

  test("renders a row for each player", () => {
    const { getByText } = render(
      <HoleScore
        hole={mockHole}
        players={mockPlayers}
        getScore={() => ""}
        onScoreChange={mock()}
      />
    );
    expect(getByText("Alice")).toBeDefined();
    expect(getByText("Bob")).toBeDefined();
  });

  test("renders score inputs for each player", () => {
    const { getAllByRole } = render(
      <HoleScore
        hole={mockHole}
        players={mockPlayers}
        getScore={() => ""}
        onScoreChange={mock()}
      />
    );
    const inputs = getAllByRole("spinbutton");
    expect(inputs.length).toBe(2);
  });

  test("shows existing scores in inputs", () => {
    const getScore = (playerId: string) => (playerId === "player-1" ? 5 : "");
    const { getAllByRole } = render(
      <HoleScore
        hole={mockHole}
        players={mockPlayers}
        getScore={getScore}
        onScoreChange={mock()}
      />
    );
    const inputs = getAllByRole("spinbutton") as HTMLInputElement[];
    expect(inputs[0].value).toBe("5");
  });

  test("renders hole length", () => {
    const { getByText } = render(
      <HoleScore
        hole={mockHole}
        players={mockPlayers}
        getScore={() => ""}
        onScoreChange={mock()}
      />
    );
    expect(getByText(/150 m/)).toBeDefined();
  });
});

