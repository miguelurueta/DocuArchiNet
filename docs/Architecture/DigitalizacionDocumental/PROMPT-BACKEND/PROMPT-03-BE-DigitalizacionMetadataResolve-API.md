# PROMPT IMPLEMENTACION - API Resolve Metadata DigitalizacionDocumental
# Fase 03 - POST /api/gestor-documental/digitalizacion/metadata/resolve

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ROL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Actua como Arquitecto Backend .NET especialista en Clean Architecture, AppResponses, TRD, inventario documental, tipologia documental, DapperCrudEngine, QueryOptions y validaciones legacy-compatible.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## OBJETIVO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Implementar:

```http
POST /api/gestor-documental/digitalizacion/metadata/resolve
```

Debe convertir una seleccion de lista de chequeo/tipologia en metadata documental completa compatible con `TrdStorageDto`, validando obligatoriedad y unicidad sin almacenar ni modificar documentos.

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
ClassWorkflowDigitalizacion.Actualiza_tipo_documento_lista_chequeo
ra_dig_tipos_docum_lista_chequeo.SolicitaDatosTipoDocumentalListaChequeo
Verifica_existencia_del_tipo_documental_en_el_gabinete_con_enlace_radicado
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTRATO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Request:

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

Response:

```csharp
AppResponses<DigitalizacionMetadataResolveResponse?>
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## UBICACION ESPERADA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Controller:

```txt
DocuArchi.Api/Controllers/GestorDocumental/Digitalizacion/DigitalizacionMetadataController.cs
```

Service:

```txt
MiApp.Services/Service/GestorDocumental/Digitalizacion/Metadata/
```

Repository:

```txt
MiApp.Repository/Repositorio/GestorDocumental/Digitalizacion/Metadata/
```

Registrar DI en `Program.cs`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## REGLAS OBLIGATORIAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

- Validar claim `defaulalias`.
- Usar `AppResponses<T>`.
- Usar `DapperCrudEngine` + `QueryOptions`.
- No usar SQL manual.
- No mutar gabinete ni registro_producion_documental.
- No actualizar indice XML.
- No crear documentos.
- Devolver TRD completo o error funcional controlado.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## VALIDACIONES
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Validar:

- request null;
- `NombreGabinete` requerido;
- configuracion no encontrada;
- lista obligatoria con `IdTipoListaChequeo <= 0`;
- tipologia inexistente;
- TRD incompleta;
- `ValidarUnicidad = true` con radicado vacio cuando la tipologia sea unica;
- duplicidad de tipologia unica por radicado.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## OBSERVABILIDAD
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Logs con:

- requestId;
- nombreGabinete;
- idConfiguracionDigitalizacion;
- idTipoListaChequeo;
- radicado;
- alias;
- durationMs.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## PRUEBAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Casos minimos:

1. resolve OK con TRD completa;
2. lista obligatoria no seleccionada;
3. tipologia inexistente;
4. tipologia unica duplicada;
5. radicado requerido para unicidad;
6. request invalido;
7. claim faltante;
8. repository exception controlada.

Ejecutar `dotnet build` y `dotnet test`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## INSTRUCCION FINAL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Implementar solo `metadata/resolve`, garantizando metadata TRD confiable para fases posteriores y sin efectos colaterales documentales.
