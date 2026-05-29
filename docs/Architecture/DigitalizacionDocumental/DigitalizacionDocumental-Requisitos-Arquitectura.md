# Modelo de requisitos y arquitectura: DigitalizacionDocumental

## Objetivo

Migrar la funcionalidad legacy de digitalizacion documental a una arquitectura moderna React + API, eliminando dependencias WebForms, jQuery, `Session` e invocaciones directas a controles ASP.NET, sin perder las reglas funcionales necesarias del proceso documental.

La nueva solucion debe separar con claridad:

- captura desde escaner;
- generacion de PDF;
- upload temporal;
- resolucion de metadata documental;
- creacion de documentos digitalizados nuevos;
- adjuntar digitalizacion a documentos PDF existentes;
- auditoria y validaciones de bloqueo.

## Contexto legacy analizado

Archivos principales:

```txt
D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net\Resources\online_demo_initpage.js
D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net\Resources\online_demo_operation.js
D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net\workflow\WebFormEscan.aspx
D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net\workflow\WebFormEscan.aspx.vb
D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net\js\workflow\WebFormEscan.js
D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net\workflow\Webform_save_digital_image.aspx
D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net\workflow\Webform_save_digital_image.aspx.vb
```

Servicios legacy relacionados con metadata:

```txt
D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net\webservice\WebServiceGestorDocumental.asmx.vb
D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net\webservice\WebService_radicacion_Simplificada.asmx.vb
D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net\workflow\ClassWorkflowDigitalizacion.vb
D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net\workflow\ClassAlmacenamiento.vb
```

## Responsabilidades legacy identificadas

El legacy no implementa una funcion aislada. Implementa un modulo acoplado que combina:

- inicializacion de Dynamsoft Web TWAIN;
- listado y configuracion de escaneres;
- captura desde escaner o webcam;
- edicion de paginas: rotacion, crop, zoom, deskew, blank-page detection;
- exportacion a PDF, TIF, JPG o BMP;
- upload temporal mediante `Webform_save_digital_image.aspx`;
- reglas de negocio por tipo de digitalizacion;
- resolucion de lista de chequeo y tipologia documental;
- almacenamiento final;
- adjuntar paginas a documento existente;
- reemplazo de version;
- auditoria y actualizacion de indices.

## Decisiones confirmadas

- TIF queda descartado.
- La salida unica de digitalizacion sera PDF.
- Se debe implementar una API nueva para adjuntar digitalizacion a documento existente.
- La digitalizacion tambien debe crear documentos nuevos.
- `DigitalizacionDocumental` sera un modulo reusable e invocable desde otros modulos, no una pantalla aislada.
- Un documento firmado, bloqueado o radicado no modificable bloquea la operacion de adjuntar.
- Existe licencia de Dynamsoft.
- El frontend React no debe acceder directamente a `DWObject`; debe usar un adapter.
- `Webform_save_digital_image.aspx` queda reemplazado por el upload temporal moderno.

## Alcance funcional

Incluye:

- modulo React de digitalizacion;
- contrato de entrada/salida para invocacion desde otros modulos;
- integracion con Dynamsoft mediante adapter;
- captura y generacion PDF;
- upload temporal usando la API moderna;
- consulta de configuracion de digitalizacion;
- consulta de lista de chequeo/tipologias;
- resolucion de metadata TRD desde lista de chequeo;
- creacion de documento digitalizado nuevo;
- adjuntar PDF digitalizado a documento PDF existente;
- validacion de documento firmado, bloqueado o radicado no modificable;
- auditoria de operaciones.

No incluye:

- soporte TIF;
- soporte JPG/BMP como formato final;
- migrar UI WebForms;
- migrar jQuery;
- reutilizar `Session` como contrato funcional;
- hacer que React construya metadata documental sin validacion backend;
- usar `AlmacenarDocumentoRequest` para adjuntar paginas a un documento existente.

## Requisitos funcionales

### RF-00 Modulo invocable por otros modulos

`DigitalizacionDocumental` debe poder desplegarse desde cualquier modulo consumidor de la aplicacion mediante un contrato de entrada/salida.

El modulo consumidor debe poder abrir digitalizacion entregando contexto documental, y `DigitalizacionDocumental` debe devolver el resultado de la operacion para que la interfaz origen pueda refrescar, registrar, completar formularios o continuar su flujo.

Contrato conceptual de entrada:

