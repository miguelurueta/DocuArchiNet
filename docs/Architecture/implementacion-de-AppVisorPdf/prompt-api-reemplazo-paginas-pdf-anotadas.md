# PROMPT ARQUITECTÓNICO — Ticket BE

SCRUMCORE-[ID] — Reemplazo Parcial de PDF por Páginas PDF Anotadas desde AppVisorEmbedPdf  
(ENTERPRISE FINAL — Reutilización ReemplazoPdfController + StorageEngine + Upload Temporal + LogDocuArchi)

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## ROL ESPERADO

Actúa como Arquitecto de Software Senior Backend .NET especialista en:

- ASP.NET Core.
- Clean Architecture.
- Controller -> Service -> Repository.
- StorageEngine.
- FileSystem seguro.
- procesamiento PDF.
- reemplazo documental seguro.
- upload temporal.
- auditoría documental legacy `logdocuarchi`.
- DapperCrudEngine.
- QueryOptions.
- AppResponses<T>.
- observabilidad avanzada.
- pruebas unitarias, integración, QT y regresión.
- documentación técnica enterprise.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## CONTEXTO REAL DEL PROYECTO

Referencias Git backend para trazabilidad:

- API: `https://github.com/miguelurueta/DocuArchi.Api.git`
- DTOs: `https://github.com/miguelurueta/MiApp.DTOs.git`
- Services: `https://github.com/miguelurueta/MiApp.Services.git`
- Repository: `https://github.com/miguelurueta/MiApp.Repository.git`
- Documentación/core: `https://github.com/miguelurueta/DocuArchiCore.git`

Repositorio Frontend:

`https://github.com/miguelurueta/DocuArchiCore.react.git`

Visor PDF FE:

`src/app/Components/UI/AppVisorEmbedPdf`

Implementación consumidora FE:

`src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`

API backend existente de reemplazo total:

`DocuArchi.Api/Controllers/GestorDocumental/Documentos/ReemplazoPdfController.cs`

Servicio backend existente:

`MiApp.Services/Service/GestorDocumental/Documentos/ReemplazoPdf/IReemplazoPdfService.cs`

Ruta base actual:

`/api/gestor-documental/documentos/reemplazopdf`

Ya existe:

- `POST /upload-temporal/init`.
- `PUT /upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}`.
- `GET /upload-temporal/{rutaTemporalId}/{archivoTemporalId}/status`.
- `POST /upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete`.
- `DELETE /upload-temporal/{rutaTemporalId}/{archivoTemporalId}`.
- `POST /api/gestor-documental/documentos/reemplazopdf` para reemplazo total.
- `IStorageLargeUploadService`.
- `IStorageUploadPathResolver`.
- `IStoragePathResolver`.
- `IStorageRouteRepository`.
- `IStorageFolderLegacyPolicy`.
- `IReemplazoPdfDocumentLocationRepository`.
- `IFirmaElectronicaDocumentoService`.
- `ILogDocuarchiRepository`.
- resolución IP actual vía `IIpHelper`.
- validación de claim `defaulalias`.
- validación de documento firmado electrónicamente.
- validación de temporal completado.
- backup previo.
- hash anterior/nuevo.
- reemplazo físico seguro.
- auditoría `logdocuarchi`.

No crear infraestructura paralela para ninguna de esas capacidades.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## CORRECCIÓN FUNCIONAL CRÍTICA

La API nueva **NO reemplaza imágenes dentro del PDF**.

La API nueva **NO recibe `image/png` ni `image/jpeg`**.

La API nueva debe reemplazar **páginas PDF completas** del documento original usando **páginas PDF ya anotadas**.

Definición exacta:

- El frontend genera PDFs temporales de páginas anotadas.
- Cada archivo temporal representa una página PDF completa.
- Cada archivo temporal debe contener exactamente una página.
- El backend abre el PDF original.
- El backend reemplaza únicamente las páginas indicadas por las páginas PDF anotadas recibidas.
- Las páginas no indicadas se conservan intactas.
- El resultado final es un PDF completo.
- Después se reutiliza el mismo flujo seguro de reemplazo total: backup, hash, reemplazo físico y `logdocuarchi`.

