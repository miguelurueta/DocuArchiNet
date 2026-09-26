<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->

## 1. Contratos y preparación

- [x] 1.1 Caracterizar en pruebas la firma, precondiciones, retornos y efectos de `PreAlmacenaDocumentoAnexosEnlaceIntegracionSII`, incluida su dependencia de `ID_TAREA_SELECCIONDA_ENLACE`, sin alterar datos reales. Origen: D-04, RQ-04
- [x] 1.2 Extender modelos/DTOs solo con los campos necesarios para anexos ENLASE, resultado recuperable y evidencia de confirmación; preservar contratos de constancias. Origen: D-01, RQ-01
- [x] 1.3 Ajustar catálogo y resolución de tipología para preparación individual/múltiple y predeterminado únicamente inequívoco. Origen: D-01, RQ-01
- [x] 1.4 Extender `ServicioPreflightImportacion` para la capacidad ENLASE y bloquear selección vacía, duplicada, ambigua o no autorizada sin escrituras. Origen: D-01, RQ-01

## 2. Intención, contexto e idempotencia

- [x] 2.1 Incorporar capacidad e identidades externas al valor canónico de idempotencia, excluyendo URL temporal y conservando una intención para toda la selección. Origen: D-02, RQ-02
- [x] 2.2 Reutilizar/extender `MySqlImportIntentRepository` para persistir identidad externa, tipología, estado e ID interno por elemento sin duplicar intención. Origen: D-02, RQ-02
- [x] 2.3 Revalidar usuario, tarea, actividad/ruta, trámite, proveedor y `ANEXOS_RADICADO_ENLASE` después de adquirir el guard y antes del primer efecto. Origen: D-03, RQ-03
- [x] 2.4 Probar repetición y dos ejecuciones concurrentes, demostrando como máximo una invocación efectiva de almacenamiento. Origen: D-02, RQ-02

## 3. Descarga y persistencia adaptada

- [x] 3.1 Reutilizar el cliente SII moderno para descargar y validar tamaño, formato y contenido antes de persistencia. Origen: D-03, RQ-03
- [x] 3.2 Implementar el adaptador de `IImportDocumentStoragePort` que invoque una sola vez `PreAlmacenaDocumentoAnexosEnlaceIntegracionSII`, sin copiar su cuerpo ni usar HTTP interno. Origen: D-04, RQ-04
- [x] 3.3 Traducir `YES`, texto de error y excepciones legacy a códigos estructurados saneados, reteniendo el ID interno solo como evidencia pendiente de verificación. Origen: D-04, RQ-04
- [x] 3.4 Componer el paso ENLASE en `WebServiceImportarServicioWebModern` sin habilitar asignación, cierre ni efectos de constancias. Origen: D-03, RQ-03

## 4. Verificación y reconciliación

- [x] 4.1 Extender el repositorio de reconciliación para correlacionar intención, tarea, proveedor, capacidad, identidad externa, documento y relación autorizada. Origen: D-05, RQ-05
- [x] 4.2 Incorporar una verificación explícita y testeable de existencia física; confirmar únicamente cuando evidencia lógica y física coincidan. Origen: D-05, RQ-05
- [x] 4.3 Clasificar registro lógico sin archivo como recuperable y permitir una nueva ejecución controlada sin duplicar un documento válido. Origen: D-05, RQ-05
- [x] 4.4 Ajustar máquina de estados, mapper y agregación para éxito, omisión idempotente, fallo previo, parcial, inconsistente e incierto; prohibir retry automático del incierto. Origen: D-06, RQ-06
- [x] 4.5 Probar respuesta perdida, evidencia completa, archivo ausente, relación duplicada, parcial y reconciliación por elemento. Origen: D-05, RQ-05

## 5. Compatibilidad, documentación y validación

- [x] 5.1 Agregar pruebas de regresión que demuestren invariancia de constancias, ASMX legacy y ausencia de asignación/cierre de tarea. Origen: D-07, RQ-07
- [x] 5.2 Integrar archivos nuevos al proyecto y ejecutar compilación y suites unitarias/integración afectadas, registrando resultados reales. Origen: D-01, RQ-01
- [x] 5.3 Documentar flujo, estados, idempotencia, mapping legacy, persistencia, reconciliación, seguridad, rollback y trazabilidad real en `Doc/Actualizacion/workflow/ImportarServiciWebEnlace/DOC-81-preparacion-persistencia-reconciliacion/`. Origen: D-04, RQ-04
- [x] 5.4 Reutilizar la plataforma E2E para escenarios de lectura y ejecución ENLASE, con política de secretos, controles `SELECT` y restauración del gate; validar una ejecución real con varios anexos en una sola intención y no ejecutar mutaciones sin autorización expresa. Origen: D-07, RQ-07
- [x] 5.5 Ejecutar `opsxj:refine`, validación OpenSpec estricta y gates técnicos; corregir toda divergencia entre artefactos y código antes del cierre. Origen: D-01, RQ-01
