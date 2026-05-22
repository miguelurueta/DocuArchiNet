# SCRUMCORE-225 - Implementación Detallada

## Alcance
Actualización visual enterprise para el Workbench: limitar el listado a **2 columnas funcionales** y aplicar un preset de sizing para mantener visibilidad, sin cambiar comportamiento funcional ni contratos backend.

## Archivos modificados / creados
- `src/app/Components/UI/AppTable/adapters/appGridToAppTableColumns.ts`
  - Scoping por `tableId="InboxListaDocumentosRadicado"`
  - Selector 2 columnas (primaria `TIPODOCUMENTO`, secundaria preferir action column)
  - Anti-legacy para `flatDocuments` (`PAG`, `ESTADO_FIRMA_DIGITAL`, `DBT`)
  - Preset sizing (`flex` + `minWidth`) aplicado a columnas seleccionadas
- `src/app/Components/UI/AppTable/tests/appGridToAppTableColumns.test.ts`
  - Unit tests `[SPEC:APPTREETABLE-225-001]` (selector + scoping + sizing)
- `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.test.ts`
  - Unit tests `[SPEC:APPTREETABLE-225-001]` (Workbench model aplica 2 columnas + sizing)
- `src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx`
  - Test de wiring: `tableColumns` de 2 columnas se propaga a `AppTreeTable` (mock)
- `playwright/gestionCorrespondencia/documentosWorkbench.columnas225.spec.ts`
  - Playwright E2E: valida 2 headers en Workbench (y placeholders `skip` para escenarios dependientes de entorno real)

Documentación (este directorio):
- `docs/modulos/gestioncorrespondencia/AppTreeTable/AjusteVisualColumnas/SCRUMCORE-225-*.md`

## Qué se actualizó (paso a paso)
1. **Scoping por `tableId`**
   - Regla: aplicar el preset solo cuando `options.tableId === "InboxListaDocumentosRadicado"`.
   - Objetivo: no afectar otros listados/tabla en el sistema.

2. **Selección determinística de 2 columnas**
   - Primaria:
     - Preferir `TIPODOCUMENTO` (columna documental en `flatDocuments`).
     - Fallback: primera columna visible no-acción.
   - Secundaria:
     - Preferir action column (`isActionColumn`) cuando exista.
     - Fallback: primera columna visible distinta a la primaria y no-legacy (cuando aplique).

3. **Anti-legacy (SCRUM-209, `flatDocuments`)**
   - Objetivo: no depender de columnas que backend ya no garantiza en esa vista.
   - Regla: excluir como candidatas secundarias `PAG`, `ESTADO_FIRMA_DIGITAL`, `DBT`.

4. **Sizing preset (Workbench)**
   - Solo para las columnas seleccionadas (2 columnas).
   - Config final:
     - Col 1 (Documento): `flex: 2`, `minWidth: 60`, `width: undefined`
     - Col 2 (Acciones): `flex: 1`, `minWidth: 80`, `width: undefined`

> Nota: el ancho final puede estar dominado por el contenido (texto/padding/indent) y por el layout del contenedor. `minWidth` solo define el piso, no el ancho definitivo.

## Por qué esta implementación (racional)
- Evita acoplar `DocumentosWorkbench` a columnas hardcodeadas y respeta el enfoque backend-driven.
- Mantiene consistencia visual enterprise para el caso Workbench (2 columnas previsibles).
- Reduce riesgo de regresión al limitar el cambio por `tableId`.

## Compatibilidad / Impacto
- No cambia `AppTreeTable` API pública.
- No cambia `AppTable` contrato externo; el ajuste es un preset condicionado por `tableId`.
- No cambia sorting/filtering/actions definidos por Dynamic UI (solo presentation/sizing).

## Validación rápida (dev)
- Unit tests: `npm.cmd test -- --run src/app/Components/UI/AppTable/tests/appGridToAppTableColumns.test.ts src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.test.ts`
- UI manual: verificar que en Workbench se visualicen `TIPODOCUMENTO` + `acciones` (sin depender de columnas legacy).
