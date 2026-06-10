# PROMPT IMPLEMENTACION - API Crear Documento Digitalizado
# Fase 04 - POST /api/gestor-documental/digitalizacion/documentos

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ROL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Actua como Arquitecto Backend .NET especialista en Clean Architecture, StorageEngineV2, upload temporal, AppResponses, TRD, inventario documental y observabilidad enterprise.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## OBJETIVO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Implementar:

```http
POST /api/gestor-documental/digitalizacion/documentos
```

Debe crear un documento nuevo desde un PDF digitalizado cargado en upload temporal, reutilizando `AlmacenamientoDocumental` y marcando `TipoAlmacenamiento = Digitalizacion`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTEXTO OBLIGATORIO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Documentos fuente:

```txt
docs/Architecture/DigitalizacionDocumental/DigitalizacionDocumental-Requisitos-Arquitectura.md
docs/Architecture/DigitalizacionDocumental/01-BE-Contratos-API-Digitalizacion.md
```

Referencias:

```txt
AlmacenamientoDocumentalController.cs
AlmacenarDocumentoUseCase.cs
AlmacenarDocumentoRequest.cs
TrdStorageDto.cs
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## RESTRICCION PRINCIPAL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

No duplicar StorageEngineV2. Este endpoint debe ser una fachada/orquestador sobre almacenamiento documental.

PROHIBIDO:

- crear un segundo flujo de alta documental;
- soportar TIF/JPG/BMP;
- crear documento sin `TipoAlmacenamiento = Digitalizacion`;
- hardcodear alias o usuario.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTRATO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Request:

```csharp
CrearDocumentoDigitalizadoRequest
```

Response:

```csharp
AppResponses<CrearDocumentoDigitalizadoResponse?>
```

Ubicacion DTO:

```txt
MiApp.DTOs/DTOs/GestorDocumental/Digitalizacion/
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## UBICACION ESPERADA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Controller:

```txt
DocuArchi.Api/Controllers/GestorDocumental/Digitalizacion/DigitalizacionDocumentosController.cs
```

Service:

```txt
MiApp.Services/Service/GestorDocumental/Digitalizacion/Documentos/
```

Registrar DI en `Program.cs`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## REGLAS OBLIGATORIAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

- Validar `defaulalias`.
- Validar `usuarioid`.
- Usar `IStorageLargeUploadService`.
- Reusar `IAlmacenarDocumentoUseCase`.
- Exponer o derivar `TipoAlmacenamiento = Digitalizacion` sin romper default manual.
- Solo PDF.
- `AppResponses<T>`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## VALIDACIONES
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Validar:

- request null;
- `NombreGabinete` requerido;
- `RutaTemporalId` requerido;
- `ArchivoTemporalId` requerido;
- `NombreDocumento` requerido;
- temporal no existe;
- temporal no completo;
- temporal no PDF;
- TRD requerida ausente;
- numero paginas invalido.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## PRUEBAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Casos minimos:

1. crea documento PDF OK;
2. temporal no existe;
3. temporal no completo;
4. temporal no PDF;
5. metadata TRD ausente cuando aplica;
6. `TipoAlmacenamiento = Digitalizacion`;
7. storage retorna error;
8. claim usuario invalido.

Ejecutar `dotnet build` y `dotnet test`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## INSTRUCCION FINAL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Implementar la API de creacion de documento digitalizado PDF reutilizando almacenamiento documental y garantizando trazabilidad de `TipoAlmacenamiento = Digitalizacion`.
