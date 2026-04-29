## 1. Setup

- [x] 1.1 Crear carpeta `src/app/Components/UI/AppVisorPdf/` con export p\u00fablico (`index.ts`)
- [x] 1.2 Definir tipos/contrato inicial (`AppVisorPdfProps`) para `source` (URL/Blob/ArrayBuffer) y callbacks `onLoadStateChange`/`onError`
- [x] 1.3 Crear componente skeleton `AppVisorPdf.tsx` con estados expl\u00edcitos: `idle/loading/ready/error`

## 2. Rendering & UX baseline

- [x] 2.1 Implementar contenedor de render estable (layout fijo) para evitar flicker en re-render
- [x] 2.2 Implementar UI b\u00e1sica de loading/error (mensajes + retry opcional) seg\u00fan contrato
- [x] 2.3 Agregar configuraci\u00f3n determinista: `initialPage`, `initialZoom` (o equivalente) y asegurar que no cambien sin props

## 3. PDF engine integration (fase 01)

- [ ] 3.1 Crear una abstracci\u00f3n de motor (adapter) para renderizar PDF (interfaces + implementaci\u00f3n concreta)
- [ ] 3.2 Implementar render m\u00ednimo (al menos 1 p\u00e1gina) y eventos de estado: loading \u2192 ready / error
- [ ] 3.3 Implementar navegaci\u00f3n b\u00e1sica (siguiente/anterior o selecci\u00f3n de p\u00e1gina) sin romper estabilidad visual

## 4. Tests (contract/spec coverage)

- [x] 4.1 Agregar tests de comportamiento (Vitest + Testing Library) para estados loading/ready/error del componente
- [ ] 4.2 Agregar tests que validen desacople de dominio (sin imports desde `src/modules/**`) y contrato por props/callbacks
- [x] 4.3 Incluir tags `[SPEC:...]` en tests para cubrir escenarios del spec del visor (crear IDs si se decide mover a YAML behavior/contract)

## 5. Documentation

- [x] 5.1 Crear `src/app/Components/UI/AppVisorPdf/README.md` con proposito, props principales, ejemplos (URL y bytes) y consideraciones de accesibilidad
