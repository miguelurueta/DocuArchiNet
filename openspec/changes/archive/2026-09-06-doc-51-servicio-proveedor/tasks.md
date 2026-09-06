# Tareas atómicas — DOC-51

## 1. Contratos

- [x] 1.1 [M] Declarar el puerto asíncrono y cancelable para capacidades, consulta, preview y descarga. Área/archivos: `Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebInterfaces.vb`. Origen: D-01, RQ-01. Verificación: cuatro firmas retornan `Task`, reciben `CancellationToken` y el puerto existe una vez.
- [x] 1.2 [S] Confirmar reutilización de DTO v1 y preview sin URL, token, ruta ni payload crudo. Área/archivos: `Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebInterfaces.vb`, `DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb`. Origen: D-07, RQ-07. Verificación: prueba focal y diff sin cambio incompatible público.

## 2. Solicitud y fábrica

- [x] 2.1 [M] Implementar descriptor inmutable de solicitud y política por operación. Área/archivos: `Infrastructure/Workflow/ImportarServicioWeb/Http/ExternalImportHttpTransport.vb`. Origen: D-02, RQ-02. Verificación: pruebas rechazan invariantes inválidos y mutación de colecciones fuente.
- [x] 2.2 [M] Construir mensajes preservando método, encabezados, bytes y media type. Área/archivos: `Infrastructure/Workflow/ImportarServicioWeb/Http/ExternalImportHttpTransport.vb`. Origen: D-02, RQ-02. Verificación: loopback observa JSON y formulario exactos.
- [x] 2.3 [M] Implementar fábrica inyectable con cliente estable y TLS predeterminado. Área/archivos: `Infrastructure/Workflow/ImportarServicioWeb/Http/ExternalImportHttpClientFactory.vb`. Origen: D-03, RQ-03. Verificación: sin mutación de defaults/callbacks y políticas concurrentes aisladas.

## 3. Transporte y validación

- [x] 3.1 [M] Enviar y leer asíncronamente con token externo y timeout enlazado. Área/archivos: `Infrastructure/Workflow/ImportarServicioWeb/Http/ExternalImportHttpTransport.vb`. Origen: D-04, RQ-04. Verificación: loopback distingue cancelación y timeout y no hay esperas bloqueantes.
- [x] 3.2 [M] Validar status, media type y longitud declarada antes del cuerpo. Área/archivos: `Infrastructure/Workflow/ImportarServicioWeb/Http/ExternalImportHttpResponseValidator.vb`. Origen: D-05, RQ-05. Verificación: rechaza 4xx/5xx, MIME inesperado y longitud excedida.
- [x] 3.3 [M] Leer stream por bloques con cancelación/límite y deserializar después. Área/archivos: `Infrastructure/Workflow/ImportarServicioWeb/Http/ExternalImportHttpResponseValidator.vb`. Origen: D-05, RQ-05. Verificación: corta chunked sobredimensionado y no produce resultado parcial con JSON inválido.
- [x] 3.4 [M] Mapear cinco categorías de error a mensajes saneados. Área/archivos: `Infrastructure/Workflow/ImportarServicioWeb/Http/ExternalImportHttpErrorMapper.vb`. Origen: D-06, RQ-06. Verificación: prueba tabular confirma código/correlationId y ausencia de datos sensibles.

## 4. Integración y pruebas

- [x] 4.1 [S] Registrar una vez los cuatro archivos HTTP. Área/archivos: `GestionDocumental-Docuarchi.net.vbproj`. Origen: D-08, RQ-08. Verificación: una entrada `Compile` por archivo.
- [x] 4.2 [S] Crear cinco fixtures HTTP v1 saneados. Área/archivos: `Tests/Fixtures/Workflow/ImportarServicioWeb/http-v1/*.json`. Origen: D-08, RQ-08. Verificación: parsean donde aplica y no contienen secretos/hosts/datos reales.
- [x] 4.3 [M] Probar contrato exacto con servidor loopback. Área/archivos: `Tests/importar-servicio-web-http-contract.test.cjs`. Origen: D-02, RQ-02. Verificación: `node --test Tests/importar-servicio-web-http-contract.test.cjs` pasa.
- [x] 4.4 [M] Probar configuración, TLS, preview y saneamiento. Área/archivos: `Tests/importar-servicio-web-http-security.test.cjs`. Origen: D-03, RQ-03. Verificación: prueba focal pasa.
- [x] 4.5 [M] Probar cancelación, timeout, contenido inválido y límite real. Área/archivos: `Tests/importar-servicio-web-http-cancellation.test.cjs`. Origen: D-04, RQ-04. Verificación: prueba focal pasa.

## 5. Validación

- [x] 5.1 [M] Ejecutar pruebas focales DOC-50/DOC-51. Área/archivos: `Tests/importar-servicio-web-*.test.cjs`. Origen: D-08, RQ-08. Verificación: `node --test Tests/importar-servicio-web-*.test.cjs` pasa.
- [x] 5.2 [S] Auditar I/O bloqueante, callbacks y datos sensibles. Área/archivos: diff DOC-51. Origen: D-06, RQ-06. Verificación: búsquedas focales limpias en archivos nuevos.
- [x] 5.3 [M] Compilar el proyecto VB.NET disponible. Área/archivos: `GestionDocumental-Docuarchi.net.vbproj`. Origen: D-08, RQ-08. Verificación: build exitoso o limitación ambiental documentada.

## 6. Documentación y cierre

- [x] 6.1 [M] Crear paquete `00-Indice.md` a `07-Metadata.md`. Área/archivos: `Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-51-clientes-http-asincronos-seguridad/`. Origen: D-08, RQ-08. Verificación: ocho documentos enlazados y coherentes con código.
- [x] 6.2 [S] Crear diagramas de componentes, secuencia, cancelación y validación. Área/archivos: paquete DOC-51 `Diagramas/`. Origen: D-08, RQ-08. Verificación: enlaces resuelven.
- [x] 6.3 [S] Registrar exclusiones y no regresión sin SII, E2E ni gate. Área/archivos: `05-PruebasEvidencia.md`, diff. Origen: D-08, RQ-08. Verificación: legacy, endpoints, almacenamiento y gate intactos.
- [x] 6.4 [S] Validar OpenSpec y sincronizar refinamiento OPSXJ. Área/archivos: cambio DOC-51. Origen: D-01, RQ-01. Verificación: validación estricta y `opsxj:refine DOC-51` exitosos.
