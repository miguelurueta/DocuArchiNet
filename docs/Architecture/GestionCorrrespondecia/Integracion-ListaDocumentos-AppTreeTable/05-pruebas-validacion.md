# SCRUMCORE-295 - Pruebas Y Validacion

## Pruebas Ejecutadas

```powershell
cmd /c npm test -- --run src/app/Components/UI/AppTable/tests/AppTableQueryWrapper.test.tsx src/modules/gestionCorrespondencia/adapters/gestionRespuestaDocumentosRequestMapper.test.ts src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.test.ts src/modules/gestionCorrespondencia/tests/useGestionRespuestaDocumentosTable.test.tsx src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx
```

Resultado observado:

```text
55 passed, 1 skipped
```

## Lint Ejecutado

```powershell
cmd /c npx eslint src/app/Components/UI/AppTable/AppTableQueryWrapper.tsx src/app/Components/UI/AppTable/tests/AppTableQueryWrapper.test.tsx src/modules/gestionCorrespondencia/adapters/gestionRespuestaDocumentosRequestMapper.ts src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.ts src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentosTable.ts src/modules/gestionCorrespondencia/adapters/gestionRespuestaDocumentosRequestMapper.test.ts src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.test.ts src/modules/gestionCorrespondencia/tests/useGestionRespuestaDocumentosTable.test.tsx src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx
```

Resultado observado: OK.

## Diff Check

```powershell
git diff --check
```

Resultado observado: OK, con advertencias normales LF/CRLF en Windows.

## Build

```powershell
cmd /c npm run build
```

Resultado observado: falla por deuda preexistente fuera de `SCRUMCORE-295`.

```text
src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaUploadDocumental.tsx(8,3):
error TS2724: "../../../almacenamientoDocumental/components/AppUploadDocumental" has no exported member named "UploadDocumentalStoredContext". Did you mean "UploadDocumentalContext"?
```

## Cobertura Funcional Validada

| Caso | Evidencia |
|---|---|
| Wrapper mantiene paginacion por default | `AppTableQueryWrapper.test.tsx`. |
| Wrapper oculta paginacion con `showPagination=false` | `AppTableQueryWrapper.test.tsx`. |
| Workbench no muestra pagina anterior/siguiente | `DocumentosWorkbench.test.tsx`. |
| Workbench delega busqueda al hook | `DocumentosWorkbench.test.tsx`. |
| Hook envia `EnablePagination=false` | `useGestionRespuestaDocumentosTable.test.tsx`. |
| Hook mantiene `Page=1` sin paginacion | `useGestionRespuestaDocumentosTable.test.tsx`. |
| Hook filtra localmente | `useGestionRespuestaDocumentosTable.test.tsx`. |
| Total filtrado en busqueda | `useGestionRespuestaDocumentosTable.test.tsx`. |
| Adapter soporta `Pagination.Total` | `documentosWorkbenchResponseAdapter.test.ts`. |
| Mapper conserva contrato DTO | `gestionRespuestaDocumentosRequestMapper.test.ts`. |

## Criterios De Aceptacion

- [x] El listado principal envia `DocumentRelationScope=documentsOnly`.
- [x] El listado principal envia `EnablePagination=false`.
- [x] La UI no muestra controles de paginacion en el listado documental.
- [x] La busqueda opera sobre todas las filas recibidas.
- [x] Backend `Search` no recorta filas en el modo full-list.
- [x] El contador muestra total filtrado cuando hay busqueda.
- [x] Los totales backend se preservan cuando no hay busqueda.
- [x] `AppTableQueryWrapper` conserva compatibilidad por default.
- [x] `AppTreeTable` no recibe reglas documentales.
- [x] Errores de validacion no disparan fallback silencioso.

## OpenSpec

Estado observado:

```text
37/37 tasks complete
```

Artefactos sincronizados:

- `proposal.md`
- `design.md`
- `specs/lista-documentos-apptretable/spec.md`
- `specs/lista-documentos-apptretable/jira-context.md`
- `tasks.md`
