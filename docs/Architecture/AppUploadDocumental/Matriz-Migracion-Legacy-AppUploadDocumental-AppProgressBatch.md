# Matriz comparativa de migracion legacy

## Objetivo

Relacionar las funcionalidades detectadas en `FileUploadHandler.js` y `JSProgresBar.js` con los nuevos componentes propuestos:

- `AppUploadDocumental`
- `AppProgressBatch`
- `AppUpload` existente
- servicios/utilidades de soporte

Esta matriz sirve como guia de alcance para la migracion y evita portar codigo legacy que debe quedar descartado.

## Resumen de componentes destino

| Componente / capa | Responsabilidad |
| --- | --- |
| `AppUpload` | UI base existente para seleccionar archivos, drag and drop, preview, lista, estados y eventos de upload. |
| `AppUploadDocumental` | Adaptador documental: configuracion por API, tipologia por archivo, metadata documental y conexion con almacenamiento. |
| `AppProgressBatch` | Orquestador reusable de procesos secuenciales: progreso global, cancelacion, errores controlados y resumen. |
| `almacenamientoDocumentalUpload.service` | Servicio para upload temporal por chunks, complete, cancel y registro final por archivo. |
| `uploadConfig.service` | Servicio para cargar extensiones, tamano maximo y reglas por proceso. |
| `tipoDocumental.service` | Servicio para cargar tipologias documentales segun contexto. |
| `tipoDocumentalSuggestion.utils` | Utilidad pura para sugerir tipologia desde el nombre del archivo. |
| `storageFile.utils` | Utilidades puras para extension, tamano, chunks, fechas y payloads. |

## Matriz de funcionalidades

