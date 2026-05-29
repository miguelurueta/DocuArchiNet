# PROMPT IMPLEMENTACION - API Validar Adjuntar Digitalizacion
# Fase 05 - GET /api/gestor-documental/documentos/{idDocumento}/adjuntar-digitalizacion/validacion

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ROL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Actua como Arquitecto Backend .NET especialista en Clean Architecture, AppResponses, reglas documentales, firma electronica, bloqueo documental, radicados y validaciones legacy-compatible.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## OBJETIVO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Implementar:

```http
GET /api/gestor-documental/documentos/{idDocumento}/adjuntar-digitalizacion/validacion
```

Debe indicar si un documento permite adjuntar un PDF digitalizado, sin mutar DB ni filesystem.

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
FirmaElectronicaDocumentoController.cs
ReemplazoPdfController.cs
ClassWorkflowDigitalizacion.Valida_adjuntar_documento_digitalizado
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTRATO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Query:

```csharp
AdjuntarDigitalizacionValidacionQuery
```

Response:

```csharp
AppResponses<AdjuntarDigitalizacionValidacionResponse?>
```

Ubicacion DTO:

```txt
MiApp.DTOs/DTOs/GestorDocumental/Documentos/AdjuntarDigitalizacion/
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## UBICACION ESPERADA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Controller:

```txt
DocuArchi.Api/Controllers/GestorDocumental/Documentos/AdjuntarDigitalizacionController.cs
```

Service:

```txt
MiApp.Services/Service/GestorDocumental/Documentos/AdjuntarDigitalizacion/
```

Repository:

```txt
MiApp.Repository/Repositorio/GestorDocumental/Documentos/AdjuntarDigitalizacion/
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## REGLAS OBLIGATORIAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

- Validar `defaulalias`.
- No requerir `usuarioid` salvo politica existente lo exija para consulta.
- No mutar nada.
- Usar `DapperCrudEngine` + `QueryOptions`.
- No SQL manual.
- PDF-only.

Bloquear si:

- documento no existe;
- documento no es PDF;
- documento firmado;
- documento bloqueado;
- radicado no modificable.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CODIGOS BLOQUEO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

```txt
DOCUMENT_NOT_FOUND
DOCUMENT_NOT_PDF
DOCUMENT_SIGNED
DOCUMENT_BLOCKED
RADICADO_NOT_MODIFIABLE
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## PRUEBAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Casos minimos:

1. permitido;
2. documento inexistente;
3. documento no PDF;
4. firmado bloquea;
5. bloqueado bloquea;
6. radicado no modificable bloquea;
7. claim faltante;
8. repository exception controlada.

Ejecutar `dotnet build` y `dotnet test`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## INSTRUCCION FINAL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Implementar solo la API de validacion previa para adjuntar digitalizacion, sin efectos colaterales y repitiendo estas reglas en el POST final.
