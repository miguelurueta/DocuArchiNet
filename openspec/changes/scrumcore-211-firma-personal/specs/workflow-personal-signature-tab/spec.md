# Capability: workflow-personal-signature-tab

## ADDED Requirements

### Requirement: Personal signature tab is available

El sistema **MUST** mostrar una pestaña llamada **"Firma personal"** dentro del modal de firmas del visor `AppVisorEmbedPdf`.

- La pestaña **MUST** coexistir con las pestañas actuales (ej. dibujar/subir) sin modificar su comportamiento.
- El `DocumentosWorkbench` **MUST NOT** recibir lógica ni estado adicional para habilitar esta pestaña.

#### Scenario: Modal renders personal signature tab
**Given** el usuario abre el modal de firma del visor  
**When** el modal renderiza sus pestañas  
**Then** el usuario ve la pestaña **"Firma personal"**.

### Requirement: Tab shows enterprise states

La pestaña **MUST** soportar estados visibles:

- `loading`: mientras se solicita metadata/binario.
- `empty`: cuando el backend indica ausencia de firma temporal o respuesta sin `data`.
- `error`: cuando ocurre un error controlado.
- `ready`: cuando existe una firma descargada lista para usar.

#### Scenario: Loading state is shown
**Given** el usuario entra a la pestaña **"Firma personal"**  
**When** la carga inicia  
**Then** el sistema muestra un estado `loading` no bloqueante.

#### Scenario: Empty state is shown
**Given** el backend responde `success=true` pero `data=null` (o caso equivalente de “sin firma”)  
**When** el usuario entra a la pestaña  
**Then** el sistema muestra estado `empty` con mensaje de negocio.

#### Scenario: Error state is shown
**Given** ocurre un error de red o el backend responde `success=false`  
**When** el usuario entra a la pestaña  
**Then** el sistema muestra estado `error` y ofrece una acción “Reintentar”.

### Requirement: Empty and error states provide user guidance

En estado `empty` el sistema **MUST** mostrar un mensaje de negocio que indique que el usuario no tiene firma personal configurada (sin depender de texto exacto).

En estado `error` el sistema **MUST**:

- Mostrar un mensaje entendible para usuario (o el `message` del backend si es seguro mostrarlo).
- Proveer una acción de recuperación (ej. “Reintentar”).

### Requirement: Use personal signature reuses existing signature pipeline

Cuando la firma personal está en estado `ready`, el sistema **MUST** permitir al usuario aplicarla como firma actual del modal con un botón explícito (ej. **"Usar firma personal"**).

- La acción **MUST** reutilizar el pipeline existente del modal (equivalente a “upload”), y **MUST NOT** introducir un nuevo tipo de firma.
- La pestaña **MUST** limpiar/normalizar su estado interno luego de “usar” la firma para evitar reusar una firma previa de manera inadvertida.

#### Scenario: User applies personal signature
**Given** la pestaña está en estado `ready` con una firma descargada  
**When** el usuario presiona **"Usar firma personal"**  
**Then** la firma pasa a ser la firma activa del modal (misma ruta que upload/draw) sin errores.
