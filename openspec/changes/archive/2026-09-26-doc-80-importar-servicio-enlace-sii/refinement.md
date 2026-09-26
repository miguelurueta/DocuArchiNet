<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-80-importar-servicio-enlace-sii

## Fuente y alcance

- Ticket: `DOC-80` — IMPORTAR-SERVICIO-ENLACE-SII.
- Cambio: `doc-80-importar-servicio-enlace-sii`.
- Perfil tecnológico: ASP.NET WebForms/VB.NET legacy con infraestructura moderna aditiva JavaScript y HTTP.
- Alcance: capacidad, consulta y preview no mutadores de anexos SII durante la preasignación de una actividad `ENLASE`.
- Fuera de alcance: preparación, almacenamiento, reconciliación, asignación de tarea y retiro del recorrido legacy.

## Contexto inspeccionado

- `Doc/Actualizacion/workflow/ImportarServiciWebEnlace/exploracion/modernizacion-importacion-anexos-sii-enlase.md` y Prompt 01.
- `workflow/Classselecciotarea.vb`: activa el administrador documental antes de asignar cuando `TipoActividad = "ENLASE"` y conserva `ID_TAREA_SELECCIONDA_ENLACE`.
- `ServiciosIntegracion/ClassAdjuntaDocumentoServicioIntegracion.vb`: valida permiso, tarea, ruta, trámite y servicio configurado.
- `Integracionccv/ClassListaAnexosSII.vb`: reconstruye código de barras, recibo y gabinete y consulta `ConsultarRadicado_sii.imagenes`.
- `Integracionccv/Class_ConsultarRadicado_sii.vb`: obtiene token con `solicitarToken` y consulta `consultarRadicado`.
- `Integracionccv/Class_lista_imagenes_sii.vb`: cada imagen contiene `idanexo`, formato, tipo, observaciones, URL, datos registrales y fecha.
- `workflow/ClassAlmacenamiento.vb`: el legacy descarga directamente `CDlistaAnexosSII.url`; se conserva solo como caracterización, no como autoridad moderna.
- `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiImportProvider.vb` y `SiiExternalImportProviderClient.vb`: proveedor `INTEGRACIONSII`, autenticación y transporte modernos.
- `Services/Workflow/ImportarServicioWeb/RegistroClientesProveedoresImportacion.vb`: rechaza identidades canónicas duplicadas.
- Infraestructura moderna de preview, telemetría y autorización de `ImportarServicioWeb`.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Mantener un solo proveedor `INTEGRACIONSII` y publicar `ANEXOS_RADICADO_ENLASE` como capacidad explícita. | `SiiImportProvider.CanonicalProviderId`; registro rechaza duplicados. | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Reconstruir en servidor el contexto de preasignación y rechazar tareas no operables o cuya actividad no sea `ENLASE`. | `Classselecciotarea.vb`; `ClassListaAnexosSII.vb`; autorización moderna. | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Consultar `consultarRadicado`, mapear `imagenes` y usar `idanexo` como identidad; vacíos y duplicados son respuesta inválida. | `ConSultarRadicado`; `Class_lista_imagenes_sii.idanexo`. | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Resolver preview por proveedor, capacidad e `idanexo`; reconsultar y mediar la URL mediante descriptor/stream seguro. | `imagenes.url`; infraestructura moderna de preview. | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Mantener consulta y preview sin mutaciones y registrar telemetría separada por operación. | `IExternalServiceAttemptRecorder`; repositorio de telemetría. | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Implementar de forma aditiva, sin modificar almacenamiento ni recorridos legacy y sin regresión de constancias. | Prompt 01; `ClassAlmacenamiento`; endpoints legacy. | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | Validar con fixtures y preparar E2E real de lectura/preview reutilizando `tools/e2e`; ejecutarla exige autorización. | Fixtures existentes; `tools/e2e`; `AGENTS.md`. | D-07 | RQ-07 | Origen: D-07, RQ-07 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | La capacidad se resuelve bajo `INTEGRACIONSII`. | Capacidad conocida se informa; proveedor/capacidad desconocidos fallan de forma segura. | No duplicar proveedor ni autenticación. |
| RQ-02 | Solo una preasignación `ENLASE` vigente puede consultar. | Contexto válido continúa; actividad, tarea, ruta o trámite inconsistentes se rechazan sin llamada SII. | Evitar contaminación entre tareas. |
| RQ-03 | Cero, uno o varios anexos se normalizan con identidad estable. | Lista nula/vacía queda vacía; `idanexo` único crea item; vacío/duplicado falla de forma segura. | No usar URL, posición o nombre como clave. |
| RQ-04 | Preview usa descriptor temporal y streaming mediado. | Cliente entrega `idanexo`, nunca URL; descriptor vencido o ajeno se rechaza. | SSRF y fuga de URL/token. |
| RQ-05 | Consulta/preview no alteran negocio y generan observabilidad segura. | Controles no cambian; telemetría distingue operaciones sin secretos. | Preservar auditoría funcional. |
| RQ-06 | Constancias y legacy conservan comportamiento. | Suites existentes pasan; no cambian almacenamiento, endpoints ni handlers legacy. | Rollback y compatibilidad. |
| RQ-07 | La entrega incluye pruebas reproducibles. | Unitarias/integración sin red pasan; E2E queda preparada y solo se ejecuta autorizada. | No inventar evidencia. |

## Resultado del refinamiento

- Estado: aprobado para implementar el alcance no mutador de DOC-80.
- La identidad se fundamenta en `idanexo`; valores vacíos o duplicados deben detener la respuesta con error contractual, no fabricar claves.
- Las decisiones D-01 a D-07 quedan trazadas en design, spec y tasks.
