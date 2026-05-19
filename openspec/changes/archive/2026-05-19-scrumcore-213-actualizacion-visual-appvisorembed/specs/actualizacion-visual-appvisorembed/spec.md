# Capability: actualizacion-visual-appvisorembed

## ADDED Requirements

### Requirement: Personal signature tab shows image preview

Cuando el tab “Firma personal” esté en estado `ready`, el sistema **MUST** renderizar un preview visual de la firma descargada:

- Debe renderizar un `<img>` cuyo `src` sea el `ObjectURL` (`blobUrl`) y `alt` sea `"Firma personal"`.
- El preview **MUST NOT** mostrar el string de la URL (`blob:`) ni el `UrlTemporal` en la UI.
- El preview **MUST** mantener proporción (no deformar) y estar contenido en un contenedor enterprise.

#### Scenario: Preview image is visible on ready
**Given** el usuario entra al tab “Firma personal” y el estado pasa a `ready`  
**When** el tab renderiza el contenido  
**Then** existe un elemento `<img alt="Firma personal">` visible.

#### Scenario: URL strings are not shown
**Given** el estado está `ready` con un `blobUrl`  
**When** la UI renderiza el preview  
**Then** la UI no muestra textos que contengan `blob:` ni `UrlTemporal`.

### Requirement: Only one CTA exists (“Usar firma”)

En el tab “Firma personal”, el sistema **MUST** mostrar un único botón de acción final:

- Label: “Usar firma”
- `aria-label`: “Usar firma”

El sistema **MUST NOT** renderizar un botón adicional “Usar firma personal”.

#### Scenario: Only one use button is available
**Given** el tab “Firma personal” está en estado `ready`  
**When** el usuario inspecciona las acciones disponibles  
**Then** existe exactamente un botón “Usar firma” y no existe “Usar firma personal”.

### Requirement: “Usar firma” starts official placement

Al presionar “Usar firma” en el tab “Firma personal”, el sistema **MUST** iniciar placement oficial de EmbedPDF usando una firma tipo stamp (upload/type) sin lógica custom de anotación.

#### Scenario: Using personal signature triggers placement
**Given** el tab está `ready` y el usuario presiona “Usar firma”  
**When** el sistema procesa la acción  
**Then** se llama la acción de placement del plugin oficial de firmas sin errores.

