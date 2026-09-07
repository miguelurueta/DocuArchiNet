<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09 -->
## 1. Contratos y acceso a datos

- [x] 1.1 [M] Definir snapshot read-only de intención, item, documento y relación. Área/archivos: `Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebModels.vb`, interfaces modernas. Origen: D-02, RQ-02. Verificación: expresa fuentes/cardinalidad sin DTO, SQL ni Session.
- [x] 1.2 [M] Agregar consultas completa y focal parametrizadas. Área/archivos: `Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportReconciliationRepository.vb`. Origen: D-03, RQ-03. Verificación: filtros incluyen intención, proveedor, external key y contexto.
- [x] 1.3 [S] Extender `ImportItemResultDto` v1 con fase y mínimos de lista. Área/archivos: DTOs y fixtures contractuales. Origen: D-07, RQ-07. Verificación: serialización compatible sin campos prohibidos.
- [x] 1.4 [S] Registrar nuevos VB en el proyecto. Área/archivos: `GestionDocumental-Docuarchi.net.vbproj`. Origen: D-09, RQ-09. Verificación: una entrada `Compile` por archivo.

## 2. Servicio y mapping

- [x] 2.1 [M] Implementar autorización opaca de Get/Reconcile. Área/archivos: `Services/Workflow/ImportarServicioWeb/ServicioReconciliacionImportacion.vb`. Origen: D-01, RQ-01. Verificación: ajeno/inexistente no filtra datos.
- [x] 2.2 [M] Clasificar relación confirmada, ausente, duplicada, parcial y cruzada. Área/archivos: servicio y snapshots. Origen: D-04, RQ-04. Verificación: solo relación única produce `Disponible`.
- [x] 2.3 [M] Implementar tabla total de estados visibles. Área/archivos: `ImportItemResultMapper.vb`. Origen: D-05, RQ-05. Verificación: toda fase tiene una salida.
- [x] 2.4 [S] Aplicar clasificación conservadora a persistencia desconocida. Área/archivos: mapper. Origen: D-06, RQ-06. Verificación: nunca disponible ni reintentable por inferencia.
- [x] 2.5 [S] Sanear códigos, mensajes y metadatos. Área/archivos: servicio, mapper y DTOs. Origen: D-07, RQ-07. Verificación: pruebas excluyen excepción, SQL, secretos y rutas.
- [x] 2.6 [M] Deduplicar confirmados y conservar correlación. Área/archivos: servicio. Origen: D-08, RQ-08. Verificación: `(TaskId, DocumentId)` aparece una vez.

## 3. Pruebas focales

- [x] 3.1 [M] Probar reconstrucción completa/parcial. Área/archivos: `Tests/importar-servicio-web-reconciliation.test.cjs`, fixtures. Origen: D-02, RQ-02. Verificación: completed/partial provienen del snapshot.
- [x] 3.2 [M] Probar aislamiento focal. Área/archivos: test y `wrong-task.json`. Origen: D-03, RQ-03. Verificación: identidad ajena no retorna.
- [x] 3.3 [M] Probar relación ausente, duplicada y cruzada. Área/archivos: `Tests/importar-servicio-web-document-list-contract.test.cjs`. Origen: D-04, RQ-04. Verificación: inconsistentes no entran en confirmados.
- [x] 3.4 [M] Cubrir tabla fase/estado. Área/archivos: test contractual/mapper. Origen: D-05, RQ-05. Verificación: todas las fases tienen salida única.
- [x] 3.5 [S] Probar timeout/persistencia desconocida. Área/archivos: test y `uncertain.json`. Origen: D-06, RQ-06. Verificación: salida conservadora.
- [x] 3.6 [S] Probar contrato saneado. Área/archivos: test contractual y fixtures v1. Origen: D-07, RQ-07. Verificación: mínimos presentes/campos prohibidos ausentes.
- [x] 3.7 [M] Probar deduplicación/correlación. Área/archivos: `duplicated-document.json`. Origen: D-08, RQ-08. Verificación: un confirmado por documento/tarea.
- [x] 3.8 [M] Probar autorización. Área/archivos: `Tests/importar-servicio-web-reconciliation-authorization.test.cjs`. Origen: D-09, RQ-09. Verificación: denegado no revela ni muta.

## 4. Integración, evidencia y documentación

- [x] 4.1 [S] Integrar Get/Reconcile con contratos modernos. Área/archivos: servicios `ImportarServicioWeb`. Origen: D-01, RQ-01. Verificación: ambas operaciones usan servicio autorizado/mapper.
- [x] 4.2 [M] Crear paquete técnico 00-07 y diagramas. Área/archivos: `Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-54-reconciliacion-lista-documentos/`. Origen: D-09, RQ-09. Verificación: cubre fuentes, joins, autorización, duplicados, estados e incertidumbre.
- [x] 4.3 [S] Auditar superficies legacy. Área/archivos: diff DOC-54. Origen: D-09, RQ-09. Verificación: ASMX, JS, escritura, almacenamiento, dato_lista y cachés intactos.
- [x] 4.4 [M] Ejecutar suite, build y validaciones. Área/archivos: solución/cambio. Origen: D-09, RQ-09. Verificación: evidencia sin base real, DDL, E2E autenticado, carga ni gates.
