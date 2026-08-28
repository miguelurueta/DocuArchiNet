<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
## 1. Línea base y decisión

- [x] 1.1 Confirmar y documentar la evidencia DOC-38 y la versión candidata como precondiciones, sin convertirlas en autorización operativa. Origen: D-01, RQ-01
- [x] 1.2 Emitir la decisión inicial de DOC-39 y registrar las precondiciones pendientes de ambiente. Origen: D-07, RQ-07

## 2. Matriz y runbook

- [x] 2.1 Crear la matriz por ambiente con autorización, versión, alcance, ventana, responsables, evidencia y continuación, sin secretos. Origen: D-02, RQ-02
- [x] 2.2 Crear el runbook de prechequeo, operación autorizada, comprobación y escalamiento sin ejecutar despliegue ni cambiar configuración. Origen: D-03, RQ-03
- [x] 2.3 Documentar las consultas `SELECT` autorizables y la sanitización de resultados para controles de liberación. Origen: D-04, RQ-04
- [x] 2.4 Documentar la reversión por gestión de despliegue y el límite de no alterar transiciones confirmadas. Origen: D-05, RQ-05

## 3. Compatibilidad y evidencia

- [x] 3.1 Verificar documentalmente que Usuario anterior conserva la ruta moderna y que las operaciones vecinas mantienen sus contratos. Origen: D-06, RQ-06
- [x] 3.2 Registrar evidencia, limitaciones y riesgos residuales sin ejecutar E2E, carga, cambios de ambiente ni una transición real. Origen: D-04, RQ-04

## 4. Cierre documental

- [x] 4.1 Actualizar el paquete técnico de liberación con matriz, runbook, decisión, aprobaciones requeridas y responsables. Origen: D-02, RQ-07
- [x] 4.2 Ejecutar refinamiento OPSXJ, validación OpenSpec y validación OPSXJ al finalizar la evidencia documental. Origen: D-07, RQ-07