Prohibido en esta implementación:

- reemplazar imágenes embebidas dentro del PDF.
- convertir páginas a imagen.
- aceptar `image/png`.
- aceptar `image/jpeg`.
- rasterizar como mecanismo principal.
- manipular bytes PDF manualmente con strings o regex.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## PROBLEMA A RESOLVER

El visor `AppVisorEmbedPdf` puede detectar páginas con anotaciones y materializarlas en PDF. Para documentos pesados, enviar el PDF completo anotado desde el frontend puede ser costoso cuando solo cambian pocas páginas.

Se requiere una API backend que reciba solo las páginas PDF anotadas y genere internamente un PDF final completo, reemplazando únicamente esas páginas dentro del PDF original.

La API debe conservar la seguridad, auditoría y robustez del reemplazo total existente.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## OBJETIVO

Implementar una API backend para:

- recibir páginas PDF anotadas desde frontend.
- validar metadata de páginas.
- validar archivos temporales PDF por página.
- recomponer un PDF final usando el PDF original como base.
- reemplazar únicamente las páginas indicadas.
- generar un PDF completo preparado en backend.
- reutilizar internamente la lógica segura del reemplazo total.
- crear backup previo.
- registrar auditoría completa en `logdocuarchi`.
- soportar documentos grandes sin recibir el PDF completo del cliente.
- mantener seguridad de rutas.
- mantener observabilidad enterprise.
- dejar pruebas completas y documentación.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## DECISIÓN ARQUITECTÓNICA PRINCIPAL

Crear una API nueva de reemplazo parcial por páginas PDF anotadas.

No modificar el contrato actual de reemplazo total.

Ruta propuesta:

`POST /api/gestor-documental/documentos/reemplazopdf/paginas-anotadas`

Diseño obligatorio:

1. Resolver y validar PDF original actual.
2. Resolver y validar archivos temporales de páginas PDF anotadas.
3. Reemplazar páginas en una copia del PDF original.
4. Crear un PDF completo preparado por backend.
5. Reutilizar el núcleo seguro de reemplazo total para backup, hash, copia final y auditoría.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## RESTRICCIÓN ARQUITECTÓNICA CRÍTICA

Prohibido:

- duplicar lógica de rutas.
- duplicar validación traversal.
- duplicar validación de firma electrónica.
- duplicar backup.
- duplicar hashing si existe helper aprobado reutilizable.
- duplicar auditoría `logdocuarchi`.
- duplicar resolución IP.
- crear tabla nueva.
- crear sistema temporal nuevo.
- hacer SQL manual.
- usar `ExecuteAsync` directo.
- usar `QueryAsync` directo.
- usar `ExecuteScalarAsync` directo.
- aceptar rutas físicas enviadas desde frontend.
- sobrescribir el archivo final sin backup.
- guardar archivos fuera del StorageEngine/temp root.
- aceptar `PageNumber` fuera del rango del PDF.
- aceptar páginas duplicadas.
- aceptar archivos temporales que no sean PDF.
- aceptar PDFs firmados electrónicamente.

Obligatorio reutilizar:

- `IStorageLargeUploadService`.
- `IStorageUploadPathResolver`.
- `IStoragePathResolver`.
- `IStorageRouteRepository`.
- `IStorageFolderLegacyPolicy`.
- `IReemplazoPdfDocumentLocationRepository`.
- `IFirmaElectronicaDocumentoService`.
- `ILogDocuarchiRepository`.
- `ReemplazoPdfService` o una extracción reutilizable de su núcleo.
- StorageEngine.
- `logdocuarchi`.
- resolución IP existente.
- DapperCrudEngine + QueryOptions para DB.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## REGLA GLOBAL DE DATOS

Todo acceso a datos debe usar:

- DapperCrudEngine.
- QueryOptions.

Ruta obligatoria:

`MiApp.Repository/Repositorio/DataAccess/DapperCrudEngine.cs`

Prohibido:

