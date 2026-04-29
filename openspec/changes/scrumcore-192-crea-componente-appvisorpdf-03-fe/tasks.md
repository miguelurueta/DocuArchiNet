## 1. Dependencies & contracts

- [x] 1.1 Agregar dependencia `fabric` y verificar tipado estricto (sin `any`)
- [x] 1.2 Definir `VisorPdfAnnotationsPayloadV1` y `AnnotateEngine` en `src/app/Components/UI/AppVisorPdf/domain/` o `engine/` seg\u00fan convenci\u00f3n
- [x] 1.3 Asegurar separaci\u00f3n Engine/UI: el engine no importa React ni conoce la virtualizaci\u00f3n del viewport (solo expone API)

## 2. Fabric engine core

- [x] 2.1 Implementar `src/app/Components/UI/AppVisorPdf/engine/fabricEngine.ts` (attach/detach/setTool/undo/redo/serialize/restore/destroy)
- [x] 2.2 Definir estrategia por p\u00e1gina: overlay canvas por p\u00e1gina visible (no global) y lifecycle acorde a virtualizaci\u00f3n
- [x] 2.3 Implementar tool routing (`setTool`) con comportamiento determinista
- [x] 2.4 Implementar lifecycle robusto: `destroy()` idempotente; `detach()` limpia listeners/refs; sin memory leaks al reciclar p\u00e1ginas

## 3. Tools

- [x] 3.1 Implementar tools en `src/app/Components/UI/AppVisorPdf/engine/tools/*` (select/freehand/text/rect/arrow)
- [x] 3.2 Asegurar que tools operan solo sobre la p\u00e1gina activa/overlay correcto
- [x] 3.3 Aclarar alcance de herramientas: `stamp_grafo` queda fuera de este ticket salvo que Jira lo exija expl\u00edcitamente (documentar decisi\u00f3n en README)

## 4. Undo/redo

- [x] 4.1 Implementar undo/redo (por p\u00e1gina o global con stacks) y definir qu\u00e9 significa para UX
- [x] 4.2 Exponer `undo()`/`redo()` desde `AnnotateEngine` y conectarlo dentro de `AppVisorPdf` (sin integrar en vistas/consumidores externos)

## 5. Serialization & restore

- [x] 5.1 Implementar `serialize()` determinista con `version: 1`, `fingerprint?` y `pages[]` (`objects: unknown[]` por contrato)
- [x] 5.2 Implementar `restore(payload)` rehidratando objetos por p\u00e1gina
- [x] 5.3 Forward compatibility: ignorar objetos desconocidos de forma segura (sin crash)

## 6. Viewport integration

- [x] 6.1 Extender viewport para montar overlays de anotaciones solo en p\u00e1ginas visibles (sin duplicar render)
- [x] 6.2 Conectar tool actual (`AppVisorPdfTool`) a `AnnotateEngine.setTool()`

## 7. Tests

- [x] 7.1 Test: crear objeto (rect/text) y `serialize()` incluye `pageNumber` + `objects`
- [x] 7.2 Test: `restore()` rehidratando objetos por p\u00e1gina (y no crashea con objetos desconocidos)
- [x] 7.3 Test: undo/redo modifica estado esperado

## 8. Documentation

- [x] 8.1 Actualizar `src/app/Components/UI/AppVisorPdf/README.md` con tools soportadas y atajos (si aplica)
- [x] 8.2 Documentar formato payload `VisorPdfAnnotationsPayloadV1` (versionado) + ejemplo y reglas de compatibilidad (incluye manejo de `objects` desconocidos)
- [x] 8.3 Documentar expl\u00edcitamente que este ticket NO integra el visor en pantallas/consumidores; solo crea/expone el componente y su engine
