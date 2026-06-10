# PROMPT IMPLEMENTACION - API Adjuntar Digitalizacion PDF
# Fase 06 - POST /api/gestor-documental/documentos/{idDocumento}/adjuntar-digitalizacion

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ROL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Actua como Arquitecto Backend .NET especialista en Clean Architecture, operaciones PDF server-side, ECM documental, locking/concurrencia, AppResponses, filesystem seguro y observabilidad enterprise.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## OBJETIVO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Implementar:

```http
POST /api/gestor-documental/documentos/{idDocumento}/adjuntar-digitalizacion
```

Debe anexar un PDF digitalizado temporal a un documento PDF existente, sin crear documento nuevo.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTEXTO OBLIGATORIO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Documentos fuente:

```txt
docs/Architecture/DigitalizacionDocumental/DigitalizacionDocumental-Requisitos-Arquitectura.md
docs/Architecture/DigitalizacionDocumental/01-BE-Contratos-API-Digitalizacion.md
docs/Architecture/DigitalizacionDocumental/PROMPT-05-BE-ValidarAdjuntarDigitalizacion-API.md
```

Legacy referencia:

```txt
ClassWorkflowDigitalizacion.Valida_adjuntar_documento_digitalizado
ClassAñadirDocumento.Añade_documento_digitalizado
ClassAñadirDocumento.Añadir_documento_pdf
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## RESTRICCION PRINCIPAL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

NO usar `AlmacenarDocumentoRequest`; este flujo no crea documento nuevo.

PROHIBIDO:

- soportar TIF;
- modificar PDF firmado;
- modificar documento bloqueado;
- modificar radicado no modificable;
- sobrescribir sin staging/backup;
- dejar DB y filesystem inconsistentes.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTRATO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Request:

```csharp
AdjuntarDigitalizacionPdfRequest
```

Response:

```csharp
AppResponses<AdjuntarDigitalizacionPdfResponse?>
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

Services:

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
- Validar `usuarioid`.
- Repetir validaciones del GET.
- Validar temporal completo y PDF.
- Usar staging/backup.
- Controlar concurrencia por documento.
- Actualizar numero de paginas.
- Registrar auditoria.
- Usar `DapperCrudEngine` + `QueryOptions`.
- No SQL manual.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## SECUENCIA OBLIGATORIA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

1. validar claims;
2. validar request;
3. cargar snapshot documento;
4. validar PDF/firmado/bloqueado/radicado;
5. validar temporal PDF;
6. crear staging/backup;
7. merge PDF;
8. validar paginas finales;
9. abrir transaccion DB;
10. lock o validacion concurrencia;
11. actualizar paginas/metadata;
12. insertar auditoria;
13. commit DB;
14. reemplazo fisico controlado;
15. limpiar temporal;
16. responder.

Si falla:

- rollback DB;
- conservar original;
- limpiar staging si es seguro;
- log estructurado.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## PRUEBAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Casos minimos:

1. append exitoso;
2. documento no PDF;
3. firmado bloquea;
4. bloqueado bloquea;
5. radicado no modificable bloquea;
6. temporal no PDF;
7. falla merge no muta DB;
8. falla DB no reemplaza fisico;
9. concurrencia bloqueada;
10. auditoria insertada;
11. no crea documento nuevo.

Ejecutar `dotnet build` y `dotnet test`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## INSTRUCCION FINAL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Implementar la API de adjuntar digitalizacion PDF garantizando PDF-only, bloqueo documental, atomicidad logica DB/filesystem, auditoria, concurrencia controlada y cero creacion de documentos nuevos.