- SQL manual.
- `ExecuteAsync` directo.
- `QueryAsync` directo.
- `ExecuteScalarAsync` directo.
- concatenación SQL.
- `SELECT *`.
- repositorios con conexión propia si contradicen el patrón local.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## RUTA API PROPUESTA

Agregar acción en:

`DocuArchi.Api/Controllers/GestorDocumental/Documentos/ReemplazoPdfController.cs`

Ruta:

`POST /api/gestor-documental/documentos/reemplazopdf/paginas-anotadas`

Alternativa aceptada si se requiere separación estricta:

`DocuArchi.Api/Controllers/GestorDocumental/Documentos/ReemplazoPdfPaginasController.cs`

Recomendación:

Mantener bajo la misma ruta base del controlador actual para claridad funcional y reutilización de dependencias.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## DTO REQUEST

Crear:

`MiApp.DTOs/DTOs/GestorDocumental/Documentos/ReemplazoPdf/ReemplazarPaginasPdfAnotadasRequest.cs`

```csharp
namespace MiApp.DTOs.DTOs.GestorDocumental.Documentos.ReemplazoPdf
{
    public sealed class ReemplazarPaginasPdfAnotadasRequest
    {
        public required string NombreGabinete { get; init; }
        public long IdDocumento { get; init; }
        public required string RutaTemporalId { get; init; }
        public required IReadOnlyList<PaginaPdfAnotadaTemporalDto> Paginas { get; init; }
        public string? Motivo { get; init; }
        public string? DescOp { get; init; }
        public string? ModuloRegistro { get; init; }
        public string? Radicado { get; init; }
        public long? IdTareaWorkflow { get; init; }
        public long? IdRutaWorkflow { get; init; }
        public string? TipologiaDocumental { get; init; }
    }

    public sealed class PaginaPdfAnotadaTemporalDto
    {
        public int PageNumber { get; init; }
        public required string ArchivoTemporalId { get; init; }
        public string? ContentType { get; init; }
        public string? HashSha256Esperado { get; init; }
    }
}
```

Reglas:

- `PageNumber` es base 1.
- `ArchivoTemporalId` referencia un archivo temporal previamente subido.
- `RutaTemporalId` es común para todos los archivos del lote.
- `ContentType`, si viene informado, debe ser `application/pdf`.
- Cada archivo temporal debe ser PDF de una sola página.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## DTO RESPONSE

Crear:

`MiApp.DTOs/DTOs/GestorDocumental/Documentos/ReemplazoPdf/ReemplazarPaginasPdfAnotadasResponse.cs`

```csharp
namespace MiApp.DTOs.DTOs.GestorDocumental.Documentos.ReemplazoPdf
{
    public sealed class ReemplazarPaginasPdfAnotadasResponse
    {
        public long IdDocumento { get; init; }
        public string NombreGabinete { get; init; } = string.Empty;
        public IReadOnlyList<int> PaginasReemplazadas { get; init; } = Array.Empty<int>();
        public string RutaArchivoFinal { get; init; } = string.Empty;
        public string RutaRespaldo { get; init; } = string.Empty;
        public long TamanoAnteriorBytes { get; init; }
        public long TamanoNuevoBytes { get; init; }
        public string HashAnteriorSha256 { get; init; } = string.Empty;
        public string HashNuevoSha256 { get; init; } = string.Empty;
        public string RequestId { get; init; } = string.Empty;
    }
}
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## CONTRATO API

Request:

```json
{
  "nombreGabinete": "WF_DOCS",
  "idDocumento": 15416,
  "rutaTemporalId": "temp_abc",
  "paginas": [
    {
      "pageNumber": 2,
      "archivoTemporalId": "page_2_pdf",
      "contentType": "application/pdf",
      "hashSha256Esperado": "..."
    },
    {
      "pageNumber": 5,
      "archivoTemporalId": "page_5_pdf",
      "contentType": "application/pdf",
      "hashSha256Esperado": "..."
    }
  ],
  "motivo": "Anotaciones agregadas desde visor PDF",
  "descOp": "AGREGA GRAFO PDF",
  "moduloRegistro": "GESTION_CORRESPONDENCIA",
  "radicado": "2026-0001",
  "idTareaWorkflow": 123,
  "idRutaWorkflow": 0,
  "tipologiaDocumental": "RESPUESTA"
}
```

Response success:

```json
{
  "success": true,
  "message": "OK",
  "data": {
    "idDocumento": 15416,
    "nombreGabinete": "WF_DOCS",
    "paginasReemplazadas": [2, 5],
    "rutaArchivoFinal": ".../DIG00015416.pdf",
    "rutaRespaldo": ".../replacement-versions/WF_DOCS/15416/20260601143000/DIG00015416.pdf",
    "tamanoAnteriorBytes": 100000000,
    "tamanoNuevoBytes": 100240000,
    "hashAnteriorSha256": "...",
    "hashNuevoSha256": "...",
    "requestId": "..."
  },
  "meta": {
    "status": "success"
  },
  "errors": []
}
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## FLUJO FUNCIONAL OBLIGATORIO

