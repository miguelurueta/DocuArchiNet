# Capability: workflow-personal-signature-fetch

## ADDED Requirements

### Requirement: Metadata endpoint is requested with JWT

El sistema **MUST** solicitar metadata de firma temporal mediante:

- `GET /api/workflow/usuarios/firma-temporal`
- Header `Authorization: Bearer <JWT>`

El sistema **MUST NOT** loggear ni exponer el JWT.

#### Scenario: Metadata request includes Authorization header
**Given** existe una sesión activa con JWT  
**When** el usuario entra a la pestaña “Firma personal” y se inicia la carga  
**Then** el request incluye el header `Authorization: Bearer <JWT>`.

### Requirement: Response wrapper is validated

El sistema **MUST** validar la respuesta:

- Si `success !== true` → estado `error` con `message` del backend (si existe).
- Si `success === true` y `data == null` → estado `empty`.
- Si `success === true` y `data != null` → continuar con descarga usando `data.UrlTemporal`.

#### Scenario: Empty is derived from success with null data
**Given** el backend responde `success=true` y `data=null`  
**When** el sistema procesa la respuesta  
**Then** el estado pasa a `empty`.

### Requirement: Download uses UrlTemporal exactly

El sistema **MUST** descargar el binario usando **exactamente** `data.UrlTemporal`:

- Si `UrlTemporal` es absoluta (`http...`) → usar tal cual.
- Si `UrlTemporal` es relativa (`/api/...`) → concatenar con `baseUrl`.

El sistema **MUST NOT** parsear, recortar ni reconstruir el token manualmente.

#### Scenario: Relative UrlTemporal is rebased with baseUrl
**Given** `data.UrlTemporal` inicia con `/api/`  
**When** el sistema construye el `downloadUrl`  
**Then** el sistema concatena `baseUrl + UrlTemporal`.

#### Scenario: Absolute UrlTemporal is used as-is
**Given** `data.UrlTemporal` inicia con `http`  
**When** el sistema construye el `downloadUrl`  
**Then** el sistema usa `UrlTemporal` tal cual, sin concatenar ni transformar.

### Requirement: Download request includes JWT

El sistema **MUST** descargar el binario mediante:

- `GET {downloadUrl}`
- Header `Authorization: Bearer <JWT>`

#### Scenario: Download includes Authorization header
**Given** el sistema construye el `downloadUrl`  
**When** realiza la descarga  
**Then** el request incluye `Authorization: Bearer <JWT>`.

### Requirement: 404 download triggers metadata refresh and one retry

Si el download responde `404`, el sistema **MUST**:

1) Re-solicitar metadata.
2) Reintentar la descarga **una sola vez**.

Si el segundo intento falla, el sistema **MUST** pasar a estado `error`.

#### Scenario: Download 404 triggers one retry
**Given** la primera descarga responde `404`  
**When** el sistema maneja el error  
**Then** re-solicita metadata y reintenta download una vez.

### Requirement: Object URL lifecycle is cleaned

Cuando el sistema convierta el `Blob` en `ObjectURL`, **MUST** revocar (`URL.revokeObjectURL`) al:

- reemplazar la firma personal por una nueva.
- cerrar el modal o desmontar la pestaña/componente.

#### Scenario: ObjectURL is revoked on cleanup
**Given** el sistema creó un `ObjectURL` para preview  
**When** el usuario cierra el modal (o el componente se desmonta)  
**Then** el sistema revoca el `ObjectURL`.

### Requirement: Temporary signature MUST NOT be persisted beyond modal lifecycle

El sistema **MUST NOT** persistir en almacenamiento durable (localStorage/sessionStorage/indexedDB) ninguno de:

- `UrlTemporal`
- `token` implícito en `UrlTemporal`
- `Blob` de firma personal
- `ObjectURL`

El uso de firma personal **MUST** ser estrictamente in-memory dentro del ciclo de vida del modal/pestaña.

#### Scenario: No durable persistence is performed
**Given** el sistema descargó la firma personal y creó un `ObjectURL`  
**When** el usuario navega o recarga el modal  
**Then** el sistema no reusa datos persistidos y vuelve a solicitar metadata si el usuario lo requiere.