| Funcionalidad legacy | Codigo legacy / senal | Destino nuevo | Estado | Descripcion de migracion |
| --- | --- | --- | --- | --- |
| Seleccion de archivos | `file_element_*`, `change`, `DataTransfer` | `AppUpload` | Migrar por reemplazo | Usar el control existente en vez de manipular `input.files` manualmente. |
| Seleccion multiple | `multi_select: "multiple"`, `CONTENT_SELECT_FILE_UPLOAD` | `AppUploadDocumental` + `AppUpload` | Migrar | La configuracion desde API define si se permite multiple seleccion; `AppUpload` aplica `maxCount`. |
| Drag and drop | `_EventDropArchivo`, `drop_Upload_file`, `dragover` | `AppUpload` | Migrar por reemplazo | Usar soporte `drag` de `AppUpload`, sin listeners DOM manuales. |
| Validacion de extension | `ValidateFileType`, `CONTENT_ESTENSION_PERMITIDA` | `uploadConfig.service` + `AppUpload.accept` | Migrar | Las extensiones vienen de API y se aplican como validacion preventiva. |
| Validacion de tamano | `CONTENT_MAXIMO_TAMANO_FILE_BYTE_UPLOAD` | `uploadConfig.service` + `AppUpload.maxSize` | Migrar | El tamano maximo viene de API y se valida antes de subir. |
| Archivo invalido visible con advertencia | `ErrorPopuver`, `Error="1"` | `AppUploadDocumental.validationMode` | Migrar opcional | Soportar `reject` o `queue-with-error` segun UX del proceso. |
| Conteo de archivos | `Archivo(s) Cargado(s)` | `AppUploadDocumental` | Migrar | Exponer conteo de seleccionados, pendientes, exitosos y con error. |
| Eliminar archivo individual | `_EventDeleteFile`, `delete_file_UploadFile` | `AppUpload` / `AppUploadDocumental` | Migrar | Usar `onRemove` y limpiar metadata asociada al `uid`. |
| Eliminar todos | `_EventDeleteAllArchivo`, `delete_file_all_UploadFile` | `AppUploadDocumental` | Migrar | Accion de limpiar cola y metadata por archivo. |
| Preview de archivo | `CargaPreview`, `_EventVisualizaFile`, `_CrearVistaVisualizaFile` | `AppUpload` | Migrar por reemplazo | Usar preview nativo/renderizado de `AppUpload`; ampliar solo si faltan formatos. |
| Icono por tipo de archivo | `obtenerIconoPorTipo`, `icono_icono_awe_some` | `AppUpload` render slots | Migrar opcional | Mantener fallback visual por extension si el producto lo requiere. |
| Tipologias documentales | `_ServiceRESTListTiposDocumentalesFile`, `CONTENT_ITEM_ROW_TIPO` | `tipoDocumental.service` | Migrar | Cargar opciones desde API moderna segun proceso y contexto. |
| Tipologia por archivo | `element_input_{rowId}`, `IdTipoDocumento` | `AppUploadDocumental` | Migrar | Cada archivo mantiene metadata independiente por `uid`. |
| Tipologia obligatoria | `option_obliga_tipologia`, `setioption_obliga_tipologia` | `AppUploadDocumental.tipologiaObligatoria` | Migrar y corregir | El nuevo requisito corrige el typo legacy y valida consistentemente. |
| Sugerencia de tipologia por nombre | `_BuscaCoinsidenciaEstructura`, `_BuscarCoincidenciaFlexible` | `tipoDocumentalSuggestion.utils` | Migrar | Utilidad pura con coincidencia flexible, minimo configurable. |
| Fecha por archivo | `CargaFecha`, `FechaCarga`, `element_date_{rowId}` | `AppUploadDocumental` + `storageFile.utils` | Migrar | Metadata `fechaCarga` por archivo con validacion `yyyy-MM-dd`. |
| Validacion de fecha | `_ValidateFechaFormControlFile`, `_EsFechaValida` | `storageFile.utils` | Migrar | Funcion pura testeable: formato, dia, mes, ano no futuro. |
| Configuracion por proceso | `evento_adjunta`, `NameLoadProceso`, `Service_parameter_upload` | `uploadConfig.service` | Migrar | Resolver reglas por `proceso` y contexto, no por variables globales. |
| Extension efectiva por modo | `upload_file_config_aceptar`, `.TIF`, `.DOCX` | `uploadConfig.service` / utilidad | Migrar | Preferir que API devuelva reglas; si no, aislar reglas frontend testeables. |
| Modo formulario dinamico | `TipoFormulario: 1`, `_CreaModalFile` | Consumidor React | Descartar | La decision modal/panel pertenece a la pantalla consumidora. |
| Modo formulario estatico | `TipoFormulario: 2`, `InitUploadFileClientAsync` | Composicion React | Descartar como legacy | React integra por props y composicion, no engancha IDs existentes. |
| Guardar todos | `_EventEnviarArchivosServer` | `AppUploadDocumental` + `AppProgressBatch` | Migrar | Procesar cola secuencialmente, con resumen final. |
| Guardar archivo individual | `_EventEnviarArchivoServer` | `AppUploadDocumental.allowSingleFileStore` | Migrar | Permitir procesar solo un archivo si el flujo lo requiere. |
| Progreso global batch | `JSProgresBar`, `CONTEN_NUM_UPLOAD_INCRE_FILE` | `AppProgressBatch` | Migrar | Progreso `x de y`, porcentaje global, item actual. |
| Progreso interno archivo | `progressHandler`, bytes cargados | `AppUpload` + servicio chunks | Migrar | Progreso por chunks y fases visibles. |
| Fase Cargando/Guardando | `"Cargando..."`, `"Guardando..."` | `AppUploadDocumental` + `AppProgressBatch` | Migrar | Modelar fases: `uploading`, `completing`, `storing`. |
| Cancelar upload actual | `ajax.abort`, `cancel_file_all_upload` | `AbortController` + API cancel | Migrar | Abort local y `DELETE upload-temporal` si hay upload temporal activo. |
| Bloquear cierre durante carga | `hide_upload_content` | `AppProgressBatch` | Migrar | Cierre durante ejecucion pasa por politica de cancelacion. |
| Error controlado continuar/cancelar | `CTRLRETURN`, `_ModalShowConfirm` | `AppProgressBatch.controlled-error` | Migrar | Pausar y pedir decision sin acoplarse al dominio. |
| Advertencia no bloqueante | `CTRL` | `AppProgressBatch.warning` | Migrar | Registrar advertencia y continuar. |
| Items omitidos/no procesados | `NumeroElmentNoProcess` | `AppProgressBatch.skipped` | Migrar | Contabilizar omitidos en resumen. |
| Lista vacia | `OptionItemSelect.length == 0` | `AppProgressBatch.emptyMessage` | Migrar | No iniciar proceso sin items. |
| Upload legacy `.ashx` | `fileuploadhandler_.ashx`, `FormData` | `almacenamientoDocumentalUpload.service` | Reemplazar | Usar API nueva por chunks y registro final JSON. |
| Registro final legacy por callback | `_RegistraArchivoInterfaz`, `insert_runtHXR` | `onStored`, `onBatchComplete` | Reemplazar | Consumidor actualiza tablas/listados con callbacks tipados. |
| Retornos variables para interfaz | `insert_row_producion_documental`, `insert_row_documento_relacionado`, `adjunta_migra_documento`, `actualiza_contador_imagen`, `actualiza_semaforo_respuesta`, `actualiza_drowp_respuesta`, `insert_new_versio_document`, `Show_row_table_boot_rue` | `UploadDocumentalInterfaceRegistration` | Migrar como contrato tipado | Convertir el resultado legacy por `funcion_name` en eventos discriminados; no concatenar strings ni mutar DOM desde el componente. |
| `funcion_name` / callbacks string | `CONTENT_NAME_UPLOAD_FUNCION` | Callbacks React | Descartar | No migrar dispatch por strings globales. |
| Metadata `tipo_adjunta` | `formdata.append("tipo_adjunta")` | `UploadDocumentalContext` | Migrar si aplica | Campo disponible en contexto; mapear solo si API/flujo lo requiere. |
| Metadata `id_respuesta` | `formdata.append("id_respuesta")` | `UploadDocumentalContext.idRespuesta` | Migrar si aplica | Mantener para flujos de respuesta o callbacks. |
| Estado adjunto/relacionado | `chek_adjunta_anexo`, `chek_adjunta_relacionado` | `UploadDocumentalContext` | Migrar si aplica | Reglas de negocio fuera del componente base. |
| Numero documento relacionado | `num_docu_relacion` | `UploadDocumentalContext.numeroDocumentoRelacionado` | Migrar si aplica | Mantener como metadata de contexto. |
| Gabinete | `gabinete`, `name_gabinete` | `context.nombreGabinete` | Migrar | Campo requerido por API nueva. |
| Expediente | `id_expediente` | `context.idExpediente` | Migrar | Mapear a `expediente.idExpediente`. |
| Workflow | `CONTENT_ID_TAREA_WORKFLOW`, `id_tarea_workflow` | `context.idTareaWorkflow` | Migrar | Mapear a `workflow.idTareaWorkflow`. |
| Imagen/documento existente | `id_image` | `context.idImagen` / consumidor | Migrar si aplica | Puede requerir flujo de nueva version/reemplazo fuera del upload base. |
| Modulo | `name_modulo` | `context.nameModulo` | Migrar si aplica | Sirve como discriminador de flujo, no como dependencia global. |
| Spinner global | `_MostrarLoading`, `_OcultarLoading` | Estados React / AppLoadingState | Reemplazar | Usar estado de loading integrado, sin overlay DOM manual. |
| Popovers Bootstrap | `data-bs-toggle="popover"` | UI React/AntD | Reemplazar | Mensajes de error por archivo en componentes React. |
| UpdatePanel ASP.NET | `element_update_panel`, `.click()` | Callback consumidor | Descartar | No hay WebForms; usar actualizacion de estado/query invalidation. |
| Manipulacion directa de tabla | `insert_row_*`, `insert_row_table` | Callback consumidor | Reemplazar | El modulo dueño actualiza su tabla/listado. |

## Lectura arquitectonica

La migracion no es una traduccion 1:1. Se divide asi:

```txt
FileUploadHandler.js
→ AppUploadDocumental
→ servicios de almacenamiento/configuracion/tipologia
→ utilidades puras

JSProgresBar.js
→ AppProgressBatch
```

El nuevo diseno conserva las capacidades de negocio que siguen siendo relevantes, pero descarta los mecanismos legacy de integracion DOM, WebForms y jQuery.
