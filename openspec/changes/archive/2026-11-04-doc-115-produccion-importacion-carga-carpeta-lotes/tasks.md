<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09 -->

# Tareas DOC-115

28 tareas completadas con evidencia local y ejecución autorizada de 07-A y RECIB-01/02/03. Publicación y cierre Jira se tramitan después de la revisión de entrega. Las caracterizaciones de disco usan .NET Framework real con repositorio doble y no acreditan MySQL. Complejidad relativa; las tareas de caracterización conservan código si cumple y corrigen únicamente la brecha reproducida, con su regresión. No se autoriza reescribir componentes por aparecer en la lista.

## 07-A — mapa, esquema y repositorios

- [x] 1.1 [S] Actualizar el mapa común con rutas existentes y propuestas DOC-115. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/design.md`. Origen: D-01, RQ-01. Verificación: cada ruta tiene propietario, estado y consumidores, sin duplicar el diccionario contractual.
- [x] 1.2 [M] Contrastar DDL operativo con garantías de identidad y repositorio. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Sql/02-carga-carpetas-mysql51.sql`. Origen: D-01, RQ-01. Verificación: matriz de seis tablas, claves únicas y FK; no se reejecuta DDL.
- [x] 1.3 [S] Precisar pre/postcomprobaciones y recuperación de instalación parcial. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Sql/02-aplicacion-carga-carpetas.md`. Origen: D-01, RQ-01. Verificación: procedimiento manual distingue instalación nueva de existente y registra si no requiere incremento.
- [x] 2.1 [M] Caracterizar doble confirmación en conexiones MySQL independientes. Área/archivos: `tools/e2e/scripts/run-doc111-persistence.ps1`. Origen: D-02, RQ-02. Verificación: 07-A autorizado demuestra un lote y una sola copia del inventario.
- [x] 2.2 [M] Caracterizar clave compartida por planes distintos. Área/archivos: `tests/ImportacionCarpetasRepositoryIntegrationTests.cs`. Origen: D-02, RQ-02. Verificación: integración prueba conflicto y ausencia de segundo lote bajo unicidad de ámbito.
- [x] 2.3 [M] Verificar rollback acotado de confirmación con el observador existente. Área/archivos: `tests/ImportacionCarpetasRepositoryIntegrationTests.cs`. Origen: D-02, RQ-02. Verificación: fallo antes de commit deja plan e inventario coherentes sin exigir IDs sin huecos.
- [x] 2.4 [M] Verificar recepción transaccional y exclusión de revisiones. Área/archivos: `tests/ImportacionCarpetasRepositoryIntegrationTests.cs`. Origen: D-06, RQ-06. Verificación: dos conexiones no confirman dos intentos ni duplican bytes/eventos de recepción confirmada.

## 07-B — autoridad, staging y recuperación

- [x] 3.1 [M] Ampliar casos de autoridad multipart con el harness existente. Área/archivos: `tests/ImportacionCarpetasBehaviorTests.cs`. Origen: D-03, RQ-03. Verificación: sesión/origen/token/campos e ID cruzado se rechazan antes de staging.
- [x] 3.2 [S] Fijar ejemplos request/response y errores con contratos vigentes. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/02-contratos-operacion.md`. Origen: D-03, RQ-03. Verificación: ejemplos coinciden con DTO y EstadoHttp sin nuevos parámetros de destino.
- [x] 3.3 [M] Caracterizar contención física y PDF real en staging. Área/archivos: `tests/ImportacionCarpetasBehaviorTests.cs`. Origen: D-04, RQ-04. Verificación: nombre hostil/enlace/stream truncado o PDF inválido nunca produce RECIBIDO.
- [x] 3.4 [M] Caracterizar cuota agregada inyectada y cuota efectiva del servidor. Área/archivos: `Infrastructure/ProduccionDocumental/ImportacionCarpetas/AlmacenStagingImportacion.vb`. Origen: D-04, RQ-04. Verificación: partes más completos se cuentan una vez; informe declara MaximoBytesLote ilimitado vigente.
- [x] 3.5 [M] Comprobar completo retenido antes del commit de fila. Área/archivos: `Services/ProduccionDocumental/ImportacionCarpetas/ServicioRecepcionImportacion.vb`. Origen: D-05, RQ-05. Verificación: fallo inyectado se reconcilia internamente sin nuevo .bin ni almacenamiento.
- [x] 3.6 [M] Comprobar fila recibida con contenido ausente o cambiado. Área/archivos: `Infrastructure/ProduccionDocumental/ImportacionCarpetas/AlmacenStagingImportacion.vb`. Origen: D-05, RQ-05. Verificación: se bloquea replay inseguro y se conserva evidencia de la primera recepción.
- [x] 3.7 [M] Comprobar reenvío tras pérdida de respuesta y clave terminal. Área/archivos: `Services/ProduccionDocumental/ImportacionCarpetas/ServicioRecepcionImportacion.vb`. Origen: D-05, RQ-05. Verificación: consulta/reenvío íntegro no duplica commit; intento rechazado no se reutiliza ciegamente.
- [x] 3.8 [M] Comprobar carrera física entre dos receptores. Área/archivos: `Infrastructure/ProduccionDocumental/ImportacionCarpetas/AlmacenStagingImportacion.vb`. Origen: D-06, RQ-06. Verificación: el bloqueo permite una escritura y preserva resultados inciertos sin liberación por timeout.
- [x] 3.9 [M] Verificar barreras congeladas sin ejecutar fases documentales. Área/archivos: `tests/ImportacionCarpetasBehaviorTests.cs`. Origen: D-07, RQ-07. Verificación: EXPEDIENTE/LOTE esperan los recibidos exigidos; parcial conserva rechazados e inventario.

