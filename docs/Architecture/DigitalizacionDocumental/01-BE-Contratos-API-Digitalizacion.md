# 01-BE Contratos API: DigitalizacionDocumental

## Objetivo

Definir los contratos API requeridos para el modulo `DigitalizacionDocumental`, alineados con el modelo actual del backend:

- rutas bajo `api/gestor-documental`;
- `[ApiController]`;
- `[Authorize]` para endpoints documentales;
- respuestas `AppResponses<T>`;
- errores `AppError`;
- `AppMeta.Status`;
- validacion de `defaulalias`;
- validacion de `usuarioid` para operaciones de escritura;
- patron Controller -> Service -> Repository.

## Endpoints requeridos

```txt
GET  /api/gestor-documental/digitalizacion/configuracion
GET  /api/gestor-documental/digitalizacion/lista-chequeo
POST /api/gestor-documental/digitalizacion/metadata/resolve
POST /api/gestor-documental/digitalizacion/documentos
GET  /api/gestor-documental/documentos/{idDocumento}/adjuntar-digitalizacion/validacion
POST /api/gestor-documental/documentos/{idDocumento}/adjuntar-digitalizacion
```

## 1. Consultar configuracion de digitalizacion

### Ruta

```http
GET /api/gestor-documental/digitalizacion/configuracion
```

### Proposito

Obtener la configuracion que habilita y parametriza la digitalizacion para un contexto de tramite, workflow, radicacion o produccion documental.

Reemplaza responsabilidades legacy asociadas a:

```txt
Class_ra_dig_config_digitalizacion.Solicita_id_configuracion_digitalizacion
Class_ra_dig_config_digitalizacion.Solicita_datos_configuracion_digitalizacion
```

### Query

```csharp
public sealed class DigitalizacionConfiguracionQuery
{
    public string TipoDigitalizacion { get; init; } = default!;
    public long? IdTramite { get; init; }
    public string? TipoTramite { get; init; }
    public string? Radicado { get; init; }
    public string NombreGabinete { get; init; } = default!;
    public long? IdTareaWorkflow { get; init; }
    public long? IdRutaWorkflow { get; init; }
}
```

### Response

```csharp
public sealed class DigitalizacionConfiguracionResponse
{
    public int IdConfiguracionDigitalizacion { get; init; }
    public string TipoDigitalizacion { get; init; } = default!;
    public string NombreGabinete { get; init; } = default!;
    public bool ActivaListaChequeo { get; init; }
    public bool ObligaListaChequeo { get; init; }
    public bool PermiteCrearDocumento { get; init; }
    public bool PermiteAdjuntarDocumento { get; init; }
    public bool RequiereMetadata { get; init; }
    public string[] FormatosPermitidos { get; init; } = ["pdf"];
}
```

### Wrapper

```csharp
AppResponses<DigitalizacionConfiguracionResponse?>
```

### Validaciones controller

- `defaulalias` requerido.
- `TipoDigitalizacion` requerido.
- `NombreGabinete` requerido.

## 2. Consultar lista de chequeo / tipologias

### Ruta

```http
GET /api/gestor-documental/digitalizacion/lista-chequeo
```

### Proposito

Obtener los tipos documentales/lista de chequeo disponibles para el contexto de digitalizacion.

Reemplaza responsabilidades legacy asociadas a:

```txt
ra_dig_tipos_docum_lista_chequeo.Solicita_listar_tipos_documentales_relacionados_al_tipo_tramite
WebServiceGestorDocumental.Service_Solicita_lista_sub_series_documentales_id_serie
WebServiceGestorDocumental.Solicita_lista_tipos_documentales_relacionados_sub_serie
```

### Query

```csharp
public sealed class DigitalizacionListaChequeoQuery
{
    public long? IdTramite { get; init; }
    public string? TipoTramite { get; init; }
    public int IdConfiguracionDigitalizacion { get; init; }
    public string NombreGabinete { get; init; } = default!;
    public string? Radicado { get; init; }
}
```

### Item response

