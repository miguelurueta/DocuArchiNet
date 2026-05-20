## Why

En `SCRUMCORE-214` se introdujo `AppTreeTable` y se integró en `DocumentosWorkbench` (rail “Listado”) como base UI sin afectar `AppVisorEmbedPdf`. Este ticket (`SCRUMCORE-215`) evoluciona ese componente para soportar renderización e integración de datos “backend-driven” (query/action) y acciones por fila, manteniendo compatibilidad hacia atrás y sin interferir con otros módulos/plugins.

El contrato funcional objetivo para la integración backend-driven es un flujo tipo **SCRUM-205 ListaDocumentosRadicados**, con endpoints:
- `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/query`
- `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/action`
- `POST /api/gestor-documental/documentos/visualizacion/resolve` (invocado por frontend, sin API->API)

## What Changes

- Se formaliza el alcance y reglas para consumo backend-driven en `AppTreeTable` (carga inicial, jerarquía lazy-load, y acciones).
- Se define separación de responsabilidades: `AppTreeTable` (UI genérica) vs. `services/hooks` del módulo consumidor (contrato de dominio).
- Se agregan specs y tasks para guiar implementación, tests y documentación enterprise antes de publish.

## Capabilities

### New Capabilities
- `renderizacion-integracion-backend-apptreetable`: Renderización backend-driven en `AppTreeTable` (query/action), con soporte jerárquico y acciones por fila.

### Modified Capabilities
- `apptreetable`: Extensión compatible hacia atrás para soportar lazy-load de hijos y handlers de acción sin acoplar el componente a dominios/endpoints.
- `gestion-correspondencia-documentos-workbench`: Consumidor (Listado) habilitado para cargar datos reales desde backend vía hooks/servicios del módulo.

## Impact

- OpenSpec completo en `openspec/changes/scrumcore-215-renderizacion-integracion-backend-apptreetable/` para guiar el desarrollo.
- Cambios esperados de código (a implementar según tasks):
  - Extensiones no-breaking en `src/app/Components/UI/AppTreeTable/*`.
  - Servicios/hooks en `src/modules/gestionCorrespondencia/**` para consumir `query/action/resolve`.
  - Integración en `DocumentosWorkbench` “Listado” sin afectar el visor.
