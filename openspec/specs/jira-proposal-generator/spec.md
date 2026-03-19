# jira-proposal-generator Specification

## Purpose
TBD - created by archiving change scrum-22-rad-llenar-lista-destinatario-restringida. Update Purpose after archive.
## Requirements
### Requirement: Destinatario restringido llena lista por interacción
El sistema MUST consultar el autocompletado de `Destinatario_Cor` únicamente cuando exista entrada del usuario y una estructura `CDeRelacionEstadoRetriccionDto` vigente para el trámite seleccionado.

#### Scenario: Consulta restringida al digitar en Destinatario_Cor
- **WHEN** el usuario digita texto en `Destinatario_Cor` con un trámite ya seleccionado
- **THEN** el frontend MUST enviar `ValueAuto` y `CDeRelacionEstadoRetriccionDto` en el payload de la API restringida

#### Scenario: Sin consulta en primer render
- **WHEN** el formulario se renderiza por primera vez sin interacción del usuario
- **THEN** el sistema MUST NOT ejecutar la consulta de restricción/autocompletado de `Destinatario_Cor`

### Requirement: Cambio de trámite sincroniza restricción
El sistema SHALL actualizar el estado de restricción al cambiar `pl-radicacion-spe-Descripcion_Documento` y usar ese estado actualizado en consultas posteriores de `Destinatario_Cor`.

#### Scenario: Trámite actualizado antes de consultar destinatario
- **WHEN** el usuario cambia el trámite y luego digita en `Destinatario_Cor`
- **THEN** la consulta MUST usar la última estructura `CDeRelacionEstadoRetriccionDto` derivada del nuevo trámite

### Requirement: Control de destinatario conserva selección manual
El sistema SHALL mantener comportamiento estándar de lista/autocomplete para `Destinatario_Cor`, sin auto-seleccionar elementos automáticamente.

#### Scenario: Opciones visibles sin selección implícita
- **WHEN** la API devuelve opciones para `Destinatario_Cor`
- **THEN** el control MUST mostrar la lista y esperar selección explícita del usuario

