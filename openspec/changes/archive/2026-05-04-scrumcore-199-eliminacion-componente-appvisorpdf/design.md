# Design — SCRUMCORE-199 Eliminación controlada de `AppVisorPdf`

## Contexto
El repo contiene un visor PDF (`src/app/Components/UI/AppVisorPdf/`) usado por módulos (por ejemplo Workbench de Documentos). Se requiere una eliminación **controlada** del componente legacy `AppVisorPdf`, evitando romper módulos y preservando la experiencia del usuario con un reemplazo o fallback estable.

## Objetivo
- Retirar `AppVisorPdf` legacy del producto sin degradar flujos críticos.
- Mantener un visor estable temporal (por ejemplo `AppVisorPdfCore`/`AppVisorPdfSimple`) o preparar la transición a un nuevo visor (por ejemplo `AppVisorEmbedPdf`).
- Eliminar imports/exports legacy, tests y documentación asociada, con limpieza incremental.

## Estrategia
1) **Inventario de uso**: identificar todos los imports de `AppVisorPdf` y variantes.
2) **Migración por consumidores**: reemplazar usos en módulos por el visor temporal aprobado (o feature flag).
3) **Deprecación**:
   - Marcar `AppVisorPdf` como deprecated (si se requiere ventana de transición).
   - O eliminar directamente si ya no hay consumidores.
4) **Remoción física**:
   - Eliminar archivos legacy, exports, tests, docs y referencias.
5) **Validación**:
   - `tsc --noEmit`
   - tests unitarios/e2e relevantes.

## Decisiones
- No eliminar “visor PDF” como capacidad del producto: solo eliminar el componente legacy.
- Evitar cambios masivos en la UI en este ticket; priorizar limpieza y no-regresión.

## Riesgos y mitigación
- **Riesgo**: módulos aún dependen de `AppVisorPdf`.
  - Mitigación: reemplazo por `AppVisorPdfCore`/`Simple` o feature flag.
- **Riesgo**: borrado de exports rompe imports.
  - Mitigación: migración completa antes de borrar exports.

