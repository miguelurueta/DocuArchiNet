# PROMPT IMPLEMENTACION - API Lista Chequeo DigitalizacionDocumental
# Fase 02 - GET /api/gestor-documental/digitalizacion/lista-chequeo

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ROL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Actua como Arquitecto Backend .NET especialista en Clean Architecture, ASP.NET Core Web API, AppResponses, TRD, tipologia documental, DapperCrudEngine, QueryOptions y migracion legacy-compatible.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## OBJETIVO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Implementar:

```http
GET /api/gestor-documental/digitalizacion/lista-chequeo
```

Debe devolver las tipologias/lista de chequeo disponibles para digitalizacion segun tramite, tipo tramite, configuracion, radicado y gabinete.

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
ra_dig_tipos_docum_lista_chequeo.Solicita_listar_tipos_documentales_relacionados_al_tipo_tramite
WebServiceGestorDocumental.Service_Solicita_lista_sub_series_documentales_id_serie
WebServiceGestorDocumental.Solicita_lista_tipos_documentales_relacionados_sub_serie
WebFormEscan.aspx.vb
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTRATO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Query:

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

Response:

```csharp
AppResponses<DigitalizacionListaChequeoResponse?>
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
DocuArchi.Api/Controllers/GestorDocumental/Digitalizacion/DigitalizacionListaChequeoController.cs
```

Service:

```txt
MiApp.Services/Service/GestorDocumental/Digitalizacion/ListaChequeo/
```

Repository:

```txt
MiApp.Repository/Repositorio/GestorDocumental/Digitalizacion/ListaChequeo/
```

Registrar DI en `Program.cs`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## REGLAS OBLIGATORIAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

- Usar `AppResponses<T>`.
- Validar claim `defaulalias`.
- Usar `[Authorize]`.
- Usar `DapperCrudEngine` + `QueryOptions`.
- No usar SQL manual.
- No resolver almacenamiento.
- No mutar gabinete, inventario ni indice.
- Marcar item como no disponible si regla funcional lo exige, sin romper toda la respuesta salvo error critico.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## VALIDACIONES
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Validar:

- `IdConfiguracionDigitalizacion` no negativo;
- `NombreGabinete` requerido;
- configuracion inexistente;
- lista vacia controlada;
- errores de catalogo TRD.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## PRUEBAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Casos minimos:

1. lista con items;
2. lista vacia controlada;
3. configuracion inexistente;
4. `NombreGabinete` vacio;
5. claim faltante;
6. repository exception controlada.

Ejecutar `dotnet build` y `dotnet test`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## INSTRUCCION FINAL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Implementar solo la API de lista de chequeo/tipologias para digitalizacion, respetando el modelo API actual y sin mutaciones documentales.