1. Validar request.
2. Validar `NombreGabinete`.
3. Validar `IdDocumento > 0`.
4. Validar `RutaTemporalId`.
5. Validar lista `Paginas`:
   - no null.
   - no vacía.
   - máximo configurable.
   - `PageNumber > 0`.
   - sin duplicados.
   - `ArchivoTemporalId` requerido.
   - `ContentType`, si viene, debe ser `application/pdf`.
6. Resolver usuario actual.
7. Validar claim `defaulalias` igual que `ReemplazoPdfController` actual.
8. Resolver ubicación documental:
   - gabinete.
   - idDocumento.
   - disco.
   - carpeta.
   - tipoDocumento.
9. Reutilizar `IReemplazoPdfDocumentLocationRepository`.
10. Validar firma electrónica con `IFirmaElectronicaDocumentoService`.
11. Si `FirmadoElectronico == true`, retornar error de validación: `No se permite reemplazar páginas de un documento firmado digitalmente`.
12. Resolver ruta física actual con StorageEngine:
   - `IStorageRouteRepository`.
   - `IStorageFolderLegacyPolicy`.
   - `IStoragePathResolver`.
13. Localizar archivo final `DIG{idDocumento}`.
14. Validar que el archivo final sea PDF.
15. Obtener cantidad de páginas del PDF original.
16. Validar que cada `PageNumber <= totalPages`.
17. Validar temporales de páginas con:

```csharp
await _largeUploadService.EnsureCompletedAsync(
    request.RutaTemporalId.Trim(),
    archivoTemporalIds,
    usuarioId);
```

18. Resolver cada temporal:

```csharp
_uploadPathResolver.GetFinalFilePath(
    request.RutaTemporalId.Trim(),
    archivoTemporalId.Trim());
```

19. Validar por archivo:
   - existe.
   - está bajo temp root.
   - extensión `.pdf`.
   - `ContentType == application/pdf` si viene informado.
   - hash coincide si `HashSha256Esperado` viene informado.
   - contiene exactamente una página.
20. Generar PDF completo preparado:
   - abrir PDF original.
   - recorrer páginas del original.
   - si la página no está en `Paginas`, copiar la página original.
   - si la página está en `Paginas`, importar la única página del PDF temporal correspondiente.
   - preservar orden.
   - preservar cantidad total de páginas.
   - guardar PDF preparado en temp root seguro.
21. Ejecutar reemplazo seguro:
   - hash anterior del PDF original.
   - hash nuevo del PDF preparado.
   - backup previo.
   - reemplazo físico.
   - verificación hash.
   - auditoría `logdocuarchi`.
22. Retornar response con páginas reemplazadas, hashes, tamaños, backup y `requestId`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## REUTILIZACIÓN DEL REEMPLAZO TOTAL

Refactor obligatorio si el código actual no permite reutilización limpia.

Extraer núcleo común desde `ReemplazoPdfService` a una abstracción interna, por ejemplo:

`IReemplazoPdfStorageExecutor`

Responsabilidad:

- validar destino ya resuelto.
- crear backup.
- copiar nuevo PDF final.
- validar hash.
- insertar `logdocuarchi`.
- construir response base.

