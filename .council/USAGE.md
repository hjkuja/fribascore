# Using the Council

## Start a session

1. Load `.council/orchestrator.md`, `.council/routing.md`, `.council/memory/decisions.md`, and `.council/memory/architecture.md`.
2. If the task is clearly single-domain, also load the matching charter and its history file.
3. State the goal, done state, and any files or docs that already matter.

If you like a Star Wars framing, you can refer to the orchestrator as the council. The canonical file name is still `orchestrator.md`.

## Talk to one agent vs the orchestrator

- Use the orchestrator when the task spans UI, API, testing, docs, or architecture.
- Go straight to `FE-2`, `BE-8`, or `QT-3` for isolated work in their own domain.
- Bring in `LD-7` when the task changes auth, storage, routing boundaries, dependencies, CI/workflows, or documented architecture.

## Hand off between agents mid-task

1. Summarize the current goal, what is already done, and the next done state.
2. Include touched files, relevant tests, and the parts of `memory/decisions.md` and `memory/architecture.md` that constrain the work.
3. Point the next agent at the right history or inbox file.
4. If the handoff changes direction or exposes a conflict, route through `LD-7` first.

## Invoke the scribe

- Run the scribe only when the human explicitly wants session decisions merged into durable memory.
- Load `.council/charters/scribe.md` plus all `memory/inbox/*.md` files.

## Add a new skill

1. Create a markdown file under `.council/skills/`.
2. Keep it short: when to use it, the repeatable steps, pitfalls, and source references.
3. Mention the new skill in the relevant history file if it captures a recurring project pattern.

## Update `routing.md` manually when

- a new top-level directory or service appears
- ownership shifts between FE-2, BE-8, QT-3, or LD-7
- the repo adds a dedicated role that should own CI, data, or security
- the human wants different escalation rules

## Example: players API plus settings UI

1. Start with the orchestrator and the memory files because the task spans `api/src`, `ui/src/components/PlayersManagement`, and tests.
2. Let `LD-7` confirm boundaries first if auth or ownership rules are part of the change.
3. `BE-8` updates the Minimal API endpoints, Application services, and any affected contracts or storage behavior.
4. `FE-2` updates the React settings flow and keeps the UI accessible and mobile-friendly.
5. `QT-3` updates `api/test/` plus the relevant UI tests.
6. End the session by running `scribe` so any new decisions in the inbox become durable project memory.
