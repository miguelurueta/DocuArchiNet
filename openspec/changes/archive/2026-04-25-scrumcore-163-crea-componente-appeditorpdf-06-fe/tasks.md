## 1. Artefactos OpenSpec del 06-FE

- [x] 1.1 Definir `design.md` para baseline de paginacion visual en `AppEditorPdf`.
- [x] 1.2 Crear `specs/app-appeditorpdf-06-fe/spec.md` con contrato de defaults + overrides.
- [x] 1.3 Alinear alcance con la fase `06-FE` previa de `AppEditor` sin duplicar implementacion interna.

## 2. Implementacion del contrato en wrapper AppEditorPdf

- [x] 2.1 Aplicar defaults de paginacion visual base en `AppEditorPdf` (`visual`, `A4`, `portrait`, margenes base).
- [x] 2.2 Mantener forwarding de props y permitir overrides explicitos de consumidores.
- [x] 2.3 Preservar contrato de accesibilidad existente del wrapper.

## 3. Pruebas y cierre tecnico

- [x] 3.1 Actualizar pruebas unitarias de `AppEditorPdf` a `SPEC:APP-APPEDITORPDF-06-FE`.
- [x] 3.2 Agregar pruebas para defaults de paginacion visual y overrides parciales.
- [x] 3.3 Ejecutar suite focal de `AppEditorPdf` y registrar resultado.
- [x] 3.4 Dejar cambio listo para `opsxj:archive`.