```csharp
public sealed class DigitalizacionListaChequeoItemDto
{
    public int IdTipoListaChequeo { get; init; }
    public string NombreTipoDocumento { get; init; } = default!;
    public int? IdArea { get; init; }
    public int? IdSerie { get; init; }
    public int? IdSubSerie { get; init; }
    public int? IdTipoDocumento { get; init; }
    public string? NombreArea { get; init; }
    public string? NombreSerie { get; init; }
    public string? NombreSubSerie { get; init; }
    public bool EsUnico { get; init; }
    public bool Obligatorio { get; init; }
    public bool Disponible { get; init; }
    public string? MensajeNoDisponible { get; init; }
}
```

### Response

```csharp
public sealed class DigitalizacionListaChequeoResponse
{
    public int IdConfiguracionDigitalizacion { get; init; }
    public bool ObligaListaChequeo { get; init; }
    public IReadOnlyList<DigitalizacionListaChequeoItemDto> Items { get; init; } = [];
}
```

### Wrapper

```csharp
AppResponses<DigitalizacionListaChequeoResponse?>
```

### Validaciones controller

- `defaulalias` requerido.
- `IdConfiguracionDigitalizacion` mayor o igual a cero.
- `NombreGabinete` requerido.

## 3. Resolver metadata documental

### Ruta

```http
POST /api/gestor-documental/digitalizacion/metadata/resolve
```

### Proposito

Resolver una seleccion de lista de chequeo/tipologia en metadata documental completa, compatible con almacenamiento documental.

Este endpoint debe validar reglas de obligatoriedad y unicidad, pero no debe almacenar documento ni modificar gabinete por si solo.

Reemplaza la parte de resolucion/validacion de:

```txt
ClassWorkflowDigitalizacion.Actualiza_tipo_documento_lista_chequeo
```

sin ejecutar la actualizacion final del documento.

### Request

```csharp
public sealed class DigitalizacionMetadataResolveRequest
{
    public string NombreGabinete { get; init; } = default!;
    public int IdTipoListaChequeo { get; init; }
    public int IdConfiguracionDigitalizacion { get; init; }
    public string? Radicado { get; init; }
    public long? IdImagen { get; init; }
    public bool ValidarUnicidad { get; init; } = true;
    public string? RequestId { get; init; }
}
```

### DTO TRD resuelto

Debe poder mapearse a `TrdStorageDto`.

```csharp
public sealed class DigitalizacionTrdMetadataDto
{
    public int? IdArea { get; init; }
    public int? IdSerie { get; init; }
    public int? IdSubSerie { get; init; }
    public int? IdTipoDocumento { get; init; }
    public string? NombreArea { get; init; }
    public string? NombreSerie { get; init; }
    public string? NombreSubSerie { get; init; }
    public string? NombreTipoDocumento { get; init; }
}
```

### Response

```csharp
public sealed class DigitalizacionMetadataResolveResponse
{
    public int IdTipoListaChequeo { get; init; }
    public int IdConfiguracionDigitalizacion { get; init; }
    public bool ObligaListaChequeo { get; init; }
    public bool EsUnico { get; init; }
    public bool UnicidadValidada { get; init; }
    public DigitalizacionTrdMetadataDto? Trd { get; init; }
}
```

### Wrapper

```csharp
AppResponses<DigitalizacionMetadataResolveResponse?>
```

### Validaciones controller

- `defaulalias` requerido.
- `NombreGabinete` requerido.
- Si configuracion obliga lista de chequeo, `IdTipoListaChequeo` debe ser mayor a cero.

### Validaciones service

- Verificar configuracion de digitalizacion.
- Verificar si lista de chequeo es obligatoria.
- Resolver datos de tipo documental.
- Resolver area, serie, subserie.
- Validar unicidad por radicado cuando aplique.
- Retornar errores `AppError` sin lanzar excepciones para reglas de negocio esperadas.

## 4. Crear documento digitalizado nuevo

### Ruta

```http
POST /api/gestor-documental/digitalizacion/documentos
```

### Proposito

Crear un documento nuevo a partir de un PDF digitalizado previamente cargado en upload temporal.