```ts
type DigitalizacionContext = {
  modo: "crear" | "adjuntar";
  nombreGabinete: string;
  radicado?: string;
  idTramite?: number;
  tipoTramite?: string;
  idTareaWorkflow?: number;
  idRutaWorkflow?: number;
  idDocumentoDestino?: number;
  requiereMetadata?: boolean;
};
```

Contrato conceptual de salida:

```ts
type DigitalizacionResult = {
  accion: "documento-creado" | "documento-adjuntado" | "cancelado";
  idDocumento?: number;
  nombreGabinete?: string;
  rutaTemporalId?: string;
  archivoTemporalId?: string;
  numeroPaginas?: number;
  trd?: unknown;
};
```

Modulos consumidores esperados:

- gestion correspondencia;
- radicacion;
- workflow;
- produccion documental;
- visor/documentos;
- interfaces que requieran datos para registro documental.

### RF-01 Captura de documentos

El sistema debe permitir capturar documentos desde escaner usando Dynamsoft en React.

### RF-02 Formato unico de salida

Toda digitalizacion debe producir un archivo PDF.

### RF-03 Upload temporal

El PDF generado debe subirse usando el mecanismo moderno de upload temporal del backend.

Endpoints backend existentes de referencia:

```txt
POST   /api/gestor-documental/almacenamiento/upload-temporal/init
PUT    /api/gestor-documental/almacenamiento/upload-temporal/{ruta}/{archivo}/chunk/{index}
GET    /api/gestor-documental/almacenamiento/upload-temporal/{ruta}/{archivo}/status
POST   /api/gestor-documental/almacenamiento/upload-temporal/{ruta}/{archivo}/complete
DELETE /api/gestor-documental/almacenamiento/upload-temporal/{ruta}/{archivo}
```

### RF-04 Crear documento digitalizado nuevo

El sistema debe permitir crear un documento nuevo desde una digitalizacion.

Este caso puede apoyarse en `AlmacenamientoDocumental`, pero debe identificar el origen como digitalizacion.

Decision requerida en backend:

```csharp
TipoAlmacenamiento = Digitalizacion
```

### RF-05 Adjuntar digitalizacion a documento existente

El sistema debe permitir anexar un PDF digitalizado a un documento PDF existente.

Este caso no crea un documento nuevo. Modifica el archivo/documento existente agregando paginas.

### RF-06 Resolver metadata documental

El sistema debe exponer una API para resolver metadata documental antes de almacenar o actualizar documentos.

Debe resolver:

- id area;
- nombre area;
- id serie;
- nombre serie;
- id subserie;
- nombre subserie;
- id tipo documental;
- nombre tipo documental;
- obligatoriedad de lista de chequeo;
- regla de unicidad por radicado;
- datos compatibles con `TrdStorageDto`.

### RF-07 Consultar configuracion de digitalizacion

El sistema debe permitir consultar la configuracion aplicable segun contexto.

Entradas esperadas:

- tipo de digitalizacion;
- id tramite;
- tipo tramite;
- radicado;
- nombre gabinete;
- id tarea workflow;
- id ruta workflow.

### RF-08 Validar restricciones de modificacion

Antes de adjuntar digitalizacion a un documento existente, el backend debe validar que el documento pueda modificarse.

Debe bloquear si:

- documento firmado;
- documento bloqueado;
- documento radicado no modificable;
- documento destino no es PDF;
- archivo temporal no es PDF.

### RF-09 Auditoria

Toda operacion debe registrar auditoria.

Eventos minimos:

- documento digitalizado creado;
- digitalizacion adjuntada a documento existente;
- cambio de tipologia/lista de chequeo;
- bloqueo por validacion;
- error controlado.

### RF-10 Compatibilidad con workflow

La digitalizacion debe conservar soporte para contexto workflow:

- id tarea workflow;
- id ruta workflow;
- radicado;
- gabinete;
- tipo tramite;
- documentos adjuntos al tramite.

## Reglas de negocio

### RN-01

El formato TIF queda descartado.

### RN-02

Toda salida de digitalizacion debe ser PDF.

### RN-03

Crear documento digitalizado y adjuntar digitalizacion son casos de uso diferentes.

### RN-04

Adjuntar digitalizacion modifica un PDF existente; no crea un nuevo documento documental.

### RN-05

La lista de chequeo puede ser obligatoria segun configuracion de digitalizacion.

### RN-06

