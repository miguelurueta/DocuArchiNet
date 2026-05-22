## Context

El modulo de gestion de correspondencia consume dos endpoints del backend para listar y ejecutar acciones sobre documentos radicados:
- `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/query`
- `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/action`

Hoy existen dos shapes validos para la respuesta de `query`:
- Legacy: configuracion de tabla en `data.Config`
- Actual: `DynamicUiTableDto` directo en `data`

El render de acciones en el front depende de que el adapter y el hook normalicen correctamente columnas, filas y metadatos de tabla. En el shape actual, acciones pueden venir en `CellActions` y `MenuActions` aun cuando `RowActions` este vacio. Si no se preservan esas acciones en `flatDocuments`, `AppTreeTable` no habilita menu ni dispara `onActionTriggered`.

Restricciones relevantes:
- No cambiar backend ni rutas.
- Mantener compatibilidad retroactiva con `data.Config`.
- No romper contratos TypeScript publicos salvo ampliacion compatible.
- Conservar `IncludeConfig: true` en `query`.

## Goals / Non-Goals

**Goals:**
- Normalizar la lectura de `DynamicUiTableDto` para soportar `data.Config` y `data` directo.
- Asegurar que las acciones (`CellActions` y `MenuActions`, con fallback legacy) lleguen al modelo consumido por `AppTreeTable`.
- Mantener `tableIdRef` sincronizado con `model.tableId` de respuesta y usar fallback estable (`InboxListaDocumentosRadicado`).
- Garantizar que `action` use `TableId` efectivo, `RowId` y payload esperado (`IdDocumento`/`DocumentId`, `NombreGabinete`).

**Non-Goals:**
- Redisenar `AppTreeTable` o contratos backend.
- Cambiar rutas o nombre de endpoints.
- Introducir nuevos patrones de estado global o refactor transversal del modulo.

## Decisions

1. Deteccion dual en adapter para origen de tabla.
Se implementa deteccion explicita en `pickDynamicUiTable`:
- Si `data.Config` existe y es objeto, se usa como fuente primaria (legacy).
- Si `data` ya contiene estructura de tabla (`Columns`, `Rows`, `CellActions`, `MenuActions`), se usa `data` directo.
Razon: garantiza compatibilidad sin bifurcar el pipeline de mapeo.
Alternativas consideradas:
- Mantener solo `data.Config`: rompe shape actual.
- Duplicar adapters por version: aumenta complejidad y riesgo de divergencia.

2. Conservacion de acciones en flattening.
`flatDocuments` debe preservar la columna principal y consolidar acciones disponibles priorizando `CellActions` + `MenuActions`, con fallback a `RowActions`.
Razon: el backend actual puede dejar `RowActions` vacio y colocar acciones solo a nivel celda/menu.
Alternativa descartada:
- Requerir `RowActions` no vacio para render de menu: rompe casos reales del endpoint.

3. Source of truth de TableId en hook.
`useGestionRespuestaDocumentosTable` actualiza `tableIdRef` cada vez que llega `model.tableId`; si no viene, mantiene fallback `InboxListaDocumentosRadicado`.
Razon: evita mismatch entre tabla renderizada y tabla usada para `action`.
Alternativa descartada:
- TableId fijo hardcodeado: no soporta respuesta dinamica del backend.

4. Contrato de action estable y compatible.
La construccion de request `action` usa siempre:
- `TableId`: desde `tableIdRef` efectivo
- `RowId`
- Payload de documento:
  - prioriza `IdDocumento` cuando coexiste con `DocumentId`
  - usa el disponible cuando solo viene uno
  - incluye `NombreGabinete` cuando exista
Razon: elimina ambiguedad del contrato y conserva compatibilidad entre variantes de datos.

5. Cobertura de pruebas focalizada en adapter.
Se agregan/ajustan tests del adapter para validar ambos shapes y preservacion de acciones en `flatDocuments`.
Razon: es el punto de normalizacion de mayor riesgo de regresion.

## Risks / Trade-offs

- [Riesgo] Deteccion de shape demasiado permisiva puede aceptar objetos parciales.
Mitigacion: validar senales minimas de tabla (`Columns`/`Rows`) y mantener fallback seguro.

- [Riesgo] Diferencias de naming entre `IdDocumento` y `DocumentId` en distintas filas.
Mitigacion: regla deterministica de prioridad (`IdDocumento` primero) y test de regresion.

- [Riesgo] `tableId` ausente en algunas respuestas puede causar acciones en tabla equivocada.
Mitigacion: fallback constante `InboxListaDocumentosRadicado` y actualizacion inmediata cuando llegue `model.tableId`.

- [Trade-off] Se mantiene logica de compatibilidad legacy en lugar de simplificar al shape nuevo.
Mitigacion: encapsular compatibilidad en adapter para aislar complejidad.

## Migration Plan

1. Implementar ajustes en adapter y hook sin cambiar contratos publicos.
2. Ejecutar tests del modulo, en especial `documentosWorkbenchResponseAdapter.test.ts`.
3. Validar manualmente que menu de acciones renderiza y dispara `onActionTriggered`.
4. Desplegar sin cambios de backend; rollback simple revertiendo commit del frontend.

## Open Questions

- El componente `AppTreeTable` requiere algun campo adicional para priorizar acciones de menu sobre acciones de fila en todos los casos?
- Existen consumers externos del modelo adaptado que dependan de un orden especifico de acciones?
- Debemos extender tests hacia el hook para validar sincronizacion de `tableIdRef` en respuestas mixtas?
