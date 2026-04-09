## 1. Mapper Verification

- [x] 1.1 Verify `mapGestionCorrespondenciaTableRequest` wraps `mapDynamicUiServerTableRequest`.
- [x] 1.2 Verify effective simple search maps trimmed `Search` and `SearchType = 2`.
- [x] 1.3 Verify empty or whitespace-only search does not force `SearchType = 2`.
- [x] 1.4 Verify `searchType = 3` is preserved for advanced search.
- [x] 1.5 Verify explicit non-advanced `searchType` without effective search text is preserved or documented.

## 2. Integration Verification

- [x] 2.1 Verify `useGestionCorrespondenciaTable` uses `mapGestionCorrespondenciaTableRequest` as the table request mapper.
- [x] 2.2 Verify `getAllMatchingRows` builds requests through `mapGestionCorrespondenciaTableRequest`.
- [x] 2.3 Verify `getBackendExportFile` builds export requests through `mapGestionCorrespondenciaTableRequest`.
- [x] 2.4 Verify `AppInputSearch` and `GestionCorrespondencia.tsx` remain free of `SearchType`, endpoint, SQL, and backend-specific mapping logic.
- [x] 2.5 Verify `mapDynamicUiServerTableRequest` remains generic and does not force `SearchType = 2`.

## 3. Tests

- [x] 3.1 Ensure mapper tests cover simple effective search with `SearchType = 2`.
- [x] 3.2 Ensure mapper tests cover empty search without forced `SearchType = 2`.
- [x] 3.3 Ensure mapper tests cover advanced `SearchType = 3`.
- [x] 3.4 Ensure mapper tests cover explicit non-advanced `SearchType` without effective search text.
- [x] 3.5 Ensure tests cover preservation of pagination, sort, `IncludeConfig`, and structured filters.
- [x] 3.6 Ensure hook or integration tests cover all-matching rows and backend export preserving active search mapping.
- [x] 3.7 Run focused Gestion Correspondencia mapper and hook tests.

## 4. Validation

- [x] 4.1 Run focused lint or TypeScript checks for touched files if code changes are needed.
- [x] 4.2 Run `openspec validate scrumcore-68-conectar-busqueda-simple-gestioncorrespo --strict`.
- [x] 4.3 Confirm no unrelated frontend or backend changes are included in the final diff.