Si la tipologia/lista de chequeo esta marcada como unica, no debe permitirse duplicarla para el mismo radicado/contexto documental.

### RN-07

Un documento firmado, bloqueado o radicado no modificable no puede recibir paginas adjuntas.

### RN-08

El almacenamiento de documentos nuevos digitalizados debe quedar marcado como `Digitalizacion`.

### RN-09

El frontend no debe manipular directamente `DWObject`; debe usar un adaptador de Dynamsoft.

### RN-10

La API debe ser la fuente de verdad para validaciones de metadata, permisos y restricciones documentales.

## Casos de uso

### CU-01 Consultar configuracion de digitalizacion

Obtiene la configuracion inicial necesaria para habilitar el modulo React.

Debe reemplazar la dependencia legacy a:

```txt
Class_ra_dig_config_digitalizacion.Solicita_id_configuracion_digitalizacion
Class_ra_dig_config_digitalizacion.Solicita_datos_configuracion_digitalizacion
```

### CU-02 Consultar lista de chequeo/tipologias

Devuelve las opciones disponibles segun tramite, tipo tramite y configuracion.

Debe reemplazar la dependencia legacy a:

```txt
ra_dig_tipos_docum_lista_chequeo.Solicita_listar_tipos_documentales_relacionados_al_tipo_tramite
WebServiceGestorDocumental.Service_Solicita_lista_sub_series_documentales_id_serie
WebServiceGestorDocumental.Solicita_lista_tipos_documentales_relacionados_sub_serie
```

### CU-03 Resolver metadata documental

Convierte una seleccion de lista de chequeo en metadata documental completa.

Debe resolver valores equivalentes a:

- `ID_TIPODOCUMENTO`;
- `TIPODOCUMENTO`;
- `ID_AREA`;
- `ID_SERIE`;
- `ID_SUB_SERIE`;
- `NOMBRESERIE`;
- `NOMBRESUBSERIE`;
- datos TRD para almacenamiento.

Debe incluir validaciones equivalentes a:

```txt
ClassWorkflowDigitalizacion.Actualiza_tipo_documento_lista_chequeo
```

Sin ejecutar todavia la actualizacion final del documento cuando solo se este resolviendo metadata.

### CU-04 Crear documento digitalizado nuevo

Recibe un PDF temporal y metadata documental resuelta, y crea un nuevo documento.

Debe apoyarse en el caso de almacenamiento documental moderno, incorporando `TipoAlmacenamiento = Digitalizacion`.

### CU-05 Adjuntar digitalizacion a PDF existente

Recibe un PDF temporal y lo anexa al documento destino.

Debe reemplazar la rama legacy:

```txt
ClassWorkflowDigitalizacion.Valida_adjuntar_documento_digitalizado
ClassAñadirDocumento.Añade_documento_digitalizado
ClassAñadirDocumento.Añadir_documento_pdf
```

Alcance moderno:

- PDF-only;
- validacion de documento destino;
- validacion de archivo temporal;
- bloqueo por firma, bloqueo o radicado no modificable;
- union de PDF existente + PDF digitalizado;
- actualizacion de numero de paginas;
- auditoria.

### CU-06 Validar si un documento permite adjuntar digitalizacion

Permite que el frontend consulte anticipadamente si el documento destino soporta append de digitalizacion.

La validacion final siempre debe repetirse en `CU-05`.

## Contratos API propuestos

### Consultar configuracion

```http
GET /api/gestor-documental/digitalizacion/configuracion
```

Query sugerido:

```txt
tipoDigitalizacion
idTramite
tipoTramite
radicado
nombreGabinete
idTareaWorkflow
idRutaWorkflow
```

### Consultar lista de chequeo

```http
GET /api/gestor-documental/digitalizacion/lista-chequeo
```

Query sugerido:

```txt
idTramite
tipoTramite
idConfiguracionDigitalizacion
radicado
nombreGabinete
```

### Resolver metadata documental

```http
POST /api/gestor-documental/digitalizacion/metadata/resolve
```

Request sugerido:

```json
{
  "nombreGabinete": "string",
  "idTipoListaChequeo": 0,
  "idConfiguracionDigitalizacion": 0,
  "radicado": "string",
  "idImagen": 0,
  "validarUnicidad": true
}
```

Response sugerido:

