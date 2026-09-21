# Secuencia

Fuentes: `WebServiceImportarServicioWebModern.GetPreview`, `SiiImportProvider.GetPreviewContentAsync`, `ImportPreviewDescriptorService.Create`, `ImportarServicioWebPreview.ProcessRequest`, `ImportPreviewContentService.Head`, `ImportPreviewContentService.Claim`.

```mermaid
sequenceDiagram
  actor Cliente
  participant ASMX as WebServiceImportarServicioWebModern
  participant SII as SiiImportProvider
  participant DS as ImportPreviewDescriptorService
  participant DB as ImportPreviewDescriptorRepository
  participant H as ImportarServicioWebPreview
  Cliente->>ASMX: GetPreview(GetPreviewRequestDto)
  ASMX->>SII: GetPreviewContentAsync(request, cancellationToken)
  SII-->>ASMX: SiiPreviewContent
  ASMX->>DS: Create(context, content, utcNow)
  DS->>DB: Create(snapshot)
  ASMX-->>Cliente: GetPreviewResponseDto(DescriptorId)
  Cliente->>H: HEAD ?d=descriptor
  H->>DB: GetAvailableMetadata(hash, authority, utcNow)
  H-->>Cliente: 200 headers, sin bytes
  Cliente->>H: GET ?d=descriptor
  H->>DB: ClaimAndLoad(hash, authority, utcNow)
  H-->>Cliente: 200 contenido por bloques
  H->>DB: MarkConsumed(id, utcNow)
```
