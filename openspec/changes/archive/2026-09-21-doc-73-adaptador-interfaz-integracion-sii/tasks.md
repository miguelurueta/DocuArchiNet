<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
## 1. Refinamiento

- [x] 1.1 Inspeccionar núcleo DOC-72, endpoints, DTOs y fixtures. Origen: D-01, RQ-01
- [x] 1.2 Aprobar decisiones, requisitos, riesgos y frontera sin mutación. Origen: D-06, RQ-06

## 2. Adaptador y contrato

- [x] 2.1 Crear mapper con validación y campos normalizados, sin parsear `ExternalKey`. Origen: D-03, RQ-03
- [x] 2.2 Crear lista con instantánea, filtros, paginación y selección locales. Origen: D-04, RQ-04
- [x] 2.3 Crear adaptador y registrarlo solo para `INTEGRACIONSII` usando API moderna. Origen: D-01, RQ-01
- [x] 2.4 Integrar estados y tabla accesible, texto saneado y sin logs sensibles. Origen: D-05, RQ-05
- [x] 2.5 Registrar módulos como `<Content>` y cargarlos bajo el gate existente. Origen: D-02, RQ-02

## 3. Pruebas

- [x] 3.1 Probar mapper con cero, uno, múltiples, metadatos y respuesta inválida. Origen: D-03, RQ-03
- [x] 3.2 Probar filtros, paginación local y exclusión de importados. Origen: D-04, RQ-04
- [x] 3.3 Probar identidad, capacidades, consulta única y ausencia de red/legacy. Origen: D-02, RQ-02
- [x] 3.4 Ejecutar regresiones DOC-72, gate y superficies legacy. Origen: D-06, RQ-06

## 4. Documentación y cierre

- [x] 4.1 Crear paquete canónico DOC-73 con arquitectura, mapping, estados, pruebas y diagramas. Origen: D-06, RQ-06
- [x] 4.2 Completar documentación Opsxj y evidencia manual sin SII real. Origen: D-05, RQ-05
- [x] 4.3 Validar OpenSpec estricto y coherencia D/RQ/tareas. Origen: D-06, RQ-06
