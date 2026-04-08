## 1. Verification

- [x] 1.1 Verify `GestionCorrespondencia.tsx` renders `AppInputSearch` inside `AppToolbar.actionContent`.
- [x] 1.2 Verify the toolbar search value comes from `table.queryState.search`.
- [x] 1.3 Verify the toolbar search updates state through `table.onQueryChange({ search })`.
- [x] 1.4 Verify `AppTableQueryWrapper` remains rendered with `showSearch={false}`.
- [x] 1.5 Verify refresh, contextual response, export, and pagination controls remain present and delegated to existing components.

## 2. Implementation

- [x] 2.1 Apply minimal code changes only if verification finds a gap against the spec.
- [x] 2.2 Keep `GestionCorrespondencia.tsx` free of backend service calls, endpoint knowledge, and manual request construction.
- [x] 2.3 Keep `toolbarSearch` limited to module-scoped layout styles and avoid changing `AppInputSearch` internal semantics or accessibility.

## 3. Tests

- [x] 3.1 Ensure `GestionCorrespondencia` tests cover exactly one visible search with accessible name `Buscar tareas workflow`.
- [x] 3.2 Ensure tests cover typing in the toolbar search and calling `table.onQueryChange({ search: value })`.
- [x] 3.3 Ensure tests cover `AppTableQueryWrapper` not rendering its internal search when toolbar search is present.
- [x] 3.4 Ensure tests cover existing toolbar actions remaining available.
- [x] 3.5 Run the focused `GestionCorrespondencia` test suite.

## 4. Validation

- [x] 4.1 Run focused lint or TypeScript checks for touched files if code changes are needed.
- [x] 4.2 Run `openspec validate scrumcore-67-integracion-appinputsearch-apptoolbar --strict`.
- [x] 4.3 Confirm no unrelated frontend or backend changes are included in the final diff.
