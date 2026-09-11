<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-56-pruebas-integracion

## Fuente y alcance

- Ticket: `DOC-56` — PRUEBAS-INTEGRACION.
- Perfil: pruebas Node.js sobre backend ASP.NET WebForms/ASMX en VB.NET, validación PowerShell y preparación Playwright en `tools/e2e`.
- Alcance: consolidar pruebas B01–B06, regresión arquitectónica y legacy, validación local, E2E autorizado y corregir la brecha productiva SII demostrada por ese E2E.

## Contexto inspeccionado

- Suites y fixtures en `Tests/importar-servicio-web-*.test.cjs` y `Tests/Fixtures/Workflow/ImportarServicioWeb/`.
- Código moderno en `DTOs/`, `Modelo/`, `Services/`, `Infrastructure/` y `webservice/WebServiceImportarServicioWebModern.asmx.vb`.
- Fronteras legacy en `workflow/ClassAlmacenamiento.vb`, `js/java_general/JSProgresBar.js` y los ASMX históricos.
- Infraestructura E2E en `tools/e2e/`; su runbook no autoriza autenticación, mutaciones ni gates.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Consolidar la pirámide local reutilizando suites y fixtures B01–B06, sin duplicarlos ni crear otro arnés. | `Tests/` y `Tests/Fixtures/Workflow/ImportarServicioWeb/` | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Cubrir contratos, seguridad, concurrencia, idempotencia, estados, fallos y reconciliación sin red ni secretos. | `Services/Workflow/ImportarServicioWeb/`, `Infrastructure/Workflow/ImportarServicioWeb/` | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Proteger que JavaScript no ejecute la importación, que el adaptador sea la única llamada nueva a `AlmacenaDocumentoTareaWorkflow(...)` y que su implementación no cambie. | `JSProgresBar.js`, `LegacyImportDocumentStorageAdapter.vb`, `ClassAlmacenamiento.vb` | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Validar las ocho operaciones modernas y `FEATURE_DISABLED` sin efectos con el gate apagado, preservando rutas legacy. | `WebServiceImportarServicioWebModern.asmx.vb`, fixtures `contracts-v1` | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Proveer un único runner local determinista, no autenticado y sin red en `tools/validation/Verify-ImportarServicioWebModern.ps1`. | `tools/validation/` y `Tests/` | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Preparar E2E sólo en `tools/e2e`, sin ejecutarlo, y separar evidencia local de evidencia autorizada. | `tools/e2e/AGENT-RUNBOOK.md`, `tools/e2e/tests/` | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | Publicar las cinco operaciones faltantes con permiso `ADJUNTAR_IMAGENES_PREDETERMINADA`; resolver infraestructura en servidor y traducir el ID TRD público al único ID contextual de lista de chequeo antes del legacy. | `InicioWorkflow.vb`, `Class_permisos_usuarios_workflow.vb`, `ra_dig_tipos_docum_lista_chequeo.vb`, `WebService_integracion_sii.asmx.vb` | D-07 | RQ-07 | Origen: D-07, RQ-07 |
| D-08 | Sustituir las rutas genéricas de DOC-55 por token + `consultarInformacionSello`, aplanar `inscripciones[].imagenes[]` conservando libro, registro e `idanexo`, y resolver la URL exclusivamente en servidor para preview/descarga. | `Class_consultarInformacionSello.vb`, `Class_ra_sii_migra_imagenes.vb`, adaptador moderno SII | D-08 | RQ-08 | Origen: D-08, RQ-08 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Cinco suites canónicas reutilizan fixtures existentes. | No aparecen fixtures, configuración ni arneses paralelos. | Evita divergencia con B01–B06. |
| RQ-02 | Éxito, rechazo, timeout, cancelación, carrera, reintento y fallo producen estados esperados. | Las pruebas pasan sin servicios externos, secretos ni mutaciones. | Los dobles locales no son evidencia de ambiente. |
| RQ-03 | El diff falla ante cambios de almacenamiento legacy, llamadas adicionales o efectos iniciados desde JavaScript. | Tres invariantes quedan caracterizadas automáticamente. | Preserva rutas y evita doble persistencia. |
| RQ-04 | Ocho operaciones usan contratos compartidos y gate apagado retorna `FEATURE_DISABLED` sin efectos. | Fixtures y regresión ASMX comprueban forma y ausencia de invocaciones. | Rollback por gate; endpoints legacy intactos. |
| RQ-05 | Un comando local ejecuta la suite y propaga fallos. | El script no autentica ni accede a red y retorna código no cero al fallar. | No confundir validación local con E2E. |
| RQ-06 | E2E queda preparado y la evidencia distingue ejecución local, ejecución autorizada y prueba no realizada. | Sin autorización no hay sesión, gate ni evidencia ficticia. | Una corrida futura restaura gate, usuarios y grupos. |
| RQ-07 | Preflight, creación, ejecución, consulta y reconciliación quedan publicadas con composición productiva fail-closed. | Permiso 13 vigente, tarea/contexto coincidentes; el ID TRD y nombre enviados se validan y traducen a una única relación de checklist en Radicación. | Preserva `ClassAlmacenamiento.vb` y evita autoridad o IDs contextuales inyectados por navegador. |
| RQ-08 | Consulta, descarga e índices usan el contrato SII real `consultarInformacionSello` y la semántica legacy de almacenamiento, no las rutas `items`, `preview` o `resource`. | Pruebas verifican token, POST, aplanado de imágenes, unicidad, descarga y matriz `mercantil`/`rup`/`esal`, separando código de barras y recibo. | Bloquea promoción de una implementación que sólo funciona con fixtures inventados o guarda índices incorrectos. |

## Reglas de trazabilidad obligatorias

1. Cada D-XX aparece en design, spec y una tarea con `Origen: D-XX, RQ-XX`.
2. DOC-56 puede corregir la composición moderna de D-07 y el adaptador productivo moderno de D-08; `ClassAlmacenamiento.vb`, ASMX legacy y `JSProgresBar.js` son de solo lectura.
3. No se ejecutan E2E, carga, SII, autenticación, consultas mutantes ni gates sin autorización explícita.
4. La evidencia nunca contiene credenciales, cookies, tokens, cadenas de conexión o respuestas externas crudas.

## Resultado del refinamiento

- Estado: aprobado.
- Alcance, decisiones, compatibilidad, trazabilidad y límites operativos definidos.
