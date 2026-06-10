# PROMPT IMPLEMENTACION - API Configuracion DigitalizacionDocumental
# Fase 01 - GET /api/gestor-documental/digitalizacion/configuracion

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ROL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Actua como Arquitecto Backend .NET especialista en Clean Architecture, ASP.NET Core Web API, AppResponses, DapperCrudEngine, QueryOptions, migracion WebForms/ASMX y ECM documental legacy-compatible.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## OBJETIVO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Implementar:

```http
GET /api/gestor-documental/digitalizacion/configuracion
```

Debe resolver la configuracion aplicable al modulo `DigitalizacionDocumental` segun contexto de tramite, workflow, radicacion o produccion documental.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTEXTO OBLIGATORIO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Documentos fuente:

```txt
docs/Architecture/DigitalizacionDocumental/DigitalizacionDocumental-Requisitos-Arquitectura.md
docs/Architecture/DigitalizacionDocumental/01-BE-Contratos-API-Digitalizacion.md
```

Legacy referencia:

```txt
Class_ra_dig_config_digitalizacion.Solicita_id_configuracion_digitalizacion
Class_ra_dig_config_digitalizacion.Solicita_datos_configuracion_digitalizacion
WebFormEscan.aspx.vb
```

Controllers referencia:

```txt
AlmacenamientoDocumentalController.cs
SolicitaEstructuraConfiguracionUploadController.cs
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTRATO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Query:

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

Response:

```csharp
AppResponses<DigitalizacionConfiguracionResponse?>
```

DTOs en:

```txt
MiApp.DTOs/DTOs/GestorDocumental/Digitalizacion/
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## UBICACION ESPERADA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Controller:

```txt
DocuArchi.Api/Controllers/GestorDocumental/Digitalizacion/DigitalizacionConfiguracionController.cs
```

Service:

```txt
MiApp.Services/Service/GestorDocumental/Digitalizacion/Configuracion/
```

Repository:

```txt
MiApp.Repository/Repositorio/GestorDocumental/Digitalizacion/Configuracion/
```

Registrar DI en `Program.cs`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## REGLAS OBLIGATORIAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

- Usar `AppResponses<T>`.
- Validar claim `defaulalias`.
- Usar `[Authorize]`.
- Usar Controller -> Service -> Repository.
- Usar `DapperCrudEngine` + `QueryOptions` para DB.
- No usar SQL manual, `Session`, strings `"YES"` ni DTOs crudos.
- No crear documentos ni mutar metadata.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## VALIDACIONES
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Validar:

- `TipoDigitalizacion` requerido;
- `NombreGabinete` requerido;
- alias vacio;
- configuracion no encontrada;
- contexto insuficiente para resolver configuracion.

Errores como `AppError` con `AppMeta.Status = "validation"`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## PRUEBAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Casos minimos:

1. configuracion encontrada;
2. configuracion no encontrada;
3. `TipoDigitalizacion` vacio;
4. `NombreGabinete` vacio;
5. claim `defaulalias` faltante;
6. exception repository controlada.

Ejecutar `dotnet build` y `dotnet test`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## INSTRUCCION FINAL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Implementar solo la API de configuracion de digitalizacion, con modelo API actual, validacion runtime, observabilidad y pruebas, sin mutaciones documentales.
