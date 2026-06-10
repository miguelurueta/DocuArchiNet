# Design — SCRUMCORE-213 (Actualización visual “Firma personal” en AppVisorEmbedPdf)

## Context

El componente `AppVisorEmbedPdf` incluye un modal de firmas (`AppPdfSignatureModal`) con un tab “Firma personal” que consume la firma temporal (SCRUM-201) y permite iniciar placement oficial de EmbedPDF.

Problemas UX actuales:

- La UI puede terminar mostrando información no deseada (URLs/strings `blob:`) en lugar de un preview claro.
- Existen dos acciones redundantes (“Usar firma personal” y “Usar firma”) cuando el objetivo final es el mismo: iniciar placement con una firma ya seleccionada.

Restricciones:

- No mover lógica al `DocumentosWorkbench`.
- No afectar zoom/rotate/scroll/render/virtualización.
- Mantener contrato SCRUM-201 y lifecycle de `ObjectURL`.

## Goals / Non-Goals

### Goals
- Renderizar preview enterprise del PNG descargado en el tab “Firma personal” mediante `<img src={ObjectURL}>`.
- No renderizar en UI la URL (ni `blob:` ni `UrlTemporal`).
- Dejar **un único botón** “Usar firma” en el tab “Firma personal”.
- Mantener accesibilidad (alt/aria-label/tooltips).
- Actualizar tests y documentación enterprise.

### Non-Goals
- Cambios en el contrato SCRUM-201.
- Cambios de plugins EmbedPDF o lógica custom de anotaciones.
- Rediseñar el modal completo o introducir un “manager” de firmas.

## Decisions

### D1 — Preview con `<img>` y CSS Modules
**Decisión:** el preview se renderiza con un `<img>` usando `personal.blobUrl` como `src` y estilos en `AppPdfSignatureModal.module.css` (`object-fit: contain`, altura controlada).

**Rationale:** UI simple, compatible y sin re-implementar render de firma.

### D2 — Un solo botón “Usar firma”
**Decisión:** en el tab “Firma personal”, el único CTA será “Usar firma”, que dispara directamente `onStartPlacement(stamp)` y luego resetea el estado del modal.

**Rationale:** evita carreras de estado (setState async), reduce pasos y hace el flujo consistente con el objetivo final.

### D3 — No mostrar URL en UI
**Decisión:** se elimina cualquier render de `blobUrl`/`UrlTemporal`; solo se mostrará metadata de negocio (`FileName`, opcional `ExpiresAt`).

**Rationale:** UX enterprise y evita exponer detalles técnicos.

## Risks / Trade-offs

- [Imagen no carga / blob inválido] → fallback visual + texto “No fue posible previsualizar la firma”.
- [Memory leaks de ObjectURL] → mantener `revokeObjectURL` en cleanup del hook y en reset de modal.
- [Regresión en placement] → pruebas unit/integration validan que se llama `onStartPlacement` con definición stamp.

## Migration Plan

1) Ajustar UI del tab “Firma personal” (preview + botón único).
2) Actualizar estilos CSS Modules del modal.
3) Actualizar tests del visor.
4) Actualizar documentación enterprise SCRUMCORE-213.

Rollback: revertir cambios en modal CSS/UI; no afecta otras capas.

## Open Questions

- Ninguna (cambio UI acotado).