Alternativa:

Crear método interno reutilizable:

`ExecuteReplacementFromPreparedPdfAsync(...)`

Entrada conceptual:

```csharp
public sealed class PreparedPdfReplacementCommand
{
    public required string NombreGabinete { get; init; }
    public long IdDocumento { get; init; }
    public required string PreparedPdfPath { get; init; }
    public string? Motivo { get; init; }
    public string? DescOp { get; init; }
    public string? ModuloRegistro { get; init; }
    public string? Radicado { get; init; }
    public long? IdTareaWorkflow { get; init; }
    public long? IdRutaWorkflow { get; init; }
    public string? TipologiaDocumental { get; init; }
    public required string Usuario { get; init; }
    public int UsuarioId { get; init; }
    public required string DefaultDbAlias { get; init; }
    public string? IpTrans { get; init; }
    public object? AuditExtraFields { get; init; }
}
```

Reglas:

- El endpoint de reemplazo total existente debe seguir funcionando sin cambios de contrato.
- No romper `ReemplazarDocumentoPdfRequest`.
- No duplicar el bloque de backup/copy/log.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## SERVICIO DE PROCESAMIENTO PDF

Primero inspeccionar dependencias existentes del backend.

Si ya existe librería PDF aprobada, reutilizarla.

Si no existe, proponer explícitamente una librería compatible con licenciamiento del proyecto antes de implementarla.

Requisitos mínimos de la librería:

- abrir PDF existente.
- contar páginas.
- importar/copiar páginas existentes.
- importar una página desde otro PDF.
- crear PDF de salida.
- guardar PDF final.

Prohibido:

- writer PDF manual.
- manipular bytes PDF con strings.
- usar regex sobre PDF.
- usar rutas frontend.
- usar librerías con licencia incompatible sin aprobación.

Crear:

`MiApp.Services/Service/GestorDocumental/Documentos/ReemplazoPdf/IPdfPageReplacementService.cs`

