## 1. Types and Service

- [x] 1.1 Create explicit Workflow Inbox autocomplete request, response, and item types without using `any`
- [x] 1.2 Create `workflowInboxAutocomplete.service.ts` under `src/modules/gestionCorrespondencia/services`
- [x] 1.3 Implement the service request to the approved autocomplete endpoint with controlled `search` and `limit`
- [x] 1.4 Map backend autocomplete response items to the internal `{ value, label? }` contract
- [x] 1.5 Ensure backend-specific fields do not leak into `AppInputSearch.options`

## 2. Autocomplete Hook

- [x] 2.1 Create `useWorkflowInboxAutocomplete.ts` under `src/modules/gestionCorrespondencia/hooks`
- [x] 2.2 Implement required `minLength` and controlled `limit` parameters
- [x] 2.3 Prevent backend calls and clear items when search text is shorter than `minLength`
- [x] 2.4 Implement debounce for suggestion requests only inside the hook
- [x] 2.5 Expose `items`, `loading`, `error`, `setSearchText`, and `clear`
- [x] 2.6 Handle service errors without throwing to the component tree
- [x] 2.7 Ignore or cancel obsolete responses so older requests cannot overwrite newer items
- [x] 2.8 Clean up pending timers or request guards on unmount

## 3. Gestion Correspondencia Integration

- [x] 3.1 Integrate `useWorkflowInboxAutocomplete` in `GestionCorrespondencia` without calling the service directly from the page
- [x] 3.2 Keep `AppTableQueryWrapper` rendered with `showSearch={false}`
- [x] 3.3 Pass autocomplete `options` and `loading` to `AppInputSearch`
- [x] 3.4 Configure `AppInputSearch` without typing debounce for the autocomplete flow
- [x] 3.5 Wire typing to autocomplete text updates without applying table search on every keystroke
- [x] 3.6 Wire Enter and search icon confirmation to `table.onQueryChange({ search: value })`
- [x] 3.7 Wire suggestion selection to `table.onQueryChange({ search: selectedValue })`
- [x] 3.8 Wire clear to clear autocomplete suggestions and update table search with `table.onQueryChange({ search: "" })`
- [x] 3.9 Preserve existing toolbar actions, export, pagination, selection, and table contracts

## 4. Tests

- [x] 4.1 Add service tests covering endpoint call and response mapping
- [x] 4.2 Add hook tests covering no request below `minLength`
- [x] 4.3 Add hook tests covering request with `search` and `limit`
- [x] 4.4 Add hook tests covering loading and success item mapping
- [x] 4.5 Add hook tests covering error handling without throwing
- [x] 4.6 Add hook tests covering cleanup when text no longer satisfies `minLength`
- [x] 4.7 Add hook tests covering obsolete response protection
- [x] 4.8 Add integration tests for typing updating autocomplete without applying table search per keystroke
- [x] 4.9 Add integration tests for Enter or search icon applying free text search
- [x] 4.10 Add integration tests for selecting a suggestion applying table search
- [x] 4.11 Add integration tests for clear updating search through the existing query state flow
- [x] 4.12 Add regression tests verifying toolbar actions, wrapper `showSearch={false}`, export, and pagination remain intact

## 5. Documentation and Validation

- [x] 5.1 Update component or module documentation if the autocomplete integration changes public usage guidance
- [x] 5.2 Run focused Vitest suites for service, hook, and Gestion Correspondencia integration
- [x] 5.3 Run focused ESLint over touched files
- [x] 5.4 Run TypeScript build validation
- [x] 5.5 Run `openspec validate scrumcore-69-implementacion-auto-complete --strict`
- [x] 5.6 Run `git diff --check`
- [x] 5.7 Record validation evidence in this task checklist before archive

## Validation Evidence

- `npm.cmd test -- src/modules/gestionCorrespondencia/tests/workflowInboxAutocomplete.service.test.ts src/modules/gestionCorrespondencia/tests/useWorkflowInboxAutocomplete.test.tsx src/modules/gestionCorrespondencia/tests/GestionCorrespondencia.test.tsx` passed outside sandbox: `5 files`, `18 tests`
- `npx.cmd eslint src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx src/modules/gestionCorrespondencia/hooks/useWorkflowInboxAutocomplete.ts src/modules/gestionCorrespondencia/services/workflowInboxAutocomplete.service.ts src/modules/gestionCorrespondencia/types/workflowInboxAutocomplete.types.ts src/modules/gestionCorrespondencia/tests/workflowInboxAutocomplete.service.test.ts src/modules/gestionCorrespondencia/tests/useWorkflowInboxAutocomplete.test.tsx src/modules/gestionCorrespondencia/tests/GestionCorrespondencia.test.tsx` passed
- `npx.cmd tsc -b` passed
- `npx.cmd openspec validate scrumcore-69-implementacion-auto-complete --strict` passed
- `git diff --check` passed
