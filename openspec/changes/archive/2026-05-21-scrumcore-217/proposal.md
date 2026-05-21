## Why

`DocumentosWorkbench` necesita mostrar documentos de forma **backend-driven** para soportar variaciones por proceso/rol/flujo sin hardcodear lógica en frontend. El objetivo es consumir datos y metadata desde backend y renderizarlos con `AppTreeTable` (wrapper sobre `AppTable`) manteniendo compatibilidad y sin afectar otros componentes del sistema.

Compatibilidad obligatoria:
- `SCRUM-205` se mantiene como base funcional (rutas/envelope/resolve estables).
- `SCRUM-209` define el consumo recomendado de `ListaDocumentosRadicados` para frontend:
  - `flatDocuments` como vista simplificada (label principal + acciones)
  - label documental formalizado por backend con fallback oficial `DOC {ID}` (frontend no recalcula)
  - en `flatDocuments` la UI no depende de columnas legacy no garantizadas (`PAG`, `ESTADO_FIRMA_DIGITAL`, etc.)

## What Changes

- `DocumentosWorkbench` obtiene filas raíz y filas hijas por nodo desde backend (carga inicial + carga incremental).
- La metadata de columnas/acciones proviene del backend (`Config` / `Columns`) y se entrega a `AppTreeTable` para renderizar `Values` y acciones sin hardcode.
- Se definen DTOs y mapping (backend → modelos UI) dentro del módulo de dominio, manteniendo `DocumentosWorkbench` como orquestador (sin axios/DTO directos).
- Se integra acción primaria `ver_documento` para actualizar el visor PDF solo en éxito.
- Se agregan tests trazables al spec `[SPEC:APPTREETABLE-217]`.

## Capabilities

### New Capabilities
- `integracion-backend-driven-documentos-workbench-apptreetable-wrapper`: Integración backend-driven para `DocumentosWorkbench` usando `AppTreeTable` como wrapper (load root/children, columnas/acciones dinámicas, selección).
- Consumo compatible con `SCRUM-209` para `flatDocuments` (vista simplificada: label principal + acciones backend-driven; sin dependencia de columnas legacy).

### Modified Capabilities
- Ninguna (no se modifica el contrato público de `AppTable`; `AppTreeTable` solo expone eventos opcionales sin breaking changes).

## Constraints / Guardrails

- No reemplazar `AppTable` ni reimplementar tabla/AG Grid en `DocumentosWorkbench`.
- No introducir `any` ni relajar TypeScript estricto.
- Mantener el cambio confinado al módulo dueño de `DocumentosWorkbench` (p.ej. `src/modules/gestionCorrespondencia/**`).
- No modificar `vite.config.ts`, router, layout, ni estilos base globales.
- No asumir columnas legacy en `flatDocuments` (SCRUM-209).
- No recalcular fallback label `DOC {ID}` en frontend (SCRUM-209).

## Impact

- Código: nuevos/ajustes en services/hook/adapters del módulo de `DocumentosWorkbench` para habilitar backend-driven.
- UI: listado jerárquico renderizado por `AppTreeTable` con carga incremental; integración de visor.
- UI (SCRUM-209): en `flatDocuments`, render simplificado (label principal + acciones) sin dependencia de columnas legacy.
- Testing: pruebas con tag `[SPEC:APPTREETABLE-217]`.
- Documentación: evidencia de tests ejecutados y contratos en artefactos OpenSpec.
