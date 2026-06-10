# Capability: workflow-personal-signature-use-in-viewer

## ADDED Requirements

### Requirement: Personal signature becomes active signature in modal

Una vez descargada, la firma personal **MUST** poder convertirse en la firma activa del modal de firmas para su uso dentro del visor.

- El sistema **MUST** reutilizar el mismo camino de datos/validaciones que el flujo de “Upload” (sin introducir lógica paralela).
- El sistema **MUST** mantener la integración con EmbedPDF: la inserción/gestión de anotaciones ocurre en los plugins oficiales ya configurados.

#### Scenario: Using personal signature sets active signature source
**Given** existe un `Blob` descargado y listo  
**When** el usuario presiona “Usar firma personal”  
**Then** el modal establece la firma activa y el usuario puede colocarla como anotación usando el comportamiento estándar del visor.

### Requirement: No changes to other plugins / operations

La implementación **MUST NOT** cambiar el comportamiento de:

- zoom
- rotate
- scroll / virtualización
- render pipeline / RenderLayer

#### Scenario: Zoom/rotate behavior is unchanged
**Given** el visor está operativo con zoom/rotate  
**When** el usuario abre/cierra el modal de firma y usa “Firma personal”  
**Then** zoom/rotate permanecen estables y sin efectos colaterales.