## Aceptación automatizada existente y extensiones mínimas

- [x] 4.1 [S] Registrar inventario de infraestructura E2E reutilizable. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc115-recepcion-plan.md`. Origen: D-08, RQ-08. Verificación: se identifican utilidades existentes y extensiones mínimas sin login ni configuración paralelos.
- [x] 4.2 [M] Añadir soporte de recepción que delegue sesión y controles existentes. Área/archivos: `tools/e2e/tests/support/doc115-recepcion-importacion.cjs`. Origen: D-08, RQ-08. Verificación: pruebas locales verifican lista de operaciones sin AvanzarLote y SELECT exclusivos.
- [x] 4.3 [S] Definir fixtures sintéticas y presupuesto de recursos por escenario. Área/archivos: `tools/e2e/fixtures/doc115-recepcion-cases.json`. Origen: D-08, RQ-08. Verificación: validador rechaza configuración incompleta sin usar datos de perfiles reales fijos.
- [x] 4.4 [M] Añadir especificación automatizada RECIB-01. Área/archivos: `tools/e2e/tests/doc115-recepcion-importacion.spec.cjs`. Origen: D-02, RQ-02. Verificación: doble confirmación y payload distinto comprueban identidad y snapshot con limitación de sesión declarada.
- [x] 4.5 [M] Añadir especificación automatizada RECIB-02. Área/archivos: `tools/e2e/tests/doc115-recepcion-importacion.spec.cjs`. Origen: D-04, RQ-04. Verificación: rechazos de integridad/autoridad y replay verifican bytes, cuota y cero efectos documentales.
- [x] 4.6 [M] Añadir especificación automatizada RECIB-03. Área/archivos: `tools/e2e/tests/doc115-recepcion-importacion.spec.cjs`. Origen: D-05, RQ-05. Verificación: corte de transferencia y respuesta perdida consultan estado antes de reenvío y conservan evidencia.
- [x] 4.7 [S] Registrar comando delgado dentro del paquete E2E existente. Área/archivos: `tools/e2e/scripts/run-doc115-recepcion-importacion.cjs`. Origen: D-08, RQ-08. Verificación: el runner delega autorización/sesión y no modifica workers/reintentos/timeouts globales.
- [x] 4.8 [M] Validar localmente selección, controles y saneamiento del soporte. Área/archivos: `tests/doc115-recepcion-contracts.test.cjs`. Origen: D-08, RQ-08. Verificación: node --test y node --check reales pasan sin autenticar ni generar documentos.

## Integración y cierre por fase

- [x] 5.1 [M] Ejecutar integración autorizada de repositorios 07-A. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc115-07a-resultados.md`. Origen: D-09, RQ-09. Verificación: informe registra SQL revisado, concurrencia y rollback reales o NO_EJECUTADO con causa.
- [x] 5.2 [M] Ejecutar RECIB-01/02/03 bajo autorización específica. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc115-07b-resultados.md`. Origen: D-09, RQ-09. Verificación: cada escenario acredita HTTP/SELECT, recursos preservados y cero creación o almacenamiento.
- [x] 5.3 [M] Ejecutar regresión local de consumidores afectados. Área/archivos: `tests/doc110-perfil-importacion-behavior.test.cjs`. Origen: D-07, RQ-07. Verificación: harness Framework y suites pertinentes conservan preparación/carga individual; MSBuild real si cambia VB/proyecto.
- [x] 5.4 [S] Consolidar evidencia y decisión de cierre separada por fase. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc115-recepcion-plan.md`. Origen: D-09, RQ-09. Verificación: matriz D/RQ/casos enlaza SHA/ambiente/comandos y no equipara dobles con integración real.
