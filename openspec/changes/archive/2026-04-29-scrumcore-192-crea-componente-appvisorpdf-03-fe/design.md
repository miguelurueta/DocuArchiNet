## Context

El ticket `SCRUMCORE-192` (03-FE) agrega un **Annotations Engine** basado en Fabric sobre
`AppVisorPdf`, complementando:

- 01-FE: shell/toolbar/contratos de `AppVisorPdf`
- 02-FE: PDF engine con `pdfjs-dist`, virtualizaci\u00f3n y cache

Esta fase incorpora herramientas de anotaci\u00f3n (freehand/text/rect/arrow/select),
undo/redo y serializaci\u00f3n/restauraci\u00f3n determinista a un payload JSON versionado.

## Goals / Non-Goals

**Goals:**
- Implementar `AnnotateEngine` desacoplado de UI (sin l\u00f3gica de Fabric en componentes de presentaci\u00f3n).
- Estrategia por p\u00e1gina: overlay de Fabric solo en p\u00e1ginas visibles (compatibles con virtualizaci\u00f3n).
- Tools operativas: `select`, `freehand`, `text`, `rect`, `arrow` (con `AppVisorPdfTool`).
- Undo/redo (por p\u00e1gina o global) con comportamiento consistente.
- `serialize()`/`restore()` deterministas y versionados (`version: 1`) con forward compatibility (ignorar objetos desconocidos).
- Mantener TypeScript estricto (sin `any`).

**Non-Goals:**
- Implementar persistencia backend; solo contrato JSON estable.
- Implementar todas las capacidades avanzadas de Fabric (grouping complejo, text editing avanzado) si no es requerido.
- Renderizar overlays para todas las p\u00e1ginas simult\u00e1neamente (debe respetar rendimiento en PDFs grandes).

## Decisions

1) **Engine separado de UI**
- **Decision:** crear `src/app/Components/UI/AppVisorPdf/engine/fabricEngine.ts` que implementa `AnnotateEngine`.
- **Why:** UI mantiene simplicidad; engine es testeable y aislable.
- **Alternatives:** usar Fabric directamente en `VisorPdfViewport` (acopla y complica testing).

2) **Overlay por p\u00e1gina (canvas separado)**
- **Decision:** cada p\u00e1gina visible tendr\u00e1 un overlay canvas independiente para Fabric, acoplado al lifecycle de virtualizaci\u00f3n (attach/detach).
- **Why:** escala mejor con PDFs grandes; evita mantener un \u00fanico canvas gigante.
- **Alternatives:** overlay global por viewport (m\u00e1s complejo para mapping por p\u00e1gina y scroll).

3) **Undo/redo**
- **Decision:** stacks de undo/redo por p\u00e1gina (default) y API global `undo()/redo()` que opera sobre la p\u00e1gina activa.
- **Why:** mapea mejor al contrato por p\u00e1gina y reduce complejidad.
- **Alternatives:** undo/redo global con merges entre p\u00e1ginas.

4) **Payload versionado**
- **Decision:** `VisorPdfAnnotationsPayloadV1` con `version: 1`, `fingerprint?` y `pages[]` con `pageNumber` y `objects` (JSON Fabric).
- **Why:** permite compatibilidad futura, migraciones y validaci\u00f3n backend.
- **Alternatives:** payload sin versi\u00f3n (dif\u00edcil evolucionar).

5) **Forward compatibility**
- **Decision:** `restore()` ignora objetos no reconocidos de forma segura y conserva lo posible.
- **Why:** evita romper usuarios al introducir nuevos objetos en el futuro.

## Risks / Trade-offs

- **[Rendimiento]** \u2192 overlays solo para p\u00e1ginas visibles + detach/destroy agresivo.
- **[Consistencia tool/estado]** \u2192 centralizar tool state en `AnnotateEngine.setTool()` y reflejarlo desde UI.
- **[Serializaci\u00f3n no determinista]** \u2192 normalizar orden de objetos/props al serializar cuando sea posible.

## Migration Plan

1) Definir tipos `VisorPdfAnnotationsPayloadV1` y `AnnotateEngine` en `domain/` o `engine/` seg\u00fan convenci\u00f3n.
2) Implementar `engine/fabricEngine.ts` + `engine/tools/*` para tools requeridas.
3) Integrar overlay canvas por p\u00e1gina en el viewport (solo visibles) y conectar attach/detach.
4) Implementar undo/redo y comandos en toolbar/UI (sin acoplar UI a Fabric).
5) Implementar serialize/restore determinista y versionado.
6) Tests obligatorios (create->serialize, restore, undo/redo).
7) Actualizar README con tools + payload + compatibilidad.

## Open Questions

- \u00bfUndo/redo por p\u00e1gina o global es suficiente para UX final?
- \u00bfSe requiere soporte de `stamp_grafo` en esta fase o queda para ticket posterior?
- \u00bfQu\u00e9 subset exacto del JSON de Fabric debe aceptarse/ignorarse en restore (hardening)?

