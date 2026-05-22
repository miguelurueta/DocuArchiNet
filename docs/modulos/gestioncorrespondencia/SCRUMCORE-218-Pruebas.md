# SCRUMCORE-218 - Pruebas

## Unitarias (obligatorias)

- `documentosWorkbenchResponseAdapter`
  - Archivo: `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.test.ts`
  - Cobertura agregada:
    - shape legacy (`data.Config`)
    - shape actual (`data` directo)
    - preservación de columna principal + acciones (`CellActions`/`MenuActions`)

- `documentosWorkbenchActionMapper`
  - Archivo: `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchActionMapper.test.ts`
  - Cobertura agregada:
    - prioridad `IdDocumento` cuando coexiste con `DocumentId`
    - fallback a `DocumentId` cuando `IdDocumento` no existe

## Integración funcional esperada

Checklist manual sugerido:
- En `flatDocuments`, el menú de acciones se renderiza aunque `RowActions` venga vacío.
- `onActionTriggered` ejecuta action con `TableId` efectivo.
- `Payload` incluye identificador esperado y `NombreGabinete`.
- Query root mantiene `IncludeConfig: true`.

## Evidencia de ejecución

Comandos ejecutados (2026-05-21):
- `npm.cmd test -- src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.test.ts`
  - Resultado: PASS (`5 tests passed`)
- `npm.cmd test -- src/modules/gestionCorrespondencia/adapters/documentosWorkbenchActionMapper.test.ts`
  - Resultado: PASS (`2 tests passed`)

## Regresión

- No se modificaron backend, rutas ni contratos públicos incompatibles.
- El cambio es acotado a adapter/hook/tests del módulo de `gestionCorrespondencia`.
