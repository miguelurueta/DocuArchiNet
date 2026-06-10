## Context

El Tab **Documentos** (módulo `gestionCorrespondencia`) actualmente conserva un layout base en `DocumentosWorkbench` y un sidebar plegable con `AppCollapseRail` (overlay en mobile/tablet). El componente `AppVisorPdf` ya existe en `src/app/Components/UI/AppVisorPdf/`, pero todavía no está integrado al flujo real del Tab Documentos.

Se requiere una integración **funcional** (negocio) donde el tab:
- liste documentos reales asociados a la respuesta (`idTareaWf`)
- permita seleccionar uno
- cargue y renderice el PDF seleccionado en `AppVisorPdf`
- preserve el responsive (incl. iPad Pro 1024×1366 y rangos tablet ~901–1122) evitando regresiones como pantallas en blanco al abrir/cerrar el overlay.

## Goals / Non-Goals

**Goals:**
- Integrar `AppVisorPdf` en el panel principal de `DocumentosWorkbench`.
- Mostrar un listado real de documentos/anexos asociados a `idTareaWf` en el rail (`AppCollapseRail`).
- Cargar el PDF seleccionado usando el flujo real de obtención (URL segura/bytes/base64) según patrones existentes del módulo.
- Manejar estados `loading/empty/error/unauthorized` de forma clara y consistente.
- Mantener UX del overlay: backdrop, bloqueo de interacción con `inert` (sin `aria-hidden` sobre elementos enfocados), y autocierre al cambiar de tab.

**Non-Goals:**
- Implementar almacenamiento/persistencia nueva de documentos.
- Definir o cambiar contratos backend si el sistema ya cuenta con endpoints (solo consumirlos).
- Implementar un editor de PDF; el alcance es visualización y herramientas soportadas por `AppVisorPdf`.

## Decisions

1) **`DocumentosWorkbench` recibe `idTareaWf` como prop**
- **Decisión:** pasar `idTareaWf` desde `GestionRespuesta` hacia `DocumentosWorkbench`.
- **Racional:** el tab Documentos necesita un identificador estable del contexto para listar/cargar anexos.
- **Alternativas:** leer directamente `useParams()` dentro del workbench. Se descarta para mantener el componente más testeable y alineado con `GestionRespuestaMainTabContent` que ya recibe `idTareaWf`.

2) **Capa de datos en `services`/`hooks` del módulo**
- **Decisión:** encapsular listado/descarga en un servicio/hook del módulo (`src/modules/gestionCorrespondencia/services|hooks`).
- **Racional:** evita acoplar `DocumentosWorkbench` a detalles HTTP y facilita test/mocks.
- **Alternativas:** fetch dentro del componente; se descarta por acoplamiento y dificultad de pruebas.

3) **Integración del visor con un `input` estable y cache por documento**
- **Decisión:** mantener un estado de “documento seleccionado” con cache del payload (URL/bytes) para evitar descargas repetidas.
- **Racional:** performance y UX fluida al alternar sidebar/viewport.

4) **Overlay y accesibilidad**
- **Decisión:** mantener el patrón de overlay con backdrop y bloqueo usando `inert` (ya implementado en `AppCollapseRail`) y evitar `aria-hidden` cuando haya foco dentro.
- **Racional:** previene warnings/bloqueos del navegador y “pantalla en blanco” en tablet.

## Risks / Trade-offs

- [API de documentos desconocida/variable] → Definir en spec los contratos mínimos esperados y dejar “Open Questions” si no existe un endpoint actual.
- [Performance al renderizar PDFs grandes] → Lazy-load del visor si existe patrón en el repo; cache por documento; placeholders durante carga.
- [Responsive/overlay en iPad Pro] → Evitar modificar `AppCollapseRail` sin necesidad; validar por breakpoints críticos y mantener z-index/backdrop estables.

## Migration Plan

- Cambios son UI-only del módulo: desplegar junto a la SPA.
- Rollback: revert commit/PR sin migraciones de datos.

## Open Questions

- ¿Cuál es el contrato backend para listar documentos por `idTareaWf`? (ruta, shape, paginación).
- ¿Cómo se obtiene el contenido PDF? (URL firmada, streaming, base64, bytes).
- ¿Se soportan documentos no-PDF? En ese caso: ¿mostrar placeholder o descargar/abrir externo?

