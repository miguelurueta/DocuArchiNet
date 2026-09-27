<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09 -->
## 1. Inventario y matriz de cierre

- [x] 1.1 [M] Inventariar suites, escenarios, perfiles, autorizaciones, controles y evidencia de DOC-80/81/82; clasificar cobertura determinista, real vigente, histórica y faltante. Área: `tests/`, `tools/e2e/`, documentación ENLASE. Origen: D-02, RQ-01. Verificación: matriz sin filas huérfanas.
- [x] 1.2 [M] Crear un manifiesto DOC-83 legible por máquina con riesgos altos/muy altos, criterios de cierre y referencias exactas, más una prueba que falle ante ausencias o escenarios no registrados. Área: `tools/e2e/validation/` o convención existente equivalente, `tests/`. Origen: D-02, RQ-01. Verificación: prueba positiva y casos negativos controlados.

## 2. Regresión determinista

- [x] 2.1 [L] Consolidar la batería afectada sin copiar fixtures: capacidad/contexto, preview/streaming, preparación 1/N, tipología, intención única, idempotencia, persistencia física, reconciliación, deduplicación, lista documental y cambio de tarea. Área: suites `importar-servicio-web-*`. Origen: D-07, RQ-04. Verificación: comandos y conteos registrados.
- [x] 2.2 [M] Cubrir asignación explícita y revalidación de `Buttonaceptar_Click`, incluyendo faltante y ausencia de autoasignación, sin inventar `ValidateAssignment`. Área: pruebas ENLASE de asignación. Origen: D-08, RQ-07. Verificación: pruebas estructurales/funcionales fallan si importar dispara asignación.
- [x] 2.3 [M] Cubrir accesibilidad, responsive, cierre por resultado y regresión de constancias/legacy. Área: UI, CSS y suites de compatibilidad existentes. Origen: D-01, RQ-02. Verificación: batería afectada PASS.

## 3. Gate, autorización y plataforma E2E

- [x] 3.1 [M] Caracterizar el gate global en backend, preview y bootstrap; demostrar gate apagado, alcance vacío, legacy disponible y ausencia de doble ruta. No crear un gate nuevo. Área: `Web.config`, feature gate, WebForms, pruebas de gate. Origen: D-05, RQ-05. Verificación: pruebas deterministas y línea base legacy.
- [x] 3.2 [L] Extender la plataforma únicamente para brechas transversales verificadas: acceso negativo, integridad de proveedor/evidencia, gate/legacy y restauración ante fallo; reutilizar registro, runner, adaptador y saneamiento existentes. Área: `tools/e2e/scripts/`, `tools/e2e/tests/`. Origen: D-03, RQ-06. Verificación: pruebas de política del runner.
- [x] 3.3 [M] Validar que los perfiles no sensibles no admitan credenciales, conexiones, SQL libre ni selección de scripts; mantener controles registrados `SELECT` y recursos con ciclo de vida. Área: perfiles y soporte de plataforma. Origen: D-06, RQ-08. Verificación: perfiles inválidos fallan antes de abrir navegador.
- [x] 3.4 [S] Actualizar el runbook solo si los escenarios DOC-83 agregan una operación o autorización nueva; conservar las reglas vigentes si basta reutilización. Área: `tools/e2e/AGENT-RUNBOOK.md`. Origen: D-04, RQ-09. Verificación: comando y autorizaciones coinciden con el registro.

## 4. Ejecución y evidencia

- [x] 4.1 [M] Ejecutar la batería determinista, validadores documentales y MSBuild; conservar fallos reales y corregir solo defectos dentro del alcance refinado. Origen: D-01, RQ-02. Verificación: resultados reproducibles registrados.
- [x] 4.2 [L] Preparar una matriz E2E que reutilice lectura DOC-80, ejecución DOC-81 y UI DOC-82, y enumere únicamente corridas reales faltantes con mutabilidad, recurso y autorizaciones separadas. Origen: D-03, RQ-03. Verificación: ninguna corrida duplica evidencia sin justificación.
- [x] 4.3 [L] Ejecutar E2E reales solo con autorización expresa vigente; lectura/UI deben mantener controles invariantes y cada mutación debe usar autorización y reserva E2E independientes. La reutilización excepcional de una tarea exige autorización expresa y una precondición funcional nueva verificable. Origen: D-04, RQ-04. Verificación: evidencia saneada o bloqueo explícito.
- [x] 4.4 [M] Verificar después de cada corrida gate `false`, usuarios/grupos vacíos, integridad legacy, recurso liberado/consumido y ausencia de secretos. Origen: D-05, RQ-05. Verificación: controles finales incluso ante error.

## 5. Documentación y cierre

- [x] 5.1 [M] Documentar inventario legacy, matriz de pruebas/riesgos, comandos, resultados, evidencia, gate, rollout, rollback y limitaciones en `Doc/Actualizacion/workflow/ImportarServiciWebEnlace/DOC-83-pruebas-gate-compatibilidad/`. Origen: D-02, RQ-01. Verificación: documentación coincide con código y resultados.
- [x] 5.2 [S] Completar los cinco documentos técnicos OPSXJ sin plantillas, listas abiertas ni afirmaciones no verificadas. Área: `Doc/Tecnica/Opsxj/doc-83-pruebas-servicio-sii-enlace/`. Origen: D-06, RQ-08. Verificación: contrato documental OPSXJ PASS.
- [x] 5.3 [M] Ejecutar `opsxj:refine`, OpenSpec estricto, revisión y `opsxj:validate`; registrar evidencia `unit` y `manual_qa` contra el SHA final. Origen: D-09, RQ-09. Verificación: compuertas PASS.
- [x] 5.4 [M] Preparar el traspaso al ciclo OPSXJ con la matriz satisfecha, especificación lista para sincronización y rama validable. El archivo, la publicación y el cierre se ejecutan después como etapas remotas de OPSXJ, sin declararlas anticipadamente. Origen: D-09, RQ-09. Verificación: validación local PASS y siguiente etapa `archive` habilitada.
