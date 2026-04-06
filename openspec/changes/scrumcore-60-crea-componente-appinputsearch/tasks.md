## 1. Component Setup

- [x] 1.1 Create `src/app/Components/UI/AppInputSearch/` with component, CSS module, and local barrel export.
- [x] 1.2 Export `AppInputSearch` from the shared UI barrel used by existing components.
- [x] 1.3 Define strict TypeScript props by composing the text-input contract from `AppInput` without introducing `any`.

## 2. AppInputSearch Implementation

- [x] 2.1 Implement `AppInputSearch` as a controlled-first component that delegates rendering and input states to `AppInput`.
- [x] 2.2 Add decorative search icon handling without changing the textbox accessible name.
- [x] 2.3 Add local styles for search-specific layout while preserving the existing `AppInput` visual contract.

## 3. AppTableQueryWrapper Integration

- [x] 3.1 Replace the `AppInput` search field in `AppTableQueryWrapper` with `AppInputSearch`.
- [x] 3.2 Preserve the existing `queryState.search`, `onQueryChange({ search })`, `searchPlaceholder`, and `showSearch` behavior.
- [x] 3.3 Remove imports or CSS that become unused after the migration.

## 4. Tests

- [x] 4.1 Add `AppInputSearch` tests covering controlled value, change notification, disabled/error states, and accessible name using `[SPEC:app-input-search]`.
- [x] 4.2 Update `AppTableQueryWrapper` tests to keep search behavior and hidden-search behavior covered with `[SPEC:app-input-search]`.
- [x] 4.3 Run focused Vitest coverage for `AppInputSearch` and `AppTableQueryWrapper`.

## 5. Validation

- [x] 5.1 Run focused ESLint for the new component and modified AppTable files.
- [x] 5.2 Run `openspec validate scrumcore-60-crea-componente-appinputsearch --strict`.
- [x] 5.3 Run a TypeScript/build validation or document any unrelated pre-existing failures.
- [x] 5.4 Update this checklist with completed tasks and evidence before archive.

## Validation Evidence

- `npm.cmd test -- src/app/Components/UI/AppInputSearch/AppInputSearch.test.tsx src/app/Components/UI/AppTable/tests/AppTableQueryWrapper.test.tsx` passed outside the sandbox after sandbox `spawn EPERM`; Vitest reported 4 files and 19 tests passed, including worktree copies under `w51/` and `w52/`.
- `npx.cmd eslint src/app/Components/UI/AppInput/AppInput.tsx src/app/Components/UI/AppInput/index.ts src/app/Components/UI/AppInputSearch/AppInputSearch.tsx src/app/Components/UI/AppInputSearch/AppInputSearch.test.tsx src/app/Components/UI/AppTable/AppTableQueryWrapper.tsx src/app/Components/UI/AppTable/tests/AppTableQueryWrapper.test.tsx src/app/Components/UI/index.ts` passed.
- `openspec.cmd validate scrumcore-60-crea-componente-appinputsearch --strict` passed.
- `git diff --check` passed.
- `npx.cmd tsc -b` passed.
- `npm.cmd run build` passed outside the sandbox after sandbox `spawn EPERM`; Vite reported the existing large chunk warning.