Debe apoyarse en el caso de uso actual de almacenamiento documental, marcando la operacion como digitalizacion.

### Request

```csharp
public sealed class CrearDocumentoDigitalizadoRequest
{
    public string NombreGabinete { get; init; } = default!;
    public string RutaTemporalId { get; init; } = default!;
    public string ArchivoTemporalId { get; init; } = default!;
    public string NombreDocumento { get; init; } = default!;
    public string? RequestId { get; init; }
    public string? Radicado { get; init; }
    public long? IdTareaWorkflow { get; init; }
    public long? IdRutaWorkflow { get; init; }
    public int? IdConfiguracionDigitalizacion { get; init; }
    public int? IdTipoListaChequeo { get; init; }
    public DigitalizacionTrdMetadataDto? Trd { get; init; }
    public int? NumeroPaginasDeclaradas { get; init; }
}
```

### Response

```csharp
public sealed class CrearDocumentoDigitalizadoResponse
{
    public long IdDocumento { get; init; }
    public string NombreGabinete { get; init; } = default!;
    public string NombreDocumento { get; init; } = default!;
    public string Extension { get; init; } = "pdf";
    public int NumeroPaginas { get; init; }
    public string? Radicado { get; init; }
    public string? RequestId { get; init; }
}
```

### Wrapper

```csharp
AppResponses<CrearDocumentoDigitalizadoResponse?>
```

### Validaciones controller

- `defaulalias` requerido.
- `usuarioid` requerido.
- `NombreGabinete` requerido.
- `RutaTemporalId` requerido.
- `ArchivoTemporalId` requerido.
- `NombreDocumento` requerido.

### Validaciones service

- Validar que el upload temporal exista y este completo.
- Validar que el archivo temporal sea PDF.
- Resolver o validar metadata TRD.
- Ejecutar almacenamiento documental con `TipoAlmacenamiento = Digitalizacion`.
- Registrar auditoria.

## 5. Validar si permite adjuntar digitalizacion

### Ruta

```http
GET /api/gestor-documental/documentos/{idDocumento}/adjuntar-digitalizacion/validacion
```

### Proposito

Permitir que el frontend consulte si un documento destino puede recibir paginas digitalizadas.

La validacion final debe repetirse en el endpoint de ejecucion.

### Query

```csharp
public sealed class AdjuntarDigitalizacionValidacionQuery
{
    public string NombreGabinete { get; init; } = default!;
    public string? Radicado { get; init; }
}
```

### Response

```csharp
public sealed class AdjuntarDigitalizacionValidacionResponse
{
    public long IdDocumento { get; init; }
    public string NombreGabinete { get; init; } = default!;
    public bool Permitido { get; init; }
    public string? CodigoBloqueo { get; init; }
    public string? MensajeBloqueo { get; init; }
    public bool EsPdf { get; init; }
    public bool EstaFirmado { get; init; }
    public bool EstaBloqueado { get; init; }
    public bool RadicadoNoModificable { get; init; }
    public int? NumeroPaginasActual { get; init; }
}
```

### Wrapper

```csharp
AppResponses<AdjuntarDigitalizacionValidacionResponse?>
```

### Validaciones controller

- `defaulalias` requerido.
- `idDocumento` mayor a cero.
- `NombreGabinete` requerido.

## 6. Adjuntar digitalizacion a PDF existente

### Ruta

```http
POST /api/gestor-documental/documentos/{idDocumento}/adjuntar-digitalizacion
```

### Proposito

Anexar un PDF digitalizado temporal a un documento PDF existente.

Este endpoint no crea documento nuevo. Modifica el documento existente agregando paginas.

Reemplaza la rama legacy:

```txt
ClassWorkflowDigitalizacion.Valida_adjuntar_documento_digitalizado
ClassAñadirDocumento.Añade_documento_digitalizado
ClassAñadirDocumento.Añadir_documento_pdf
```

### Request

