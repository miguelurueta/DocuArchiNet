# SCRUMCORE-236 - Arquitectura

## Objetivo

Alinear la capa frontend del visor PDF con el contrato oficial de permisos sin mover responsabilidades entre componentes.

## Responsabilidades

- `AppTreeTable`: emite seleccion o accion de fila.
- `DocumentosWorkbench`: orquesta documento activo y llama `AppVisorEmbedPdf.load`.
- `AppVisorEmbedPdf`: resuelve `codigoImpl`, consulta permisos y aplica policy.
- `AppVisorEmbedPdf.service.ts`: consume el endpoint y valida el envelope.
- Backend: resuelve `IdUsuario` desde JWT y retorna permisos efectivos.

## Flujo

```mermaid
sequenceDiagram
  autonumber
  participant U as Usuario
  participant T as AppTreeTable
  participant W as DocumentosWorkbench
  participant O as AppDocumentViewerOrchestrator
  participant V as AppVisorEmbedPdf
  participant S as AppVisorEmbedPdf.service
  participant API as Backend permisos visor

  U->>T: Click en documento
  T->>W: onSelectRow(rowId)
  W->>O: visualizarDocumento(documentId, nombreGabinete, context)
  O-->>W: documentoActivo con fileUrl
  W->>V: load({ url, nombre_modulo })
  V->>V: resolveCodigoImplementacion(nombre_modulo)
  V->>S: fetchMisPermisosVisorPdf(codigoImpl)
  S->>API: GET /implementaciones/{codigoImpl}/mis-permisos
  API-->>S: Envelope con data.Permissions
  S-->>V: data
  V->>V: mapPermisosVisorPdfToEffectivePermissions
  V-->>U: Toolbar/acciones segun permisos
```

## Diagrama de responsabilidades

```mermaid
flowchart TD
  A[Modulo funcional] --> B[nombre_modulo]
  B --> C[DocumentosWorkbench]
  C --> D[AppTreeTable]
  C --> E[Orquestador documento]
  C --> F[AppVisorEmbedPdf.load]
  F --> G[resolveCodigoImplementacion]
  G --> H[Service permisos]
  H --> I[Backend]
  I --> J[Permisos efectivos]
  J --> K[Toolbar visor]

  D -. no consulta permisos .-> D
  C -. no decide policy .-> C
  F -. aplica policy .-> K
```

## ADRs

### ADR-236-01: Unwrap estricto del envelope

Se consume el contrato oficial `{ success, message, data, meta, errors }` y se retorna solo `data` al visor.

### ADR-236-02: No enviar idUsuario

`mis-permisos` usa JWT. Frontend no envia `idUsuario`.

### ADR-236-03: No ampliar UI para permisos no conectados

`pdf.view`, `pdf.zoom` y `pdf.rotate` quedan documentados sin nuevas capacidades UI.