```csharp
public interface IPdfPageReplacementService
{
    Task<PdfPageReplacementResult> ReplacePagesWithPdfPagesAsync(
        PdfPageReplacementCommand command,
        CancellationToken cancellationToken = default);
}

public sealed class PdfPageReplacementCommand
{
    public required string SourcePdfPath { get; init; }
    public required string OutputPdfPath { get; init; }
    public required IReadOnlyList<PdfPageReplacementItem> Pages { get; init; }
}

public sealed class PdfPageReplacementItem
{
    public int PageNumber { get; init; }
    public required string AnnotatedSinglePagePdfPath { get; init; }
}

public sealed class PdfPageReplacementResult
{
    public int TotalPages { get; init; }
    public IReadOnlyList<int> PagesReplaced { get; init; } = Array.Empty<int>();
    public long OutputSizeBytes { get; init; }
}
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## REGLAS DE PROCESAMIENTO PDF

La unidad de reemplazo es una página PDF completa.

Para cada `PageNumber`:

- abrir el PDF temporal indicado.
- validar que tiene exactamente una página.
- importar esa página al PDF de salida.
- ubicarla en la posición `PageNumber` del documento original.
- no alterar páginas no indicadas.

El PDF resultante debe tener exactamente la misma cantidad de páginas que el PDF original.

No se debe:

- dibujar imagen sobre página blanca.
- rasterizar.
- reemplazar objetos internos de imagen.
- modificar el PDF original in-place.

Siempre generar un PDF preparado nuevo y luego pasarlo al flujo seguro de reemplazo total.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## VERSIONADO / RESPALDO

Reutilizar exactamente la política existente:

`{TempRoot}\replacement-versions\{gabinete}\{idDocumento}\{yyyyMMddHHmmss}\`

Reglas:

- crear estructura si no existe.
- no sobrescribir backup.
- timestamp obligatorio.
- usar raíz temporal existente desde `IStorageUploadPathResolver.GetTempRoot()`.
- validar que el backup queda bajo root seguro.
- backup antes del reemplazo físico.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## TRANSACCIONALIDAD HÍBRIDA

DB + FileSystem no son una transacción ACID única.

Secuencia obligatoria:

1. validar.
2. generar PDF preparado.
3. backup.
4. replace físico.
5. verificar hash.
6. auditoría DB.

Si falla generación PDF:

- no backup.
- no replace.
- no auditoría de éxito.

Si falla replace:

- conservar backup si ya existe.
- retornar error controlado.
- no insertar auditoría de éxito.

Si falla auditoría después del replace:

- no revertir replace físico automáticamente.
- log crítico de inconsistencia híbrida.
- retornar error controlado con `requestId`.
- documentar runbook manual.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## CONTROL DE CONCURRENCIA

Implementar lock lógico por documento.

Objetivo:

- evitar dos reemplazos simultáneos del mismo documento.
- evitar corrupción física.
- evitar backup inconsistente.

Clave:

`{NombreGabinete}:{IdDocumento}`

Implementación aceptada:

- lock in-memory si la app es single-instance.
- si hay múltiples instancias, documentar deuda y proponer lock distribuido.

No usar lock global para todos los documentos.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## AUDITORÍA LOGDOCUARCHI

Tabla oficial:

`logdocuarchi`

Registrar todos los campos posibles:

- `id_tran`.
- `desc_op`.
- `USER_OPER`.
- `DATE_TRANS`.
- `RUT_DOCU`.
- `GABINETE`.
- `CAMPOS`.
- `IP_TRANS`.
- `HORA_REGISTRO`.
- `MODULO_REGISTRO`.
- `RADICADO`.
- `ID_TAREA_WF`.
- `ID_RUTA_WF`.
- `USER_PROPIETARIO`.
- `TIPOLOGIA_DOCUMENTAL`.

`DescOp` permitido:

- `AGREGA GRAFO PDF`.

Opcional solo si negocio lo exige:

- `AGREGAR GRAFO MANUSCRITO`.

Prohibido:

- `REEMPLAZO_PDF_FRONT`.
- valores libres sin control.
- `CAMPOS` vacío.

`CAMPOS` JSON debe contener:

- `idDocumento`.
- `rutaTemporalId`.
- `paginasReemplazadas`.
- `archivosTemporalesPdf`.
- `rutaArchivoOriginal`.
- `rutaPdfPreparado`.
- `rutaRespaldo`.
- `hashAnterior`.
- `hashNuevo`.
- `tamanoAnterior`.
- `tamanoNuevo`.
- `motivo`.
- `modo = "REEMPLAZO_PAGINAS_PDF_ANOTADAS"`.
- `totalPaginasOriginal`.
- `cantidadPaginasReemplazadas`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## OBSERVABILIDAD

Logs `Information`:

- inicio reemplazo páginas PDF anotadas.
- documento resuelto.
- firma validada.
- temporales validados.
- PDF original leído.
- PDF preparado generado.
- backup creado.
- replace físico OK.
- auditoría insertada.
- operación completada.

Logs `Warning`:

- documento inexistente.
- temporal inexistente.
- página duplicada.
- página fuera de rango.
- `ContentType` no permitido.
- PDF temporal multipágina.
- documento firmado electrónicamente.

Logs `Error`:

- error generando PDF preparado.
- error reemplazando página.
- replace físico falló.
- verificación hash falló.
- auditoría falló.
- inconsistencia híbrida.

Campos obligatorios:

- `requestId`.
- `gabinete`.
- `idDocumento`.
- `usuarioId`.
- `usuario`.
- `ipTrans`.
- `paginasSolicitadas`.
- `paginasReemplazadas`.
- `totalPages`.
- `rutaTemporalId`.
- `tamanoAnterior`.
- `tamanoNuevo`.
- `hashAnterior`.
- `hashNuevo`.
- `duracionMs`.
- `fase`.

Prohibido loguear:

- bytes de PDF.
- contenido del PDF.
- Authorization.
- rutas con tokens si existieran.
- valores sensibles de campos.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## VALIDACIONES

Request:

- request no null.
- `NombreGabinete` requerido.
- `IdDocumento > 0`.
- `RutaTemporalId` requerido.
- `Paginas` requerida.
- `Paginas` no vacía.
- `PageNumber > 0`.
- `PageNumber` sin duplicados.
- `ArchivoTemporalId` requerido.
- `ContentType == application/pdf` si viene informado.
- `IdTareaWorkflow >= 0` si viene.
- `IdRutaWorkflow >= 0` si viene.

Temporales:

- `EnsureCompletedAsync`.
- archivo existe.
- archivo bajo temp root.
- extensión `.pdf`.
- hash esperado coincide si viene.
- cada PDF temporal tiene exactamente una página.

Documento:

- existe en gabinete.
- archivo físico existe.
- archivo físico es PDF.
- no firmado electrónicamente.
- `PageNumber <= totalPages`.

Seguridad:

- path traversal bloqueado.
- no usar rutas del request.
- no aceptar extensiones dobles peligrosas.
- no sobrescribir temporales externos.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## PRUEBAS OBLIGATORIAS

Unitarias Controller:

- request válido retorna OK.
- request null retorna BadRequest.
- claim `defaulalias` faltante retorna BadRequest.
- usuario inválido retorna BadRequest.
- service exception retorna 500 controlado.

Unitarias Service:

- valida `NombreGabinete` requerido.
- valida `IdDocumento` inválido.
- valida `RutaTemporalId` requerido.
- valida `Paginas` vacías.
- valida `PageNumber` duplicado.
- valida `ArchivoTemporalId` requerido.
- valida `ContentType` no PDF.
- valida `IdTareaWorkflow` negativo.
- valida `IdRutaWorkflow` negativo.
- bloquea documento firmado electrónicamente.
- bloquea documento inexistente.
- bloquea temporal inexistente.
- bloquea página fuera de rango.
- bloquea PDF temporal multipágina.
- genera PDF preparado.
- invoca backup antes de replace.
- inserta `logdocuarchi` completo.
- retorna hashes y tamaños.

Unitarias `PdfPageReplacementService`:

- reemplaza una página.
- reemplaza múltiples páginas.
- conserva páginas no anotadas.
- mantiene total de páginas.
- falla si PDF temporal no existe.
- falla si PDF temporal tiene más de una página.
- falla si `PageNumber` está fuera de rango.
- falla con archivo corrupto.
- maneja documento portrait.
- maneja documento landscape.

Integración:

- upload temporal de páginas PDF + endpoint páginas anotadas.
- reemplazo de PDF real con una página.
- reemplazo de PDF real con varias páginas.
- backup creado.
- `logdocuarchi` insertado.
- hash nuevo distinto cuando cambia contenido.
- documento firmado bloqueado.
- rollback lógico si falla generación antes de replace.

QT / Seguridad:

- traversal en `RutaTemporalId` bloqueado.
- traversal en `ArchivoTemporalId` bloqueado.
- extensión `.exe` bloqueada.
- doble extensión bloqueada si aplica.
- `PageNumber` 0 bloqueado.
- `PageNumber` negativo bloqueado.
- `PageNumber` mayor total bloqueado.
- no SQL manual.
- no `ExecuteAsync` directo.
- no rutas físicas desde frontend.

Regresión:

- no rompe `POST` reemplazo total existente.
- no rompe `upload-temporal/init`.
- no rompe upload chunk.
- no rompe complete.
- no rompe cancel.
- no rompe firma electrónica.
- no rompe `logdocuarchi` actual.
- no rompe StorageEngine.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## DOCUMENTACIÓN OBLIGATORIA

Ruta:

`Docs/GestionDocumental/ReemplazoPdfPaginasAnotadas/`

Crear:

- `SCRUM-[ID]-Arquitectura.md`
- `SCRUM-[ID]-Contrato-API.md`
- `SCRUM-[ID]-Implementacion-Detallada.md`
- `SCRUM-[ID]-Procesamiento-PDF.md`
- `SCRUM-[ID]-Auditoria-LogDocuArchi.md`
- `SCRUM-[ID]-Pruebas.md`
- `SCRUM-[ID]-Observabilidad.md`
- `SCRUM-[ID]-Seguridad.md`
- `SCRUM-[ID]-Runbook.md`
- `SCRUM-[ID]-Metadata.md`

Cada documento debe referenciar explícitamente:

- reutilización de reemplazo total.
- reutilización de upload temporal.
- no reemplazo de imágenes.
- reemplazo de páginas PDF completas.
- transaccionalidad híbrida.
- auditoría `logdocuarchi`.
- riesgos y mitigaciones.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## ENTREGABLES

Código:

- DTO request páginas PDF anotadas.
- DTO response páginas PDF anotadas.
- action nueva en controller.
- service nuevo o extensión controlada.
- `IPdfPageReplacementService`.
- command/result models.
- refactor reutilizable de `ReemplazoPdfService`.
- DI.
- tests unitarios.
- tests integración.
- tests QT/regresión.

Documentación:

- arquitectura.
- contrato API.
- implementación detallada.
- procesamiento PDF.
- auditoría `logdocuarchi`.
- pruebas.
- observabilidad.
- seguridad.
- runbook.
- metadata.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## CRITERIOS DE ACEPTACIÓN

- API recibe páginas PDF anotadas.
- API no recibe imágenes.
- API no reemplaza imágenes internas del PDF.
- Cada archivo temporal por página es PDF.
- Cada PDF temporal tiene exactamente una página.
- Backend reemplaza páginas completas por `PageNumber`.
- Páginas no anotadas quedan intactas.
- El PDF final conserva la cantidad total de páginas.
- Se reutiliza reemplazo total seguro para backup/hash/`logdocuarchi`.
- Se reutiliza upload temporal existente.
- Documento firmado electrónicamente se bloquea.
- No se rompe endpoint actual de reemplazo total.
- No hay SQL manual.
- DapperCrudEngine y QueryOptions se mantienen obligatorios para DB.
- Hay observabilidad por `requestId`.
- Hay pruebas completas.
- Hay documentación enterprise completa.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## RESTRICCIONES

No modificar contrato actual de reemplazo total.

No crear sistema temporal nuevo.

No aceptar base64.

No aceptar imágenes.

No aceptar rutas físicas del frontend.

No sobrescribir sin backup.

No reemplazar documentos firmados electrónicamente.

No manipular PDF manualmente por strings/regex.

No SQL manual.

No `ExecuteAsync` directo.

No `QueryAsync` directo.

No tabla nueva de auditoría.

No omitir `logdocuarchi`.

No duplicar hashing/rutas/backup si puede reutilizarse.

No romper endpoints existentes de upload temporal.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## RIESGOS Y DECISIONES A DOCUMENTAR

Riesgo:

Librería PDF puede tener limitaciones o licencia incompatible.

Mitigación:

Verificar dependencias existentes y licenciamiento antes de implementar.

Riesgo:

DB + FileSystem no son ACID.

Mitigación:

Documentar transaccionalidad híbrida, logs críticos y runbook.

Riesgo:

Aplicación multi-instancia.

Mitigación:

Si el lock es in-memory, documentar deuda técnica y alternativa con lock distribuido.

Riesgo:

PDF temporal por página no corresponde visualmente al tamaño/orientación original.

Mitigación:

Validar dimensiones de MediaBox/CropBox cuando la librería lo permita o documentar comportamiento esperado.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## INSTRUCCIÓN FINAL

Implementar una API backend enterprise para reemplazar páginas PDF completas por páginas PDF anotadas, reutilizando al máximo la API y servicio actuales de reemplazo total:

- `ReemplazoPdfController`.
- `ReemplazoPdfService`.
- upload temporal.
- StorageEngine.
- path resolvers.
- validación firma electrónica.
- backup.
- hash.
- `logdocuarchi`.
- resolución IP.
- DapperCrudEngine / QueryOptions.

La solución debe evitar subir PDFs completos desde frontend cuando solo cambian algunas páginas, pero debe mantener la misma seguridad, trazabilidad, auditoría y robustez del reemplazo total existente.

Esta API no reemplaza imágenes embebidas, no recibe imágenes rasterizadas y no modifica el contrato del reemplazo total actual.