```csharp
public sealed class AdjuntarDigitalizacionPdfRequest
{
    public string NombreGabinete { get; init; } = default!;
    public string RutaTemporalId { get; init; } = default!;
    public string ArchivoTemporalId { get; init; } = default!;
    public string? RequestId { get; init; }
    public string? Radicado { get; init; }
    public long? IdTareaWorkflow { get; init; }
    public long? IdRutaWorkflow { get; init; }
    public string? Motivo { get; init; }
    public string? ModuloRegistro { get; init; } = "DIGITALIZACION";
    public string? TipologiaDocumental { get; init; }
}
```

### Response

```csharp
public sealed class AdjuntarDigitalizacionPdfResponse
{
    public long IdDocumento { get; init; }
    public string NombreGabinete { get; init; } = default!;
    public string Extension { get; init; } = "pdf";
    public int NumeroPaginasAnterior { get; init; }
    public int NumeroPaginasAgregadas { get; init; }
    public int NumeroPaginasFinal { get; init; }
    public bool DocumentoActualizado { get; init; }
    public string? RequestId { get; init; }
}
```

### Wrapper

```csharp
AppResponses<AdjuntarDigitalizacionPdfResponse?>
```

### Validaciones controller

- `defaulalias` requerido.
- `usuarioid` requerido.
- `idDocumento` mayor a cero.
- `NombreGabinete` requerido.
- `RutaTemporalId` requerido.
- `ArchivoTemporalId` requerido.

### Validaciones service

- Validar que el documento destino exista.
- Validar que el documento destino sea PDF.
- Validar que el archivo temporal exista y este completo.
- Validar que el archivo temporal sea PDF.
- Bloquear si el documento destino esta firmado.
- Bloquear si el documento destino esta bloqueado.
- Bloquear si el documento/radicado no permite modificacion.
- Crear backup o estrategia de recuperacion antes de sobrescribir.
- Unir PDF existente + PDF digitalizado.
- Actualizar numero de paginas.
- Registrar auditoria.
- Limpiar o marcar temporal como usado.

## Modelo de errores

Los errores deben seguir el patron:

```csharp
new AppResponses<T?>
{
    success = false,
    message = "Mensaje funcional",
    data = null,
    meta = new AppMeta { Status = "validation" },
    errors =
    [
        new AppError
        {
            Type = "Validation",
            Field = "campo",
            Message = "Detalle del error"
        }
    ]
}
```

Codigos de bloqueo recomendados para `AdjuntarDigitalizacionValidacionResponse.CodigoBloqueo`:

```txt
DOCUMENT_NOT_FOUND
DOCUMENT_NOT_PDF
DOCUMENT_SIGNED
DOCUMENT_BLOCKED
RADICADO_NOT_MODIFIABLE
TEMPORARY_FILE_NOT_FOUND
TEMPORARY_FILE_NOT_PDF
```

## Ubicacion propuesta

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
      ├─ Digitalizacion
      └─ Documentos
         └─ AdjuntarDigitalizacion

MiApp.Services
└─ Service
   └─ GestorDocumental
      ├─ Digitalizacion
      └─ Documentos
         └─ AdjuntarDigitalizacion

MiApp.Repository
└─ Repositorio
   └─ GestorDocumental
      ├─ Digitalizacion
      └─ Documentos
         └─ AdjuntarDigitalizacion
```

## Dependencias a reutilizar

- `IStorageLargeUploadService` para upload temporal.
- `IAlmacenarDocumentoUseCase` para crear documento nuevo.
- Infraestructura de `AlmacenamientoDocumental` para TRD, inventario, expediente e indice.
- Servicios/repositorios existentes de firma electronica cuando apliquen.
- Patron de auditoria usado por almacenamiento, reemplazo y eliminacion documental.

## Decisiones pendientes

- Confirmar si los endpoints de digitalizacion tendran upload temporal propio o usaran directamente `/api/gestor-documental/almacenamiento/upload-temporal`.
- Confirmar campo/tabla fuente para `EstaBloqueado`.
- Confirmar regla exacta de `RadicadoNoModificable`.
- Confirmar libreria de merge PDF.
- Confirmar si `CrearDocumentoDigitalizadoRequest.Trd` se envia completo desde `metadata/resolve` o si el backend debe resolver siempre por `IdTipoListaChequeo`.
