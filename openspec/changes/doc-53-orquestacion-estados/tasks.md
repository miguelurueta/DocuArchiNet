## 1. Contratos y persistencia

- [x] 1.1 [M] Extender modelos con resultado, persistencia conocida, reintento, correlación y auditoría. Área/archivos: `Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebModels.vb`. Origen: D-01, RQ-01. Verificación: expresa éxito, fallo, incertidumbre y detención sin WebForms.
- [x] 1.2 [M] Agregar actualización optimista de intención, item y transición. Área/archivos: interfaces y `MySqlImportIntentRepository.vb`. Origen: D-02, RQ-02. Verificación: versión obsoleta no actualiza ni repite efectos.
- [x] 1.3 [S] Completar DTO y fixtures Execute/Get v1. Área/archivos: DTOs y `Tests/Fixtures/Workflow/ImportarServicioWeb/`. Origen: D-08, RQ-08. Verificación: serializa fase, persistenceKnown, retryable, código, correlación y versión.

## 2. Orquestación y fronteras

- [x] 2.1 [M] Implementar ejecución estrictamente secuencial. Área/archivos: `Services/Workflow/ImportarServicioWeb/ImportServiceOrchestrator.vb`. Origen: D-02, RQ-02. Verificación: máximo una operación de item activa.
- [x] 2.2 [M] Implementar tabla pura de transiciones. Área/archivos: `ImportIntentStateMachine.vb`. Origen: D-03, RQ-03. Verificación: matriz cubre fases normales y alternativas.
- [x] 2.3 [M] Coordinar proveedor, expediente, índices, storage y caché en el orden aprobado. Área/archivos: orquestador y puertos modernos. Origen: D-04, RQ-04. Verificación: fallo por fase conserva última confirmación.
- [x] 2.4 [M] Crear puerto y adaptador exclusivo de storage. Área/archivos: `ImportDocumentStoragePort.vb`, `Storage/LegacyImportDocumentStorageAdapter.vb`. Origen: D-05, RQ-05. Verificación: caracteriza 16 argumentos y YES/error/excepción sin editar legacy.
- [x] 2.5 [M] Clasificar fallos e incertidumbre sin reintento inseguro. Área/archivos: `ImportItemResultFactory.vb`, orquestador. Origen: D-06, RQ-06. Verificación: retryable solo sin posible efecto mutador.
- [x] 2.6 [M] Implementar detención cooperativa. Área/archivos: orquestador. Origen: D-07, RQ-07. Verificación: conserva confirmados y no invoca pendientes.
- [x] 2.7 [S] Mapear Execute/Get desde la instantánea persistida. Área/archivos: servicios modernos. Origen: D-08, RQ-08. Verificación: Get no usa Session.
- [x] 2.8 [S] Registrar cinco archivos VB nuevos. Área/archivos: `GestionDocumental-Docuarchi.net.vbproj`. Origen: D-09, RQ-09. Verificación: una entrada Compile por archivo sin mover legacy.

## 3. Pruebas focales

- [x] 3.1 [M] Probar autorización e intención operable. Área/archivos: `Tests/importar-servicio-web-orchestrator.test.cjs`. Origen: D-01, RQ-01. Verificación: rechazo no llama mutadores.
- [x] 3.2 [M] Probar secuencia y conflicto optimista. Área/archivos: tests de orquestador/repositorio. Origen: D-02, RQ-02. Verificación: orden exacto y cero efectos con token obsoleto.
- [x] 3.3 [M] Probar estados y auditoría saneada. Área/archivos: `Tests/importar-servicio-web-state-machine.test.cjs`. Origen: D-03, RQ-03. Verificación: transición inválida no muta.
- [x] 3.4 [M] Inyectar fallos antes/después de cada fase. Área/archivos: `Tests/importar-servicio-web-failure-injection.test.cjs`. Origen: D-04, RQ-04. Verificación: cada caso conserva fase demostrable.
- [x] 3.5 [M] Caracterizar adaptador storage con dobles. Área/archivos: `Tests/importar-servicio-web-storage-adapter.test.cjs`. Origen: D-05, RQ-05. Verificación: argumentos/resultados coinciden con legacy.
- [x] 3.6 [M] Probar retryable e incertidumbre. Área/archivos: test de fallos. Origen: D-06, RQ-06. Verificación: incierto nunca reintenta automáticamente.
- [x] 3.7 [M] Probar detención con confirmados y pendientes. Área/archivos: test de orquestador. Origen: D-07, RQ-07. Verificación: no inicia siguiente ni revierte anterior.
- [x] 3.8 [S] Probar contrato Execute/Get v1. Área/archivos: tests contractuales/fixtures. Origen: D-08, RQ-08. Verificación: JSON válido, saneado y compatible.

## 4. Evidencia y documentación

- [ ] 4.1 [M] Crear paquete 00-07 y diagramas canónicos. Área/archivos: `Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-53-orquestacion-estados-compensacion/`. Origen: D-09, RQ-09. Verificación: cubre secuencia, estados, fallos, transacciones y frontera inmutable.
- [ ] 4.2 [S] Auditar rutas legacy prohibidas. Área/archivos: diff DOC-53. Origen: D-09, RQ-09. Verificación: ASMX, JS, Integracionccv, ServiciosIntegracion y ClassAlmacenamiento intactos.
- [ ] 4.3 [M] Ejecutar suite, build, OpenSpec y OPSXJ. Área/archivos: solución/cambio. Origen: D-09, RQ-09. Verificación: resultados registrados sin DDL, base real, E2E ni gate.
