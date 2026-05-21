## Why

`DocumentosWorkbench` necesita mostrar un listado jerárquico de documentos (tree) de forma **backend-driven** para soportar variaciones por proceso/rol/flujo sin hardcodear lógica en frontend. El objetivo es consumir datos y metadata desde backend y renderizarlos con `AppTreeTable` (wrapper sobre `AppTable`) manteniendo compatibilidad y sin afectar otros componentes del sistema.

## What Changes

- `DocumentosWorkbench` obtiene filas raíz y filas hijas por nodo desde backend (carga inicial + carga incremental).
- La metadata de columnas proviene del backend y se entrega a `AppTreeTable` para renderizar `values` en orden.
- Se definen DTOs y mapping (backend → `AppTreeTableRow`) dentro del módulo de dominio, manteniendo `AppTreeTable` sin cambios de contrato.
- Se agregan tests (unitarios del mapper e integración del workbench) trazables al spec `[SPEC:APPTREETABLE-217]`.

## Capabilities

### New Capabilities
- `integracion-backend-driven-documentos-workbench-apptreetable-wrapper`: Integración backend-driven para `DocumentosWorkbench` usando `AppTreeTable` como wrapper (carga root/children, columnas dinámicas, selección).

### Modified Capabilities
- Ninguna (no se modifica el contrato público de `AppTreeTable` ni de `AppTable`).

## Constraints / Guardrails

- No reemplazar `AppTable` ni reimplementar tabla/AG Grid en `DocumentosWorkbench`.
- No introducir `any` ni relajar TypeScript estricto.
- Mantener el cambio confinado al módulo dueño de `DocumentosWorkbench` (p.ej. `src/modules/gestionCorrespondencia/**`).
- No modificar `vite.config.ts`, router, layout, ni estilos base globales.

## Impact

- Código: nuevos/ajustes en services/hook/mapping del módulo de `DocumentosWorkbench` para habilitar backend-driven.
- UI: listado jerárquico renderizado por `AppTreeTable` con columnas provenientes de backend; soporte de expand/collapse con carga incremental.
- Testing: se agregan pruebas con tag `[SPEC:APPTREETABLE-217]`.
- Documentación: el cambio se documenta y deja evidencia de tests ejecutados en artefactos OpenSpec.
