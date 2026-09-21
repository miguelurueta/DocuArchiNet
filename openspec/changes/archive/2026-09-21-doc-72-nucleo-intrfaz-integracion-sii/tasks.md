<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
## 1. Núcleo y contratos

- [x] 1.1 Crear `importar-servicio-web-api.js` con operaciones contractuales, transporte inyectable y normalización ASMX. Origen: D-01, RQ-01
- [x] 1.2 Crear registro de proveedores, identidad canónica, capacidades y errores seguros sin fallback a SII. Origen: D-02, RQ-02
- [x] 1.3 Crear máquina de estados cerrada y garantizar una ejecución programática por intención; la selección y confirmación visual quedan fuera de DOC-72. Origen: D-03, RQ-03

## 2. Integración WebForms y accesibilidad

- [x] 2.1 Agregar modal y bootstrap desde `ctw-document-action-service`, preservando `btnloadservice`. Origen: D-04, RQ-04
- [x] 2.2 Registrar scripts y CSS desde `Webworkflow.aspx.vb` bajo el gate aplicable. Origen: D-04, RQ-04
- [x] 2.3 Implementar UI con render por estado, foco, teclado y `aria-live`. Origen: D-05, RQ-05
- [x] 2.4 Crear estilos aislados y registrar assets como `<Content>` en el `.vbproj`. Origen: D-05, RQ-05

## 3. Pruebas y documentación

- [x] 3.1 Crear pruebas de core para transiciones, ejecución única y resultados. Origen: D-03, RQ-03
- [x] 3.2 Crear pruebas de registro/UI para capacidades, errores, gate y legacy. Origen: D-02, RQ-02
- [x] 3.3 Crear pruebas de accesibilidad para foco, teclado, diálogo y anuncios. Origen: D-05, RQ-05
- [x] 3.4 Ejecutar suites focales y regresiones sin E2E real; registrar resultados. Origen: D-06, RQ-06
- [x] 3.5 Completar documentación canónica con arquitectura, contrato, estados, pruebas, diagramas y metadata. Origen: D-06, RQ-06