```json
{
  "idTipoListaChequeo": 0,
  "obligaListaChequeo": true,
  "esUnico": false,
  "trd": {
    "idArea": 0,
    "idSerie": 0,
    "idSubSerie": 0,
    "idTipoDocumento": 0,
    "nombreArea": "string",
    "nombreSerie": "string",
    "nombreSubSerie": "string",
    "nombreTipoDocumento": "string"
  }
}
```

### Crear documento digitalizado nuevo

```http
POST /api/gestor-documental/digitalizacion/documentos
```

Request sugerido:

```json
{
  "nombreGabinete": "string",
  "rutaTemporalId": "string",
  "archivoTemporalId": "string",
  "nombreDocumento": "string",
  "requestId": "string",
  "radicado": "string",
  "idTareaWorkflow": 0,
  "idRutaWorkflow": 0,
  "metadata": {
    "idTipoListaChequeo": 0,
    "trd": {}
  }
}
```

### Adjuntar digitalizacion a PDF existente

```http
POST /api/gestor-documental/documentos/{idDocumento}/adjuntar-digitalizacion
```

Request sugerido:

```json
{
  "nombreGabinete": "string",
  "rutaTemporalId": "string",
  "archivoTemporalId": "string",
  "requestId": "string",
  "radicado": "string",
  "idTareaWorkflow": 0,
  "idRutaWorkflow": 0,
  "motivo": "string",
  "moduloRegistro": "DIGITALIZACION",
  "tipologiaDocumental": "string"
}
```

Response sugerido:

```json
{
  "idDocumento": 0,
  "nombreGabinete": "string",
  "numeroPaginas": 0,
  "extension": "pdf",
  "documentoActualizado": true
}
```

### Validar append de digitalizacion

```http
GET /api/gestor-documental/documentos/{idDocumento}/adjuntar-digitalizacion/validacion
```

Query sugerido:

```txt
nombreGabinete
radicado
```

## Arquitectura backend propuesta

```txt
DocuArchi.Api
└─ Controllers
   └─ GestorDocumental
      ├─ Digitalizacion
      │  ├─ DigitalizacionConfiguracionController.cs
      │  ├─ DigitalizacionListaChequeoController.cs
      │  ├─ DigitalizacionMetadataController.cs
      │  └─ DigitalizacionDocumentosController.cs
      └─ Documentos
         └─ AdjuntarDigitalizacionController.cs

MiApp.DTOs
└─ DTOs
   └─ GestorDocumental
      └─ Digitalizacion

MiApp.Services
└─ Service
   └─ GestorDocumental
      ├─ Digitalizacion
      │  ├─ Configuracion
      │  ├─ ListaChequeo
      │  ├─ Metadata
      │  └─ Documentos
      └─ Documentos
         └─ AdjuntarDigitalizacion

MiApp.Repository
└─ Repositorio
   └─ GestorDocumental
      ├─ Digitalizacion
      └─ Documentos
         └─ AdjuntarDigitalizacion
```

## Arquitectura frontend propuesta

```txt
src/modules/digitalizacion/
├─ components/
│  ├─ DigitalizacionPage/
│  ├─ ScannerToolbar/
│  ├─ ScannerPreview/
│  ├─ ScannerThumbnails/
│  ├─ DigitalizacionMetadataPanel/
│  └─ DigitalizacionActions/
├─ hooks/
│  ├─ useDigitalizacionScanner.ts
│  ├─ useDigitalizacionUpload.ts
│  ├─ useDigitalizacionMetadata.ts
│  └─ useAdjuntarDigitalizacion.ts
├─ services/
│  ├─ digitalizacionApi.ts
│  └─ adjuntarDigitalizacionApi.ts
├─ types/
│  └─ digitalizacion.types.ts
└─ infrastructure/
   └─ dynamsoft/
      ├─ DynamsoftTwainClient.ts
      └─ loadDynamsoftScripts.ts
```

## Flujo crear documento digitalizado nuevo

```txt
React Digitalizacion
  -> Dynamsoft adapter
  -> generar PDF
  -> upload temporal
  -> resolver metadata/lista de chequeo
  -> POST /api/gestor-documental/digitalizacion/documentos
  -> AlmacenamientoDocumental con TipoAlmacenamiento = Digitalizacion
  -> auditoria
  -> refrescar documentos
```

## Flujo adjuntar digitalizacion a PDF existente

