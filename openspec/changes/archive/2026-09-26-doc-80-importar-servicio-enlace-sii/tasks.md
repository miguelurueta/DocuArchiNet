<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
## 1. Refinement

- [x] 1.1 Caracterizar el flujo `ENLASE`, el contrato `consultarRadicado/imagenes` y los componentes modernos reutilizables. Origen: D-03, RQ-03
- [x] 1.2 Aprobar decisiones D-01 a D-07, requisitos RQ-01 a RQ-07, riesgos y alcance no mutador de DOC-80. Origen: D-01, RQ-01

## 2. Contratos y contexto

- [x] 2.1 Extender contratos para declarar capacidad y operación sin romper solicitudes existentes. Origen: D-01, RQ-01
- [x] 2.2 Implementar reconstrucción y autorización del contexto de preasignación `ENLASE`. Origen: D-02, RQ-02

## 3. Proveedor y consulta

- [x] 3.1 Implementar cliente/adaptador de anexos reutilizando autenticación y transporte SII. Origen: D-01, RQ-01
- [x] 3.2 Implementar mapper, validación de `idanexo`, listas vacías y errores contractuales seguros. Origen: D-03, RQ-03
- [x] 3.3 Publicar consulta moderna no mutadora y telemetría diferenciada. Origen: D-05, RQ-05

## 4. Preview seguro

- [x] 4.1 Resolver preview por `idanexo` mediante reconsulta autoritativa, descriptor temporal y streaming existente. Origen: D-04, RQ-04
- [x] 4.2 Validar expiración, pertenencia, formato, tamaño y hosts permitidos. Origen: D-04, RQ-04

## 5. Pruebas y compatibilidad

- [x] 5.1 Agregar fixtures y pruebas focales de capacidad, contexto, mapping, preview, errores y no mutación. Origen: D-07, RQ-07
- [x] 5.2 Ejecutar regresión de constancias y verificar invariancia de almacenamiento/legacy. Origen: D-06, RQ-06
- [x] 5.3 Extender `tools/e2e` con escenario de lectura/preview, sin ejecutarlo hasta contar con autorización expresa. Origen: D-07, RQ-07

## 6. Documentación y validación

- [x] 6.1 Completar paquete técnico DOC-80 con contrato, mapping, seguridad, diagramas y evidencia real. Origen: D-04, RQ-04
- [x] 6.2 Ejecutar revisión técnica y validación OPSXJ para el SHA final. Origen: D-06, RQ-06

## 7. Contrato documental estructural

- [x] 7.1 Auditar contra código el alcance, las firmas, decisiones y códigos HTTP de consulta/preview. Origen: D-07, RQ-07
- [x] 7.2 Entregar diagramas UML 2 en fuentes PlantUML de texto, casos de uso e inventario técnico con trazabilidad explícita. Origen: D-07, RQ-07
- [x] 7.3 Integrar y ejecutar validación automática de diagramas, referencias y firmas con Node y Roslyn. Origen: D-07, RQ-07

## 8. Corrección de autoridad del streaming ENLASE

- [x] 8.1 Resolver en el handler la tarea ENLASE desde `ID_TAREA_SELECCIONDA_ENLACE` y exigir coincidencia con `SELECCIONTEMPORAL`, sin fallback estándar. Origen: D-02, RQ-02
- [x] 8.2 Agregar regresión automática de autorización y actualizar diagramas, inventario y hallazgos. Origen: D-04, RQ-04