# 02 — Consulta y preview SII

Fuentes: `SiiExternalImportProviderClient.vb`, `SiiImportProvider.vb`, `SiiImportContractMapper.vb`, `SiiPreviewResponseFactory.vb`.

```mermaid
sequenceDiagram
    autonumber
    participant FE as Frontend
    participant WS as WebServiceImportarServicioWebModern
    participant SC as SiiExternalImportProviderClient
    participant HTTP as ExternalImportHttpClient
    participant SII as SII
    participant TM as IExternalServiceAttemptRecorder

    FE->>WS: QueryItems(request: TaskId, ProviderId, CodigoBarras, OperationId, CorrelationId)
    WS->>WS: FeatureEnabled + ValidRequest + TryBuildImportContext
    WS->>SC: QueryItemsAsync(request, CancellationToken.None)
    SC->>HTTP: solicitarToken
    HTTP->>SII: solicitud de token
    SII-->>HTTP: token o error
    SC->>TM: Observe(SOLICITAR_TOKEN, contexto opcional)
    SC->>HTTP: consultarInformacionSello({radicado: CodigoBarras})
    HTTP->>SII: POST sello
    SII-->>HTTP: inscripciones[].imagenes[]
    SC->>TM: Observe(CONSULTAR_SELLO, TaskId, CodigoBarras, ReferenciaProveedor)
    SC->>SC: SiiImportContractMapper.MapQueryResponse
    SC->>SC: BuildExternalKey(codigoBarras, libro, registro, idanexo)
    SC-->>FE: Items[] sin URL + ProviderResultCode + conteos

    FE->>WS: GetPreview(request: ExternalKey)
    WS->>SC: GetPreviewAsync
    SC->>SC: ParseExternalKey
    SC->>SII: token + consultarInformacionSello
    SC->>SC: ResolveImage(libro, registro, idanexo)
    SC->>HTTP: descarga de URL resuelta en servidor
    SC->>SC: valida esquema, host, tamaño y formato
    SC-->>WS: ExternalPreviewSource
    WS->>WS: SiiPreviewResponseFactory.Create
    WS-->>FE: DescriptorId, ContentType, Length, Disposition, ExpiresAtUtc
```

La URL de imagen y el token no aparecen en ningún DTO público.
