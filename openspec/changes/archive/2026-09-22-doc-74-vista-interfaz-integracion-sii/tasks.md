<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
## 1. Estado y contrato de preview

- [x] 1.1 Crear `importar-servicio-web-preview-state.js` con transiciones puras para preparando, disponible, formato no visualizable, vencido, proveedor no disponible, no autorizado y bloqueado. Origen: D-02, RQ-02
- [x] 1.2 Crear `importar-servicio-web-preview.js` y solicitar `GetPreview` mediante el cliente existente; derivar el handler solo del descriptor seguro. Origen: D-01, RQ-01
- [x] 1.3 Implementar renovación explícita y deduplicación durante foco, resize y rerender. Origen: D-02, RQ-02
- [x] 1.4 Implementar fallback de descarga por el mismo handler para formatos no visualizables. Origen: D-05, RQ-05

## 2. Integración visual y accesibilidad

- [x] 2.1 Agregar a `workflow/Webworkflow.aspx` el panel, estados, cierre, renovación, descarga y `Volver a la lista`. Origen: D-03, RQ-03
- [x] 2.2 Extender el CSS existente con layout lateral y subvista responsive. Origen: D-03, RQ-03
- [x] 2.3 Integrar la lista conservando selección, filtros, scroll y foco; distinguir recurso externo y documento importado. Origen: D-03, RQ-03; D-04, RQ-04
- [x] 2.4 Mostrar `Ver documento importado` solo con identidad interna autorizada y delegar al visor existente. Origen: D-04, RQ-04
- [x] 2.5 Traducir gate, expiración, proveedor y autorización a estados cerrados. Origen: D-05, RQ-05

## 3. Registro y compatibilidad

- [x] 3.1 Registrar scripts en orden en code-behind y `GestionDocumental-Docuarchi.net.vbproj`. Origen: D-06, RQ-06
- [x] 3.2 Añadir invariancias contra segundo cliente HTTP, JavaScript inline, `window.open` principal y cambios en almacenamiento o visores. Origen: D-01, RQ-01; D-06, RQ-06

## 4. Pruebas

- [x] 4.1 Crear `Tests/importar-servicio-web-preview.test.cjs` para estados, renovación, fallback y descarga única. Origen: D-02, RQ-02
- [x] 4.2 Crear `Tests/importar-servicio-web-preview-security.test.cjs` para mediación, fallo cerrado y prohibición de URL externa/base64. Origen: D-01, RQ-01; D-05, RQ-05
- [x] 4.3 Crear `Tests/importar-servicio-web-preview-accessibility.test.cjs` para foco, cierre, volver, anuncios y responsive. Origen: D-03, RQ-03
- [x] 4.4 Ejecutar pruebas focales y regresión relacionada; no ejecutar E2E real ni activar gates sin autorización. Origen: D-05, RQ-05; D-06, RQ-06

## 5. Documentación y validación

- [x] 5.1 Crear el paquete documental DOC-74 con índice, arquitectura, flujo, contrato, estados, evidencia, diagramas y metadata. Origen: D-06, RQ-06
- [x] 5.2 Documentar dependencia B10, rollout, riesgos, rollback y funciones modificadas. Origen: D-05, RQ-05; D-06, RQ-06
- [x] 5.3 Registrar comandos/resultados en `05-PruebasEvidencia.md` y validar OpenSpec/opsxj. Origen: D-06, RQ-06
