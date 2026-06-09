# alineamiento-api-permisos-appvisorpdf Specification

## Purpose
TBD - created by archiving change scrumcore-236-alineamiento-api-permisos-appvisorpdf. Update Purpose after archive.
## Requirements
### Requirement: Consumo del envelope oficial de permisos visor PDF
El sistema SHALL consumir el contrato oficial de `mis-permisos` usando el envelope de API y SHALL retornar al visor el payload interno `data`.

#### Scenario: Service retorna data del envelope exitoso
- **WHEN** el backend responde `success=true` con `data.Permissions`
- **THEN** `fetchMisPermisosVisorPdf` SHALL retornar `data`
- **AND** el visor SHALL mapear permisos desde `data.Permissions`

#### Scenario: Service rechaza contrato invalido
- **WHEN** el backend responde sin `success=true` o sin `data.Permissions`
- **THEN** `fetchMisPermisosVisorPdf` SHALL lanzar error
- **AND** `AppVisorEmbedPdf` SHALL aplicar fail-closed para acciones sensibles

#### Scenario: Request mantiene cancelacion
- **WHEN** un nuevo `load()` cancela la consulta anterior
- **THEN** el request de permisos SHALL respetar `AbortSignal`
- **AND** la respuesta obsoleta SHALL NOT sobrescribir permisos del documento activo

### Requirement: Codigo de implementacion de Gestion Correspondencia
El sistema SHALL resolver el modulo `gestioncorrespondencia` al `codigoImpl` oficial `gestion_correspondencia`.

#### Scenario: Mapping de modulo a implementacion
- **WHEN** `AppVisorEmbedPdf.load()` recibe `nombre_modulo="gestioncorrespondencia"`
- **THEN** `resolveCodigoImplementacion` SHALL retornar `gestion_correspondencia`
- **AND** el service SHALL consultar `/implementaciones/gestion_correspondencia/mis-permisos`

#### Scenario: Frontend no envia idUsuario
- **WHEN** el visor consulta `mis-permisos`
- **THEN** el request SHALL NOT enviar `idUsuario` en path, query ni body
- **AND** el usuario SHALL resolverse por backend desde el JWT

### Requirement: Mapping de permisos backend a permisos efectivos del visor
El sistema SHALL mapear los codigos documentados por backend a `ViewerEffectivePermissions`.

#### Scenario: Print
- **WHEN** `Permissions["pdf.print"]` es `true`
- **THEN** `allowPrint` SHALL ser `true`

#### Scenario: Download/export
- **WHEN** `Permissions["pdf.download"]` es `true`
- **THEN** `allowExport` SHALL ser `true`

#### Scenario: Firma placement
- **WHEN** `Permissions["pdf.annotate.signature.place"]` es `true`
- **THEN** `allowSignaturePlacement` SHALL ser `true`

#### Scenario: Firma delete
- **WHEN** `Permissions["pdf.annotate.signature.delete"]` es `true`
- **THEN** `allowSignatureDelete` SHALL ser `true`

#### Scenario: Firma lock toggle
- **WHEN** `Permissions["pdf.annotate.signature.lock"]` o `Permissions["pdf.annotate.signature.unlock"]` es `true`
- **THEN** `allowSignatureLockToggle` SHALL ser `true`

#### Scenario: Permisos vacios
- **WHEN** `Permissions` esta vacio
- **THEN** todos los permisos efectivos de acciones sensibles SHALL ser `false`

### Requirement: Alcance visual acotado
El sistema SHALL documentar `pdf.view`, `pdf.zoom` y `pdf.rotate` sin conectarlos a nuevas capacidades visuales en esta iteracion.

#### Scenario: Permisos no conectados a UI nueva
- **WHEN** backend retorna `pdf.view`, `pdf.zoom` o `pdf.rotate`
- **THEN** el frontend SHALL preserve el contrato recibido
- **AND** SHALL NOT ampliar `ViewerEffectivePermissions` ni cambiar controles visuales salvo requerimiento explicito

### Requirement: Documentacion enterprise del contrato
El sistema SHALL generar documentacion enterprise completa del alineamiento API permisos visor.

#### Scenario: Documentacion creada
- **WHEN** se complete el refinement
- **THEN** deberan existir documentos en `docs/Architecture/AlineamientoContratoApiPermisosVisor/`
- **AND** la documentacion SHALL cubrir metadata, arquitectura, contrato API, implementacion, pruebas y prompt enterprise

