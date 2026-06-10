# Auditoria de brechas legacy vs requisitos

## Objetivo

Registrar los comportamientos detectados en los archivos legacy que no estaban cubiertos o estaban cubiertos de forma incompleta en los modelos iniciales de requisitos.

Archivos auditados:

- `docs\Architecture\AppUploadDocumental\legacy\FileUploadHandler.legacy.js`
- `docs\Architecture\AppProgressBatch\legacy\JSProgresBar.legacy.js`

## Brechas detectadas y decision

### 1. Guardar archivo individual

El legacy permite guardar todos los archivos y tambien guardar un archivo puntual desde su fila.

Evidencia:

```txt
_EventEnviarArchivosServer
_EventEnviarArchivoServer
```

Decision:

- Agregar `allowSingleFileStore`.
- Agregar requisito de accion por archivo.

Cubierto en:

- `RF-UD-03A`

### 2. Fecha por archivo

El legacy permite capturar fecha por documento cuando `CargaFecha == 1`.

Evidencia:

```txt
CargaFecha
element_date_{rowId}
FechaCarga
_ValidateFechaFormControlFile
_EsFechaValida
```

Decision:

- Agregar `requiereFechaCarga`.
- Agregar `fechaCarga` a metadata por archivo.
- Validar formato `yyyy-MM-dd`, dia, mes, longitud y ano no futuro.

Cubierto en:

- `RF-UD-07B`
- `RNF-UD-04`

### 3. Extension efectiva por modo documental

El legacy no usa siempre la lista general de extensiones. Algunos modos fuerzan `.TIF` o `.DOCX`.

Evidencia:

```txt
upload_file_config_aceptar
upload_adjunto_doc_visor_event_cheked_adjunto
upload_adjunto_doc_respuesta_event_cheked_adjunto
CONTENT_ESTENSION_PERMITIDA_EXCLUSE = ".TIF"
CONTENT_ESTENSION_PERMITIDA_EXCLUSE = ".DOCX"
```

Decision:

- Agregar `modoDocumento`.
- Resolver extension efectiva desde API de configuracion cuando sea posible.
- Si hay reglas frontend, aislarlas en utilidad testeable.

Cubierto en:

- `RF-UD-07A`

### 4. Archivo invalido visible en cola

En una variante legacy, archivos con tamano invalido pueden mostrarse con advertencia y bloquear guardado, en vez de ser descartados inmediatamente.

Evidencia:

```txt
ErrorPopuver
ElementRow.setAttribute("Error", "1")
No se puede guardar el archivo
```

Decision:

- Agregar politica `validationMode`.
- Soportar `reject` y `queue-with-error`.

Cubierto en:

- `RF-UD-02`

### 5. Metadata documental heredada adicional

El legacy envia mas campos que tipologia y expediente.

Evidencia:

```txt
tipo_adjunta
id_respuesta
evento_adjunta
chek_adjunta_relacionado
chek_adjunta_anexo
num_docu_relacion
gabinete
id_image
id_expediente
name_modulo
```

Decision:

- Ampliar `UploadDocumentalContext`.
- Mapear a nueva API solo lo compatible.
- Exponer datos restantes al consumidor por callbacks o extensiones.

Cubierto en:

- `RF-UD-05A`
- `RF-UD-11`

### 6. Fases visibles de upload

El legacy diferencia visualmente entre carga y guardado.

Evidencia:

```txt
progressHandler
"Cargando..."
"Guardando..."
loadstartEvent
loadenEvent
```

Decision:

- Agregar fases por archivo.
- Permitir que `AppProgressBatch` muestre etiqueta de fase.

Cubierto en:

- `RF-UD-16`
- `RF-PB-13`

### 7. Conteo de archivos

El legacy muestra conteo de archivos cargados/pendientes.

Evidencia:

```txt
Archivo(s) Cargado(s)
CONTEN_NUM_UPLOAD_FILE
CONTEN_NUM_UPLOAD_INCRE_FILE
```

Decision:

- Agregar requisito de conteo.

Cubierto en:

- `RF-UD-17`

### 8. Lista vacia en progress batch

`JSProgresBar` valida lista vacia antes de iniciar.

Evidencia:

```txt
if (this.settings.OptionItemSelect.length == 0)
```

Decision:

- Agregar `emptyMessage`.
- No ejecutar `processItem` sin items.

Cubierto en:

- `RF-PB-02A`

### 9. Advertencias y omitidos en progress batch

El legacy distingue resultados tipo `CTRL` y `CTRLRETURN`. `CTRL` permite continuar registrando elemento no procesado; `CTRLRETURN` pausa y pregunta.

Evidencia:

```txt
ResultadoPeocesing.Value == "CTRL"
ResultadoPeocesing.Value == "CTRLRETURN"
NumeroElmentNoProcess
ErrorElmentNoProcess
```

Decision:

- Agregar `warning`.
- Agregar `skipped`.
- Mantener `controlled-error` para pausa con confirmacion.

Cubierto en:

- `RF-PB-07A`
- `RF-PB-07B`

### 10. Ciclo de vida explicito del batch

El legacy usa `estado_control` con valores numericos.

Evidencia:

```txt
estado_control = 0
estado_control = 1
estado_control = 2
```

Decision:

- Modelar ciclo de vida con estados nominales.

Cubierto en:

- `RF-PB-11`

## Brechas descartadas intencionalmente

### UI dinamica vs estatica legacy

El legacy tiene `TipoFormulario` para crear modal dinamico o enganchar formulario estatico.

Decision:

- No migrar como requisito del nuevo componente.
- `AppUploadDocumental` debe ser embebible por composicion React; la decision modal/panel pertenece al consumidor.

### Callbacks globales por nombre

El legacy despacha comportamiento por `funcion_name` y `CONTENT_NAME_UPLOAD_FUNCION`.

Decision:

- No migrar callbacks por string.
- Reemplazar por `onStored`, `onBatchComplete`, `onError` y callbacks del modulo consumidor.

### HTML/tabla legacy

El legacy renderiza filas y controles manualmente.

Decision:

- No migrar HTML manual.
- La UI se resuelve con React, `AppUpload` y slots/render props si hacen falta.

### XHR legacy y `FormData`

El legacy usa `XMLHttpRequest` y `FormData` hacia `fileuploadhandler_.ashx`.

Decision:

- No migrar.
- Reemplazar por API nueva de upload temporal por chunks y registro final.

