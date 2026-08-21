## Context

See `proposal.md` for motivation and `specs/e2e-enviar-usuario-workflow/spec.md` for the observable contract. The existing E2E package has a reusable Gestión login helper and separate DOC-10 preview and DOC-11 execution suites, but neither endpoint exercises the DOC-28 user-destination contract. The new ASMX methods require all preview arguments explicitly and return a user destination and token that the caller must use for execution.

The repository runbook prohibits authenticated E2E, load, gate activation and database mutations without explicit environment and account authorization. Control queries are read-only, and any execution must leave the gate disabled with empty scope.

## Goals / Non-Goals

**Goals:**

- Add a DOC-28 Playwright suite that calls `PreviewEnviarUsuario` and `EjecutarEnvioUsuario` through a real Gestión session when an authorized environment is supplied.
- Make preview the normal E2E path and verify its state and audit non-mutation using before/after fingerprints from approved `SELECT` statements.
- Make a mutation impossible to start unless the runner declares explicit authorization, a disposable task and all read-only controls.
- Produce short, sanitized JSON evidence that identifies endpoint, result, counts and hashes without storing credentials, cookies, connection strings or response bodies.

**Non-Goals:**

- Do not alter the ASMX contracts, Workflow business rules, the legacy flow, the feature gate or its user/group scope.
- Do not create test data, reset a transitioned task, infer environment permission or run browsers against any URL as part of implementation.
- Do not make an E2E run a merge prerequisite when the required authorized environment is unavailable; the absence must remain explicit evidence rather than a fabricated pass.

## Decisions

### One DOC-28 suite with separately gated modes

Create a dedicated user-send test suite and command validation script under `tools/e2e`. The suite will use the established Gestión session helper and explicit JSON envelope assertions, but will send the DOC-28 preview payload with all four ASMX arguments. It will expose distinct commands/tags for anonymous, authenticated validation, read-only preview and authorized execution.

This preserves the existing DOC-10/11 evidence and gives the user-specific endpoint an independently identifiable report. Extending the DOC-10 suite was rejected because that suite models a different contract and its environment variables/report would make a user-send result ambiguous.

### Derive execution input from the current preview

The execution test will first perform a successful preview using the same authenticated context, then select a returned destination and its `TokenVersion` for the execution payload. It will not accept destination IDs or a token as test configuration.

This prevents stale or operator-supplied destination data from hiding a preview/execution contract mismatch. Supplying destination IDs through environment variables was rejected because it would bypass the behavior that needs E2E proof.

### Fail closed before browser or HTTP activity

The command validator will reject missing or malformed prerequisite variables before Playwright creates a context. Execution additionally requires an exact affirmative authorization variable, a positive disposable task ID, expected outcome and read-only state/audit controls. Preview requires only the environment, valid session and read-only controls for its full mode.

This is stricter than relying on test naming or CI selection and ensures a copied command cannot silently make a transition. A single general-purpose command was rejected because it could accidentally include an execution-tagged test.

### Preserve evidence integrity and local invariants

All database control statements will be validated as one `SELECT` using exactly one positional task parameter. The runner will write only hashes, code, success flag, destination count and mutation flags. Its documentation will require an initial/final local gate check and verification that legacy pages are unchanged.

Raw response capture, screenshots and traces are unsuitable as evidence because they can contain session artifacts or server data. The Playwright configuration will keep its existing failure diagnostics, but generated artifacts remain ignored and are never committed.

## Risks / Trade-offs

- [An authorized QA deployment does not match the branch under test] → Record the deployed revision and environment with the sanitized evidence; do not describe the result as proof of another revision.
- [No task has an available user destination] → Treat it as a functional setup result, retain the blocking code and ask the environment owner for a disposable task; never manufacture a destination.
- [The task state changes outside the test] → Compare state and audit fingerprints and mark the evidence inconclusive rather than passing it.
- [A DB account has unintended write privilege] → Validate query shape in the harness and require a separately provisioned read-only account; the test never issues write SQL.
- [Execution proof requires a destructive transition] → Limit it to an explicitly authorized disposable task, preserve before/after evidence and omit concurrency/load testing unless separately authorized.
- [Local gate configuration differs from the deployed environment] → Confirm the local invariant before/after and require the environment owner to attest its deployment configuration; the harness must not change either one.

## Migration Plan

1. Add the DOC-28 test suite, command validators and safe documentation without invoking them against a remote URL.
2. Run static/source tests and the local test command validation paths that intentionally reject incomplete configuration.
3. In an explicitly authorized QA environment, run anonymous and authenticated preview first; run the full read-only preview only after its SQL controls are approved.
4. Run execution only after explicit authorization names a disposable task; preserve sanitized evidence and recheck the local gate and legacy-file invariants.
5. If the harness must be withdrawn, remove its commands and files; it changes no runtime API, schema or deployed configuration.
