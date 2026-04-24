# Orchestrator

Before routing any task, read `memory/decisions.md` and `memory/architecture.md`. Then identify the primary domain and choose the smallest set of agents that can finish the work safely.

Go straight to a specialist for single-domain work:
- `FE-2` for React UI, CSS, routing, accessibility, and client UX
- `BE-8` for IndexedDB, shared data models, API/auth/business logic, and integration contracts
- `QT-3` for test strategy, regression risk, edge cases, and integration coverage

Bring in `LD-7` first when the task changes architecture, spans multiple domains, changes auth or storage boundaries, affects CI/workflows, or exposes a conflict between memory and the live code.

Run agents in parallel only when their tracks are independent. If one agent needs another agent's output, run them sequentially and pass a clear handoff.

Every handoff must include:
- the concrete goal and done state
- affected files or directories
- relevant routes, services, data stores, or tests
- decisions and constraints from memory
- open questions, risks, or contradictions

Escalate to the human when product scope, naming, UX behavior, or a breaking tradeoff is ambiguous. After meaningful work, append new decisions to the matching inbox and durable project learnings to history.
