<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10 -->
## 1. Inventario y límites

- [x] 1.1 Verificar registro único de archivos 2FA en el `.vbproj`; no modificar producto. Área: proyecto y documentación DOC-98. Origen: D-01, RQ-01. Verificación: inventario falla por símbolo ausente/duplicado.
- [x] 1.2 Contrastar tabla, diez columnas e índices con DOC-92 y crear `Sql/01-preflight-readonly.sql` solo con `SELECT`/`information_schema`. Origen: D-02, RQ-02. Verificación: prueba rechaza DDL/DML.
- [x] 1.3 Documentar controles no reveladores para llaves, Session=10, módulo y SMTP único. Origen: D-03, RQ-03. Verificación: prueba rechaza secretos y conexiones.

## 2. Runbook y operación reversible

- [x] 2.1 Crear runbook con roles, autorizaciones, respaldo, orden, puertas y evidencia. Origen: D-04, RQ-04. Verificación: cada fase tiene precondición, GO/NO-GO y aborto.
- [x] 2.2 Crear scripts guardados de activación/desactivación piloto con snapshot, catálogo central, cardinalidad uno y una fila afectada; no ejecutarlos. Origen: D-05, RQ-05. Verificación: negativos para alcance ambiguo/concurrente.
- [x] 2.3 Crear observabilidad agregada fiel a columnas y diagnósticos, sin datos sensibles. Origen: D-06, RQ-06. Verificación: contrato rechaza `CodeHash`, `AuthUserId`, correo, login y conexión en salidas.
- [x] 2.4 Documentar rotación install-first/remove-last con `v1:keyId:mac`, 10 minutos y margen aprobado. Origen: D-07, RQ-07. Verificación: no retira llave con elegibles.
- [x] 2.5 Documentar/preparar limpieza manual guardada de terminales >30 días, sin scheduler ni ejecución. Origen: D-08, RQ-08. Verificación: preview, límite, transacción y autorización aparte.
- [x] 2.6 Definir rollback: OFF piloto, expiración natural, evidencia y paquete previo, sin DDL destructivo. Origen: D-09, RQ-09. Verificación: cubre configuración, OTP, legacy y fallos.
- [x] 2.7 Crear el manifiesto exhaustivo de filas centrales para DocuArchi, Gestor, Radicación y Workflow y el diagnóstico agregado de identidades/correos. Origen: D-10, RQ-10. Verificación: confirma que la activación depende solo del módulo; no proyecta datos personales ni crea listas por usuario.
- [x] 2.8 Crear el script de activación global por oleadas con IDs/snapshots/conteos explícitos y rollback por fila; prohibir `UPDATE` sin manifiesto. Origen: D-10, RQ-10. Verificación: casos negativos prueban cardinalidad, concurrencia y fila fuera de alcance.

## 3. Pruebas y puertas

- [x] 3.1 Implementar prueba CJS DOC-98 para inventario, runbook, trazabilidad, secretos y SQL. Origen: D-01, RQ-01. Trazabilidad adicional: D-02/RQ-02 y D-03/RQ-03. Verificación: negativos indican archivo/regla.
- [x] 3.2 Validar sintaxis/guardas SQL sin infraestructura e integrar al workflow existente solo si falta la puerta. Origen: D-05, RQ-05. Trazabilidad adicional: D-06/RQ-06 y D-08/RQ-08. Verificación: CI no muta ni usa secretos.
- [x] 3.3 Ejecutar suite 2FA estable, build, OpenSpec y OPSXJ; registrar salidas saneadas. Origen: D-01, RQ-01. Trazabilidad adicional: D-04/RQ-04. Verificación: resultado reproducible o bloqueo explícito.
- [x] 3.4 Ensayar operación/E2E solo con autorización, infraestructura descartable y controles `SELECT`; si no, registrar puertas bloqueadas. Origen: D-04, RQ-04. Trazabilidad adicional: D-05/RQ-05 y D-09/RQ-09. Verificación: no afirmar operaciones no ejecutadas.
- [ ] 3.5 Ejecutar con autorización vigente el piloto, observarlo y activar las filas restantes por oleadas; detener y revertir la oleada ante cualquier puerta fallida. Origen: D-10, RQ-10. Trazabilidad adicional: D-04/RQ-04, D-05/RQ-05 y D-09/RQ-09. Verificación: evidencia saneada por oleada y `affected_rows` igual al manifiesto.
- [ ] 3.6 Verificar el estado final: cuatro módulos activos en todas las empresas autorizadas, aplicación transversal a cualquier principal y cero filas ajenas modificadas. Origen: D-10, RQ-10. Verificación: controles `SELECT`, smoke/E2E representativo por módulo y ausencia de configuración o bypass por usuario.

## 4. Documentación y cierre

- [x] 4.1 Completar matriz, roles, observabilidad, rotación, limpieza, rollback, activación global, riesgos y evidencia DOC-98. Origen: D-04, RQ-04. Trazabilidad adicional: D-06/RQ-06, D-07/RQ-07, D-08/RQ-08, D-09/RQ-09 y D-10/RQ-10. Verificación: consistente con scripts.
- [ ] 4.2 Completar documentos OPSXJ, declarar ambientes realmente activados o bloqueados y confirmar que no cambió lógica. Origen: D-01, RQ-01. Trazabilidad adicional: D-09/RQ-09 y D-10/RQ-10. Verificación: `opsxj:validate` sin evidencia fabricada.
