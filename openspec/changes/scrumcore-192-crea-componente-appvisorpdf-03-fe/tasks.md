## 1. Dependencies & contracts

- [ ] 1.1 Agregar dependencia `fabric` y verificar tipado estricto (sin `any`)
- [ ] 1.2 Definir `VisorPdfAnnotationsPayloadV1` y `AnnotateEngine` en `src/app/Components/UI/AppVisorPdf/domain/` o `engine/` seg\u00fan convenci\u00f3n

## 2. Fabric engine core

- [ ] 2.1 Implementar `src/app/Components/UI/AppVisorPdf/engine/fabricEngine.ts` (attach/detach/setTool/undo/redo/serialize/restore/destroy)
- [ ] 2.2 Definir estrategia por p\u00e1gina: overlay canvas por p\u00e1gina visible (no global) y lifecycle acorde a virtualizaci\u00f3n
- [ ] 2.3 Implementar tool routing (`setTool`) con comportamiento determinista

## 3. Tools

- [ ] 3.1 Implementar tools en `src/app/Components/UI/AppVisorPdf/engine/tools/*` (select/freehand/text/rect/arrow)
- [ ] 3.2 Asegurar que tools operan solo sobre la p\u00e1gina activa/overlay correcto

## 4. Undo/redo

- [ ] 4.1 Implementar undo/redo (por p\u00e1gina o global con stacks) y definir qu\u00e9 significa para UX
- [ ] 4.2 Conectar undo/redo a acciones UI (sin exponer detalles Fabric)

## 5. Serialization & restore

- [ ] 5.1 Implementar `serialize()` determinista con `version: 1`, `fingerprint?` y `pages[]`
- [ ] 5.2 Implementar `restore(payload)` rehidratando objetos por p\u00e1gina
- [ ] 5.3 Forward compatibility: ignorar objetos desconocidos de forma segura (sin crash)

## 6. Viewport integration

- [ ] 6.1 Extender viewport para montar overlays de anotaciones solo en p\u00e1ginas visibles (sin duplicar render)
- [ ] 6.2 Conectar tool actual (`AppVisorPdfTool`) a `AnnotateEngine.setTool()`

## 7. Tests

- [ ] 7.1 Test: crear objeto (rect/text) y `serialize()` incluye `pageNumber` + `objects`
- [ ] 7.2 Test: `restore()` rehidrata objetos en overlay canvas
- [ ] 7.3 Test: undo/redo modifica estado esperado

## 8. Documentation

- [ ] 8.1 Actualizar `src/app/Components/UI/AppVisorPdf/README.md` con tools soportadas y atajos (si aplica)
- [ ] 8.2 Documentar formato payload `VisorPdfAnnotationsPayloadV1` (versionado) + ejemplo y reglas de compatibilidad