```txt
React Digitalizacion
  -> seleccionar documento destino
  -> validar append permitido
  -> Dynamsoft adapter
  -> generar PDF
  -> upload temporal
  -> POST /api/gestor-documental/documentos/{idDocumento}/adjuntar-digitalizacion
  -> backend valida firmado/bloqueado/radicado/PDF
  -> backend une PDF existente + PDF digitalizado
  -> backend actualiza paginas y metadata fisica
  -> auditoria
  -> refrescar visor/listado
```

## Integracion con backend actual

### Compatible

- Upload temporal de `AlmacenamientoDocumental`.
- DTO `TrdStorageDto` para metadata TRD.
- Validadores TRD existentes en almacenamiento.
- Infraestructura de indice electronico y descriptors.
- Patron Controller -> Service -> Repository.
- `ReemplazoPdfController` como referencia cercana para operaciones sobre PDF existente.

### Modelo API actual a respetar

La nueva API de digitalizacion debe seguir el modelo usado por los controllers actuales en:

```txt
DocuArchi.Api\Controllers\GestorDocumental\AlmacenamientoDocumental\AlmacenamientoDocumentalController.cs
DocuArchi.Api\Controllers\GestorDocumental\Documentos\ReemplazoPdfController.cs
DocuArchi.Api\Controllers\GestorDocumental\Documentos\FirmaElectronicaDocumentoController.cs
DocuArchi.Api\Controllers\GestorDocumental\ConfiguracionUpload\SolicitaEstructuraConfiguracionUploadController.cs
```

Convenciones observadas:

- controllers bajo rutas `api/gestor-documental/...`;
- `[ApiController]`;
- `[Authorize]` en endpoints documentales sensibles;
- respuestas siempre envueltas en `AppResponses<T>`;
- errores de validacion como `AppError` con `Type`, `Field`, `Message`;
- metadata de estado con `AppMeta { Status = "validation" | "error" | ... }`;
- validacion de claim `defaulalias` para resolver alias/base;
- validacion de claim `usuarioid` en operaciones de escritura;
- resolucion de usuario e IP para auditoria en operaciones transaccionales;
- controllers delgados que delegan en servicios;
- servicios registrados por interfaz;
- naming de rutas en kebab-case;
- responses `Ok(result)` cuando `result.success = true`;
- `BadRequest(result)` cuando el servicio retorna `success = false`;
- `StatusCode(500, AppResponses<T>)` para excepciones no controladas de operaciones criticas.

Implicacion para digitalizacion:

```txt
DigitalizacionController
  -> valida claims
  -> valida request basico
  -> resuelve usuario / ip cuando aplique
  -> llama service
  -> retorna AppResponses<T>
```

No se debe introducir un modelo de respuesta diferente ni endpoints que retornen DTOs crudos.

### Requiere extension

- Exponer o derivar `TipoAlmacenamiento = Digitalizacion`.
- Crear API de metadata/lista de chequeo.
- Crear API de adjuntar digitalizacion PDF-only.
- Implementar validacion de firmado/bloqueado/radicado no modificable para append.
- Implementar servicio de union PDF existente + PDF digitalizado.

## Riesgos

- La metadata legacy mezcla consulta, validacion y actualizacion en una misma funcion.
- La unicidad de lista de chequeo por radicado debe verificarse con reglas equivalentes al legacy.
- La operacion de append PDF debe ser atomica: si falla la union o auditoria, no debe dejar archivo inconsistente.
- La validacion de documento radicado no modificable debe definirse con precision segun las reglas actuales del negocio.
- Dynamsoft requiere manejo cuidadoso de licencia, scripts/runtime y errores de entorno local.

## Pendientes de diseno detallado

- Definir DTOs finales.
- Confirmar tabla/campos exactos para documento bloqueado y radicado no modificable.
- Confirmar estrategia de backup antes de sobrescribir PDF existente.
- Confirmar libreria backend para merge PDF.
- Confirmar si `DigitalizacionDocumentosController` llama internamente a `AlmacenarDocumentoUseCase` o si expone una fachada de aplicacion.
- Confirmar permisos requeridos para crear y adjuntar digitalizacion.

## Proxima fase recomendada

Crear contratos API detallados y prompts de implementacion por fases:

```txt
01-BE-DigitalizacionMetadata-API.md
02-BE-CrearDocumentoDigitalizado.md
03-BE-AdjuntarDigitalizacionPdf.md
04-FE-DynamsoftAdapter.md
05-FE-DigitalizacionWorkbench.md
06-TESTS-Digitalizacion.md
```
