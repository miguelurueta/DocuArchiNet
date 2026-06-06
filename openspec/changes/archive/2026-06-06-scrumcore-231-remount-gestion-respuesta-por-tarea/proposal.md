## Why

Después de consolidar el estado transversal y la integración de documentos/visor en SCRUMCORE-219/220/221/222, el cambio de ruta `/dashboard/gestion-correspondencia/respuesta/:id` entre tareas distintas puede reutilizar el mismo árbol de `GestionRespuesta`.

Esto permite que estados locales/contextuales persistentes (adjuntos, fila activa, visor/edición) contaminen una nueva tarea y provoquen respuestas “stale”, regresiones intermitentes y comportamiento no determinístico. Se requiere un remount completo por `:id` para restaurar aislamiento entre tareas sin cambiar lógica funcional.

## What Changes

- Aplicar identidad React basada en `parsedId` sobre el subárbol completo de detalle de `GestionRespuesta` para forzar desmontaje/montaje total cuando cambia la tarea.
- Asegurar que el subárbol remounteado incluya providers, tabs, `DocumentosWorkbench`, visor y estado asociado de adjuntos, editor y archivo activo.
- Implementar limpieza de lifecycle de requests y efectos para evitar que respuestas asíncronas de tareas previas muten estado del detalle recién montado.
- Mantener intacto el shell de navegación (master-detail), `AppTable`, `AppTreeTable`, contratos de ruta y flujo de bloqueo existente.
- No introducir cambios de UI visual ni alterar endpoints/contratos backend.
- Añadir y/o ajustar pruebas de navegación, integración y regresión para validar que el aislamiento funciona en transiciones rápidas entre tareas.

## Capabilities

### New Capabilities

- `remount-detalle-gestion-respuesta-por-tarea`: aislamiento por `:id` en detalle de respuesta con remount completo y anti-stale.

### Modified Capabilities

- `implmenta-carga-detalle-gestion-respuesta`: ajustar requerimientos de transición de carga/ready/blocked para incluir aislamiento de estado al cambiar `:id` y estabilidad de remount del detalle completo.
- `gestion-correspondencia`: asegurar que el ciclo de vida de navegación por detalle no impacte la integración de documentos/visor/adjuntos existente.

## Impact

- **Rutas**: `src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.tsx`
- **Página detalle**: `src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx`
- **Subárbol de detalle**: providers, tabs, `DocumentosWorkbench`, `AppVisorEmbedPdf`, componentes de adjuntos y estado local de detalle.
- **Hooks/Provider existentes** ya utilizados por el flujo (sin cambios de contrato público).
- **No hay cambios** en backend, endpoints, servicios externos, AppTable/AppTreeTable, ni contratos de contratos/datos.
- **Riesgo operativo controlado**: evitar stale updates, focus instability y memory leaks con cleanup explícito en effects al desmontar por cambio de `parsedId`.
