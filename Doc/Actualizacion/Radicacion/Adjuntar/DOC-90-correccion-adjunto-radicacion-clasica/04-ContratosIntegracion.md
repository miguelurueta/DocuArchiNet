# DOC-90 — Contratos e integraciones

## Contratos de radicación

| Origen | Evento | Identidad autoritativa | Firma | Preparación | Resultado |
|---|---|---|---:|---|---|
| Radicación Entrante | `ADJUNTARADICACION_CLASICA` | Módulo, plantilla y `RA_ID_REGISTRO_RADICADO` de sesión; `stru_registro_estado` de servidor | 10 | `PreAlmacenaDocumentosRadicacion` | `uploadFiles` y fila relacionada |
| Radicación Simplificada | `ADJUNTARADICACION` | ID explícito, usuario y repositorio DOC-85 | 12 | `PreAlmacenaDocumentosRadicacionConContexto` | `uploadFiles` y fila de Simplificada |

Ambas ramas devuelven el contrato existente de `uploadFiles`. La clásica completa `name_gabinete`, `id_image`, `radicado`, `tipodocumental`, `notitipodocumental`, `id_tarea_workflow`, firma, contador, icono, registro, fecha, alias y nombre de archivo.

## Inventario de no regresión del handler

| Evento | Módulo/cliente verificado | Operación o firma | Sesión relevante | Respuesta/evidencia |
|---|---|---|---|---|
| `GESTION_PQRS` | PQRS | Temporal, sin `UploadSaveFile` | No cambia | Ruta y `YES` |
| `ADJUNTAVERSION` | `gestion_version_documento.js` | `AdjuntaVersionDocumento` | Usuarios existentes | Detalle de versión |
| `REMPLAZAVERSION` | `gestion_version_documento.js` | `AdjuntaVersionDocumento` | Usuarios existentes | Detalle de versión |
| `INTRUESII` | `WebFormGestionFlujoTrabajoCamaras.js` | `Solicita_lista_archivo_sii_rue` | No cambia | Filas SII |
| `INTVIRTUALSII` | `WebFormGestionFlujoTrabajoCamaras.js` | `Solicita_lista_archivo_virtual_sii` | No cambia | Filas SII virtual |
| `MIGRACION` | Migración documental | `Adjunta_documento_migracion` | Usuario Gestión | Registro y URL |
| `GESTION_RESPUESTA` | Gestión de respuestas | `UploadSaveFile(10)` | Ruta temporal | Contrato documental |
| `WORKFLOWSELECCION` | `Webworkflow.js` | `UploadSaveFile(10)` | `WF_TIPO_ADJUNTA=LISTA` | Contrato documental |
| `WORKFLOWENLACE` | `Webworkflow.js` | `UploadSaveFile(10)` | `WF_TIPO_ADJUNTA=ENLACE` | Contrato documental |
| `ADJUNTARADICACION_CLASICA` | Radicación Entrante | `UploadSaveFile(10)` | Tipo clásico, ruta y registro | Contrato documental completo |
| `ADJUNTARADICACION` | Radicación Simplificada | `UploadSaveFile(12)` | Tipo Simplificada | Contrato DOC-85 |
| `PRODUCCION` | `WebFormProducionDocumental.js` | `UploadSaveFile(10)` | `WF_TIPO_ADJUNTA=PRODUCCION` | Contrato DOC-88 |
| `SUBE_RESPUESTA` | Correspondencia | Métodos `Classgestionrespuesta` | Ruta de respuesta | Radicado, imagen y semáforo |
| `SUBE_ANEXO` | Correspondencia | `upload_subir_anexo_a_la_respuesta` | Ruta de respuesta | ID y nombre de anexo |
| `RADICA_WORKFLOW` | Radicación/Workflow | `UploadSaveFile(10)` | `WF_TIPO_ADJUNTA=ENLACE_RADICADO` | Contrato documental y versión |

También permanecen inventariadas las llamadas directas de diez argumentos en `workflow/Webworkflow.aspx.vb` y `webservice/WebServiceRadicacion.asmx.vb`. El inventario automatizado exige nueve llamadas totales: ocho legacy y una exclusiva de Simplificada.

No hay cambio de esquema, endpoint, autenticación ni formato JSON.

## Contrato E2E autorizado

El escenario DOC-90 usa la autenticación compartida de `tools/e2e`, selecciona el registro clásico por su `id_estado_radicado` y observa el multipart real. Debe encontrar `evento_adjunta = ADJUNTARADICACION_CLASICA` y no debe encontrar `id_registro_estado_radicacion` ni `radicado_radicacion`, pues esos campos pertenecen exclusivamente a Simplificada. Dos consultas configurables y parametrizadas de solo lectura verifican el contexto previo y el incremento unitario de documentos.

La no regresión funcional completa requiere una corrida separada del escenario DOC-85. No se reutiliza el mismo recurso descartable ni se acredita una corrida sin sus autorizaciones independientes.
